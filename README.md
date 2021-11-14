<h1>
    <center>ValueBuffer</center>
</h1>

<div style="text-align:center;">

[![.NET](https://github.com/Cricle/ValueBuffer/actions/workflows/dotnet.yml/badge.svg)](https://github.com/Cricle/ValueBuffer/actions/workflows/dotnet.yml)

[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=Cricle_ValueBuffer&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=Cricle_ValueBuffer)

[![codecov](https://codecov.io/gh/Cricle/ValueBuffer/branch/master/graph/badge.svg?token=85FsB1JWtx)](https://codecov.io/gh/Cricle/ValueBuffer)

</div>

# NET461

## Objects

``` ini

BenchmarkDotNet=v0.13.1, OS=Windows 10.0.19043.1288 (21H1/May2021Update)
Intel Core i5-7200U CPU 2.50GHz (Kaby Lake), 1 CPU, 4 logical and 2 physical cores
  [Host]     : .NET Framework 4.8 (4.8.4420.0), X64 RyuJIT
  Job-ADZAZI : .NET Framework 4.8 (4.8.4420.0), X64 RyuJIT

OutlierMode=DontRemove  MemoryRandomization=True  

```
|    Method |  Count |            Mean |           Error |          StdDev | Ratio | RatioSD |    Gen 0 |    Gen 1 |    Gen 2 |    Allocated |
|---------- |------- |----------------:|----------------:|----------------:|------:|--------:|---------:|---------:|---------:|-------------:|
|      **List** |     **45** |        **707.5 ns** |        **14.06 ns** |        **37.29 ns** |  **1.00** |    **0.00** |   **0.7496** |        **-** |        **-** |      **1,179 B** |
| ValueList |     45 |      2,245.7 ns |        45.93 ns |       135.42 ns |  3.19 |    0.27 |   0.0153 |        - |        - |         24 B |
|           |        |                 |                 |                 |       |         |          |          |          |              |
|      **List** |   **1234** |     **15,279.2 ns** |       **309.20 ns** |       **911.68 ns** |  **1.00** |    **0.00** |  **20.9656** |        **-** |        **-** |     **33,150 B** |
| ValueList |   1234 |     41,123.2 ns |       822.45 ns |     2,137.66 ns |  2.68 |    0.21 |        - |        - |        - |         24 B |
|           |        |                 |                 |                 |       |         |          |          |          |              |
|      **List** | **876542** | **21,777,319.0 ns** |   **737,936.46 ns** | **2,175,822.31 ns** |  **1.00** |    **0.00** | **718.7500** | **718.7500** | **718.7500** | **16,779,880 B** |
| ValueList | 876542 | 32,067,157.2 ns | 1,048,681.12 ns | 3,092,059.94 ns |  1.49 |    0.20 | 375.0000 | 375.0000 | 375.0000 | 11,186,490 B |

## ValueType

``` ini

BenchmarkDotNet=v0.13.1, OS=Windows 10.0.19043.1288 (21H1/May2021Update)
Intel Core i5-7200U CPU 2.50GHz (Kaby Lake), 1 CPU, 4 logical and 2 physical cores
  [Host]     : .NET Framework 4.8 (4.8.4420.0), X64 RyuJIT
  Job-ADZAZI : .NET Framework 4.8 (4.8.4420.0), X64 RyuJIT

OutlierMode=DontRemove  MemoryRandomization=True  

```
|    Method |  Count |            Mean |         Error |         StdDev |          Median | Ratio | RatioSD |     Gen 0 |     Gen 1 |     Gen 2 |   Allocated |
|---------- |------- |----------------:|--------------:|---------------:|----------------:|------:|--------:|----------:|----------:|----------:|------------:|
|      **List** |     **45** |        **774.1 ns** |     **109.45 ns** |       **322.7 ns** |        **551.8 ns** |  **1.00** |    **0.00** |    **0.4177** |         **-** |         **-** |       **658 B** |
| ValueList |     45 |      1,409.3 ns |      87.86 ns |       259.0 ns |      1,321.8 ns |  2.18 |    0.97 |         - |         - |         - |           - |
|           |        |                 |               |                |                 |       |         |           |           |           |             |
|      **List** |   **1234** |     **10,867.3 ns** |     **722.93 ns** |     **2,131.6 ns** |     **10,253.7 ns** |  **1.00** |    **0.00** |   **10.6354** |         **-** |         **-** |    **16,738 B** |
| ValueList |   1234 |     21,365.8 ns |     426.13 ns |     1,021.0 ns |     21,322.3 ns |  1.94 |    0.32 |         - |         - |         - |           - |
|           |        |                 |               |                |                 |       |         |           |           |           |             |
|      **List** | **876542** | **12,171,848.1 ns** | **395,026.23 ns** | **1,164,743.8 ns** | **12,099,961.7 ns** |  **1.00** |    **0.00** | **1984.3750** | **1984.3750** | **1984.3750** | **8,397,136 B** |
| ValueList | 876542 | 17,057,396.3 ns | 416,440.97 ns | 1,227,885.6 ns | 16,950,243.8 ns |  1.41 |    0.16 |  171.8750 |  171.8750 |  171.8750 | 5,592,472 B |


# NET5.0

## Object

``` ini

BenchmarkDotNet=v0.13.1, OS=Windows 10.0.19043.1288 (21H1/May2021Update)
Intel Core i5-7200U CPU 2.50GHz (Kaby Lake), 1 CPU, 4 logical and 2 physical cores
.NET SDK=5.0.400
  [Host]     : .NET 5.0.9 (5.0.921.35908), X64 RyuJIT
  Job-LUYVNB : .NET 5.0.9 (5.0.921.35908), X64 RyuJIT

OutlierMode=DontRemove  MemoryRandomization=True  

```
|    Method |  Count |            Mean |         Error |          StdDev | Ratio | RatioSD |    Gen 0 |    Gen 1 |    Gen 2 |    Allocated |
|---------- |------- |----------------:|--------------:|----------------:|------:|--------:|---------:|---------:|---------:|-------------:|
|      **List** |     **45** |        **671.6 ns** |      **16.11 ns** |        **47.49 ns** |  **1.00** |    **0.00** |   **0.7439** |        **-** |        **-** |      **1,168 B** |
| ValueList |     45 |        748.1 ns |      14.89 ns |        41.74 ns |  1.12 |    0.09 |   0.0153 |        - |        - |         24 B |
|           |        |                 |               |                 |       |         |          |          |          |              |
|      **List** |   **1234** |     **14,249.0 ns** |     **282.37 ns** |       **728.88 ns** |  **1.00** |    **0.00** |  **20.9656** |        **-** |        **-** |     **33,032 B** |
| ValueList |   1234 |     12,321.1 ns |     320.16 ns |       943.99 ns |  0.87 |    0.09 |   0.0153 |        - |        - |         24 B |
|           |        |                 |               |                 |       |         |          |          |          |              |
|      **List** | **876542** | **19,104,256.0 ns** | **551,917.41 ns** | **1,627,340.94 ns** |  **1.00** |    **0.00** | **593.7500** | **593.7500** | **593.7500** | **16,777,926 B** |
| ValueList | 876542 | 10,285,582.9 ns | 267,046.40 ns |   787,392.35 ns |  0.54 |    0.06 | 359.3750 | 359.3750 | 359.3750 | 11,185,786 B |



## ValueType

``` ini

BenchmarkDotNet=v0.13.1, OS=Windows 10.0.19043.1288 (21H1/May2021Update)
Intel Core i5-7200U CPU 2.50GHz (Kaby Lake), 1 CPU, 4 logical and 2 physical cores
.NET SDK=5.0.400
  [Host]     : .NET 5.0.9 (5.0.921.35908), X64 RyuJIT
  Job-THZAVD : .NET 5.0.9 (5.0.921.35908), X64 RyuJIT

OutlierMode=DontRemove  MemoryRandomization=True  

```
|    Method |  Count |            Mean |         Error |          StdDev |          Median | Ratio | RatioSD |     Gen 0 |     Gen 1 |     Gen 2 |   Allocated |
|---------- |------- |----------------:|--------------:|----------------:|----------------:|------:|--------:|----------:|----------:|----------:|------------:|
|      **List** |     **45** |        **379.4 ns** |      **12.74 ns** |        **37.56 ns** |        **367.9 ns** |  **1.00** |    **0.00** |    **0.4129** |         **-** |         **-** |       **648 B** |
| ValueList |     45 |        410.8 ns |       8.15 ns |        22.04 ns |        404.8 ns |  1.08 |    0.11 |         - |         - |         - |           - |
|           |        |                 |               |                 |                 |       |         |           |           |           |             |
|      **List** |   **1234** |      **7,452.5 ns** |     **148.39 ns** |       **319.42 ns** |      **7,408.5 ns** |  **1.00** |    **0.00** |   **10.5820** |         **-** |         **-** |    **16,640 B** |
| ValueList |   1234 |      6,952.8 ns |     138.33 ns |       285.67 ns |      6,862.8 ns |  0.93 |    0.05 |         - |         - |         - |           - |
|           |        |                 |               |                 |                 |       |         |           |           |           |             |
|      **List** | **876542** | **10,794,245.6 ns** | **424,273.27 ns** | **1,250,979.30 ns** | **10,791,766.4 ns** |  **1.00** |    **0.00** | **1984.3750** | **1984.3750** | **1984.3750** | **8,389,031 B** |
| ValueList | 876542 |  5,768,033.1 ns | 183,915.50 ns |   542,279.01 ns |  5,653,466.0 ns |  0.54 |    0.08 |  359.3750 |  359.3750 |  359.3750 | 5,592,675 B |
