namespace TypeRealm.Server
{
    public static class ConnectionExtensions
    {
        public static bool TryWrite(this IConnection connection, object message)
        {
            try
            {
                connection.Write(message);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
