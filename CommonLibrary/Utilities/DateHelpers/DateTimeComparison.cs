namespace CommonLibrary.Utilities.DateHelpers
{
    /// <summary>
    /// Provides utility methods for date-time comparison based on the operating system.
    /// </summary>
    public static class DateTimeComparison
    {
        /// <summary>
        /// Returns a date-time comparison string based on the operating system (Linux or others).
        /// </summary>
        /// <returns>A formatted date-time string for comparison.</returns>
        public static string GetDateTimeComparisonString()
        {
            return OperatingSystem.IsLinux()
                ? "01/01/0001 00:00:00"
                : "1/1/0001 12:00:00 AM";
        }
    }
}
