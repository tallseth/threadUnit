// 
// Author: Seth McCarthy
// Copyright (c) 2012
// 
// Licensed under the Microsoft Public License (Ms-PL).
// See the file LICENSE.txt for details.
// 

using System;
using System.Linq;
using System.Collections.Generic;

namespace ThreadUnit
{
    public static class ThreadTest
    {
        private static readonly TimeSpan defaultTimeout_ = TimeSpan.FromHours(1);
        
        public static void SimultaneousThreads(Action toTest, int numberOfThreads)
        {
            SimultaneousThreads(toTest, numberOfThreads, defaultTimeout_);
        }

        public static void SimultaneousThreads(Action toTest, int numberOfThreads, TimeSpan threadTimeout)
        {
            RunTest(SimultaneousThreadTest.GetInstance(toTest, numberOfThreads, threadTimeout));
        }
        
        public static void SimultaneousThreads(IEnumerable<Action> actions, int numberOfThreads)
        {
            RunTest(SimultaneousThreadTest.GetInstance(actions, numberOfThreads, defaultTimeout_));
        }
        
        private static void RunTest(Test toRun)
        {
            TestRunner.GetInstance().RunTest(toRun);
        }
    }
}
