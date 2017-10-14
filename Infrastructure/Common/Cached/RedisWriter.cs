
using Microsoft.Extensions.Options;
using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Common.Cached
{
  public class RedisWriter: ICacheClient,Domain.DI.ISingletonDependency
  {
    private readonly string[] ReadOnlyHosts;
    private readonly string[] ReadWriteHosts;
    public RedisWriter(IOptions<RedisConfiguration> optionsAccessor)
    {
      var configure = optionsAccessor.Value;
      ReadOnlyHosts = configure.RedisReadOnlyHosts.Split(';');
      ReadWriteHosts = configure.RedisReadWriteHosts.Split(';');
      prcm = CreateManager(ReadWriteHosts, ReadOnlyHosts);
    }
    //private static readonly string[] ReadOnlyHosts = ConfigurationManager.AppSettings["RedisReadOnlyHosts"].Split(';');
    //private static readonly string[] ReadWriteHosts = ConfigurationManager.AppSettings["RedisReadWriteHosts"].Split(';');
    public PooledRedisClientManager prcm { get; set; }

    private PooledRedisClientManager CreateManager(string[] readWriteHost, string[] readOnlyHost)
    {
      return new PooledRedisClientManager(readWriteHost, readOnlyHost, new RedisClientManagerConfig()
      {
        MaxReadPoolSize = 5,  // “读”链接池链接数  
        MaxWritePoolSize = 5,  // “写”链接池链接数 
        AutoStart = true
      });
    }
    public bool AddCache<T>(string key, T value)
    {
      using (IRedisClient redis = prcm.GetClient())
      {
        return redis.Add(key, value);
      }
    }

    public bool AddCache<T>(string key, T value, DateTime exp)
    {
      using (IRedisClient redis = prcm.GetClient())
      {
        return redis.Add(key, value);
      }
    }

    public bool DeleteCache(string key)
    {
      using (IRedisClient redis = prcm.GetClient())
      {
        return redis.Remove(key);
      }
    }
    /// <summary>
    /// 删除缓存中某个值
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value"></param>
    /// <returns></returns>
    public bool DeleteCache<T>(T value)
    {
      using (IRedisClient redis = prcm.GetClient())
      {
        bool isOk = false;
        foreach (var item in redis.GetAllKeys())
        {
          if (redis.Get<T>(item).Equals(value))
          {
            isOk = redis.Remove(item);
          }
        }
        return isOk;
      }
    }

    public T GetCache<T>(string key)
    {
      using (IRedisClient redis = prcm.GetClient())
      {
        if (redis.ContainsKey(key))
        {
          return redis.Get<T>(key);
        }
        return default(T);
      }
    }

    public bool SetCache<T>(string key, T value)
    {
      using (IRedisClient redis = prcm.GetClient())
      {
        return redis.Set(key, value);
      }
    }

    public bool SetCache<T>(string key, T value, DateTime exp)
    {
      using (IRedisClient redis = prcm.GetClient())
      {
        return redis.Set(key, value, exp);
      }
    }
  }
}

