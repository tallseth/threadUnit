namespace ThreadUnit.Exceptions
{
    public class ThreadTestTimeoutException : ThreadUnitException
    {
        private ThreadTestTimeoutException(){}

        internal static ThreadTestTimeoutException GetInstance()
        {
            return new ThreadTestTimeoutException();
        }
    }
}