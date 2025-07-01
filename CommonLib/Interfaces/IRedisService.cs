using System.Data;

namespace CommonLib.Interfaces
{
    public interface IRedisService
    {
        Task DeleteAsync(string key);
        Task<bool> ExistsAsync(string key);
        Task<List<T>?> GetListAsync<T>(string key);
        Task<T?> GetObjectAsync<T>(string key);
        Task SetAsync<T>(string key, T value, TimeSpan? expire = null);
        Task<bool> SetIfNotExistsAsync<T>(string key, T value, TimeSpan? expire = null);
        Task SetDataSetAsync(string key, DataSet value, TimeSpan? expire = null);
        Task<DataSet?> GetDataSetAsync(string key);
    }
}