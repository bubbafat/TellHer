using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TellHer.Domain;
using StructureMap;

namespace TellHer.SubscriptionService.Actions
{
    class Suggest : ClientAction
    {
        public override string Action
        {
            get { return Name; }
        }

        public static string Name
        {
            get { return "suggest"; }
        }

        protected override void PerformSubscriber(Domain.IncomingSmsMessage message)
        {
            SaveSuggestion(message);
        }

        protected override void PerformAdmin(Domain.IncomingSmsMessage message)
        {
            SaveSuggestion(message);
        }

        private void SaveSuggestion(Domain.IncomingSmsMessage message)
        {
            Say(ObjectFactory.GetInstance<IConfiguration>().AdminNumber,
                string.Format("sug: {0}", message.Message));
            Say(message.From, "Thanks for the feedback!  You can also contact me as @tellhernow on Twitter");
        }
    }
}
