namespace TypeRealm.ConsoleApp.Data
{
    public sealed class Road
    {
        public Road(RoadSide forwardSide, RoadSide backwardSide)
        {
            ForwardSide = forwardSide;
            BackwardSide = backwardSide;
        }

        public RoadSide ForwardSide { get; }
        public RoadSide BackwardSide { get; }
    }

    public sealed class RoadSide
    {
        public RoadSide(string name, string description)
        {
            Name = name;
            Description = description;
        }

        public string Name { get; }
        public string Description { get; }
    }
}
