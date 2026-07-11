namespace CommonLibrary.Responses
{
    /// <summary>
    /// Class for paginated responses.
    /// </summary>
    /// <typeparam name="T">Type of the paginated data</typeparam>
    public class PaginatedResponse<T>
    {
        /// <summary>
        /// The list of results.
        /// </summary>
        public IEnumerable<T> Results { get; set; }

        /// <summary>
        /// Total number of records for the paginated results.
        /// </summary>
        public int? TotalRecords { get; set; }

        /// <summary>
        /// Constructor for initializing paginated response.
        /// </summary>
        public PaginatedResponse((IEnumerable<T>, int) data)
        {
            TotalRecords = data.Item2;
            Results = data.Item1;
        }
    }
}
