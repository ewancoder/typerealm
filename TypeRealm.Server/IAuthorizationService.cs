using TypeRealm.Domain;

namespace TypeRealm.Server
{
    internal interface IAuthorizationService
    {
        // Returns player identity or null if unsuccessful.
        PlayerId AuthorizeOrCreate(string login, string password, PlayerName playerName);
    }
}
