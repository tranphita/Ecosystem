namespace Ecosystem.Application.Abstractions.Data;

/// <summary>
/// Marker interface để đánh dấu các query có thể cache được.
/// </summary>
public interface ICacheableQuery
{
    /// <summary>
    /// Cache key duy nhất cho query này.
    /// </summary>
    string CacheKey { get; }

    /// <summary>
    /// Thời gian cache (tính bằng giây).
    /// </summary>
    TimeSpan? Expiration => null;
} 