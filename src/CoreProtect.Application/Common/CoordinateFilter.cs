namespace CoreProtect.Application.Common;

public sealed record CoordinateFilter(
    int? XMin,
    int? XMax,
    int? YMin,
    int? YMax,
    int? ZMin,
    int? ZMax);
