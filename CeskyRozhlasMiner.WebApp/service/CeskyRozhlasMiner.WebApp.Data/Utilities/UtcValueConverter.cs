using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;

namespace CeskyRozhlasMiner.WebApp.Data.Utilities
{
    public class UtcValueConverter : ValueConverter<DateTime, DateTime>
    {
        public UtcValueConverter() : base(v => v, v => DateTime.SpecifyKind(v, DateTimeKind.Utc))
        {
        }
    }
}
