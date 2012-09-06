using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TellHer.Domain;

namespace TellHer.SubscriptionService.Actions
{
    class Unblock : ClientAction
    {
        public override string Action
        {
            get { return Name; }
        }

        public static string Name
        {
            get { return "unblock"; }
        }

        protected override void PerformAdmin(Domain.IncomingSmsMessage message)
        {
            string phone;
            if (TryCrackMessage(message.Message, out phone))
            {
                NumberBlocker.Unblock(phone);
                Say(message.From, "All blocks for number {0} have been removed.");
            }
            else
            {
                Say(message.From, "Usage: UNBLOCK <+1222333444>    Example: UNBLOCK +19005551212");
            }
        }
    }
}
