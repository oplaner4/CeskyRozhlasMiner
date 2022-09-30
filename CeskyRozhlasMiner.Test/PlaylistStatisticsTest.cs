using Xunit;
using CeskyRozhlasMiner.Lib.Playlist;
using CeskyRozhlasMiner.Lib.Playlist.DataProcessing;
using CeskyRozhlasMiner.Lib.Common;
using System.Linq;

namespace CeskyRozhlasMiner.Test
{
    public class PlaylistStatisticsTest
    {
        [Fact]
        public void NoSong_ShouldFail()
        {
            Assert.Throws<ArgumentNullException>(() => new PlaylistStatistics(Array.Empty<PlaylistSong>()));
        }

        [Fact]
        public void OneSong_ShouldSucceed()
        {
            PlaylistSong song = new("ELTON JOHN, DUA LIPA", "COLD HEART(PNAU REMIX)", DateTime.Now.AddDays(-40), RozhlasStation.Zurnal);
            PlaylistStatistics statistics = new(new PlaylistSong[] { song });

            var mostPlayed = statistics.GetMostPlayedSong();
            Assert.Equal(1, mostPlayed.Value);
            Assert.Equal(song.ToString(), mostPlayed.Key);

            var artist = statistics.GetMostFrequentArtist();
            Assert.Equal(1, artist.Value);
            Assert.Equal(song.Artist, artist.Key);
        }

        [Fact]
        public void MoreSongs_ShouldSucceed()
        {
            PlaylistSong song1 = new("Billie Eilish", "Your Power (Billie Eilish O'Connell & Finneas O'Connell)",
                DateTime.Parse("19.08.2022 17:54"), RozhlasStation.Vltava);
            PlaylistSong song2 = new("JANIS JOPLIN", "PIECE OF MY HEART", DateTime.Parse("12.09.2022 21:37"), RozhlasStation.ZurnalSport);
            PlaylistSong song3 = new("COCOROSIE", "We Are On Fire", DateTime.Parse("01.08.2022 1:04"), RozhlasStation.Wave);
            PlaylistSong song4 = new("JANIS JOPLIN", "PIECE OF MY HEART", DateTime.Parse("03.08.2022 13:37"), RozhlasStation.ZurnalSport);
            PlaylistSong song5 = new("Billie Eilish", "Therefore I Am", DateTime.Parse("10.08.2022 16:05"), RozhlasStation.Vltava);
            PlaylistSong song6 = new("Billie Eilish", "Wish You Were Gay", DateTime.Parse("12.09.2022 2:17"), RozhlasStation.Vltava);

            PlaylistSong[] songs = new PlaylistSong[] { song1, song2, song3, song4, song5, song6 };

            Random rng = new();
            foreach (IEnumerable<PlaylistSong> shuffled in Enumerable.Range(0, 5).Select(i => songs.OrderBy(a => rng.Next()))) {
                PlaylistStatistics statistics = new(shuffled);
                var mostPlayed = statistics.GetMostPlayedSong();
                Assert.Equal(2, mostPlayed.Value);
                Assert.Equal(song2.ToString(), mostPlayed.Key);
                Assert.Equal(song4.ToString(), mostPlayed.Key);

                var artist = statistics.GetMostFrequentArtist();
                Assert.Equal(3, artist.Value);
                Assert.Equal(song1.Artist, artist.Key);
                Assert.Equal(song5.Artist, artist.Key);
                Assert.Equal(song6.Artist, artist.Key);
            }
        }

        [Fact]
        public void MoreSongsEqualCount_ShouldSucceed()
        {
            PlaylistSong song1 = new("PHIL COLLINS", "CAN'T STOP LOVING YOU", DateTime.Parse("01.08.2022 1:11"), RozhlasStation.Zurnal);
            PlaylistSong song2 = new("PHIL COLLINS", "YOU'LL BE IN MY HEART", DateTime.Parse("01.08.2022 9:16"), RozhlasStation.Zurnal);
            PlaylistSong song3 = new("CRANBERRIES", "ODE TO MY FAMILY", DateTime.Parse("01.08.2022 20:11"), RozhlasStation.Zurnal);
            PlaylistSong song4 = new("TOTO", "AFRICA", DateTime.Parse("01.08.2022 22:21"), RozhlasStation.Zurnal);
            PlaylistSong song5 = new("TOTO", "AFRICA", DateTime.Parse("12.08.2022 14:13"), RozhlasStation.Dvojka);
            PlaylistSong song6 = new("CRANBERRIES", "ODE TO MY FAMILY", DateTime.Parse("13.08.2022 15:49"), RozhlasStation.Zurnal);
            PlaylistSong song7 = new("U 2", "ORDINARY LOVE", DateTime.Parse("13.08.2022 13:25"), RozhlasStation.Zurnal);

            PlaylistStatistics statistics1 = new(new PlaylistSong[] { song1, song2, song3, song4, song5, song6, song7 });

            var mostPlayed1 = statistics1.GetMostPlayedSong();
            Assert.Equal(2, mostPlayed1.Value);
            Assert.Equal(song3.ToString(), mostPlayed1.Key);
            Assert.Equal(song6.ToString(), mostPlayed1.Key);

            var artist1 = statistics1.GetMostFrequentArtist();
            Assert.Equal(2, artist1.Value);
            Assert.Equal(song1.Artist, artist1.Key);
            Assert.Equal(song2.Artist, artist1.Key);


            PlaylistStatistics statistics2 = new(new PlaylistSong[] { song5, song1, song2, song3, song6, song4, song7 });

            var mostPlayed2 = statistics2.GetMostPlayedSong();
            Assert.Equal(2, mostPlayed2.Value);
            Assert.Equal(song4.ToString(), mostPlayed2.Key);
            Assert.Equal(song5.ToString(), mostPlayed2.Key);

            var artist2 = statistics2.GetMostFrequentArtist();
            Assert.Equal(2, artist2.Value);
            Assert.Equal(song4.Artist, artist2.Key);
            Assert.Equal(song5.Artist, artist2.Key);


            PlaylistStatistics statistics3 = new(new PlaylistSong[] { song7, song3, song1, song2, song6, song4, song5 });

            var mostPlayed3 = statistics3.GetMostPlayedSong();
            Assert.Equal(2, mostPlayed2.Value);
            Assert.Equal(song3.ToString(), mostPlayed3.Key);
            Assert.Equal(song6.ToString(), mostPlayed3.Key);

            var artist3 = statistics3.GetMostFrequentArtist();
            Assert.Equal(2, artist2.Value);
            Assert.Equal(song3.Artist, artist3.Key);
            Assert.Equal(song6.Artist, artist3.Key);
        }
    }
}