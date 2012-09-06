using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TellHer.Data;


namespace TellHer.SubscriptionService.Actions
{
    class Remove : ClientAction
    {
        public override string Action
        {
            get { return Name; }
        }

        public static string Name
        {
            get { return "remove"; }
        }

        protected override void PerformAdmin(Domain.IncomingSmsMessage message)
        {
            int id;
            if (TryCrackId(message.Message, out id))
            {
                IDataStore store = DataStore.GetInstance();
                DailyIdea lookup = store.DailyIdeas.Where(l => l.Id == id).FirstOrDefault();

                if (lookup != null)
                {
                    store.Remove(lookup);
                    Say(message.From, "Removed idea with ID {0}", id);
                }
                else
                {
                    Say(message.From, "Idea Not Found: {0}", id);
                }
            }
            else
            {
                Say(message.From, "Unable to parse input.  Usage: REMOVE <id>   Example:  REMOVE 10");
            }
        }

    }
}
