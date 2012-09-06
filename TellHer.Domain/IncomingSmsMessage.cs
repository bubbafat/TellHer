using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TellHer.Domain
{
    public class IncomingSmsMessage
    {
        public string Message { get; set; }
        public string From { get; set; }
    }
}
