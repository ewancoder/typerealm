namespace TypeRealm.ConsoleApp.Data
{
    public interface IDataStore
    {
        Location GetLocation(int locationId);
        Road GetRoad(int roadId);
    }

    public interface ITextStore
    {
        // TODO: Change to IEnumerable<string> GetRandomText() and make methods on top of it.
        /// <summary>
        /// Gets random word or phrase that you need to type to select / click something.
        /// </summary>
        string GetActionPhrase();

        /// <summary>
        /// Gets text paragraph that you need to type in order to get somewhere.
        /// </summary>
        /// <param name="length">Length of paragraph. It's usually the distance of the road.</param>
        string GetText(int length);
    }
}
