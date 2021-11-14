using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace ValueBuffer.Benchmarks.Actions
{
    [MemoryDiagnoser]
    [MemoryRandomization]
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
            foreach (var item in list)
            {

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
                var enu = list.GetEnumerator();
                while (enu.MoveNext())
                {
                    _ = enu.Current;
                }
                list.Dispose();
            }
        }
    }
}
