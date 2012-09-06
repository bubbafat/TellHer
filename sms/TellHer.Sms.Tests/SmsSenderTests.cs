using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TellHer.Data;
using System.Threading;

using TellHer.Domain;
using TellHer.Test.Common;
using System.Globalization;
using StructureMap;

namespace TellHer.Sms.Tests
{
    
    [TestClass]
    public class SmsSenderTests
    {
        [TestMethod]
        public void SenderSends()
        {
            ObjectFactory.Initialize(x =>
            {
                x.For<ILogging>().Use<TraceLogger>();
                x.For<ISmsTransport>().Use<SuccessSmsSender>();
                x.For<IDataStore>().Use<TestDataStore>();
            });

            int toSend = 10;
            int[] count = { 0 };
            ManualResetEvent done = new ManualResetEvent(false);
            bool[] sendFailed = new bool[1] { false };

            SendCompleteCallback complete = (SentSmsMessageLogEntry result, OutgoingSmsMessage msg) =>
                {
                    Assert.AreEqual((int)MessageSendStatus.Success, result.Status);
                    Assert.AreEqual(count[0], msg.Id, "The subscription ID was incorrect");

                    count[0]++;

                    if (count[0] == toSend)
                    {
                        done.Set();
                    }
                };

            SmsSenderExceptionThrownCallback failed = (OutgoingSmsMessage message, Exception exception) =>
                {
                    sendFailed[0] = true;
                    Assert.Fail(string.Format(CultureInfo.InvariantCulture, "Sending message {0} failed?", message.Id));
                };

            SmsSenderQueue sender = new SmsSenderQueue();

            for (int i = 0; i < 10; i++)
            {
                OutgoingSmsMessage msg = new OutgoingSmsMessage
                {
                    Message = "This is the message",
                    Destination = "+11234567890",
                    Id = i,
                };

                sender.Send(msg, complete, failed);
            }

            Assert.IsTrue(done.WaitOne(TimeSpan.FromSeconds(toSend)), "Sending timed out ... that's not good!");
            Assert.IsFalse(sendFailed[0], "The send was marked as failed ... wtf???");
        }

        [TestMethod]
        public void SenderFailsCallback()
        {
            ObjectFactory.Initialize(x =>
            {
                x.For<ILogging>().Use<TraceLogger>();
                x.For<ISmsTransport>().Use<FailureSmsSender>();
                x.For<IDataStore>().Use<TestDataStore>();
            });

            int toSend = 10;
            int[] count = { 0 };
            ManualResetEvent done = new ManualResetEvent(false);
            bool[] sendSucceeded = new[] { false };

            SendCompleteCallback complete = (SentSmsMessageLogEntry result, OutgoingSmsMessage msg) =>
            {
                sendSucceeded[0] = result.Status != (int)MessageSendStatus.Error;
                Assert.AreEqual(count[0], msg.Id, "The subscription ID was incorrect");
                Assert.AreEqual((int)MessageSendStatus.Error, result.Status);

                count[0]++;

                if (count[0] == toSend)
                {
                    done.Set();
                }
            };

            SmsSenderExceptionThrownCallback failed = (OutgoingSmsMessage msg, Exception exception) =>
            {
                Assert.Fail("The exception callback should not have been called");
            };

            SmsSenderQueue sender = new SmsSenderQueue();

            for (int i = 0; i < 10; i++)
            {
                OutgoingSmsMessage msg = new OutgoingSmsMessage
                {
                    Message = "This is the message",
                    Destination = "+11234567890",
                    Id = i,
                };

                sender.Send(msg, complete, failed);
            }

            Assert.IsTrue(done.WaitOne(TimeSpan.FromSeconds(toSend)), "Sending timed out ... that's not good!");
            Assert.IsFalse(sendSucceeded[0], "The send should not have been marked as successful");
        }
    }
}
