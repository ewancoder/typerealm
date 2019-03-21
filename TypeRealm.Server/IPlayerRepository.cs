namespace TypeRealm.Server
{
    internal interface IPlayerRepository
    {
        Player AuthenticateOrCreate(string login, string password);
    }
}
