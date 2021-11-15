using BenchmarkDotNet.Attributes;
using System.Collections.Generic;
using System.Text;

namespace ValueBuffer.Benchmarks.Actions
{
    [MemoryDiagnoser]
    [MemoryRandomization]
    public class AddObject
    {
        [Params(45, 1234, 876542)]
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
            foreach (var item in list)
            {

            }
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
                var enu = list.GetEnumerator();
                while (enu.MoveNext())
                {
                    _ = enu.Current;
                }
                list.Dispose();
            }
        }
    }
    [MemoryDiagnoser]
    [MemoryRandomization]
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
