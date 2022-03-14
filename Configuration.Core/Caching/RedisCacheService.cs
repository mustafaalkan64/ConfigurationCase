using ConfigurationCase.Core.Caching;
using ConfigurationCase.Core.CustomExceptions;
using ConfigurationCase.Core.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigurationCase.Core.Caching
{
    public class RedisCacheService : IRedisCacheService
    {
        #region Fields

        public readonly IOptions<RedisServerConfig> _config;
        private readonly RedisEndpoint conf = null;

        #endregion

        public RedisCacheService(IOptions<RedisServerConfig> config)
        {
            _config = config;
            conf = new RedisEndpoint
            {
                Host = _config.Value.RedisEndPoint,
                Port = _config.Value.RedisPort,
                Password = _config.Value.RedisPassword,
                RetryTimeout = 1000,
                Ssl = _config.Value.RedisSsl
            };
        }
        public T Get<T>(string key)
        {
            try
            {
                using (IRedisClient client = new RedisClient(conf))
                {
                    return client.Get<T>(key);
                }
            }
            catch
            {
                //throw new RedisNotAvailableException();
                return default;
            }
        }

        public IList<T> GetAll<T>(string key)
        {
            try
            {
                using (IRedisClient client = new RedisClient(conf))
                {
                    var keys = client.SearchKeys(key);
                    if (keys.Any())
                    {
                        IEnumerable<T> dataList = client.GetAll<T>(keys).Values;
                        return dataList.ToList();
                    }
                    return new List<T>();
                }
            }
            catch
            {

                throw new RedisNotAvailableException();
            }
        }

        public void Set(string key, object data)
        {
            Set(key, data, DateTime.Now.AddMinutes(_config.Value.RedisTimeout));
        }

        public void Set(string key, object data, double timeOut)
        {
            Set(key, data, DateTime.Now.AddMinutes(timeOut));
        }

        public void Set(string key, object data, DateTime time)
        {
            try
            {
                using (IRedisClient client = new RedisClient(conf))
                {
                    var dataSerialize = JsonConvert.SerializeObject(data, Formatting.Indented, new JsonSerializerSettings
                    {
                        PreserveReferencesHandling = PreserveReferencesHandling.Objects
                    });
                    client.Set(key, Encoding.UTF8.GetBytes(dataSerialize), time);
                }
            }
            catch (Exception ex)
            {
                //throw new RedisNotAvailableException();                
            }
        }

        public void SetAll<T>(IDictionary<string, T> values)
        {
            try
            {
                using (IRedisClient client = new RedisClient(conf))
                {
                    client.SetAll(values);
                }
            }
            catch
            {

                throw new RedisNotAvailableException();
            }

        }

        public int Count(string key)
        {
            try
            {
                using (IRedisClient client = new RedisClient(conf))
                {
                    return client.SearchKeys(key).Where(s => s.Contains(":") && s.Contains("Mobile-RefreshToken")).ToList().Count;
                }
            }
            catch
            {

                throw new RedisNotAvailableException();
            }
        }

        public bool IsSet(string key)
        {
            try
            {
                using (IRedisClient client = new RedisClient(conf))
                {
                    return client.ContainsKey(key);
                }
            }
            catch
            {

                throw new RedisNotAvailableException();
            }
        }

        public void Remove(string key)
        {
            try
            {
                using (IRedisClient client = new RedisClient(conf))
                {
                    client.Remove(key);
                }
            }
            catch
            {
                throw new RedisNotAvailableException();
            }
        }

        public void RemoveByPattern(string pattern)
        {
            try
            {
                using (IRedisClient client = new RedisClient(conf))
                {
                    var keys = client.SearchKeys(pattern);
                    client.RemoveAll(keys);
                }
            }
            catch
            {

                throw new RedisNotAvailableException();
            }
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }
        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
