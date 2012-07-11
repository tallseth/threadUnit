namespace ThreadUnit.Exceptions
{
    /// <summary>
    /// Indicates a ThreadTest failure
    /// </summary>
    public class ThreadTestFailureException : ThreadUnitException
    {
        private ThreadTestFailureException()
        {
        }

        internal static ThreadTestFailureException GetInstance()
        {
            return new ThreadTestFailureException();
        }
    }
}