namespace TypeRealm.Server.Networking
{
    public interface IConnection
    {
        void Write(object message);
        object Read();
    }
}
