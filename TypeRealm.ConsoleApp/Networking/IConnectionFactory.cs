namespace TypeRealm.ConsoleApp.Networking
{
    public interface IConnectionFactory
    {
        INetworkConnection Connect();
    }
}
