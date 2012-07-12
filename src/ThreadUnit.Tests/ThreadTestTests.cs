using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using NUnit.Framework;
using ThreadUnit.Exceptions;

namespace ThreadUnit.Tests
{
    [TestFixture]
    public class ThreadTestTests
    {
        private HitCounter counter_;

        [SetUp]
        public void Setup()
        {
            counter_ = new HitCounter();
        }

        [Test]
        public void ThreadTestFailsForUnsafeOperations()
        {
            var numberOfThreads = 10;
            try
            {
                var lockObject = new object();
                ThreadTest.SimultaneousThreads(() => { counter_.Touch(); ThreadUnsafeOperation(lockObject); }, numberOfThreads);
                Assert.Fail();
            }
            catch (ThreadTestFailureException)
            {
                
            }

            Assert.That(counter_.GetHits(), Is.EqualTo(numberOfThreads));
        }

        [Test]
        public void ThreadTestPassesOnSafeOperations()
        {
            ThreadTest.SimultaneousThreads(() => counter_.Touch(), 10);
            Assert.That(counter_.GetHits(), Is.EqualTo(10));
        }

        [Test]
        public void TestFailsIfActionFails()
        {
            try
            {
                ThreadTest.SimultaneousThreads(() => { throw new Exception(); }, 2);
                Assert.Fail();
            }
            catch (ThreadTestFailureException ex)
            {
                Assert.That(ex.Errors.Length, Is.EqualTo(2));
            }

        }

        [Test]
        [Timeout(3000)]
        public void InfiniteThreadsDoNotWaitForever()
        {
            try
            {
                ThreadTest.SimultaneousThreads(() => { counter_.Touch();  Thread.Sleep(TimeSpan.FromSeconds(5)); }, 2, TimeSpan.FromSeconds(1));
                Assert.Fail();
            }
            catch(ThreadTestTimeoutException)
            {
                
            }
            Assert.That(counter_.GetHits(), Is.EqualTo(2));
        }

        private void ThreadUnsafeOperation(object toLock)
        {
            var entered = Monitor.TryEnter(toLock, 5);
            try
            {
                if(entered)
                    Thread.Sleep(10);
                else
                    throw new Exception("Failed to enter monitor");
            }
            finally
            {
                Monitor.Exit(toLock);
            }
        }
    }

    internal class HitCounter
    {
        private int hits_ = 0;

        internal void Touch()
        {
            hits_++;
            Console.WriteLine("touched " + hits_);
        }

        internal int GetHits()
        {
            return hits_;
        }
    }
}
