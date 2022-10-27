namespace Microsoft.DSX.ProjectTemplate.Data.DTOs
{
    public class StatisticsEntryDto
    {
        public StatisticsEntryDto(string value, int count)
        {
            Value = value;
            Count = count;
        }

        public string Value { get; set; }
        public int Count { get; set; }
    }
}
