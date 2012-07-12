using System.Collections.Generic;
using System;
using System.Linq;
using System.Text;

namespace ThreadUnit.Exceptions
{
    /// <summary>
    /// Indicates a ThreadTest failure
    /// </summary>
    public class ThreadTestFailureException : ThreadUnitException
    {
        private readonly Exception[] errors_;

        private ThreadTestFailureException(IEnumerable<Exception> errors)
        {
            errors_ = errors.ToArray();
        }

        internal static ThreadTestFailureException GetInstance(IEnumerable<Exception> errors)
        {
            return new ThreadTestFailureException(errors);
        }

        public Exception[] Errors { get { return errors_.ToArray(); } }

        public override string ToString()
        {
            var output = new StringBuilder("Exceptions occurred in other threads: " + errors_.Length);

            foreach (var ex in errors_)
            {
                output.AppendLine("==============================");
                output.AppendLine(ex.ToString());
            }

            return output.ToString();
        }
    }
}