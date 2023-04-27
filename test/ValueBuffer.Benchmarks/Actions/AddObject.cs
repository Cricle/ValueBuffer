using BenchmarkDotNet.Attributes;
using Collections.Pooled;
using System.Collections.Generic;
using System.Text;

namespace ValueBuffer.Benchmarks.Actions
{
    [MemoryDiagnoser]
    [MemoryRandomization]
    [AllStatisticsColumn]
    public class AddObject
    {
        [Params(1234, 876542)]
        public int Count { get; set; }

        [Benchmark(Baseline = true)]
        public void List()
        {
            var obj = new object();
            var list = new List<object>();
            for (int i = 0; i < Count; i++)
            {
                list.Add(obj);
            }
            list.ToArray();
        }
        [Benchmark]
        public void ListCapacity()
        {
            var obj = new object();
            var list = new List<object>(Count);
            for (int i = 0; i < Count; i++)
            {
                list.Add(obj);
            }
            list.ToArray();
        }
        [Benchmark]
        public void ValueList()
        {
            var obj = new object();
            using (var list = new ValueList<object>())
            {
                for (int i = 0; i < Count; i++)
                {
                    list.Add(obj);
                }
                list.ToArray();
            }
        }
        [Benchmark]
        public void ValueListCapacity()
        {
            var obj = new object();
            using (var list = new ValueList<object>(Count))
            {
                for (int i = 0; i < Count; i++)
                {
                    list.Add(obj);
                }
                list.ToArray();
            }
        }
        [Benchmark]
        public void PoolList()
        {
            var obj = new object();
            using (var list = new PooledList<object>())
            {
                for (int i = 0; i < Count; i++)
                {
                    list.Add(obj);
                }
                list.ToArray();
            }
        }
        [Benchmark]
        public void PoolListCapacity()
        {
            var obj = new object();
            using (var list = new PooledList<object>(Count))
            {
                for (int i = 0; i < Count; i++)
                {
                    list.Add(obj);
                }
                list.ToArray();
            }
        }
    }
    [MemoryDiagnoser]
    [MemoryRandomization]
    [AllStatisticsColumn]
    public class StringBuilderObject
    {
        [Params(45, 1234, 876542)]
        public int Count { get; set; }

        [Benchmark(Baseline = true)]
        public void StringBuilder()
        {
            var list = new StringBuilder();
            for (int i = 0; i < Count; i++)
            {
                list.Append("hello world!!!");
            }
            list.ToString();
        }
        [Benchmark]
        public void ValueStringBuilder()
        {
            var list = new ValueStringBuilder();
            for (int i = 0; i < Count; i++)
            {
                list.Append("hello world!!!");
            }
            list.ToString();
            list.Dispose();
        }
        [Benchmark]
        public void ValueZString()
        {
            using (var list = Cysharp.Text.ZString.CreateUtf8StringBuilder())
            {
                for (int i = 0; i < Count; i++)
                {
                    list.Append("hello world!!!");
                }
                list.ToString();
            }
        }
    }
}
