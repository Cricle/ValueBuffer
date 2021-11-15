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
|    Method |  Count |            Mean |         Error |          StdDev | Ratio | RatioSD |    Gen 0 |    Gen 1 |    Gen 2 |    Allocated |
|---------- |------- |----------------:|--------------:|----------------:|------:|--------:|---------:|---------:|---------:|-------------:|
|      **List** |     **45** |        **679.9 ns** |      **15.16 ns** |        **44.71 ns** |  **1.00** |    **0.00** |   **0.7496** |        **-** |        **-** |      **1,179 B** |
| ValueList |     45 |      2,046.6 ns |      48.12 ns |       141.89 ns |  3.02 |    0.26 |   0.0153 |        - |        - |         24 B |
|           |        |                 |               |                 |       |         |          |          |          |              |
|      **List** |   **1234** |     **15,147.3 ns** |     **399.19 ns** |     **1,177.03 ns** |  **1.00** |    **0.00** |  **20.9656** |        **-** |        **-** |     **33,150 B** |
| ValueList |   1234 |     39,676.2 ns |     784.09 ns |     2,079.30 ns |  2.62 |    0.26 |        - |        - |        - |         24 B |
|           |        |                 |               |                 |       |         |          |          |          |              |
|      **List** | **876542** | **20,622,038.5 ns** | **772,545.15 ns** | **2,277,866.80 ns** |  **1.00** |    **0.00** | **718.7500** | **718.7500** | **718.7500** | **16,779,880 B** |
| ValueList | 876542 | 26,971,048.5 ns | 537,978.75 ns | 1,158,056.01 ns |  1.35 |    0.17 |        - |        - |        - |            - |

## ValueType
``` ini

BenchmarkDotNet=v0.13.1, OS=Windows 10.0.19043.1288 (21H1/May2021Update)
Intel Core i5-7200U CPU 2.50GHz (Kaby Lake), 1 CPU, 4 logical and 2 physical cores
  [Host]     : .NET Framework 4.8 (4.8.4420.0), X64 RyuJIT
  Job-ADZAZI : .NET Framework 4.8 (4.8.4420.0), X64 RyuJIT

OutlierMode=DontRemove  MemoryRandomization=True  

```
|    Method |  Count |            Mean |         Error |          StdDev |          Median | Ratio | RatioSD |     Gen 0 |     Gen 1 |     Gen 2 |   Allocated |
|---------- |------- |----------------:|--------------:|----------------:|----------------:|------:|--------:|----------:|----------:|----------:|------------:|
|      **List** |     **45** |        **492.3 ns** |       **9.80 ns** |        **23.85 ns** |        **488.3 ns** |  **1.00** |    **0.00** |    **0.4182** |         **-** |         **-** |       **658 B** |
| ValueList |     45 |      1,095.1 ns |      22.45 ns |        66.19 ns |      1,091.6 ns |  2.23 |    0.17 |         - |         - |         - |           - |
|           |        |                 |               |                 |                 |       |         |           |           |           |             |
|      **List** |   **1234** |      **9,302.7 ns** |     **187.13 ns** |       **551.77 ns** |      **9,188.1 ns** |  **1.00** |    **0.00** |   **10.6354** |         **-** |         **-** |    **16,738 B** |
| ValueList |   1234 |     21,168.1 ns |     457.40 ns |     1,348.64 ns |     20,798.5 ns |  2.28 |    0.19 |         - |         - |         - |           - |
|           |        |                 |               |                 |                 |       |         |           |           |           |             |
|      **List** | **876542** | **13,685,507.5 ns** | **380,991.34 ns** | **1,123,361.56 ns** | **13,609,475.8 ns** |  **1.00** |    **0.00** | **1984.3750** | **1984.3750** | **1984.3750** | **8,397,136 B** |
| ValueList | 876542 | 14,351,324.1 ns | 286,480.85 ns |   675,269.08 ns | 14,191,528.1 ns |  1.06 |    0.10 |         - |         - |         - |           - |

## StringBuilder

``` ini

BenchmarkDotNet=v0.13.1, OS=Windows 10.0.19043.1288 (21H1/May2021Update)
Intel Core i5-7200U CPU 2.50GHz (Kaby Lake), 1 CPU, 4 logical and 2 physical cores
  [Host]     : .NET Framework 4.8 (4.8.4420.0), X64 RyuJIT
  Job-ADZAZI : .NET Framework 4.8 (4.8.4420.0), X64 RyuJIT

OutlierMode=DontRemove  MemoryRandomization=True  

```
|             Method |  Count |            Mean |           Error |          StdDev | Ratio | RatioSD |     Gen 0 |     Gen 1 |    Gen 2 | Allocated |
|------------------- |------- |----------------:|----------------:|----------------:|------:|--------:|----------:|----------:|---------:|----------:|
|      **StringBuilder** |     **45** |        **859.9 ns** |        **17.14 ns** |        **36.16 ns** |  **1.00** |    **0.00** |    **2.4471** |         **-** |        **-** |      **4 KB** |
| ValueStringBuilder |     45 |      1,793.2 ns |        35.55 ns |        79.52 ns |  2.09 |    0.13 |    1.3256 |         - |        - |      2 KB |
|       ValueZString |     45 |      2,941.5 ns |        58.78 ns |       121.39 ns |  3.43 |    0.21 |    0.8202 |         - |        - |      1 KB |
|                    |        |                 |                 |                 |       |         |           |           |          |           |
|      **StringBuilder** |   **1234** |     **15,568.7 ns** |       **361.17 ns** |     **1,064.93 ns** |  **1.00** |    **0.00** |   **52.6123** |         **-** |        **-** |     **82 KB** |
| ValueStringBuilder |   1234 |     44,655.3 ns |       884.90 ns |     2,033.21 ns |  2.85 |    0.24 |   41.6260 |         - |        - |     64 KB |
|       ValueZString |   1234 |     77,387.7 ns |     1,686.16 ns |     4,971.69 ns |  5.00 |    0.52 |   21.7285 |         - |        - |     34 KB |
|                    |        |                 |                 |                 |       |         |           |           |          |           |
|      **StringBuilder** | **876542** | **45,907,629.0 ns** | **1,354,677.44 ns** | **3,994,296.98 ns** |  **1.00** |    **0.00** | **4600.0000** | **2100.0000** | **900.0000** | **48,107 KB** |
| ValueStringBuilder | 876542 | 67,337,006.6 ns | 2,680,280.92 ns | 7,902,868.77 ns |  1.48 |    0.21 |  625.0000 |  625.0000 | 625.0000 | 69,952 KB |
|       ValueZString | 876542 | 91,706,388.0 ns | 2,274,994.27 ns | 6,707,871.93 ns |  2.01 |    0.22 |  500.0000 |  500.0000 | 500.0000 | 54,688 KB |


# NET5.0

## Object
``` ini

BenchmarkDotNet=v0.13.1, OS=Windows 10.0.19043.1288 (21H1/May2021Update)
Intel Core i5-7200U CPU 2.50GHz (Kaby Lake), 1 CPU, 4 logical and 2 physical cores
.NET SDK=5.0.400
  [Host]     : .NET 5.0.9 (5.0.921.35908), X64 RyuJIT
  Job-TEXSLW : .NET 5.0.9 (5.0.921.35908), X64 RyuJIT

OutlierMode=DontRemove  MemoryRandomization=True  

```
|    Method |  Count |            Mean |         Error |          StdDev |          Median | Ratio | RatioSD |    Gen 0 |    Gen 1 |    Gen 2 |    Allocated |
|---------- |------- |----------------:|--------------:|----------------:|----------------:|------:|--------:|---------:|---------:|---------:|-------------:|
|      **List** |     **45** |        **658.0 ns** |      **14.21 ns** |        **41.90 ns** |        **650.9 ns** |  **1.00** |    **0.00** |   **0.7439** |        **-** |        **-** |      **1,168 B** |
| ValueList |     45 |        636.9 ns |      14.21 ns |        41.88 ns |        635.1 ns |  0.97 |    0.09 |   0.0153 |        - |        - |         24 B |
|           |        |                 |               |                 |                 |       |         |          |          |          |              |
|      **List** |   **1234** |     **14,834.7 ns** |     **381.72 ns** |     **1,125.50 ns** |     **14,798.3 ns** |  **1.00** |    **0.00** |  **20.9656** |        **-** |        **-** |     **33,032 B** |
| ValueList |   1234 |     11,452.8 ns |     228.50 ns |       496.74 ns |     11,395.8 ns |  0.79 |    0.06 |   0.0153 |        - |        - |         24 B |
|           |        |                 |               |                 |                 |       |         |          |          |          |              |
|      **List** | **876542** | **19,703,578.8 ns** | **834,030.75 ns** | **2,459,158.49 ns** | **19,136,081.2 ns** |  **1.00** |    **0.00** | **625.0000** | **593.7500** | **593.7500** | **16,777,925 B** |
| ValueList | 876542 |  7,994,048.3 ns | 162,373.56 ns |   478,762.11 ns |  7,793,785.2 ns |  0.41 |    0.05 |        - |        - |        - |         33 B |


## ValueType
``` ini

BenchmarkDotNet=v0.13.1, OS=Windows 10.0.19043.1288 (21H1/May2021Update)
Intel Core i5-7200U CPU 2.50GHz (Kaby Lake), 1 CPU, 4 logical and 2 physical cores
.NET SDK=5.0.400
  [Host]     : .NET 5.0.9 (5.0.921.35908), X64 RyuJIT
  Job-TEXSLW : .NET 5.0.9 (5.0.921.35908), X64 RyuJIT

OutlierMode=DontRemove  MemoryRandomization=True  

```
|    Method |  Count |            Mean |         Error |          StdDev |          Median | Ratio | RatioSD |     Gen 0 |     Gen 1 |     Gen 2 |   Allocated |
|---------- |------- |----------------:|--------------:|----------------:|----------------:|------:|--------:|----------:|----------:|----------:|------------:|
|      **List** |     **45** |        **364.0 ns** |       **7.30 ns** |        **20.59 ns** |        **362.3 ns** |  **1.00** |    **0.00** |    **0.4129** |         **-** |         **-** |       **648 B** |
| ValueList |     45 |        366.0 ns |       7.39 ns |        16.07 ns |        364.8 ns |  1.01 |    0.07 |         - |         - |         - |           - |
|           |        |                 |               |                 |                 |       |         |           |           |           |             |
|      **List** |   **1234** |      **6,877.6 ns** |     **137.16 ns** |       **331.26 ns** |      **6,836.3 ns** |  **1.00** |    **0.00** |   **10.5820** |         **-** |         **-** |    **16,640 B** |
| ValueList |   1234 |      5,993.9 ns |     119.06 ns |       253.72 ns |      5,893.6 ns |  0.87 |    0.05 |         - |         - |         - |           - |
|           |        |                 |               |                 |                 |       |         |           |           |           |             |
|      **List** | **876542** | **11,583,284.8 ns** | **471,508.34 ns** | **1,390,252.99 ns** | **11,347,892.2 ns** |  **1.00** |    **0.00** | **1984.3750** | **1984.3750** | **1984.3750** | **8,389,029 B** |
| ValueList | 876542 |  4,352,202.4 ns |  91,527.26 ns |   269,870.20 ns |  4,337,825.8 ns |  0.38 |    0.06 |         - |         - |         - |         4 B |

## StringBuilder

``` ini

BenchmarkDotNet=v0.13.1, OS=Windows 10.0.19043.1288 (21H1/May2021Update)
Intel Core i5-7200U CPU 2.50GHz (Kaby Lake), 1 CPU, 4 logical and 2 physical cores
.NET SDK=5.0.400
  [Host]     : .NET 5.0.9 (5.0.921.35908), X64 RyuJIT
  Job-TEXSLW : .NET 5.0.9 (5.0.921.35908), X64 RyuJIT

OutlierMode=DontRemove  MemoryRandomization=True  

```
|             Method |  Count |            Mean |           Error |          StdDev | Ratio | RatioSD |     Gen 0 |     Gen 1 |    Gen 2 | Allocated |
|------------------- |------- |----------------:|----------------:|----------------:|------:|--------:|----------:|----------:|---------:|----------:|
|      **StringBuilder** |     **45** |        **999.9 ns** |        **26.38 ns** |        **77.78 ns** |  **1.00** |    **0.00** |    **2.4471** |         **-** |        **-** |      **4 KB** |
| ValueStringBuilder |     45 |        653.2 ns |        13.01 ns |        31.92 ns |  0.65 |    0.06 |    0.8202 |         - |        - |      1 KB |
|       ValueZString |     45 |      1,282.5 ns |        25.51 ns |        68.10 ns |  1.28 |    0.12 |    0.8202 |         - |        - |      1 KB |
|                    |        |                 |                 |                 |       |         |           |           |          |           |
|      **StringBuilder** |   **1234** |     **18,195.0 ns** |       **363.07 ns** |     **1,059.10 ns** |  **1.00** |    **0.00** |   **52.6123** |         **-** |        **-** |     **82 KB** |
| ValueStringBuilder |   1234 |     14,776.0 ns |       318.56 ns |       939.27 ns |  0.81 |    0.07 |   21.7285 |         - |        - |     34 KB |
|       ValueZString |   1234 |     32,533.0 ns |     1,330.62 ns |     3,923.38 ns |  1.79 |    0.24 |   21.7285 |         - |        - |     34 KB |
|                    |        |                 |                 |                 |       |         |           |           |          |           |
|      **StringBuilder** | **876542** | **67,788,357.8 ns** | **2,038,149.17 ns** | **6,009,528.83 ns** |  **1.00** |    **0.00** | **4750.0000** | **2125.0000** | **875.0000** | **48,052 KB** |
| ValueStringBuilder | 876542 | 39,162,574.8 ns | 1,125,823.73 ns | 3,319,516.66 ns |  0.58 |    0.08 |  593.7500 |  593.7500 | 593.7500 | 45,984 KB |
|       ValueZString | 876542 | 53,875,490.3 ns | 1,935,108.46 ns | 5,705,710.94 ns |  0.80 |    0.10 |  700.0000 |  700.0000 | 700.0000 | 54,688 KB |
