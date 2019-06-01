namespace TypeRealm.ConsoleApp.Messaging
{
    public interface IMessageDispatcher
    {
        void Dispatch(object message);
    }
}
