// 
// Author: Seth McCarthy
// Copyright (c) 2012
// 
// Licensed under the Microsoft Public License (Ms-PL).
// See the file LICENSE.txt for details.
// 

using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;

namespace ThreadUnit
{
    internal class SimultaneousThreadTest : Test
    {
        private readonly IEnumerable<Action> toTest_;
        private readonly int numberOfThreads_;
        private readonly TimeSpan joinTimeout_;
        private readonly object threadAbortState_ = new object();

        private SimultaneousThreadTest(IEnumerable<Action> toTest, int numberOfThreads, TimeSpan joinTimeout)
        {
            toTest_ = toTest;
            numberOfThreads_ = numberOfThreads;
            joinTimeout_ = joinTimeout;
        }
        
        internal static Test GetInstance(Action toTest, int numberOfThreads, TimeSpan joinTimeout)
        {
            return GetInstance(new []{toTest}, numberOfThreads, joinTimeout);
        }

        internal static Test GetInstance(IEnumerable<Action> toTest, int numberOfThreads, TimeSpan joinTimeout)
        {
            return new SimultaneousThreadTest(toTest, numberOfThreads, joinTimeout);
        }

        internal override TestResult Execute()
        {
            var result = new TestResult();
            var threads = new List<Thread>();
            
            var threadCounter = 0;
            
            while(threadCounter < numberOfThreads_)
            {
                foreach (var action in toTest_) 
                {
                    var unmodifiedClosure = action;
                    threads.Add(new Thread(()=>RunTest(result.Exceptions, unmodifiedClosure)));
                    threadCounter++;
                    if(threadCounter >= numberOfThreads_)
                    {
                        break;
                    }
                }
                
            } 
            
            threads.ForEach(t=>t.Start());
            threads.ForEach(t=>ForceJoin(t, result));

            return result;
        }

        private void ForceJoin(Thread toJoin, TestResult result)
        {
            if(!toJoin.Join(joinTimeout_))
            {
                toJoin.Abort(threadAbortState_);
                result.Timeouts++;
            }
                
        }

        private void RunTest(ICollection<Exception> exceptions, Action testAction)
        {
            try
            {
                testAction();
            }
            catch(ThreadAbortException)
            {
                //TODO: should test the thread abort state before ignore this exception
                Thread.ResetAbort();
            }
            catch(Exception ex)
            {
                exceptions.Add(ex);
            }
        }
    }
}