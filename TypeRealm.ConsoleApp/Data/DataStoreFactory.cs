using TypeRealm.Data;

namespace TypeRealm.ConsoleApp.Data
{
    public sealed class DataStoreFactory
    {
        public IDataStore LoadFromFile(string fileName)
        {
            var data = DataLoader.Load(fileName);

            return new DataStore(data);
        }
    }
}
