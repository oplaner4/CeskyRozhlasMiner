using RadiozurnalMiner.Lib.Common;
using System;
using System.IO;

namespace RadiozurnalMiner.Lib.Diagnostics
{
    public class Logging
    {
        public enum Severity
        {
            Error,
            Warning,
            Success
        }

        public static async void SaveRecord(
            Severity severity, string title, string message)
        {
            string row =
                $"{DateTime.Now}{Settings.CsvSeparator}{severity}{Settings.CsvSeparator}" +
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
