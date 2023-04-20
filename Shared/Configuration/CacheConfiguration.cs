using System.Text.Json;
using StackExchange.Redis;

namespace Shared.Configuration;

public sealed class CacheConfiguration<T> : ICacheConfiguration<T>
{
    private readonly IConnectionMultiplexer _connectionMultiplexer;
    private readonly int _defaultCacheDb = -1;

    /// <summary>
    /// Default Constructor
    /// </summary>
    /// <param name="connectionMultiplexer">Dependency Injected</param>
    /// <exception cref="ArgumentNullException">If Connection multiplexer is null</exception>
    public CacheConfiguration(
        IConnectionMultiplexer connectionMultiplexer
    )
    {
        _connectionMultiplexer =
            connectionMultiplexer ?? throw new ArgumentNullException(nameof(connectionMultiplexer));
    }

    /// <summary>
    /// specific database
    /// </summary>
    /// <param name="connectionMultiplexer">Dependency Injected</param>
    /// <param name="defaultCacheDb">Database value</param>
    /// <exception cref="ArgumentNullException">If Connection multiplexer is null</exception>
    public CacheConfiguration(
        IConnectionMultiplexer connectionMultiplexer,
        int defaultCacheDb
    )
    {
        _connectionMultiplexer =
            connectionMultiplexer ?? throw new ArgumentNullException(nameof(connectionMultiplexer), "");

        _defaultCacheDb = defaultCacheDb;
    }

    /// <summary>
    /// Get property value
    /// </summary>
    /// <param name="obj">Model Class where property is located</param>
    /// <param name="label">The name of property to look in obj</param>
    /// <returns>value of property</returns>
    /// <exception cref="MissingMemberException"></exception>
    private string? GetKey(T obj, string? label, bool GetModelName)
    {
        if (GetModelName || string.IsNullOrEmpty(label))
        {
            label = typeof(T).Name;
            return label;
        }

        var propertyInfo =
            typeof(T).GetProperty(label) ?? throw new MissingMemberException(typeof(T).FullName, label);

        return propertyInfo.GetValue(obj)?.ToString();
    }

    /// <summary>
    /// Maps an object consisting of Key, Id and default database
    /// </summary>
    /// <param name="obj">Model Class where id is not null</param>
    /// <returns>Get Key,id of the T Class and database interface </returns>
    private CacheObj GetCacheObj(T obj)
    {
        const string idLabel = "Id";
        var key = GetKey(obj, null, true);
        var id = GetKey(obj, idLabel, false);

        return new()
        {
            CacheDatabase = _connectionMultiplexer.GetDatabase(_defaultCacheDb),
            Key = key,
            Id = id
        };
    }

    /// <summary>
    /// Get Record from Cache Memory of that particular key using id
    /// </summary>
    /// <param name="obj">Model Class</param>
    /// <returns>Model Class with data</returns>
    public async Task<T?> GetFromCacheMemoryByIdAsync(T obj)
    {
        var cacheObj = GetCacheObj(obj);
        var cacheValue = await cacheObj.CacheDatabase!.HashGetAsync(cacheObj.Key, $"{cacheObj.Key}:{cacheObj.Id}");
        var resultObj = cacheValue.HasValue ? JsonSerializer.Deserialize<T>(cacheValue) : default;
        return resultObj;
    }

    /// <summary>
    /// Get All Record from Cache Memory for that particular key
    /// </summary>
    /// <param name="obj">Model Class</param>
    /// <returns>List of Model Classes with data</returns>
    public async Task<List<T?>> GetAllFromCacheMemoryAsync(T obj)
    {
        var cacheObj = GetCacheObj(obj);
        var cacheValue = await cacheObj.CacheDatabase!.HashGetAllAsync(cacheObj.Key);
        return Array.ConvertAll(cacheValue, val =>
            JsonSerializer.Deserialize<T>(val.Value)).ToList();
    }

    /// <summary>
    /// Upserts Record in the Cache Memory
    /// </summary>
    /// <param name="obj">Model Class</param>
    public async void SetInCacheMemoryAsync(T obj)
    {
        try
        {

            var cacheObj = GetCacheObj(obj);

            var serialObj = JsonSerializer.Serialize(obj);
            var allExistingKeys = await GetAllFromCacheMemoryAsync(obj);
            var size = allExistingKeys.Count + 1;
            HashEntry[] hashArray;

            hashArray = new[]
            {
                new HashEntry($"{cacheObj.Key}:{cacheObj.Id}", serialObj)
            };

            if (size > 1)
            {
                for (var i = 0; i < allExistingKeys.Count; i++)
                {
                    var id = typeof(T).GetProperty("Id")!.GetValue(obj)!.ToString();
                    var serial = JsonSerializer.Serialize(allExistingKeys[i]);
                    var entry = new HashEntry($"{cacheObj.Key}:{cacheObj.Id}", serial);
                    hashArray.Append(entry);
                }
            }

            await cacheObj.CacheDatabase!.HashSetAsync(cacheObj.Key, hashArray);
        }
        catch (Exception e)
        {
            var ex  = e.ToString();
        }

    }

    /// <summary>
    /// Deletes Record of that particular key with id
    /// </summary>
    /// <param name="obj">Model Class with Id</param>
    public async void DeleteFromCacheMemory(T obj)
    {
        var cacheObj = GetCacheObj(obj);
        await cacheObj.CacheDatabase!.HashDeleteAsync(cacheObj.Key, cacheObj.Id);
    }

}

public interface ICacheConfiguration<T>
{

    /// <summary>
    /// Get Record from Cache Memory of that particular key using id
    /// </summary>
    /// <param name="obj">Model Class</param>
    /// <returns>Model Class with data</returns>
    public Task<T?> GetFromCacheMemoryByIdAsync(T obj);

    /// <summary>
    /// Get All Record from Cache Memory for that particular key
    /// </summary>
    /// <param name="obj">Model Class</param>
    /// <returns>List of Model Classes with data</returns>
    public Task<List<T?>> GetAllFromCacheMemoryAsync(T obj);

    /// <summary>
    /// Upserts Record in the Cache Memory
    /// </summary>
    /// <param name="obj">Model Class</param>
    public void SetInCacheMemoryAsync(T obj);

    /// <summary>
    /// Deletes Record of that particular key with id
    /// </summary>
    /// <param name="obj">Model Class with Id</param>
    public void DeleteFromCacheMemory(T obj);
}


/// <summary>
/// POCO Class
/// </summary>
public class CacheObj
{
    public IDatabase? CacheDatabase { get; set; }
    public string Key { get; set; } = string.Empty;
    public string Id { get; set; } = string.Empty;
}
