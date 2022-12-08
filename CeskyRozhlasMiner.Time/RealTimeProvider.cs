using System;

namespace CeskyRozhlasMiner.Time
{
    public class RealTimeProvider : ITimeProvider
    {
        public DateTime UtcNow => DateTime.UtcNow;
    }
}