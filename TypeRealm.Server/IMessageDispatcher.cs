namespace TypeRealm.Server
{
    internal interface IMessageDispatcher
    {
        void Dispatch(ConnectedClient client, object message);
    }
}
