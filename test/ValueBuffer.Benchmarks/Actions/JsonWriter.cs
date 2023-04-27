using BenchmarkDotNet.Attributes;
using BetterStreams;
using System.IO;
using System.Text.Json;

namespace ValueBuffer.Benchmarks.Actions
{
    [MemoryDiagnoser]
    [RankColumn]
    public class MemoryJsonWriter
    {
        [Params(10)]
        public int Size { get; set; }

        [Params(10, 1_000_000)]
        public int Count { get; set; }

        private void WriterJson(Utf8JsonWriter writer)
        {
            writer.WriteStartObject();
            for (int d = 0; d < Size; d++)
            {
                writer.WriteStartArray(d.ToString());
                for (int i = 0; i < Count; i++)
                {
                    writer.WriteNumberValue(i);
                }
                writer.WriteEndArray();
            }
            writer.WriteEndObject();
        }
        [Benchmark(Baseline = true)]
        public void PoolListBuffer()
        {
            using (var w = new PooledMemoryStream())
            {
                using (var writer = new Utf8JsonWriter(w, new JsonWriterOptions { SkipValidation = true }))
                {
                    WriterJson(writer);
                }
                w.CopyTo(Stream.Null);
            }
        }
        [Benchmark]
        public void ValueListBuffer()
        {
            using (var w = new ValueListBufferWriter<byte>())
            {
                using (var writer = new Utf8JsonWriter(w, new JsonWriterOptions { SkipValidation = true }))
                {
                    WriterJson(writer);
                }
                w.List.WriteToStream(Stream.Null);
            }
        }
    }
}
