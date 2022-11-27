﻿using BenchmarkDotNet.Attributes;
using BetterStreams;
using Microsoft.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValueBuffer.Benchmarks.Actions
{
    [MemoryDiagnoser]
    [RankColumn]
    public class MemoryStreamWrite
    {
        private static readonly RecyclableMemoryStreamManager mgr = new RecyclableMemoryStreamManager();

        [Params(10,72205)]
        public int Size { get; set; }

        [Params(20, 53)]
        public int Count { get; set; }

        private byte[] value;

        [GlobalSetup]
        public void Setup()
        {
            value = new byte[Size];
            value.AsSpan().Fill(1);
        }

        [Benchmark(Baseline =true)]
        public void SystemMemoryStream()
        {
            for (int i = 0; i < Count; i++)
            {
                using (var mem = new MemoryStream())
                {
                    for (int j = 0; j < Count; j++)
                    {
                        mem.Write(value, 0, value.Length);
                    }
                }
            }
        }
        [Benchmark]
        public void MicrosoftMemoryStream()
        {
            for (int i = 0; i < Count; i++)
            {
                using (var mem = mgr.GetStream())
                {
                    for (int j = 0; j < Count; j++)
                    {
                        mem.Write(value, 0, value.Length);
                    }
                }
            }
        }
        [Benchmark]
        public void ValueBufferMemoryStream()
        {
            for (int i = 0; i < Count; i++)
            {
                using (var mem = new ValueBufferMemoryStream())
                {
                    for (int j = 0; j < Count; j++)
                    {
                        mem.Write(value, 0, value.Length);
                    }
                }
            }
        }
        [Benchmark]
        public void PooledBufferMemoryStream()
        {
            for (int i = 0; i < Count; i++)
            {
                using (var mem = new PooledMemoryStream())
                {
                    for (int j = 0; j < Count; j++)
                    {
                        mem.Write(value, 0, value.Length);
                    }
                }
            }
        }
    }
}