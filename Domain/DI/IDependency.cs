using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.DI
{
  /// <summary>
  /// 瞬时（每次都重新实例）
  /// </summary>
  public interface ITransientDependency{}
  /// <summary>
  /// 一个请求内唯一（线程内唯一）
  /// </summary>
  public interface IScopedDependency{}
  /// <summary>
  /// 单例（全局唯一）
  /// </summary>
  public interface ISingletonDependency{}
}
