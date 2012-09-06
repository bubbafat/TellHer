using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TellHer.Data;
using StructureMap;

namespace TellHer.SubscriptionService.Actions
{
    class Status : ClientAction
    {
        public override string Action
        {
            get { return Name; }
        }

        public static string Name
        {
            get { return "status"; }
        }

        protected override void PerformAdmin(Domain.IncomingSmsMessage message)
        {
            IDataStore store = ObjectFactory.GetInstance<IDataStore>();

            DateTime fromWhen = DateTime.UtcNow.AddDays(-1);

            Say(message.From, "Subscriptions: {0}  Last 24/hr Received: {1}, Send Success {2}, Send fail {3}, Pending Send {4}",
                store.Subscriptions.Count(),
                store.IncomingMessageLogs.Count(m => m.Date > fromWhen),
                store.SentSmsMessageLogEntries.Count(m => m.Date > fromWhen && m.Status == (int)MessageSendStatus.Success),
                store.SentSmsMessageLogEntries.Count(m => m.Date > fromWhen && m.Status != (int)MessageSendStatus.Success),
                store.OutgoingMessages.Count());
        }
    }
}
