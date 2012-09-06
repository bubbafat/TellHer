using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using TellHer.Data;
using TellHer.Test.Common;
using StructureMap;

namespace TellHer.Domain.Tests
{
    [TestClass]
    public class DailyIdeaTests
    {
        [TestInitialize]
        public void SetupData()
        {
            ObjectFactory.Initialize(x =>
            {
                x.For<ILogging>().Use<TraceLogger>();
                x.For<IConfiguration>().Use<MockConfig>();
                x.For<IDataStore>().Use<TestDataStore>();
            });

            TestDataStore.Reset();
        }

        [TestMethod]
        public void DailyIdea_ScheduledDates()
        {
            IDataStore store = DataStore.GetInstance();

            for (int i = 0; i < 5; i++)
            {
                store.Save(new DailyIdea
                {
                    Idea = "This is idea " + i.ToString(),
                });
            }

            DateTime scheduled1_date = DateTime.UtcNow.AddYears(-1).Date;
            string scheduled1_idea = "This is scheduled idea 1";
            store.Save(new DailyIdea
            {
                Idea = scheduled1_idea,
                ScheduledDate = scheduled1_date
            });

            DateTime scheduled2_date = scheduled1_date.AddDays(1);
            string scheduled2_idea = "This is scheduled idea 1";
            store.Save(new DailyIdea
            {
                Idea = scheduled2_idea,
                ScheduledDate = scheduled2_date
            });

            for (int i = 0; i < 5; i++)
            {
                store.Save(new DailyIdea
                {
                    Idea = "This is idea " + (5 + i).ToString(),
                });
            }

            for (int i = 0; i < 100; i++)
            {
                DailyIdea idea = DailyIdea.TodaysIdea(DateTime.UtcNow.AddDays(i));
                Assert.IsTrue(idea.Idea.StartsWith("This is idea "));
            }

            DailyIdea sch1 = DailyIdea.TodaysIdea(scheduled1_date);
            Assert.AreEqual(scheduled1_idea, sch1.Idea);

            DailyIdea sch2 = DailyIdea.TodaysIdea(scheduled1_date);
            Assert.AreEqual(scheduled1_idea, sch2.Idea);

            for (int i = 0; i < 100; i++)
            {
                DailyIdea idea = DailyIdea.TodaysIdea(DateTime.UtcNow.AddDays(i));
                Assert.IsTrue(idea.Idea.StartsWith("This is idea "));
            }
        }


        [TestMethod]
        public void DailyIdea_DoesNotRepeat()
        {
            IDataStore store = DataStore.GetInstance();
            
            for(int i = 0; i < 5; i++)
            {
                store.Save(new DailyIdea
                {
                    Idea = "This is idea " + i.ToString(),
                });
            }

            DateTime fixedDate = new DateTime(2012, 1, 1, 2, 0, 0);

            HashSet<int> ids = new HashSet<int>();
            HashSet<string> ideas = new HashSet<string>();

            for (int i = 0; i < 5; i++)
            {
                DailyIdea idea = DailyIdea.TodaysIdea(fixedDate.AddDays(i));
                Assert.IsNotNull(idea);

                Assert.IsFalse(ids.Contains(idea.Id));
                ids.Add(idea.Id);

                Assert.IsFalse(ideas.Contains(idea.Idea));
                ideas.Add(idea.Idea);
            }

            for (int i = 0; i < 5; i++)
            {
                DailyIdea idea = DailyIdea.TodaysIdea(fixedDate.AddDays(i));
                DailyIdea ideam1 = DailyIdea.TodaysIdea(fixedDate.AddDays(i).AddHours(-1));
                DailyIdea ideap12 = DailyIdea.TodaysIdea(fixedDate.AddDays(i).AddHours(12));

                Assert.AreEqual(idea.Id, ideam1.Id);
                Assert.AreEqual(idea.Id, ideap12.Id);
            }
        }
    }
}
