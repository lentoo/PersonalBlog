using System;
using System.Collections.Generic;
using System.Linq;

namespace Domain.Event
{
  /// <summary>
  /// 事件总线
  /// 发布与订阅处理逻辑
  /// 核心功能代码
  /// </summary>
  public class EventBus
  {
    private EventBus() { }
    private static readonly EventBus _eventBus = null;
    private readonly object _sync = new object();
    private static readonly Dictionary<Type, List<object>> EventHandles = new Dictionary<Type, List<object>>();
    public static EventBus Instance => _eventBus ?? new EventBus();
    /// <summary>
    /// 检查两个事件处理程序是否相等。 如果事件处理程序是一个动作委派的，只需简单
    /// 比较两者与object.Equals覆盖（因为它是通过比较两个代表来覆盖的，否则，
    /// 将使用事件处理程序的类型，因为我们不需要注册相同类型的事件处理程序
    /// 每次具体事件多次。
    /// </summary>
    private readonly Func<object, object, bool> eventHandlerEquals = (o1, o2) =>
    {
      //var o1Type = o1.GetType();
      //var o2Type = o2.GetType();
      //if (o1Type.IsGenericType &&
      //    o1Type.GetGenericTypeDefinition() == typeof(ActionDelegatedEventHandler<>) &&
      //    o2Type.IsGenericType &&
      //    o2Type.GetGenericTypeDefinition() == typeof(ActionDelegatedEventHandler<>))
      //  return o1.Equals(o2);
      //return o1Type == o2Type;
      return true;
    };

    /// <summary>
    /// 订阅事件
    /// </summary>
    /// <typeparam name="TEvent"></typeparam>
    /// <param name="eventHandle"></param>
    public void Subscribe<TEvent>(IEventHandle<TEvent> eventHandle) where TEvent : IEvent
    {
      lock (_sync)
      {
        var eventType = typeof(TEvent);
        if (EventHandles.ContainsKey(eventType))
        {
          var handles = EventHandles[eventType];
          if (handles != null)
          {
            handles.Add(eventHandle);
          }
          else
          {
            handles = new List<object>()
            {
              eventHandle
            };
          }
        }
        else
        {
          EventHandles.Add(eventType,new List<object>(){eventHandle});
        }
      }
    }

    public void Publish<TEvent>(TEvent tEvent, Action<TEvent, bool, Exception> callback) where TEvent:IEvent
    {
      var eventType = typeof(TEvent);
      lock (_sync)
      {
        if (EventHandles.ContainsKey(eventType) && EventHandles[eventType] != null)
        {
          var handles = EventHandles[eventType];
          try
          {
            foreach (var handle in handles)
            {
              var eventHandler = handle as IEventHandle<TEvent>;

              eventHandler?.Handle(tEvent);
              callback(tEvent, true, null);
            }
          }
          catch (Exception exception)
          {
            callback(tEvent, false, exception);
          }
        }
        else
        {
          callback(tEvent, false, null);
        }
      }
    }
  }
}
