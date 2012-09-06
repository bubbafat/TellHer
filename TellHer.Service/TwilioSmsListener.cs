using System;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Web;

using TellHer.Data;
using TellHer.Domain;
using Configuration = TellHer.Domain.Configuration;

namespace TellHer.Service
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class TwilioSmsListener : ITwilioSmsListener
    {
        IConfiguration _Configuration = Configuration.GetInstance();

        public void Incoming(Stream input)
        {
            NameValueCollection param = new NameValueCollection();
            using (StreamReader sr = new StreamReader(input))
            {
                param = HttpUtility.ParseQueryString(sr.ReadToEnd());
            }

            OutgoingWebResponseContext context = WebOperationContext.Current.OutgoingResponse;

            string body = param["Body"];
            string from = param["From"];

            IncomingMessageLog incomingLog = new IncomingMessageLog
            {
                Date = DateTime.UtcNow,
                From = from,
                Message = body,
            };

            DataStore.GetInstance().Add(incomingLog);

            LogManager.Log.Trace("SMS RECV: {0} {1}", from, body);

            context.Headers.Add(System.Net.HttpResponseHeader.CacheControl, "no-cache");

            if (!ValidatePhoneNumber(from))
            {
                LogManager.Log.Warn("SMS RECV: Invalid Phone Number {0} (reference IncomingMessageLog {1})",
                    from == null ? "<no from>" : from,
                    incomingLog.Id);

                context.StatusCode = System.Net.HttpStatusCode.Forbidden;
            }
            else
            {
                ICommandProcessor handler = CommandProcessor.GetInstance();
                handler.Add(new IncomingSmsMessage { From = from, Message = body });
                context.StatusCode = System.Net.HttpStatusCode.OK;
            }
        }

        private bool ValidatePhoneNumber(string num)
        {
            if (string.IsNullOrEmpty(num))
            {
                return false;
            }

            if(!num.StartsWith("+1"))
            {
                return false;
            }

            num = num.Substring(2);

            if (num.Length != 10)
            {
                return false;
            }

            foreach (char c in num)
            {
                if (!char.IsDigit(c))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
