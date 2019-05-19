namespace TypeRealm.ConsoleApp.Data
{
    public sealed class Location
    {
        public Location(string name, string description)
        {
            Name = name;
            Description = description;
        }

        public string Name { get; }
        public string Description { get; }
    }
}
