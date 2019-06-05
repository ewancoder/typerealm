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
}
