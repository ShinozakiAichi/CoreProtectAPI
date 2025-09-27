using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CoreProtect.Application.Abstractions;
using CoreProtect.Domain.ValueObjects;
using SharpNBT;

namespace CoreProtect.Infrastructure.Decoding;

public sealed class MetadataDecoder : IMetadataDecoder
{
    private const int MaxParseBytes = 2 * 1024 * 1024;
    private const int FallbackBytes = 4 * 1024;

    public MetadataDocument Decode(ReadOnlyMemory<byte> data)
    {
        if (data.IsEmpty)
        {
            return MetadataDocument.Empty;
        }

        if (data.Length > MaxParseBytes)
        {
            return BuildFallback(data);
        }

        if (TryParseJava(data, out var javaResult))
        {
            return new MetadataDocument(javaResult, null, null);
        }

        if (TryParseNbt(data, out var nbtResult))
        {
            return new MetadataDocument(nbtResult, null, null);
        }

        return BuildFallback(data);
    }

    private static bool TryParseJava(ReadOnlyMemory<byte> data, out object? result)
    {
        var parser = new JavaSerializationParser(data);
        if (parser.TryParse(out result))
        {
            return true;
        }

        result = null;
        return false;
    }

    private static bool TryParseNbt(ReadOnlyMemory<byte> data, out object? result)
    {
        try
        {
            using var stream = new MemoryStream(data.ToArray(), writable: false);
            var reader = new TagReader(stream, FormatOptions.Java);
            var tag = reader.ReadTag();
            result = ConvertTag(tag);
            return true;
        }
        catch
        {
            result = null;
            return false;
        }
    }

    private static object? ConvertTag(Tag tag)
    {
        return tag switch
        {
            TagCompound compound => compound.ToDictionary(kvp => kvp.Key, kvp => ConvertTag(kvp.Value), StringComparer.Ordinal),
            TagList list => list.Select(ConvertTag).ToList(),
            TagByte tagByte => tagByte.Value,
            TagShort tagShort => tagShort.Value,
            TagInt tagInt => tagInt.Value,
            TagLong tagLong => tagLong.Value,
            TagFloat tagFloat => tagFloat.Value,
            TagDouble tagDouble => tagDouble.Value,
            TagString tagString => tagString.Value,
            TagByteArray byteArray => byteArray.Value.ToArray(),
            TagIntArray intArray => intArray.Value.ToArray(),
            TagLongArray longArray => longArray.Value.ToArray(),
            null => null,
            _ => tag.ToString()
        };
    }

    private static MetadataDocument BuildFallback(ReadOnlyMemory<byte> data)
    {
        var sliceLength = Math.Min(data.Length, FallbackBytes);
        var slice = data.Span.Slice(0, sliceLength);
        var hex = Convert.ToHexString(slice);
        var base64 = Convert.ToBase64String(slice);
        return new MetadataDocument(null, hex, base64);
    }
}
