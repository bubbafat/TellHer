using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TellHer.Data;
using StructureMap;
using TellHer.Domain;

namespace TellHer.SubscriptionService.Actions
{
    class Quit : ClientAction
    {
        public override string Action
        {
            get { return Name; }
        }

        public static string Name
        {
            get { return "quit"; }
        }

        protected override void PerformUnknownUser(Domain.IncomingSmsMessage message)
        {
            DoQuit(message);
        }

        protected override void PerformSubscriber(Domain.IncomingSmsMessage message)
        {
            DoQuit(message);
        }

        protected override void PerformAdmin(Domain.IncomingSmsMessage message)
        {
            DoQuit(message);
        }

        private static void DoQuit(Domain.IncomingSmsMessage message)
        {
            IDataStore store = ObjectFactory.GetInstance<IDataStore>();
            store.RemoveAllForPhone(message.From);

            Say(message.From, SmsResponseStrings.Quit_AllRemoved(message.From));
        }
    }
}
