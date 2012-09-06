using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TellHer.Data;
using StructureMap;
using TellHer.Domain;

namespace TellHer.SubscriptionService.Actions
{
    class Last : ClientAction
    {
        public override string Action
        {
            get { return Name; }
        }

        public static string Name
        {
            get { return "last"; }
        }

        protected override void PerformAdmin(Domain.IncomingSmsMessage message)
        {
            string action;
            if (TryCrackMessage(message.Message, out action))
            {
                switch (action.ToLowerInvariant())
                {
                    case "error":
                        SayLastError(message);
                        break;
                    case "log":
                        SayLostLog(message);
                        break;
                    default:
                        Say(message.From, SmsResponseStrings.Last_Help());
                        break;
                }
            }
        }

        private void SayLastError(Domain.IncomingSmsMessage message)
        {
            IDataStore store = ObjectFactory.GetInstance<IDataStore>();

            LogItem lastError = store.LogItems.Where(l => l.Exception != null).OrderByDescending(l => l.Date).FirstOrDefault();
            if (lastError != null)
            {
                Say(message.From, "Error entry {0} at occurred at {1} (UTC)",
                    lastError.ID,
                    lastError.Date);

                if(!string.IsNullOrEmpty(lastError.Message))
                {
                    Say(message.From, SubOrNull(lastError.Message, 0, 140));
                }

                // send at most 3 messages
                List<string> parts = new List<string>();
                parts.Add(SubOrNull(lastError.Exception, 0, 140));
                parts.Add(SubOrNull(lastError.Exception, 140, 140));
                parts.Add(SubOrNull(lastError.Exception, 280, 140));

                foreach (string part in parts)
                {
                    if (part != null)
                    {
                        Say(message.From, part);
                    }
                }
            }
            else
            {
                Say(message.From, "No errors in the log.  Hurray?");
            }
        }

        private string SubOrNull(string input, int start, int length)
        {
            if (input.Length < start)
            {
                return null;
            }

            // input.Length = 148
            // start        = 140
            // length       = 140
            // set length to input.Length - start = 8
            if (input.Length < (start + length))
            {
                length = input.Length - start;
            }

            return input.Substring(start, length);
        }

        private void SayLostLog(Domain.IncomingSmsMessage message)
        {
            IDataStore store = ObjectFactory.GetInstance<IDataStore>();

            LogItem lastLog = store.LogItems.OrderByDescending(l => l.Date).FirstOrDefault();
            if (lastLog != null)
            {
                // send at most 3 messages
                List<string> parts = new List<string>();
                parts.Add(SubOrNull(lastLog.Message, 0, 140));
                parts.Add(SubOrNull(lastLog.Message, 140, 140));
                parts.Add(SubOrNull(lastLog.Message, 280, 140));

                foreach (string part in parts)
                {
                    if (part != null)
                    {
                        Say(message.From, part);
                    }
                }
            }
        }

    }
}
