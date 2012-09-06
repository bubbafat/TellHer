using TellHer.SubscriptionService.Actions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using TellHer.Domain;

using TellHer.Data;
using System.Linq;
using System.Globalization;


namespace SubscriptionServices.Tests
{


    [TestClass()]
    public class JoinTest : ActionTestBase
    {


        private TestContext testContextInstance;

        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        [TestMethod()]
        [DeploymentItem("TellHer.SubscriptionService.dll")]
        public void Perform_MissingUser_Creates()
        {
            Join_Accessor target = new Join_Accessor(); // TODO: Initialize to an appropriate value

            IncomingSmsMessage message = new IncomingSmsMessage
            {
                From = base.NonSubscriberPhone,
                Message = target.Action,
            };

            Assert.AreEqual(0, SubscriptionForNumber(message.From).Count);

            target.PerformAdmin(message);

            Assert.AreEqual(1, SubscriptionForNumber(message.From).Count);
            base.OutgoingMessageExists(message.From, SmsResponseStrings.Join_Created());
        }

        [TestMethod()]
        [DeploymentItem("TellHer.SubscriptionService.dll")]
        public void Perform_ExistingUser_Skip()
        {
            Join_Accessor target = new Join_Accessor(); // TODO: Initialize to an appropriate value

            IncomingSmsMessage message = new IncomingSmsMessage
            {
                From = base.ExistingSubscriberPhone,
                Message = target.Action,
            };

            Assert.AreEqual(1, SubscriptionForNumber(message.From).Count);

            target.PerformAdmin(message);

            Assert.AreEqual(1, SubscriptionForNumber(message.From).Count);
            base.OutgoingMessageExists(message.From, SmsResponseStrings.Join_AlreadyExists());
        }

        [TestMethod()]
        [DeploymentItem("TellHer.SubscriptionService.dll")]
        public void Perform_MissingUser_BetaFull_Sorry()
        {
            Join_Accessor target = new Join_Accessor(); // TODO: Initialize to an appropriate value

            IDataStore store = DataStore.GetInstance();
            IConfiguration config = Configuration.GetInstance();

            int count = 0;

            while(store.Subscriptions.Count() < config.BetaLimit)
            {
                store.Save(new Subscription
                {
                    Next = DateTime.UtcNow,
                    Phone = string.Format(CultureInfo.InvariantCulture, "+1111222{0}", count++),
                });
            }

            IncomingSmsMessage message = new IncomingSmsMessage
            {
                From = base.NonSubscriberPhone,
                Message = target.Action,
            };

            Assert.AreEqual(config.BetaLimit, store.Subscriptions.Count());

            target.PerformAdmin(message);

            Assert.AreEqual(config.BetaLimit, store.Subscriptions.Count());

            base.OutgoingMessageExists(message.From, SmsResponseStrings.Join_SorryBetaFull());
            base.OutgoingMessageExists(config.AdminNumber, SmsResponseStrings.Join_AdminBetaReject());
        }


        [TestMethod()]
        public void ActionTest()
        {
            Join target = new Join(); // TODO: Initialize to an appropriate value
            Assert.AreEqual("join", target.Action);
        }
    }
}
