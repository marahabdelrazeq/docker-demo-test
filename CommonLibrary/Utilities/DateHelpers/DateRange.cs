namespace CommonLibrary.Utilities.DateHelpers
{
    /// <summary>
    /// Represents a range of dates with a start and end date.
    /// </summary>
    public class DateRange
    {
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }

        /// <summary>
        /// Initializes a new instance of the DateRange class using DateTime objects.
        /// </summary>
        /// <param name="startDate">The start date.</param>
        /// <param name="endDate">The end date.</param>
        public DateRange(DateTime startDate, DateTime endDate)
        {
            StartDate = DateOnly.FromDateTime(startDate);
            EndDate = DateOnly.FromDateTime(endDate);
        }

        /// <summary>
        /// Initializes a new instance of the DateRange class using DateOnly objects.
        /// </summary>
        /// <param name="startDate">The start date.</param>
        /// <param name="endDate">The end date.</param>
        public DateRange(DateOnly startDate, DateOnly endDate)
        {
            StartDate = startDate;
            EndDate = endDate;
        }
    }
}

