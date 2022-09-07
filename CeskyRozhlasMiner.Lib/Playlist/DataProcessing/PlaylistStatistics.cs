using RadiozurnalMiner.Lib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using StatisticsPair = System.Collections.Generic.KeyValuePair<string, int>;

namespace RadiozurnalMiner.Lib.Playlist.DataProcessing
{
    public class PlaylistStatistics
    {
        private readonly IEnumerable<PlaylistSong> _songs;

        /// <summary>
        /// Initializes class
        /// </summary>
        /// <param name="songs">Source data for the statistics. At
        /// least one song is expected.</param>
        /// <exception cref="ArgumentNullException">Songs enumerable was null</exception>
        public PlaylistStatistics(IEnumerable<PlaylistSong> songs)
        {
            if (songs == null || !songs.Any())
            {
                throw new ArgumentNullException(nameof(songs));
            }

            _songs = songs;
        }

        /// <summary>
        /// Gets the song that was played the most times.
        /// In case that more songs match these criteria,
        /// the first is considered the most played.
        /// </summary>
        /// <returns>Pair where key represents a name of the song and
        /// value represents how many times it was played.</returns>
        public StatisticsPair GetMostPlayedSong()
        {
            return StatisticsUtil.GetPropAndCount(_songs, song => song.ToString())
                .MaxBy(pair => pair.Value);
        }

        /// <summary>
        /// Gets the artist whose songs were played the most times.
        /// In case that more artists match these criteria,
        /// the first is considered the most frequent.
        /// </summary>
        /// <returns>Pair where key represents a name of the artist
        /// and value represents how many times that artist was played.</returns>
        public StatisticsPair GetMostFrequentArtist()
        {
            return StatisticsUtil.GetPropAndCount(_songs, song => song.Artist)
                .MaxBy(pair => pair.Value);
        }

        /// <summary>
        /// Gets the list of most played songs in the descending order 
        /// (hit-parade).
        /// </summary>
        /// <param name="take">Get just a specific count of songs. 
        /// 10 by default.</param>
        /// <returns>Enumerable pairs where key represents a name of the song
        /// and value represents how many times it was played.</returns>
        public IEnumerable<StatisticsPair> GetLeaderBoard(int take = 10)
        {
            IEnumerable<StatisticsPair> songAndCount = StatisticsUtil
                .GetPropAndCount(_songs, song => song.ToString()).AsEnumerable();

            return songAndCount.OrderByDescending(pair => pair.Value).Take(take);
        }
    }
}
