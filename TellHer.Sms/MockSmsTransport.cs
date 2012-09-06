using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TellHer.Domain;
using StructureMap;
using TellHer.Data;

namespace TellHer.Sms
{
    public class MockSmsTransport : ISmsTransport
    {
        public SentSmsMessageLogEntry Send(string phoneNumber, string message)
        {
            ObjectFactory.GetInstance<ILogging>().Trace("SMS Mock to {0} \"{1}\"", phoneNumber, message);

            return new SentSmsMessageLogEntry
            {
                Date = DateTime.UtcNow,
                Destination = phoneNumber,
                Message = message,
                Status = (int)MessageSendStatus.Success,
                Details = "MockSmsTransport",
            };
        }

        public int DelayMs
        {
            get { return 1000; }
        }
    }
}
