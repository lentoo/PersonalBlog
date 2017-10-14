using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.DI
{
  public class DependencyAttribute
  {
    /// <summary>
    /// 瞬时（每次都重新实例）
    /// </summary>
    public class TransientDependency:Attribute { }
    /// <summary>
    /// 一个请求内唯一（线程内唯一）
    /// </summary>
    public class ScopedDependency : Attribute { }
    /// <summary>
    /// 单例（全局唯一）
    /// </summary>
    public class SingletonDependency : Attribute { }
  }
}
