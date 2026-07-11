namespace CommonLibrary.Utilities.DateHelpers
{
    /// <summary>
    /// Provides methods for processing and comparing date ranges.
    /// </summary>
    public class DateProcessor
    {
        /// <summary>
        /// Calculates the number of overlapping days between two date ranges.
        /// </summary>
        /// <param name="range1">The first date range.</param>
        /// <param name="range2">The second date range.</param>
        /// <returns>The number of overlapping days.</returns>
        public static int GetOverlappingDays(DateRange range1, DateRange range2)
        {
            // Find the latest start date
            DateOnly maxStart = range1.StartDate > range2.StartDate ? range1.StartDate : range2.StartDate;
            // Find the earliest end date
            DateOnly minEnd = range1.EndDate < range2.EndDate ? range1.EndDate : range2.EndDate;

            // Calculate the number of overlapping days
            int overlapDays = (minEnd.ToDateTime(TimeOnly.MinValue) - maxStart.ToDateTime(TimeOnly.MinValue)).Days + 1;

            // If there is no overlap, return 0
            return overlapDays > 0 ? overlapDays : 0;
        }

        /// <summary>
        /// Calculates the number of overlapping days between a date range and a set of workdays represented by a bitmask.
        /// </summary>
        /// <param name="range">The date range.</param>
        /// <param name="daysBitmask">A bitmask representing the days of the week.</param>
        /// <param name="reverse">If true, counts non-working days; otherwise, counts working days.</param>
        /// <returns>The number of overlapping days based on the bitmask.</returns>
        public static int GetOverlappingDays(DateRange range, int daysBitmask, bool reverse = false)
        {
            int overlapDays = 0;

            for (var date = range.StartDate; date <= range.EndDate; date = date.AddDays(1))
            {
                // Determine if the day is in the bitmask
                int dayOfWeekBit = 1 << (int)date.DayOfWeek;
                bool isDayMatching = (daysBitmask & dayOfWeekBit) != 0;

                // Count matching days or non-matching days based on reverse flag
                if (reverse && !isDayMatching || !reverse && isDayMatching)
                {
                    overlapDays++;
                }
            }

            return overlapDays;
        }
    }
}
