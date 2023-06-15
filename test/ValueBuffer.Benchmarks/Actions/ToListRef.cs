using BenchmarkDotNet.Attributes;
using Collections.Pooled;
using System.Collections.Generic;
using System.Linq;

namespace ValueBuffer.Benchmarks.Actions
{
    [MemoryDiagnoser]
    public class ToListRef
    {
        [Params(100, 1_222_221)]
        public int Count { get; set; }

        private object obj = new object();

        private IEnumerable<object> GetItems()
        {
            for (int i = 0; i < Count; i++)
            {
                yield return obj;
            }
        }

        [Benchmark(Baseline = true)]
        public void FromEnumerable()
        {
            GetItems().ToList();
        }
        [Benchmark]
        public void FromValueBuffer()
        {
            using (var vl = new ValueList<object>())
            {
                using (var enu = GetItems().GetEnumerator())
                {
                    while (enu.MoveNext())
                    {
                        vl.Add(enu.Current);
                    }
                }
                vl.ToList();
            }
        }
        [Benchmark]
        public void FromCollectionPool()
        {
            using (var lst = GetItems().ToPooledList())
            {
                lst.ToList();
            }
        }
    }
}
