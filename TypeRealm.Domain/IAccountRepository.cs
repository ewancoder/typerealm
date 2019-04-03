namespace TypeRealm.Domain
{
    public interface IAccountRepository
    {
        Account FindByLogin(string login);
        void Save(Account account);

        AccountId NextId();
    }
}
