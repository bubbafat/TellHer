using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TellHer.Data;
using StructureMap;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TellHer.Domain;
using TellHer.Test.Common;

namespace SubscriptionServices.Tests
{
    [TestClass]
    public class ActionTestBase
    {
        [TestInitialize]
        public void CreateMocksAddData()
        {
            ObjectFactory.Initialize(x =>
            {
                x.For<ILogging>().Use<TraceLogger>();
                x.For<IConfiguration>().Use<MockConfig>();
                x.For<IDataStore>().Use<TestDataStore>();
            });

            TestDataStore.Reset();

            IDataStore store = ObjectFactory.GetInstance<IDataStore>();
            store.Save(new Subscription
            {
                Next = DateTime.UtcNow,
                Phone = ExistingSubscriberPhone,
            });
        }

        protected string ExistingSubscriberPhone
        {
            get
            {
                return "+15552223333";
            }
        }

        protected string NonSubscriberPhone
        {
            get
            {
                return "+11112223333";
            }
        }

        protected IList<Subscription> SubscriptionForNumber(string phone)
        {
            IDataStore store = ObjectFactory.GetInstance<IDataStore>();
            return store.Subscriptions.Where(s => s.Phone == phone).ToList();
        }

        protected void OutgoingMessageExists(string phone, string message)
        {
            IDataStore store = ObjectFactory.GetInstance<IDataStore>();
            Assert.AreEqual(1, store.OutgoingMessages.Count(m => m.Destination == phone && m.Message == message));
        }
    }
}
