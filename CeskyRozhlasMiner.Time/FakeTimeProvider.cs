using System;

namespace CeskyRozhlasMiner.Time
{
    public class FakeTimeProvider : ITimeProvider
    {
        public FakeTimeProvider(DateTime fakeUtcNow)
        {
            UtcNow = fakeUtcNow;
        }

        public DateTime UtcNow { get; set; }
    }
}