// 
// Author: Seth McCarthy
// Copyright (c) 2012
// 
// Licensed under the Microsoft Public License (Ms-PL).
// See the file LICENSE.txt for details.
// 

using ThreadUnit.Exceptions;

namespace ThreadUnit
{
    internal class TestRunner
    {
        private TestRunner()
        {
        }

        internal static TestRunner GetInstance()
        {
            return new TestRunner();
        }

        internal void RunTest(Test toRun)
        {
            var result = toRun.Execute();

            if (result.Timeouts > 0)
                throw ThreadTestTimeoutException.GetInstance(result.Timeouts);

            if (result.Failed)
                throw ThreadTestFailureException.GetInstance(result.Exceptions);
        }
    }
}