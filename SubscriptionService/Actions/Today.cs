using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TellHer.Data;

namespace TellHer.SubscriptionService.Actions
{
    class Today : ClientAction
    {
        public override string Action
        {
            get { return Name; }
        }

        public static string Name
        {
            get { return "today"; }
        }

        protected override void PerformSubscriber(Domain.IncomingSmsMessage message)
        {
            RepeatToday(message);
        }

        protected override void PerformAdmin(Domain.IncomingSmsMessage message)
        {
            RepeatToday(message);
        }

        private void RepeatToday(Domain.IncomingSmsMessage message)
        {
            DailyIdea idea = DailyIdea.TodaysIdea(DateTime.UtcNow);

            string ideaMsg = idea == null ?
                "ERROR: There is no idea for today!" :
                idea.Idea;

            Say(message.From, ideaMsg);
        }
    }
}
