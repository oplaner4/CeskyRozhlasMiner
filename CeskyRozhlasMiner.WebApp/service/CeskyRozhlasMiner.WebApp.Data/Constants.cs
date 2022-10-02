namespace Microsoft.DSX.ProjectTemplate.Data
{
    public static class Constants
    {
        public static class Environments
        {
            public const string Local = "local";
            public const string Test = "test";
            public const string Dev = "dev";
            public const string Development = "development";
        }

        public static class MaximumLengths
        {
            public const int StringColumn = 512;

            public const int MaxDaysPlaylistRange = 10;
        }

        public static class MimimumLengths
        {
            public const int Password = 8;
        }
    }
}
