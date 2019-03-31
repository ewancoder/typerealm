namespace TypeRealm.Server
{
    internal interface IConnection
    {
        void Write(object message);
        object Read();
    }
}
