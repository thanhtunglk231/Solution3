using CommonLib.Handles;
using CommonLib.Interfaces;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace CommonLib.Implementations
{
    public class RedisService : IRedisService
    {
     
        private readonly IConnectionMultiplexer _redis;
        private readonly IErrorHandler _errorHandler;

        public RedisService(IConnectionMultiplexer redis, IErrorHandler errorHandler)
        {
      
            _redis = redis;
            _errorHandler = errorHandler;
        }
        private IDatabase _db => _redis.GetDatabase();
        public async Task<DataSet?> GetDataSetAsync(string key)
        {
            try
            {
                var value = await _db.StringGetAsync(key);
                return value.HasValue ? JsonConvert.DeserializeObject<DataSet>(value) : null;
            }
            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                return null;
            }
        }

        public async Task SetDataSetAsync(string key, DataSet value, TimeSpan? expire = null)
        {
            try
            {
                var json = JsonConvert.SerializeObject(value);
                await _db.StringSetAsync(key, json, expire);
            }
            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);
            }
        }


        // Dùng cho get tất cả 
        public async Task<List<T>?> GetListAsync<T>(string key)
        {
            try
            {
                var value = await _db.StringGetAsync(key);
                if (value.HasValue)
                {
                    return JsonConvert.DeserializeObject<List<T>>(value);
                }
            }
            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                _errorHandler.WriteStringToFuncion(nameof(RedisService), nameof(GetListAsync));
            }

            return null;
        }

        // Dùng cho Get theo mã 
        public async Task<T?> GetObjectAsync<T>(string key)
        {
            try
            {
                var value = await _db.StringGetAsync(key);
                if (value.HasValue)
                {
                    return JsonConvert.DeserializeObject<T>(value);
                }
            }
            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                _errorHandler.WriteStringToFuncion(nameof(RedisService), nameof(GetObjectAsync));
            }

            return default;
        }

        // Dùng chung cho cả Set 
        public async Task SetAsync<T>(string key, T value, TimeSpan? expire = null)
        {
            try
            {
                var json = JsonConvert.SerializeObject(value);
                await _db.StringSetAsync(key, json, expire);
            }
            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                _errorHandler.WriteStringToFuncion(nameof(RedisService), nameof(SetAsync));
            }
        }

        public async Task<bool> ExistsAsync(string key)
        {
            try
            {
                return await _db.KeyExistsAsync(key);
            }
            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                _errorHandler.WriteStringToFuncion(nameof(RedisService), nameof(ExistsAsync));
                return false;
            }
        }

        public async Task<bool> SetIfNotExistsAsync<T>(string key, T value, TimeSpan? expire = null)
        {
            try
            {
                var json = JsonConvert.SerializeObject(value);
                return await _db.StringSetAsync(key, json, expire, When.NotExists);
            }
            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                _errorHandler.WriteStringToFuncion(nameof(RedisService), nameof(SetIfNotExistsAsync));
                return false;
            }
        }

        public async Task DeleteAsync(string key)
        {
            try
            {
                await _db.KeyDeleteAsync(key);
            }
            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                _errorHandler.WriteStringToFuncion(nameof(RedisService), nameof(DeleteAsync));
            }
        }
    }
}
