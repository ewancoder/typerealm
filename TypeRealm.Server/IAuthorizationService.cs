using System;

namespace TypeRealm.Server
{
    internal interface IAuthorizationService
    {
        // Returns player identity or null if unsuccessful.
        Guid? AuthorizeOrCreate(string login, string password, string playerName);
    }
}
