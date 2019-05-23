namespace TypeRealm.Server
{
    public interface IConnection
    {
        void Write(object message);
        object Read();
    }
}
