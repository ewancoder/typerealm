namespace TypeRealm.ConsoleApp.Networking
{
    public interface IMessageDispatcher
    {
        void Dispatch(object message);
    }
}
