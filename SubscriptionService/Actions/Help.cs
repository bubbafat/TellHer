using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TellHer.SubscriptionService.Actions
{
    class Help : ClientAction
    {
        public override string Action
        {
            get { return Name; }
        }

        protected override void PerformAdmin(Domain.IncomingSmsMessage message)
        {
            Say(message.From, "join, quit, help, say <id>, remove <id>, edit <id> <new>, add <new>");
        }

        public static string Name
        {
            get
            {
                return "help";
            }
        }
    }
}
