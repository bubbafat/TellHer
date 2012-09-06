using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TellHer.Data;
using StructureMap;

namespace TellHer.SubscriptionService.Actions
{
    class Edit : ClientAction
    {
        public override string Action
        {
            get { return Name; }
        }

        public static string Name
        {
            get { return "edit"; }
        }

        protected override void PerformAdmin(Domain.IncomingSmsMessage message)
        {
            int id;
            string editedIdea;
            if (TryCrackIdMessage(message.Message, out id, out editedIdea))
            {
                IDataStore store = ObjectFactory.GetInstance<IDataStore>();
                DailyIdea idea = store.DailyIdeas.Where(i => i.Id == id).FirstOrDefault();

                if (idea != null)
                {
                    if (editedIdea.Length < 10)
                    {
                        Say(message.From, "EDIT failed.  Your idea is really short ... probably a mistake.  Want to try again?");
                    }
                    else
                    {
                        idea.Idea = editedIdea;
                        store.Save(idea);

                        Say(message.From, "Idea {0} has been updated to your new text", idea.Id);
                    }
                }
                else
                {
                    Say(message.From, "Idea {0} does not exist.  Use add to create a new idea", id);
                }
            }
            else
            {
                Say(message.From, "Unable to parse input.  Usage: EDIT <id> <new message>   Example:  EDIT 1 this is the new idea");
            }
        }

    }
}
