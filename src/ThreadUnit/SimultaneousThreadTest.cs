using System;
using System.Collections.Generic;
using System.Threading;

namespace ThreadUnit
{
    internal class SimultaneousThreadTest : Test
    {
        private readonly Action toTest_;
        private readonly int numberOfThreads_;
        private readonly TimeSpan joinTimeout_;
        private readonly object threadAbortState_ = new object();

        private SimultaneousThreadTest(Action toTest, int numberOfThreads, TimeSpan joinTimeout)
        {
            toTest_ = toTest;
            numberOfThreads_ = numberOfThreads;
            joinTimeout_ = joinTimeout;
        }

        internal static Test GetInstance(Action toTest, int numberOfThreads, TimeSpan joinTimeout)
        {
            return new SimultaneousThreadTest(toTest, numberOfThreads, joinTimeout);
        }

        internal override TestResult Execute()
        {
            var result = new TestResult();
            var threads = new List<Thread>();
            for (int i = 0; i < numberOfThreads_; i++)
            {
                threads.Add(new Thread(()=>RunTest(result.Exceptions)));
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

        private void RunTest(ICollection<Exception> exceptions)
        {
            try
            {
                toTest_();
            }
            catch(ThreadAbortException)
            {
                Thread.ResetAbort();
            }
            catch(Exception ex)
            {
                exceptions.Add(ex);
            }
        }
    }
}