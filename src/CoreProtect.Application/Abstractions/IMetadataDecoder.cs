using CoreProtect.Domain.ValueObjects;

namespace CoreProtect.Application.Abstractions;

public interface IMetadataDecoder
{
    MetadataDocument Decode(ReadOnlyMemory<byte> data);
}
