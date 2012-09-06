using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TellHer.Data;
using StructureMap;

namespace TellHer.SubscriptionService.Actions
{
    class Say : ClientAction
    {
        public override string Action
        {
            get { return Name; }
        }

        public static string Name
        {
            get { return "say"; }
        }

        protected override void PerformAdmin(Domain.IncomingSmsMessage message)
        {
            int id;
            if (TryCrackId(message.Message, out id))
            {
                IDataStore store = ObjectFactory.GetInstance<IDataStore>();
                DailyIdea idea = store.DailyIdeas.Where(i => i.Id == id).FirstOrDefault();
                if (idea != null)
                {
                    Say(message.From, idea.Idea);
                }
            }
        }
    }
}
