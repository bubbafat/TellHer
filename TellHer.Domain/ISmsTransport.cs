using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TellHer.Data;

namespace TellHer.Domain
{
    public interface ISmsTransport
    {
        SentSmsMessageLogEntry Send(string phoneNumber, string message);
        int DelayMs { get; }
    }
}
