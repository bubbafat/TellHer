using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TellHer.Data;
using StructureMap;

namespace TellHer.SubscriptionService.Actions
{
    class Sub : ClientAction
    {
        public override string Action
        {
            get { return Name; }
        }

        public static string Name
        {
            get { return "sub"; }
        }

        protected override void PerformAdmin(Domain.IncomingSmsMessage message)
        {
            int id;
            if (TryCrackId(message.Message, out id))
            {
                IDataStore store = ObjectFactory.GetInstance<IDataStore>();
                Subscription sub = store.Subscriptions.Where(s => s.Id == id).FirstOrDefault();

                if (sub != null)
                {
                    Say(message.From, "Subscription {0} has phone {1} and is due on {2}",
                        sub.Id,
                        sub.Phone,
                        sub.Next);
                }
                else
                {
                    Say(message.From, "Subscription Not Found: {0}", id);
                }
            }
            else
            {
                Say(message.From, "Unable to parse input.  Usage: SUB <id>   Example:  SUB 10");
            }
        }
    }
}
