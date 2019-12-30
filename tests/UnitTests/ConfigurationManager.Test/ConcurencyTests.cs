using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace ConfigurationManager.Test
{
    public class ConcurencyTests
    {
        ITestOutputHelper output;
        private static int _runCount = 0;
        private static int _changeCount = 0;
        private static readonly ConcurrentDictionary<string, string> _dictionary
            = new ConcurrentDictionary<string, string>();
        public ConcurencyTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        /// <summary>
        /// two concurrent thread testing for value changing for correctly
        /// </summary>
        [Fact]
        public void concurrent_threads_tests()
        {
            PrintValue(output, 0, "The zero value");
            var task1 = Task.Run(() => PrintThread(output, 1, "The first value"));
            var task2 = Task.Run(() => ChangeThread(output, 2, "The second value"));
            var task3 = Task.Run(() => PrintThread(output, 3, "The third value"));
            var task4 = Task.Run(() => ChangeThread(output, 4, "The fourth value"));
            Task.WaitAll(task1, task2, task3, task4);

            PrintValue(output, 7, "The seventh value");
            ChangeValue(output, 8, "The eighth value");

            output.WriteLine($"Run count: {_runCount}");
            output.WriteLine($"Change count: {_changeCount}");
            Assert.True(_runCount == 1);
            Assert.True(_changeCount == 4);
        }

        //[Fact]
        //public void concurrent_threads_tests_for_three_thread()
        //{
        //    PrintValue(output, 0, "The zero value");
        //    var task1 = Task.Run(() => PrintThread(output, 1, "The first value"));
        //    var task2 = Task.Run(() => ChangeThread(output, 2, "The second value"));
        //    var task3 = Task.Run(() => PrintThread(output, 3, "The third value"));
        //    var task4 = Task.Run(() => ChangeThread(output, 4, "The fourth value"));
        //    var task5 = Task.Run(() => PrintThread(output, 5, "The fifth value"));
        //    var task6 = Task.Run(() => ChangeThread(output, 6, "The sixth value"));
        //    Task.WaitAll(task1, task2, task3, task4);

        //    PrintValue(output, 7, "The seventh value");
        //    ChangeValue(output, 8, "The eighth value");
        //    PrintValue(output, 9, "The ninth value");


        //    output.WriteLine($"Run count: {_runCount}");
        //    output.WriteLine($"Change count: {_changeCount}");
        //    Assert.True(_runCount == 1);
        //    Assert.True(_changeCount == 7);
        //}
        private Action<ITestOutputHelper, int, string> PrintThread = (ITestOutputHelper output, int order, string value) =>
        {
            output.WriteLine($"Print Action Started With : {order}-{value}");
            PrintValue(output, order, value);
        };

        private Action<ITestOutputHelper, int, string> ChangeThread = (ITestOutputHelper output, int order, string value) =>
        {
            output.WriteLine($"Change Action Started With : {order}-{value}");
            ChangeValue(output, order, value);
        };

        public static void PrintValue(ITestOutputHelper output, int order, string valueToPrint)
        {
            output.WriteLine($"{order}th print request came ");
            var valueFound = _dictionary.GetOrAdd("key",
                x =>
                {
                    Interlocked.Increment(ref _runCount);
                    output.WriteLine($"{order} - running : {_runCount}");
                    Thread.Sleep(100);
                    return valueToPrint;
                });
            output.WriteLine($"{order} - {valueFound}");
        }

        public static void ChangeValue(ITestOutputHelper output, int order, string valueToAddOrUpdate)
        {
            output.WriteLine($"{order}th change request came ");
            //if (!_dictionary.TryAdd("key", valueToAddOrUpdate))
            //{
            //    if (_dictionary.TryUpdate("key", valueToAddOrUpdate, valueToAddOrUpdate))
            //    {
            //        Interlocked.Increment(ref _changeCount);
            //        output.WriteLine($"{order} - changing : {_changeCount}");
            //    }
            //    else
            //    {
            //        if (_dictionary.TryGetValue("key", out string value))
            //        {
            //            if (_dictionary.TryUpdate("key", valueToAddOrUpdate, value))
            //            {
            //                Interlocked.Increment(ref _changeCount);
            //                output.WriteLine($"{order} - changing : {_changeCount}");
            //            }
            //        }
            //    }
            //}

            var lazyResult = _dictionary.AddOrUpdate("key", valueToAddOrUpdate,
                (test, t) =>
                {
                    Interlocked.Increment(ref _changeCount);
                    output.WriteLine($"{order} - changing : {_changeCount}");
                    Thread.Sleep(100);
                    return valueToAddOrUpdate;
                });
            PrintValue(output, order, valueToAddOrUpdate);
        }
    }
}
