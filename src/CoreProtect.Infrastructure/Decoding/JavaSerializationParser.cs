using System;
using System.Buffers.Binary;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using CoreProtect.Domain.ValueObjects;

namespace CoreProtect.Infrastructure.Decoding;

internal sealed class JavaSerializationParser
{
    private const ushort StreamMagic = 0xACED;
    private const ushort StreamVersion = 0x0005;
    private const int HandleBase = 0x7E0000;

    private readonly ReadOnlyMemory<byte> _data;
    private readonly List<object?> _handles = new();
    private int _position;

    public JavaSerializationParser(ReadOnlyMemory<byte> data)
    {
        _data = data;
    }

    public bool TryParse(out object? result)
    {
        result = null;
        if (_data.Length < 4)
        {
            return false;
        }

        if (!ReadStreamHeader())
        {
            return false;
        }

        try
        {
            result = ReadContent();
            return result is not null;
        }
        catch
        {
            return false;
        }
    }

    private bool ReadStreamHeader()
    {
        var magic = ReadUInt16();
        var version = ReadUInt16();
        return magic == StreamMagic && version == StreamVersion;
    }

    private object? ReadContent()
    {
        var token = ReadByte();
        return token switch
        {
            JavaTokens.Null => null,
            JavaTokens.Reference => ReadReference(),
            JavaTokens.Object => ReadNewObject(),
            JavaTokens.String => ReadNewString(),
            JavaTokens.LongString => ReadNewLongString(),
            JavaTokens.Array => ReadNewArray(),
            JavaTokens.Enum => ReadEnum(),
            JavaTokens.ClassDesc => ReadClassDesc(),
            _ => throw new NotSupportedException($"Unsupported Java serialization token: 0x{token:X2}")
        };
    }

    private object? ReadReference()
    {
        var handle = ReadInt32();
        var index = handle - HandleBase;
        return index >= 0 && index < _handles.Count ? _handles[index] : null;
    }

    private JavaClassDescriptor? ReadClassDesc()
    {
        var token = ReadByte();
        if (token == JavaTokens.Null)
        {
            return null;
        }

        if (token == JavaTokens.Reference)
        {
            var handle = ReadInt32();
            return (JavaClassDescriptor?)_handles[handle - HandleBase];
        }

        if (token != JavaTokens.ClassDesc)
        {
            throw new NotSupportedException($"Unsupported class descriptor token: 0x{token:X2}");
        }

        var handleIndex = RegisterPlaceholder();
        var name = ReadUtf();
        var serialVersion = ReadInt64();
        var flags = ReadByte();
        var fieldCount = ReadUInt16();
        var fields = new JavaFieldDescriptor[fieldCount];
        for (var i = 0; i < fieldCount; i++)
        {
            var typeCode = ReadByte();
            var fieldName = ReadUtf();
            string? className = null;
            if (typeCode is (byte)'L' or (byte)'[')
            {
                className = ReadUtf();
            }

            fields[i] = new JavaFieldDescriptor((char)typeCode, fieldName, className);
        }

        SkipClassAnnotations();
        var superClass = ReadClassDesc();

        var descriptor = new JavaClassDescriptor(name, serialVersion, flags, fields, superClass);
        SetHandle(handleIndex, descriptor);
        return descriptor;
    }

    private void SkipClassAnnotations()
    {
        while (true)
        {
            var token = ReadByte();
            if (token == JavaTokens.EndBlockData)
            {
                return;
            }

            _position--; // unread
            ReadContent();
        }
    }

    private object? ReadNewObject()
    {
        var classDesc = ReadClassDesc();
        if (classDesc is null)
        {
            return null;
        }

        var handleIndex = RegisterPlaceholder();
        object? instance = classDesc.Name switch
        {
            "java.util.HashMap" => ReadHashMap(classDesc),
            "java.util.LinkedHashMap" => ReadHashMap(classDesc),
            "java.util.TreeMap" => ReadHashMap(classDesc),
            "java.util.EnumMap" => ReadHashMap(classDesc),
            "java.util.ArrayList" => ReadArrayList(classDesc),
            "java.util.LinkedList" => ReadArrayList(classDesc),
            "java.util.Collections$UnmodifiableMap" => ReadMapWrapper(classDesc),
            "java.util.Collections$SingletonMap" => ReadSingletonMap(classDesc),
            "com.google.common.collect.ImmutableMap$SerializedForm" => ReadImmutableMap(classDesc),
            _ => ReadPojo(classDesc)
        };

        SetHandle(handleIndex, instance);
        return instance;
    }

    private object? ReadPojo(JavaClassDescriptor classDesc)
    {
        var state = ReadClassDataHierarchy(classDesc);
        return state;
    }

    private Dictionary<string, object?> ReadClassDataHierarchy(JavaClassDescriptor descriptor)
    {
        var state = descriptor.SuperClass is null
            ? new Dictionary<string, object?>(StringComparer.Ordinal)
            : ReadClassDataHierarchy(descriptor.SuperClass);

        foreach (var field in descriptor.Fields)
        {
            state[field.Name] = ReadFieldValue(field);
        }

        if (descriptor.HasWriteMethod)
        {
            SkipOptionalBlockData();
        }

        return state;
    }

    private IDictionary<string, object?> ReadHashMap(JavaClassDescriptor descriptor)
    {
        var state = ReadClassDataHierarchy(descriptor);
        var size = ReadInt32();
        var map = new Dictionary<string, object?>(size, StringComparer.Ordinal);
        for (var i = 0; i < size; i++)
        {
            var key = ReadContent();
            var value = ReadContent();
            if (key is null)
            {
                continue;
            }

            map[Convert.ToString(key, CultureInfo.InvariantCulture) ?? string.Empty] = value;
        }

        return map;
    }

    private IDictionary<string, object?> ReadSingletonMap(JavaClassDescriptor descriptor)
    {
        var state = ReadClassDataHierarchy(descriptor);
        var map = new Dictionary<string, object?>(1, StringComparer.Ordinal);
        if (state.TryGetValue("key", out var key) && key is not null)
        {
            map[Convert.ToString(key, CultureInfo.InvariantCulture) ?? string.Empty] = state.TryGetValue("value", out var value) ? value : null;
        }

        return map;
    }

    private IDictionary<string, object?> ReadMapWrapper(JavaClassDescriptor descriptor)
    {
        var state = ReadClassDataHierarchy(descriptor);
        if (state.TryGetValue("m", out var inner) && inner is IDictionary<string, object?> dictionary)
        {
            return dictionary;
        }

        return new Dictionary<string, object?>();
    }

    private object ReadImmutableMap(JavaClassDescriptor descriptor)
    {
        var state = ReadClassDataHierarchy(descriptor);
        if (!state.TryGetValue("keys", out var keys) || keys is not IList keyList)
        {
            return state;
        }

        if (!state.TryGetValue("values", out var values) || values is not IList valueList)
        {
            return state;
        }

        var map = new Dictionary<string, object?>(keyList.Count, StringComparer.Ordinal);
        for (var i = 0; i < keyList.Count; i++)
        {
            var key = keyList[i];
            if (key is null)
            {
                continue;
            }

            var keyString = Convert.ToString(key, CultureInfo.InvariantCulture) ?? string.Empty;
            var value = i < valueList.Count ? valueList[i] : null;
            map[keyString] = value;
        }

        return map;
    }

    private IList<object?> ReadArrayList(JavaClassDescriptor descriptor)
    {
        var state = ReadClassDataHierarchy(descriptor);
        var size = state.TryGetValue("size", out var sizeValue) ? Convert.ToInt32(sizeValue, CultureInfo.InvariantCulture) : 0;
        var list = new List<object?>(size);
        RegisterHandle(list);
        for (var i = 0; i < size; i++)
        {
            list.Add(ReadContent());
        }

        return list;
    }

    private void SkipOptionalBlockData()
    {
        while (_position < _data.Length)
        {
            var marker = _data.Span[_position];
            if (marker == JavaTokens.EndBlockData)
            {
                _position++;
                continue;
            }

            if (marker == JavaTokens.BlockData)
            {
                _position++;
                var length = ReadByte();
                _position += length;
                continue;
            }

            if (marker == JavaTokens.BlockDataLong)
            {
                _position++;
                var length = ReadInt32();
                _position += length;
                continue;
            }

            break;
        }
    }

    private object? ReadFieldValue(JavaFieldDescriptor field)
    {
        return field.TypeCode switch
        {
            'B' => (sbyte)ReadByte(),
            'C' => (char)ReadUInt16(),
            'D' => ReadDouble(),
            'F' => ReadFloat(),
            'I' => ReadInt32(),
            'J' => ReadInt64(),
            'S' => (short)ReadUInt16(),
            'Z' => ReadBoolean(),
            'L' or '[' => ReadContent(),
            _ => throw new NotSupportedException($"Unsupported field type: {field.TypeCode}")
        };
    }

    private object? ReadEnum()
    {
        var classDesc = ReadClassDesc();
        if (classDesc is null)
        {
            return null;
        }

        var constant = ReadContent();
        RegisterValue(constant);
        return constant;
    }

    private object? ReadNewArray()
    {
        var classDesc = ReadClassDesc();
        if (classDesc is null)
        {
            return null;
        }

        var elementType = classDesc.Name[1];
        var length = ReadInt32();
        switch (elementType)
        {
            case 'B':
                var handleIndex = RegisterPlaceholder();
                var bytes = _data.Span.Slice(_position, length).ToArray();
                _position += length;
                SetHandle(handleIndex, bytes);
                return bytes;
            case 'I':
                var handleIndex = RegisterPlaceholder();
                var ints = new int[length];
                SetHandle(handleIndex, ints);
                for (var i = 0; i < length; i++)
                {
                    ints[i] = ReadInt32();
                }

                return ints;
            case 'L':
            case '[':
                var list = new List<object?>(length);
                RegisterHandle(list);
                for (var i = 0; i < length; i++)
                {
                    list.Add(ReadContent());
                }

                return list;
            default:
                throw new NotSupportedException($"Unsupported array component type: {elementType}");
        }
    }

    private string ReadNewString()
    {
        var value = ReadUtf();
        RegisterValue(value);
        return value;
    }

    private string ReadNewLongString()
    {
        var length = ReadInt64();
        if (length > int.MaxValue)
        {
            throw new NotSupportedException("Long strings exceeding Int32 are not supported.");
        }

        var span = _data.Span.Slice(_position, (int)length);
        var value = System.Text.Encoding.UTF8.GetString(span);
        _position += (int)length;
        RegisterValue(value);
        return value;
    }

    private int RegisterPlaceholder()
    {
        _handles.Add(null);
        return _handles.Count - 1;
    }

    private void SetHandle(int index, object? value)
    {
        _handles[index] = value;
    }

    private void RegisterValue(object? value)
    {
        _handles.Add(value);
    }

    private byte ReadByte()
    {
        if (_position >= _data.Length)
        {
            throw new InvalidOperationException("Unexpected end of stream.");
        }

        return _data.Span[_position++];
    }

    private ushort ReadUInt16()
    {
        var value = BinaryPrimitives.ReadUInt16BigEndian(_data.Span.Slice(_position, 2));
        _position += 2;
        return value;
    }

    private short ReadInt16()
    {
        var value = BinaryPrimitives.ReadInt16BigEndian(_data.Span.Slice(_position, 2));
        _position += 2;
        return value;
    }

    private int ReadInt32()
    {
        var value = BinaryPrimitives.ReadInt32BigEndian(_data.Span.Slice(_position, 4));
        _position += 4;
        return value;
    }

    private long ReadInt64()
    {
        var value = BinaryPrimitives.ReadInt64BigEndian(_data.Span.Slice(_position, 8));
        _position += 8;
        return value;
    }

    private float ReadFloat()
    {
        var value = BinaryPrimitives.ReadInt32BigEndian(_data.Span.Slice(_position, 4));
        _position += 4;
        return BitConverter.Int32BitsToSingle(value);
    }

    private double ReadDouble()
    {
        var value = BinaryPrimitives.ReadInt64BigEndian(_data.Span.Slice(_position, 8));
        _position += 8;
        return BitConverter.Int64BitsToDouble(value);
    }

    private bool ReadBoolean() => ReadByte() != 0;

    private string ReadUtf()
    {
        var length = ReadUInt16();
        if (length == 0)
        {
            return string.Empty;
        }

        var span = _data.Span.Slice(_position, length);
        var value = System.Text.Encoding.UTF8.GetString(span);
        _position += length;
        return value;
    }
}

internal static class JavaTokens
{
    public const byte Null = 0x70;
    public const byte Reference = 0x71;
    public const byte ClassDesc = 0x72;
    public const byte Object = 0x73;
    public const byte String = 0x74;
    public const byte Array = 0x75;
    public const byte Class = 0x76;
    public const byte BlockData = 0x77;
    public const byte EndBlockData = 0x78;
    public const byte Reset = 0x79;
    public const byte BlockDataLong = 0x7A;
    public const byte Exception = 0x7B;
    public const byte LongString = 0x7C;
    public const byte ProxyClassDesc = 0x7D;
    public const byte Enum = 0x7E;
}

internal sealed record JavaClassDescriptor(
    string Name,
    long SerialVersionUid,
    byte Flags,
    IReadOnlyList<JavaFieldDescriptor> Fields,
    JavaClassDescriptor? SuperClass)
{
    public bool HasWriteMethod => (Flags & 0x01) != 0;
}

internal sealed record JavaFieldDescriptor(char TypeCode, string Name, string? ClassName);
