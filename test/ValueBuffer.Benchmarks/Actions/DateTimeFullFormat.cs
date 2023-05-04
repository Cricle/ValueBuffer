using BenchmarkDotNet.Attributes;
using System;

namespace ValueBuffer.Benchmarks.Actions
{
#if NET7_0_OR_GREATER
    [MemoryDiagnoser]
    [RankColumn]
    public class DateTimeFullFormat
    {
        [Benchmark(Baseline = true)]
        public void TryFormat()
        {
            Span<char> span = stackalloc char[19];
            DateTime.Now.TryFormat(span, out _,"yyyy-MM-dd HH-mm:ss");
        }
        [Benchmark]
        public void ToFullString()
        {
            Span<char> span = stackalloc char[19];
            var now = DateTime.Now;
            DateTimeToStringHelper.ToFullString(ref now, ref span);
        }
    }
#endif
}
