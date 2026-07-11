namespace CommonLibrary.Utilities.DateHelpers
{
    public static class DayOfWeekBitmaskHelper
    {
        /// <summary>
        /// Provides bitmask values for each day of the week.
        /// Example: Sunday = 1, Monday = 2, etc.
        /// </summary>
        public static readonly Dictionary<DayOfWeek, int> DayOfWeekBitmasks = new Dictionary<DayOfWeek, int>
        {
            { DayOfWeek.Sunday, 1 << 0 },
            { DayOfWeek.Monday, 1 << 1 },
            { DayOfWeek.Tuesday, 1 << 2 },
            { DayOfWeek.Wednesday, 1 << 3 },
            { DayOfWeek.Thursday, 1 << 4 },
            { DayOfWeek.Friday, 1 << 5 },
            { DayOfWeek.Saturday, 1 << 6 }
        };

        /// <summary>
        /// Returns the bitmask for the provided DayOfWeek.
        /// </summary>
        public static int GetBitmaskForDay(DayOfWeek dayOfWeek)
        {
            return DayOfWeekBitmasks.ContainsKey(dayOfWeek) ? DayOfWeekBitmasks[dayOfWeek] : 0;
        }
    }
}
