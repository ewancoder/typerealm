namespace TypeRealm.Domain
{
    public interface IPlayerRepository
    {
        Player AuthenticateOrCreate(string login, string password);
    }
}
