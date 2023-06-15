using BenchmarkDotNet.Attributes;
using Collections.Pooled;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValueBuffer.Benchmarks.Actions
{
    [MemoryDiagnoser]
    public class ToArray
    {
        [Params(100,1_222_221)]
        public int Count { get; set; }

        private IEnumerable<int> GetItems() 
        {
            for (int i = 0; i < Count; i++)
            {
                yield return i;
            }
        }

        [Benchmark(Baseline =true)]
        public void FromEnumerable()
        {
            GetItems().ToArray();
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
                _ = vl.ToArray();
            }
        }
        [Benchmark]
        public void FromCollectionPool()
        {
            using (var lst = GetItems().ToPooledList())
            {
                lst.ToArray();
            }
        }
    }
}
