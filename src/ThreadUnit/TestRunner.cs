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
                throw ThreadTestTimeoutException.GetInstance();

            if (result.Failed)
                throw ThreadTestFailureException.GetInstance(result.Exceptions);
        }
    }
}