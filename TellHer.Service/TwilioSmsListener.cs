using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.ServiceModel.Web;
using System.IO;
using System.Web;
using System.Collections.Specialized;
using System.Security;
using System.Configuration;
using StructureMap;
using TellHer.Domain;
using System.Web.Util;
using TellHer.Data;
using System.ServiceModel.Activation;
using System.ServiceModel.Channels;

namespace TellHer.Service
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class TwilioSmsListener : ITwilioSmsListener
    {
        IConfiguration _Configuration = ObjectFactory.GetInstance<IConfiguration>();

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

            ObjectFactory.GetInstance<IDataStore>().Add(incomingLog);

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
                ISubscriptionService handler = ObjectFactory.GetInstance<ISubscriptionService>();
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
