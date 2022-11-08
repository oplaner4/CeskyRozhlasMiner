using CeskyRozhlasMiner.WebApp.API.Immutable.Own.Google;

namespace CeskyRozhlasMiner.WebApp.API.Immutable.Own
{
    /// <summary>
    /// Own settings
    /// </summary>
    public class OwnSettings
    {
        /// <summary>
        /// Google settings
        /// </summary>
        public OwnSettingsGoogle Google { get; set; }

        /// <summary>
        /// Songs fetch limit
        /// </summary>
        public int SongsFetchLimit { get; set; }

        /// <summary>
        /// Expiration interval of token in minutes.
        /// </summary>
        public int TokenExpirationMinutes { get; set; }

    }
}
