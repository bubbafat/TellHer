using System;
using System.Configuration;
using System.Globalization;
using TellHer.Data;
using TellHer.Domain;
using Twilio;
using Configuration = TellHer.Domain.Configuration;

namespace TellHer.Sms
{
    public class TwilioSmsSender : ISmsTransport
    {
        IConfiguration _Configuration = Configuration.GetInstance();
        TwilioRestClient _client;
        readonly object _lock = new object();

        public SentSmsMessageLogEntry Send(string phoneNumber, string message)
        {
            SentSmsMessageLogEntry result = new SentSmsMessageLogEntry
            {
                Date = DateTime.UtcNow,
                Message = message,
                Destination = phoneNumber,
            };

            SMSMessage msg = Client.SendSmsMessage(_Configuration.FromNumber, phoneNumber, message);
            if (msg.Status == "failed")
            {
                result.Status = (int)MessageSendStatus.Error;

                if (msg.RestException != null)
                {
                    result.Details = string.Format(CultureInfo.InvariantCulture, "Code: {0} : Status: {1}",
                        msg.RestException.Code,
                        msg.RestException.Status);
                }
                else
                {
                    result.Details = "Unknown error (Twilio did not report a RestException)";
                }
            }
            else
            {
                result.MessageId = msg.Sid;
                result.Status = (int)MessageSendStatus.Success;
            }

            return result;
        }

        public int DelayMs
        {
            get { return 1000; }
        }

        private TwilioRestClient Client
        {
            get
            {
                if (_client == null)
                {
                    lock (_lock)
                    {
                        if (_client == null)
                        {
                            _client = new TwilioRestClient(_Configuration.AccountSid, _Configuration.AuthToken);
                        }
                    }
                }

                return _client;
            }
        }
    }
}
