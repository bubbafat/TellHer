using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TellHer.Data
{
    public class SentSmsMessageLogEntry
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string MessageId { get; set; }
        public string Destination { get; set; }
        public string Message { get; set; }
        public int Status { get; set; }
        public string Details { get; set; }
    }

    public enum MessageSendStatus
    {
        Unknown,
        Success,
        Error,
        Abandoned,
    }
}
