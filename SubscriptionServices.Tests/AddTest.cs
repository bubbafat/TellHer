using TellHer.SubscriptionService.Actions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using TellHer.Domain;
using StructureMap;
using TellHer.Data;
using System.Linq;
using System.Globalization;

namespace SubscriptionServices.Tests
{

    [TestClass()]
    public class AddTest : ActionTestBase
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
        public void Perform_RealAdmin_Help()
        {
            string destination = ObjectFactory.GetInstance<IConfiguration>().AdminNumber;
            Add_Accessor target = new Add_Accessor(); // TODO: Initialize to an appropriate value
            IncomingSmsMessage message = new IncomingSmsMessage
            {
                From = destination,
                Message = "add",
            };

            target.Perform(message);

            IDataStore store = ObjectFactory.GetInstance<IDataStore>();

            Assert.AreEqual(0, store.DailyIdeas.Count());
            OutgoingMessageExists(destination, SmsResponseStrings.Add_Help());
        }

        [TestMethod()]
        [DeploymentItem("TellHer.SubscriptionService.dll")]
        public void ActionCorrect()
        {
            Add_Accessor target = new Add_Accessor();
            Assert.AreEqual("add", target.Action);
        }

        [TestMethod()]
        [DeploymentItem("TellHer.SubscriptionService.dll")]
        public void Perform_RealAdmin()
        {
            Add_Accessor target = new Add_Accessor(); // TODO: Initialize to an appropriate value
            IncomingSmsMessage message = new IncomingSmsMessage
            {
                From = ObjectFactory.GetInstance<IConfiguration>().AdminNumber,
                Message = "add This is the added message",
            };

            target.Perform(message);

            IDataStore store = ObjectFactory.GetInstance<IDataStore>();

            DailyIdea addedIdea = store.DailyIdeas.Where(i => i.Idea == "This is the added message").First();
            OutgoingSmsMessage response = store.OutgoingMessages.Where(o => o.Message ==  SmsResponseStrings.Add_Success_AddedNewIdea(addedIdea.Id)).First();
            Assert.AreEqual(ObjectFactory.GetInstance<IConfiguration>().AdminNumber, response.Destination);
        }

        [TestMethod()]
        [DeploymentItem("TellHer.SubscriptionService.dll")]
        public void Perform_RealAdmin_ExistingIdea()
        {
            Add_Accessor target = new Add_Accessor(); // TODO: Initialize to an appropriate value

            IDataStore store = ObjectFactory.GetInstance<IDataStore>();

            DailyIdea idea = new DailyIdea
            {
                 Idea = "This is the added message",                  
            };

            store.Save(idea);

            Assert.AreNotEqual(0, idea.Id);

            IncomingSmsMessage message = new IncomingSmsMessage
            {
                From = ObjectFactory.GetInstance<IConfiguration>().AdminNumber,
                Message = string.Format(CultureInfo.InvariantCulture, "add {0}", idea.Idea),
            };

            target.Perform(message);

            Assert.AreEqual(1, store.DailyIdeas.Count(i => i.Idea == idea.Idea));
            OutgoingSmsMessage response = store.OutgoingMessages.Where(o => o.Message == SmsResponseStrings.Add_Failed_ExistingIdea(idea.Id)).First();
            Assert.AreEqual(ObjectFactory.GetInstance<IConfiguration>().AdminNumber, response.Destination);
        }


        [TestMethod()]
        [DeploymentItem("TellHer.SubscriptionService.dll")]
        public void Perform_UnknownUser()
        {
            string destinationNumber = "+11112223333";

            Add_Accessor target = new Add_Accessor(); // TODO: Initialize to an appropriate value
            IncomingSmsMessage message = new IncomingSmsMessage
            {
                From = destinationNumber,
                Message = "add This is the added message",
            };

            target.Perform(message);

            IDataStore store = ObjectFactory.GetInstance<IDataStore>();

            Assert.IsFalse(store.DailyIdeas.Any());
            OutgoingSmsMessage response = store.OutgoingMessages.Where(o => o.Message == SmsResponseStrings.PublicHelp()).First();
            Assert.AreEqual(destinationNumber, response.Destination);
        }

        [TestMethod()]
        [DeploymentItem("TellHer.SubscriptionService.dll")]
        public void Perform_Subscriber()
        {
            IDataStore store = ObjectFactory.GetInstance<IDataStore>();

            Add_Accessor target = new Add_Accessor(); // TODO: Initialize to an appropriate value
            IncomingSmsMessage message = new IncomingSmsMessage
            {
                From = ExistingSubscriberPhone,
                Message = "add This is the added message",
            };

            target.Perform(message);

            Assert.IsFalse(store.DailyIdeas.Any());
            OutgoingMessageExists(ExistingSubscriberPhone, SmsResponseStrings.PublicHelp());
        }
    }
}
