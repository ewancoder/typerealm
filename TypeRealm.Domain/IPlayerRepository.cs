namespace TypeRealm.Domain
{
    public interface IPlayerRepository
    {
        /// <summary>
        /// Finds player by name. Name should be globally unique.
        /// </summary>
        /// <param name="name">Player name.</param>
        /// <returns>Player instance.</returns>
        Player FindByName(PlayerName name);
        void Save(Player player);

        PlayerId NextId();
    }
}
