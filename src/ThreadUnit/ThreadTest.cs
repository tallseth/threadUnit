using System;
using System.Linq;
using System.Collections.Generic;

namespace ThreadUnit
{
    public static class ThreadTest
    {
        public static void SimultaneousThreads(Action toTest, int numberOfThreads)
        {
            SimultaneousThreads(toTest, numberOfThreads, TimeSpan.FromHours(1));
        }


        public static void SimultaneousThreads(Action toTest, int numberOfThreads, TimeSpan threadTimeout)
        {
            TestRunner.GetInstance().RunTest(SimultaneousThreadTest.GetInstance(toTest, numberOfThreads, threadTimeout));
        }
    }
}
