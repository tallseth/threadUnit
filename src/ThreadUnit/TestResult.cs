// 
// Author: Seth McCarthy
// Copyright (c) 2012
// 
// Licensed under the Microsoft Public License (Ms-PL).
// See the file LICENSE.txt for details.
// 

using System;
using System.Collections.Generic;

namespace ThreadUnit
{
    internal sealed class TestResult
    {
        public TestResult()
        {
            Exceptions = new List<Exception>();
        }

        internal List<Exception> Exceptions { get; set; }

        internal bool Failed { get { return Exceptions.Count != 0; } }

        public int Timeouts { get; set; }
    }
}