namespace TypeRealm.Data
{
    public sealed class RoadData
    {
        public int RoadId { get; set; }
        public RoadSideData Forward { get; set; }
        public RoadSideData Backward { get; set; }
    }
}
