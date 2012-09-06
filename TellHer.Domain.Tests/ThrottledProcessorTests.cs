using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;
using StructureMap;


namespace TellHer.Domain.Tests
{
    [TestClass]
    public class ThrottledProcessorTests
    {
        [TestInitialize]
        public void Initialize()
        {
            ObjectFactory.Initialize(x =>
            {
                x.For<ILogging>().Use<TraceLogger>();
            });
        }

        [TestMethod]
        public void TimerFires()
        {
            Dictionary<int, DateTime> calls = new Dictionary<int, DateTime>();
            List<int> order = new List<int> { 1, 2, 3, 4, 5 };

            ThrottledProcessor<int> queue = new ThrottledProcessor<int>(
                100, 
                (int value) => calls.Add(value, DateTime.UtcNow),
                (int value) => Assert.IsTrue(calls.ContainsKey(value)),
                (int value, Exception ex) => Assert.Fail("Failed during queue callback"));

            foreach (int i in order)
            {
                queue.Add(i);
            }

            while (queue.Length > 0)
            {
                Thread.Sleep(100);
            }

            DateTime last = DateTime.MinValue;
            foreach (int expected in order)
            {
                Assert.IsTrue(calls.ContainsKey(expected));
                Assert.IsTrue(last < calls[expected]);
                last = calls[expected];
            }
        }

        [TestMethod]
        public void FailedCallbackFires()
        {
            List<int> order = new List<int> { 1, 2, 3, 4, 5 };
            List<int> errors = new List<int>();

            ThrottledProcessor<int> queue = new ThrottledProcessor<int>(
                100,
                (int value) => { throw new InvalidOperationException("This is the message"); },
                (int value) => Assert.Fail("The success path should never be called"),
                (int value, Exception ex) => 
                    {
                        Assert.AreEqual("This is the message", ex.Message);
                        errors.Add(value);
                    });

            foreach (int i in order)
            {
                queue.Add(i);
            }

            while (queue.Length > 0)
            {
                Thread.Sleep(100);
            }

            foreach (int expected in order)
            {
                int actual = errors.First();
                errors.RemoveAt(0);

                Assert.AreEqual(expected, actual, "The items were failed in the wrong order");
            }
        }
    }
}
