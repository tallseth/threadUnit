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

        public static void CrossThreadInteractions(IEnumerable<Action> actions, int numberOfThreads, ActionSortStrategy order)
        {

        }
    }

    public static class RunOrder
    {
        public static ActionSortStrategy AsGiven()
        {
            return new TrivialSort();
        }

        public static ActionSortStrategy Random()
        {
            return new RandomSort();
        }

        public static ActionSortStrategy Lumpy()
        {
            return new LumpySort();
        }

        private class LumpySort : ActionSortStrategy
        {
            internal override IEnumerable<Action> Sort(IEnumerable<Action> toSort, int fill)
            {
                var source = toSort.ToArray();
                var chunkSize = source.Length / fill;
                var returnCount = 0;
                foreach (var action in source)
                {
                    for (int i = 0; i < chunkSize; i++)
                    {
                        returnCount++;
                        yield return action;
                    }
                }

                //TODO: fill the remainder
            }
        }

        private class RandomSort : ActionSortStrategy
        {
            private Random random_ = new Random();

            internal override IEnumerable<Action> Sort(IEnumerable<Action> toSort, int fill)
            {
                var source = toSort.ToArray();
                for (int i = 0; i < fill; i++)
                {
                    yield return source[random_.Next(source.Length)];
                }
            }
        }

        private class TrivialSort : ActionSortStrategy
        {
            internal override IEnumerable<Action> Sort(IEnumerable<Action> toSort, int fill)
            {
                var enumerator = toSort.GetEnumerator();
                for (int i = 0; i < fill; i++)
                {
                    if (!enumerator.MoveNext())
                    {
                        enumerator.Reset();
                        enumerator.MoveNext();
                    }
                    yield return enumerator.Current;
                }
            }
        }
    }

    public abstract class ActionSortStrategy
    {
        internal ActionSortStrategy(){}

        internal abstract IEnumerable<Action> Sort(IEnumerable<Action> toSort, int fill);
    }
}
