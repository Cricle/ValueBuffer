using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValueBuffer.Benchmarks.Actions
{
    [MemoryDiagnoser]
    public class ForeachValueType
    {
        private List<int> values1;
        private ValueList<int> values2;

        [Params(100,100_000)]
        public int Count { get; set; }

        [GlobalSetup]
        public void Setup()
        {
            values1 = new List<int>();
            values2 = new ValueList<int>();
            for (int i = 0; i < Count; i++)
            {
                values1.Add(i);
                values2.Add(i);
            }
        }
        [Benchmark(Baseline =true)]
        public void ByList()
        {
            foreach (var item in values1)
            {
            }
        }
        [Benchmark]
        public void ByValueList()
        {
            foreach (var item in values2)
            {
            }
        }
    }
}
