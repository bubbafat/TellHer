using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using TellHer.Domain;
using System.Collections.Specialized;
using System.IO;
using System.Web;

using System.ServiceModel.Web;
using TellHer.Sms;

namespace TellHer.Service
{
    public class TwilioServiceAuthorizationManager : IServiceAuthorizationManager
    {
        IConfiguration _config = Configuration.GetInstance();

        public bool CheckAccess(OperationContext operationContext, ref System.ServiceModel.Channels.Message message)
        {
            var copy = message.CreateBufferedCopy(2048);
            string base64Value = copy.CreateNavigator().SelectSingleNode("//Binary").Value;
            if (string.IsNullOrEmpty(base64Value))
            {
                LogManager.Log.Warn("CheckAccess returned false - the message body was not found");
                return false;
            }

            NameValueCollection param = new NameValueCollection();

            using (MemoryStream stream = new MemoryStream(Convert.FromBase64String(base64Value)))
            using (StreamReader sr = new StreamReader(stream))
            {
                param = HttpUtility.ParseQueryString(sr.ReadToEnd());
            }

            message = copy.CreateMessage();
            copy.Close();

            ITwilioRequestValidator req = RequestValidator.GetInstance();

            if (WebOperationContext.Current.IncomingRequest.UriTemplateMatch == null)
            {
                LogManager.Log.Warn("Incoming request missing UriTemplateMatch - probably didn't go to the correct controller - rejected SMS");
                return false;
            }

            string uri = WebOperationContext.Current.IncomingRequest.UriTemplateMatch.RequestUri.OriginalString;

            string sig = WebOperationContext.Current.IncomingRequest.Headers["X-Twilio-Signature"];
            if (string.IsNullOrEmpty(sig))
            {
                LogManager.Log.Warn("X-Twilio-Signature missing - rejected SMS");
                return false;
            }

            if (req.IsValid(uri, sig, param))
            {
                return true;
            }

            LogManager.Log.Warn("X-Twilio-Signature failure - rejected SMS");
            return false;
        }
    }
}
