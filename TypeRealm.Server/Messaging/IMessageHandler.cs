namespace TypeRealm.Server.Messaging
{
    internal interface IMessageHandler
    {
        void Handle(ConnectedClient sender, object message);
    }
}
