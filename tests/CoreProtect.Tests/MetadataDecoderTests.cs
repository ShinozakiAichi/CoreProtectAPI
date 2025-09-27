using System;
using System.Collections.Generic;
using System.IO;
using CoreProtect.Infrastructure.Decoding;
using SharpNBT;
using Xunit;

namespace CoreProtect.Tests;

public sealed class MetadataDecoderTests
{
    private readonly MetadataDecoder _decoder = new();

    [Fact]
    public void Decode_WhenEmpty_ReturnsEmpty()
    {
        var result = _decoder.Decode(ReadOnlyMemory<byte>.Empty);
        Assert.Null(result.Json);
        Assert.Null(result.RawHex);
        Assert.Null(result.Base64);
    }

    [Fact]
    public void Decode_WhenTooLarge_ReturnsFallback()
    {
        var data = new byte[5000];
        var result = _decoder.Decode(data);
        Assert.Null(result.Json);
        Assert.NotNull(result.RawHex);
        Assert.NotNull(result.Base64);
    }

    [Fact]
    public void Decode_NbtDocument_ReturnsDictionary()
    {
        var compound = new CompoundTag(null)
        {
            new StringTag("name", "diamond_sword"),
            new IntTag("damage", 5)
        };

        using var stream = new MemoryStream();
        var writer = new TagWriter(stream, FormatOptions.Java);
        writer.WriteTag(compound);
        var bytes = stream.ToArray();

        var result = _decoder.Decode(bytes);
        Assert.NotNull(result.Json);
        var dictionary = Assert.IsType<Dictionary<string, object?>>(result.Json);
        Assert.Equal("diamond_sword", dictionary["name"]);
        Assert.Equal(5, dictionary["damage"]);
    }
}
