using Newtonsoft.Json;
using System.IO;

namespace TypeRealm.Data
{
    public static class DataLoader
    {
        public static Data Load(string fileName)
        {
            return JsonConvert.DeserializeObject<Data>(File.ReadAllText(fileName));
        }
    }
}
