// 
// Author: Seth McCarthy
// Copyright (c) 2012
// 
// Licensed under the Microsoft Public License (Ms-PL).
// See the file LICENSE.txt for details.
// 

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
        [Timeout(10000)]
        public void InfiniteThreadsDoNotWaitForever()
        {
            const int numThreads = 3;
            try
            {
                ThreadTest.SimultaneousThreads(() => { counter_.Touch();  Thread.Sleep(TimeSpan.FromSeconds(30)); }, numThreads, TimeSpan.FromSeconds(1));
                Assert.Fail();
            }
            catch(ThreadTestTimeoutException ex)
            {
                Assert.That(ex.Timeouts, Is.EqualTo(numThreads));
            }
            Assert.That(counter_.GetHits(), Is.EqualTo(numThreads));
        }
        
        [Test]
        public void MultipleActionsInRoundRobinOrder()
        {
            var hitQueue = new Queue<string>();
            Action first = ()=>hitQueue.Enqueue("first");
            Action second = ()=>{Thread.Sleep(TimeSpan.FromMilliseconds(1000)); hitQueue.Enqueue("second");};
            Action third = ()=>{Thread.Sleep(TimeSpan.FromMilliseconds(2000)); hitQueue.Enqueue("third");};
            
            ThreadTest.SimultaneousThreads(new []{first, second, third}, 3);
            
            Assert.That(hitQueue.Dequeue(), Is.EqualTo("first"));
            Assert.That(hitQueue.Dequeue(), Is.EqualTo("second"));
            Assert.That(hitQueue.Dequeue(), Is.EqualTo("third"));
            Assert.That(hitQueue.Count, Is.EqualTo(0));
        }
        
        [Test]
        public void MultipleActionsRunCorrectNumberOfThreads_LessThreadsThanActions()
        {
            Action a = ()=>counter_.Touch();
            Action b = ()=>counter_.Touch();
            Action noCalls = ()=>{throw new Exception();};
            
            ThreadTest.SimultaneousThreads(new []{a, b, noCalls}, 2);
            
            Assert.That(counter_.GetHits(), Is.EqualTo(2));
        }
        
        [Test]
        public void MultipleActionsRunCorrectNumberOfThreads_MoreThreadsThanActions()
        {
            var calledOnce = false;
            
            Action a = ()=>counter_.Touch();
            Action b = ()=>counter_.Touch();
            Action oneCall = ()=>
                                {
                                    if(calledOnce)
                                    {
                                        throw new Exception();
                                    }
                                    calledOnce = true; 
                                    counter_.Touch();
                                };
            
            ThreadTest.SimultaneousThreads(new []{a, b, oneCall}, 5);
            
            Assert.That(counter_.GetHits(), Is.EqualTo(5));
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
        }

        internal int GetHits()
        {
            return hits_;
        }
    }
}
