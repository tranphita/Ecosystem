using Ecosystem.Application.Caching;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Ecosystem.Infrastructure.Caching;

/// <summary>
/// Dịch vụ bộ nhớ đệm Redis triển khai ICacheService.
/// </summary>
internal sealed class RedisCacheService : ICacheService
{
    private readonly IDistributedCache _cache;

    /// <summary>
    /// Khởi tạo một instance mới của RedisCacheService.
    /// </summary>
    /// <param name="cache">Đối tượng IDistributedCache để thao tác với Redis.</param>
    public RedisCacheService(IDistributedCache cache)
    {
        _cache = cache;
    }

    /// <summary>
    /// Lấy giá trị từ cache theo key.
    /// </summary>
    /// <typeparam name="T">Kiểu dữ liệu của giá trị.</typeparam>
    /// <param name="key">Khóa của giá trị cần lấy.</param>
    /// <param name="cancellationToken">Token hủy bỏ thao tác.</param>
    /// <returns>Giá trị lấy được hoặc mặc định nếu không tồn tại.</returns>
    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        var json = await _cache.GetStringAsync(key, cancellationToken);
        return json is null ? default : JsonSerializer.Deserialize<T>(json);
    }

    /// <summary>
    /// Lưu giá trị vào cache với key và thời gian hết hạn tùy chọn.
    /// </summary>
    /// <typeparam name="T">Kiểu dữ liệu của giá trị.</typeparam>
    /// <param name="key">Khóa của giá trị cần lưu.</param>
    /// <param name="value">Giá trị cần lưu.</param>
    /// <param name="expiration">Thời gian hết hạn (tùy chọn).</param>
    /// <param name="cancellationToken">Token hủy bỏ thao tác.</param>
    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
    {
        var options = new DistributedCacheEntryOptions();
        if (expiration.HasValue)
        {
            options.SetAbsoluteExpiration(expiration.Value);
        }

        var json = JsonSerializer.Serialize(value);
        await _cache.SetStringAsync(key, json, options, cancellationToken);
    }

    /// <summary>
    /// Xóa giá trị khỏi cache theo key.
    /// </summary>
    /// <param name="key">Khóa của giá trị cần xóa.</param>
    /// <param name="cancellationToken">Token hủy bỏ thao tác.</param>
    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        await _cache.RemoveAsync(key, cancellationToken);
    }
}
