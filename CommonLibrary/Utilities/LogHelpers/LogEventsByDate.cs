namespace CommonLibrary.Utilities.LogHelpers
{
    public class LogEventsByDate<T> where T : class
    {
        public long Date { get; set; }
        public List<T> Events { get; set; }
    }
    public static class EventRecordExtensions
    {
        public static IEnumerable<LogEventsByDate<T>> GroupByDay<T>(this IEnumerable<T> events, Func<T, DateTime> dateSelector) where T : class
        {
            return events
                .GroupBy(e => dateSelector(e).Date)
                .Select(group => new LogEventsByDate<T>
                {
                    Date = new DateTimeOffset(group.Key).ToUnixTimeSeconds(),
                    Events = group.ToList()
                })
                .ToList();
        }
    }
}
