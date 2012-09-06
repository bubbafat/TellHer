using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TellHer.Data;

namespace TellHer.Domain
{
    public delegate void SmsSenderExceptionThrownCallback(OutgoingSmsMessage message, Exception exception);
    public delegate void SendCompleteCallback(SentSmsMessageLogEntry result, OutgoingSmsMessage message);

    public interface IOutgoingSmsQueue : IDisposable
    {
        void Send(OutgoingSmsMessage msg, SendCompleteCallback complete, SmsSenderExceptionThrownCallback exceptionThrown);
        int Length { get; }
        int MessagesPerMinute { get; }
    }
}
