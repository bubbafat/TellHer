using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TellHer.Data;

using TellHer.Domain;

namespace TellHer.SubscriptionService.Actions
{
    class Join : ClientAction
    {
        public override string Action
        {
            get { return Name; }
        }

        public static string Name
        {
            get { return "join"; }
        }

        protected override void PerformUnknownUser(Domain.IncomingSmsMessage message)
        {
            IDataStore store = DataStore.GetInstance();
            IConfiguration config = Configuration.GetInstance();

            if (store.Subscriptions.Count() < config.BetaLimit)
            {
                Subscription s = new Subscription
                {
                    Phone = message.From,
                    Next = DateTime.UtcNow,
                };

                store.Save(s);

                Say(message.From, SmsResponseStrings.Join_Created());
            }
            else
            {
                Say(message.From, SmsResponseStrings.Join_SorryBetaFull());
                Say(config.AdminNumber, SmsResponseStrings.Join_AdminBetaReject());
            }
        }

        protected override void PerformSubscriber(Domain.IncomingSmsMessage message)
        {
            Say(message.From, SmsResponseStrings.Join_AlreadyExists());
        }

        protected override void PerformAdmin(Domain.IncomingSmsMessage message)
        {
            IDataStore store = DataStore.GetInstance();
            if (!store.Subscriptions.Any(s => s.Phone == message.From))
            {
                PerformUnknownUser(message);
            }
            else
            {
                PerformSubscriber(message);
            }
        }
    }
}
