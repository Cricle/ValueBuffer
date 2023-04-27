using System;
using ValueBuffer.Benchmarks.Actions;

namespace ValueBuffer.Benchmarks
{
    class Program
    {
        static void Main(string[] args)
        {
            new MemoryJsonWriter { Count = 10, Size = 1_000_000 }.ValueListBuffer();
            //BenchmarkDotNet.Running.BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
        }
    }
}
