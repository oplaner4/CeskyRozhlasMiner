using CeskyRozhlasMiner.Lib.Common;
using CeskyRozhlasMiner.Time;
using System;
using System.IO;

namespace CeskyRozhlasMiner.Lib.Diagnostics
{
    /// <summary>
    /// Logging utility
    /// </summary>
    public class Logging
    {
        private readonly ITimeProvider _timeProvider;

        public enum Severity
        {
            Error,
            Warning,
            Success
        }

        /// <summary>
        /// Initializes utility.
        /// </summary>
        /// <param name="time">Injected time provider.</param>
        public Logging(ITimeProvider time)
        {
            _timeProvider = time;
        }

        public async void SaveRecord(
            Severity severity, string title, string message)
        {
            string row =
                $"{_timeProvider.UtcNow}{Settings.CsvSeparator}{severity}{Settings.CsvSeparator}" +
                $"{title}{Settings.CsvSeparator}{message}{Environment.NewLine}";

            if (File.Exists(Settings.LoggingCsvFile))
            {
                await File.AppendAllTextAsync(Settings.LoggingCsvFile, row);
            }
            else
            {
                await File.WriteAllTextAsync(Settings.LoggingCsvFile, row);
            }

        }
    }
}
