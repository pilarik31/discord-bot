namespace ModeratorBot.Data
{
    /// <summary>
    /// User model.
    /// </summary>
    public class User
    {
        /// <summary>
        /// User id.
        /// </summary>
        /// <value>ID</value>
        public ulong Id { get; set; }
        /// <summary>
        /// User points.
        /// </summary>
        /// <value>points</value>
        public int Points { get; set; }
    }
}