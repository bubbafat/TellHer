using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TellHer.SubscriptionService.Actions
{
    class Ping : ClientAction
    {
        public override string Action
        {
            get { return Name; }
        }

        public static string Name
        {
            get { return "ping"; }
        }

        protected override void PerformAdmin(Domain.IncomingSmsMessage message)
        {
            Say(message.From, "pong");
        }
    }
}
