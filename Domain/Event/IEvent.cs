namespace Domain.Event
{
  public interface IEvent
  {
  }
  public interface IEventHandle<TEvent> where TEvent : IEvent
  {
    void Handle(TEvent evt);
  }
}
