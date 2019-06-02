namespace TypeRealm.ConsoleApp.Data
{
    public sealed class Road
    {
        public Road(string name, string description)
        {
            Name = name;
            Description = description;
        }

        public string Name { get; }
        public string Description { get; }
    }
}
