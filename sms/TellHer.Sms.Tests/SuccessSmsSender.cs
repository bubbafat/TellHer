using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TellHer.Domain;
using TellHer.Data;

namespace TellHer.Sms.Tests
{
    class SuccessSmsSender : ISmsTransport
    {
        public SentSmsMessageLogEntry Send(string phoneNumber, string message)
        {
            return new SentSmsMessageLogEntry
            {
                Date = DateTime.UtcNow,
                Destination = phoneNumber,
                Message = message,
                Status = (int)MessageSendStatus.Success,
                Details = "SuccessSmsSender",
            };
        }



        public int DelayMs
        {
            get { return 25; }
        }
    }
}
