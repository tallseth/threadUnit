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
        [Test]
        public void ThreadTestFailsForUnsafeOperations()
        {
            try
            {
                var lockObject = new object();
                ThreadTest.SimultaneousThreads(() => ThreadUnsafeOperation(lockObject), 1000);
                Assert.Fail();
            }
            catch (ThreadTestFailureException)
            {
                
            }
        }

        [Test]
        public void ThreadTestPassesOnSafeOperations()
        {
            int i = 0;
            ThreadTest.SimultaneousThreads(() => i++, 10);
        }

        [Test]
        [Timeout(3000)]
        public void InfiniteThreadsDoNotWaitForever()
        {
            try
            {
                ThreadTest.SimultaneousThreads(()=>Thread.Sleep(TimeSpan.FromSeconds(5)), 2, TimeSpan.FromSeconds(1));
                Assert.Fail();
            }
            catch(ThreadTestTimeoutException)
            {
                
            }
        }

        private void ThreadUnsafeOperation(object toBreak)
        {
            var entered = Monitor.TryEnter(toBreak, 5);
            try
            {
                if(entered)
                    Thread.Sleep(10);
                else
                    throw new Exception("Failed to enter monitor");
            }
            finally
            {
                Monitor.Exit(toBreak);
            }
        }
    }
}
