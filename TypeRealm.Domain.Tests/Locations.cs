namespace TypeRealm.Domain.Tests
{
    public static class Locations
    {
        public static LocationId Village()
        {
            return new LocationId(10);
        }

        public static LocationId Forest()
        {
            return new LocationId(20);
        }

        public static LocationId Castle()
        {
            return new LocationId(30);
        }
    }
}
