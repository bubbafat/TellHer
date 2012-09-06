using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TellHer.Data;

using TellHer.Domain;

namespace TellHer.SubscriptionService.Actions
{
    class Add : ClientAction
    {
        public override string Action
        {
            get { return Name; }
        }

        public static string Name
        {
            get { return "add"; }
        }

        protected override void PerformAdmin(Domain.IncomingSmsMessage message)
        {
            string newIdea;
            if (TryCrackMessage(message.Message, out newIdea))
            {
                IDataStore store = DataStore.GetInstance();
                DailyIdea idea = store.DailyIdeas.Where(i => i.Idea == newIdea).FirstOrDefault();

                if (idea == null)
                {
                    idea = new DailyIdea
                    {
                        Idea = newIdea,
                    };
                    store.Save(idea);

                    Say(message.From, SmsResponseStrings.Add_Success_AddedNewIdea(idea.Id));
                }
                else
                {
                    Say(message.From, SmsResponseStrings.Add_Failed_ExistingIdea(idea.Id));
                }
            }
            else
            {
                Say(message.From, SmsResponseStrings.Add_Help());
            }
        }
    }
}
