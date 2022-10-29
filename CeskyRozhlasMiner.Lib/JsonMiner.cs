using CeskyRozhlasMiner.Lib.Common;
using CeskyRozhlasMiner.Lib.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace CeskyRozhlasMiner.Lib
{
    /// <summary>
    /// Utility which can be used to fetch and deserialize json from api.
    /// </summary>
    /// <typeparam name="T">The type used when deserializing fetched json stream.</typeparam>
    internal class JsonMiner<T>
    {
        private readonly HttpClient _client;
        private readonly string _uri;
        private readonly string _briefLogInfo;

        internal JsonMiner(string uri, string briefLogInfo)
        {
            _uri = uri;
            _briefLogInfo = briefLogInfo;
            _client = new();
        }

        /// <summary>
        /// Makes http request, logs unusual behaviour and deserializes using given type 
        /// <typeparamref name="T"/>.
        /// </summary>
        /// <returns>Pair (successfully fetched, <typeparamref name="T"/> deserialized object). The second 
        /// (<typeparamref name="T"/> deserialized object) is set to <value>default</value> when any 
        /// operation did not succeed.</returns>
        internal async Task<(bool, T)> Fetch()
        {
            HttpResponseMessage response;

            try
            {
                response = await _client.GetAsync(_uri);
            }
            catch (HttpRequestException ex)
            {
                Logging.SaveRecord(Logging.Severity.Error, nameof(Fetch),
                    $"{_briefLogInfo}. Unable to make http request. {ex.Message}");
                return (false, default);
            }

            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                Logging.SaveRecord(Logging.Severity.Error, nameof(Fetch),
                    $"{_briefLogInfo}. Request to access Json stream failed with status {response.StatusCode}.");
                return (false, default);
            }

            using (Stream jsonStream = await response.Content.ReadAsStreamAsync())
            {
                try
                {
                    T result = await JsonSerializer.DeserializeAsync<T>(jsonStream, Settings.DeserializeSettings);
                    return (true, result);
                }
                catch (JsonException ex)
                {
                    Logging.SaveRecord(Logging.Severity.Error, nameof(Fetch),
                        $"{_briefLogInfo}. Error while deserializing json. {ex.Message}");
                    return (false, default);
                }
            }
        }
    }
}
