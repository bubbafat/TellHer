using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StructureMap;

namespace TellHer.Data
{
    public class DailyIdea
    {
        public int Id { get; set; }
        public string Idea { get; set; }
        public DateTime? LastUsed { get; set; }
        public DateTime? ScheduledDate { get; set; }

        private static DailyIdea _todaysCachedIdea = null;
        public static DailyIdea TodaysIdea(DateTime today)
        {
            if (_todaysCachedIdea != null && _todaysCachedIdea.LastUsed == today.Date)
            {
                return _todaysCachedIdea;
            }

            IDataStore db = ObjectFactory.GetInstance<IDataStore>();

            // is there one scheduled for today?
            DateTime todayDate = today.Date;
            DailyIdea result = db.DailyIdeas.Where(i => i.ScheduledDate == todayDate).FirstOrDefault();

            // is there one we're using today?
            if (result == null)
            {
                result = db.DailyIdeas.Where(i => i.LastUsed == todayDate).FirstOrDefault();
            }

            if (result == null)
            {
                // is there one we haven't used?
                result = db.DailyIdeas.Where(i => i.LastUsed == null && i.ScheduledDate == null).FirstOrDefault();

                if (result == null)
                {
                    // pick the one we haven't used in the longest time
                    result = db.DailyIdeas.Where(i => i.ScheduledDate == null).OrderBy(i => i.LastUsed).FirstOrDefault();
                }

                if (result != null)
                {
                    result.LastUsed = today.Date;
                    db.Save(result);
                }
            }

            return result;
        }
    }
}
