namespace TypeRealm.Server.Messaging
{
    internal interface IMessageDispatcher
    {
        void Dispatch(ConnectedClient client, object message);
    }
}
