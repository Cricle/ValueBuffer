using BenchmarkDotNet.Analysers;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Validators;
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
            BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args, new MyConfig());
        }
    }
    public class MyConfig : ManualConfig
    {
        public MyConfig()
        {
            AddLogger(BenchmarkDotNet.Loggers.ConsoleLogger.Unicode);
            AddJob(Job.ShortRun.WithPlatform(Platform.X64).WithGcServer(true).WithRuntime(CoreRuntime.Core70));
            AddJob(Job.ShortRun.WithPlatform(Platform.X64).WithGcServer(true).WithRuntime(NativeAotRuntime.Net70).WithId("AOT"));
            AddExporter(BenchmarkDotNet.Exporters.MarkdownExporter.GitHub);
            AddAnalyser(EnvironmentAnalyser.Default
                , OutliersAnalyser.Default
                , MinIterationTimeAnalyser.Default
                , MultimodalDistributionAnalyzer.Default
                , RuntimeErrorAnalyser.Default
                , ZeroMeasurementAnalyser.Default
                , BaselineCustomAnalyzer.Default
                , HideColumnsAnalyser.Default
                );
            AddValidator(BaselineValidator.FailOnError
                , SetupCleanupValidator.FailOnError
                , JitOptimizationsValidator.FailOnError
                , RunModeValidator.FailOnError
                , GenericBenchmarksValidator.DontFailOnError
                , DeferredExecutionValidator.FailOnError
                , ParamsAllValuesValidator.FailOnError
                );
            AddColumnProvider(BenchmarkDotNet.Columns.DefaultColumnProviders.Instance);
        }
    }
}
