// 
// Author: Seth McCarthy
// Copyright (c) 2012
// 
// Licensed under the Microsoft Public License (Ms-PL).
// See the file LICENSE.txt for details.
// 

namespace ThreadUnit.Exceptions
{
    public class ThreadTestTimeoutException : ThreadUnitException
    {
        private readonly int timeouts_;

        private ThreadTestTimeoutException(int timeouts)
        {
            timeouts_ = timeouts;
        }

        internal static ThreadTestTimeoutException GetInstance(int numberOfTimeouts)
        {
            return new ThreadTestTimeoutException(numberOfTimeouts);
        }

        public int Timeouts { get { return timeouts_; } }
    }
}