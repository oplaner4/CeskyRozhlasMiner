using System;

namespace CeskyRozhlasMiner.Time
{
    public interface ITimeProvider
    {
        public DateTime UtcNow { get; }
    }
}