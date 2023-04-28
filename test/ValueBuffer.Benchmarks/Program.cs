using System;
using ValueBuffer.Benchmarks.Actions;

namespace ValueBuffer.Benchmarks
{
    class Program
    {
        static void Main(string[] args)
        {
            //for (int i = 0; i < 2; i++)
            //{
            //    new MemoryJsonWriter { Count = 100, Size = 1_000_000 }.ValueListBuffer();
            //}
            BenchmarkDotNet.Running.BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
        }
    }
}
