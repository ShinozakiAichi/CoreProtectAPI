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
            CompoundTag compound => ConvertCompound(compound),
            ListTag list => list.Select(ConvertTag).ToList(),
            ByteTag byteTag when byteTag.IsBool => byteTag.Bool,
            ByteTag byteTag => byteTag.Value,
            ShortTag shortTag => shortTag.Value,
            IntTag intTag => intTag.Value,
            LongTag longTag => longTag.Value,
            FloatTag floatTag => floatTag.Value,
            DoubleTag doubleTag => doubleTag.Value,
            StringTag stringTag => stringTag.Value,
            ByteArrayTag byteArray => byteArray.Memory.ToArray(),
            IntArrayTag intArray => intArray.Memory.ToArray(),
            LongArrayTag longArray => longArray.Memory.ToArray(),
            null => null,
            _ => tag.ToString()
        };
    }

    private static IDictionary<string, object?> ConvertCompound(CompoundTag compound)
    {
        var dictionary = new Dictionary<string, object?>(compound.Count, StringComparer.Ordinal);
        foreach (var child in (IEnumerable<KeyValuePair<string, Tag>>)compound)
        {
            dictionary[child.Key] = ConvertTag(child.Value);
        }

        return dictionary;
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
