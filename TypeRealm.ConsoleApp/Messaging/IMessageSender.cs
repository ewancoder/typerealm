namespace TypeRealm.ConsoleApp.Messaging
{
    internal interface IMessageSender
    {
        void Send(object message);
    }
}
