using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TellHer.Data;

using TellHer.Domain;

namespace TellHer.SubscriptionService.Actions
{
    class Block : ClientAction
    {
        public override string Action
        {
            get { return Name; }
        }

        public static string Name
        {
            get { return "block"; }
        }

        protected override void PerformAdmin(Domain.IncomingSmsMessage message)
        {
            string[] parts = message.Message.Split(new char[] { ' ' }, 4, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != 3)
            {
                Say(message.From, "Usage: BLOCK <+1222333444> <days>    Example: BLOCK +19005551212 7");
            }

            string number = parts[1];
            int days;
            if (!int.TryParse(parts[2], out days))
            {
                Say(message.From, "Usage: BLOCK <+1222333444> <days>    Example: BLOCK +19005551212 7");
            }

            NumberBlocker.Block(number, days);

            Say(message.From, "The number {0} has been blocked for {1} days.", number, days);
        }
    }
}
