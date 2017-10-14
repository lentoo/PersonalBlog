using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Common.Cached
{
  public class RedisConfiguration
  {
    public RedisConfiguration()
    {

    }
    public string RedisReadWriteHosts { get; set; }
    public string RedisReadOnlyHosts { get; set; }    
  }
}
