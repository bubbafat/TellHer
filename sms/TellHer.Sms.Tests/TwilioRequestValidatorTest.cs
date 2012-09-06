using TellHer.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Specialized;
using StructureMap;
using TellHer.Test.Common;

namespace TellHer.Sms.Tests
{
    
    
    /// <summary>
    ///This is a test class for TwilioRequestValidatorTest and is intended
    ///to contain all TwilioRequestValidatorTest Unit Tests
    ///</summary>
    [TestClass()]
    public class TwilioRequestValidatorTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
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
        [TestInitialize()]
        public void MyTestInitialize()
        {
            ObjectFactory.Initialize(x =>
            {
                x.For<ILogging>().Use<TraceLogger>();
                x.For<IConfiguration>().Use<MockConfig>();
            });
        }
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for BuildInputString
        ///</summary>
        [TestMethod()]
        [DeploymentItem("TellHer.Domain.dll")]
        public void BuildInputStringTest()
        {
            string uriAndPath = "https://mycompany.com/myapp.php?foo=1&bar=2";

            NameValueCollection post = new NameValueCollection();

            post["Digits"] = "1234";
            post["To"] = "+18005551212";
            post["From"] = "+14158675309";
            post["Caller"] = "+14158675309";
            post["CallSid"] = "CA1234567890ABCDE";

            string expected = "https://mycompany.com/myapp.php?foo=1&bar=2CallSidCA1234567890ABCDECaller+14158675309Digits1234From+14158675309To+18005551212";
            string actual = TwilioRequestValidator_Accessor.BuildInputString(uriAndPath, post);

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for IsValid
        ///</summary>
        [TestMethod()]
        public void IsValidTest()
        {
            TwilioRequestValidator target = new TwilioRequestValidator(); // TODO: Initialize to an appropriate value
            string uri = "https://mycompany.com/myapp.php?foo=1&bar=2";
            NameValueCollection post = new NameValueCollection();

            post["Digits"] = "1234";
            post["To"] = "+18005551212";
            post["From"] = "+14158675309";
            post["Caller"] = "+14158675309";
            post["CallSid"] = "CA1234567890ABCDE";

            bool expected = true;
            bool actual = target.IsValid(uri, "RSOYDt4T1cUTdK1PDd93/VVr8B8=", post);

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for Sign
        ///</summary>
        [TestMethod()]
        [DeploymentItem("TellHer.Domain.dll")]
        public void SignTest()
        {
            TwilioRequestValidator_Accessor target = new TwilioRequestValidator_Accessor();
            string data = "https://mycompany.com/myapp.php?foo=1&bar=2CallSidCA1234567890ABCDECaller+14158675309Digits1234From+14158675309To+18005551212";
            string expected = "RSOYDt4T1cUTdK1PDd93/VVr8B8=";
            string actual = target.Sign(data);

            Assert.AreEqual(expected, actual);
        }
    }
}
