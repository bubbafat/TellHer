using TellHer.SubscriptionService.Actions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using TellHer.Domain;
using StructureMap;
using TellHer.Data;
using System.Linq;

namespace SubscriptionServices.Tests
{


    [TestClass()]
    public class QuitTest : ActionTestBase
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
        public void ActionTest()
        {
            Quit_Accessor target = new Quit_Accessor(); // TODO: Initialize to an appropriate value
            Assert.AreEqual("quit", target.Action);
        }

        [TestMethod()]
        [DeploymentItem("TellHer.SubscriptionService.dll")]
        public void Perform_MissingUser()
        {
            Quit_Accessor target = new Quit_Accessor(); // TODO: Initialize to an appropriate value
            
            IncomingSmsMessage message = new IncomingSmsMessage
            {
                From = "+11112223333",
                Message = target.Action,
            };

            target.Perform(message);


            IDataStore store = ObjectFactory.GetInstance<IDataStore>();

            OutgoingMessageExists(message.From, SmsResponseStrings.Quit_AllRemoved(message.From));
        }

        [TestMethod()]
        [DeploymentItem("TellHer.SubscriptionService.dll")]
        public void Perform_ExistingUser()
        {
            Quit_Accessor target = new Quit_Accessor(); // TODO: Initialize to an appropriate value

            IncomingSmsMessage message = new IncomingSmsMessage
            {
                From = ExistingSubscriberPhone,
                Message = target.Action,
            };

            Assert.AreEqual(1, base.SubscriptionForNumber(message.From).Count);

            target.Perform(message);

            OutgoingMessageExists(base.ExistingSubscriberPhone, SmsResponseStrings.Quit_AllRemoved(base.ExistingSubscriberPhone));
            Assert.AreEqual(0, base.SubscriptionForNumber(message.From).Count);
        }

        [TestMethod()]
        [DeploymentItem("TellHer.SubscriptionService.dll")]
        public void Perform_ExistingUser_QueuedMessagesRemoved()
        {
            Quit_Accessor target = new Quit_Accessor(); // TODO: Initialize to an appropriate value

            IDataStore store = ObjectFactory.GetInstance<IDataStore>();

            // add an existing outgoing message - this should be removed when the quit is done
            store.Save(new OutgoingSmsMessage
            {
                Destination = ExistingSubscriberPhone,
                Message = "Does not matter",
            });

            // add an existing outgoing message - this should be removed when the quit is done
            store.Save(new OutgoingSmsMessage
            {
                Destination = ExistingSubscriberPhone,
                Message = "Also does not matter",
            });

            Assert.AreEqual(2, store.OutgoingMessages.Count());

            IncomingSmsMessage message = new IncomingSmsMessage
            {
                From = ExistingSubscriberPhone,
                Message = target.Action,
            };

            Assert.AreEqual(1, base.SubscriptionForNumber(message.From).Count);

            target.Perform(message);

            // there should be only one message in the outgoing queue
            Assert.AreEqual(1, store.OutgoingMessages.Count());

            // and it should be this one...
            OutgoingMessageExists(base.ExistingSubscriberPhone, SmsResponseStrings.Quit_AllRemoved(base.ExistingSubscriberPhone));
            
            Assert.AreEqual(0, store.Subscriptions.Count());
        }
    }
}
