using BenchmarkDotNet.Attributes;
using Collections.Pooled;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ValueBuffer.Benchmarks.Actions
{
    [MemoryDiagnoser]
    [MemoryRandomization]
    [AllStatisticsColumn]
    public class AddValueType
    {
        [Params(45,1234,876542)]
        public int Count { get; set; }

        [Benchmark(Baseline =true)]
        public void List()
        {
            var list = new List<int>();
            for (int i = 0; i < Count; i++)
            {
                list.Add(i);
            }
        }
        [Benchmark]
        public void ListCapacity()
        {
            var list = new List<int>(Count);
            for (int i = 0; i < Count; i++)
            {
                list.Add(i);
            }
        }
        [Benchmark]
        public void ValueList()
        {
            using (var list = new ValueList<int>())
            {
                for (int i = 0; i < Count; i++)
                {
                    list.Add(i);
                }
                list.Dispose();
            }
        }
        [Benchmark]
        public void ValueListCapacity()
        {
            using (var list = new ValueList<int>(Count))
            {
                for (int i = 0; i < Count; i++)
                {
                    list.Add(i);
                }
                list.Dispose();
            }
        }
        [Benchmark]
        public void PoolList()
        {
            using (var list = new PooledList<int>())
            {
                for (int i = 0; i < Count; i++)
                {
                    list.Add(i);
                }
                list.Dispose();
            }
        }
        [Benchmark]
        public void PoolListCapacity()
        {
            using (var list = new PooledList<int>(Count))
            {
                for (int i = 0; i < Count; i++)
                {
                    list.Add(i);
                }
                list.Dispose();
            }
        }
    }
}
