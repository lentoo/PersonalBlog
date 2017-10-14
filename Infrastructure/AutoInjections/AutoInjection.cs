using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Domain.DI;
using static Domain.DI.DependencyAttribute;

namespace Infrastructure.AutoInjections
{
  public class AutoInjection
  {
    public static List<Type> assembly = new List<Type>();
    public static void Initial(IServiceCollection services)
    {
      
      InitialInterface(services);
    }
    /// <summary>
    /// 注入实现DI接口的类
    /// </summary>
    /// <param name="services"></param>
    public static void InitialInterface(IServiceCollection services)
    {
      //获取标注了"ITransientDependency接口"的类或接口
      var transientInterfaceDependency = assembly
            .Where(t => t.GetInterfaces().Contains(typeof(ITransientDependency))).ToList();
      transientInterfaceDependency.Select(t => t.GetInterfaces().Where(f => !f.FullName.Contains("ITransientDependency"))).ToList();
      //自动注入标记了ITransientDependency接口
      //往services中注册瞬态对象
      foreach (var interfaceName in transientInterfaceDependency)
      {
        var type = assembly.Where(t => t.GetInterfaces().Contains(interfaceName)).FirstOrDefault();
        if (type != null)
          services.AddTransient(interfaceName, type);
      }

      //获取标注了"IScopedDependency接口"的类或接口
      var scopedInterfaceDependency = assembly.Where(t => t.GetInterfaces().Contains(typeof(IScopedDependency))).ToList();
      scopedInterfaceDependency.Select(t => t.GetInterfaces().Where(f => !f.FullName.Contains("IScopedDependency"))).ToList();
      //往services中注册请求唯一对象
      foreach (var interfaceName in scopedInterfaceDependency)
      {
        var type = assembly.Where(t => t.GetInterfaces().Contains(interfaceName)).FirstOrDefault();
        if (type != null)
          services.AddScoped(interfaceName, type);
      }
      //获取标注了"ISingletonDependenc接口"的类或接口
      var singletonInterfaceDependency = assembly.Where(t => t.GetInterfaces().Contains(typeof(ISingletonDependency))).ToList();
      singletonInterfaceDependency.Select(t => t.GetInterfaces().Where(f => !f.FullName.Contains("ISingletonDependency"))).ToList();
      //往services中注册单例对象
      foreach (var interfaceName in singletonInterfaceDependency)
      {
        var type = assembly.Where(t => t.GetInterfaces().Contains(interfaceName)).FirstOrDefault();
        if (type != null)
          services.AddSingleton(interfaceName, type);
      }
    }

    //public static void InitialAttribute(IServiceCollection services)
    //{
    //  assembly.Where(t => t.GetCustomAttribute(typeof(TransientDependency)) != null);
    //}
    public static void LoadAssembly(string assemblyPath)
    {
      assembly.AddRange(Assembly.Load(assemblyPath).GetTypes());
    }
  }
}
