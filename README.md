<h1 align="center">
    <center>ValueBuffer</center>
</h1>
<div style="text-align:center;" align="center">
  The very fast, low memory, powerfull buffer for Stream, List, BufferWriter
</div>
<br/>
<div align="center">

[![.NET](https://github.com/Cricle/ValueBuffer/actions/workflows/dotnet.yml/badge.svg)](https://github.com/Cricle/ValueBuffer/actions/workflows/dotnet.yml)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=Cricle_ValueBuffer&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=Cricle_ValueBuffer)
[![codecov](https://codecov.io/gh/Cricle/ValueBuffer/branch/master/graph/badge.svg?token=85FsB1JWtx)](https://codecov.io/gh/Cricle/ValueBuffer)

</div>

## Useage

### Using ValueList like List

```csharp
using (var list = new ValueList<object>())
{
    for (int i = 0; i < Count; i++)
    {
        list.Add(obj);
    }
    list.ToArray();
}
```

### Using ValueBufferMemoryStream for stream write and read

```csharp
using (var mem = new ValueBufferMemoryStream())
{
    for (int j = 0; j < Count; j++)
    {
        mem.Add(value);
    }
    mem.Position = 0;
    mem.CopyTo(Stream.Null);
}
```

### Using ValueListBufferWriter<byte> for Utf8JsonWriter

```csharp
using (var w = new ValueListBufferWriter<byte>())
{
    using (var writer = new Utf8JsonWriter(w, new JsonWriterOptions { SkipValidation = true }))
    {
        WriterJson(writer);
    }
    w.List.WriteToStream(Stream.Null);
}
```

## Benchmarks

[Benchmark](./test/Benchmarks.md)
