using BenchmarkDotNet.Attributes;
using Collections.Pooled;
using System.Collections.Generic;
using System.Linq;

namespace ValueBuffer.Benchmarks.Actions
{
    [MemoryDiagnoser]
    public class ToList
    {
        [Params(100, 1_222_221)]
        public int Count { get; set; }

        private IEnumerable<int> GetItems()
        {
            for (int i = 0; i < Count; i++)
            {
                yield return i;
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
            using (var vl = new ValueList<int>())
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
            using (var lst= GetItems().ToPooledList())
            {
                lst.ToList();
            }
        }
    }
}
