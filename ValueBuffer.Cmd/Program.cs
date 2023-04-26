﻿using System.Diagnostics;
using System.Text.Json;

namespace ValueBuffer.Cmd
{
    internal class Program
    {
        static void Main(string[] args)
        {
            for (int q = 0; q < 5; q++)
            {
                var m = GC.GetTotalMemory(true);
                var sw = Stopwatch.GetTimestamp();
                using (var mem = new ValueBufferMemoryStream())
                {
                    using (var w = new StreamValueBufferWriter(mem))
                    using (var writer = new Utf8JsonWriter(w))
                    {
                        writer.WriteStartObject();
                        for (int d = 0; d < 10; d++)
                        {
                            writer.WriteStartArray(d.ToString());
                            for (int i = 0; i < 1_000_000; i++)
                            {
                                writer.WriteNumberValue(i);
                            }
                            writer.WriteEndArray();
                        }
                        writer.WriteEndObject();
                    }
                    var buffer = mem.Buffer;
                    for (int i = 0; i < buffer.BufferSlotIndex; i++)
                    {
                        _ = buffer.DangerousGetArray(i);
                    }
                }
                Console.WriteLine(new TimeSpan(Stopwatch.GetTimestamp() - sw));
                Console.WriteLine($"{(GC.GetTotalMemory(false) - m) / 1024 / 1024.0:F3}");
            }
        }
    }
}