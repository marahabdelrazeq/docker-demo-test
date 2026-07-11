namespace CommonLibrary.Responses
{
    /// <summary>
    /// The class for wrapping response objects.
    /// </summary>
    /// <typeparam name="T">Wrapped object type</typeparam>
    public class ApiResponse<T>
    {
        /// <summary>
        /// Indicates if the response is successful (no errors).
        /// </summary>
        public bool Ok => Errors is null; // If Errors is null, the response is successful

        /// <summary>
        /// Data returned in the response, can be a paginated or non-paginated object.
        /// </summary>
        public dynamic Data { get; set; }

        /// <summary>
        /// A list of errors if the response failed.
        /// </summary>
        public List<Error> Errors { get; set; }


        /// <summary>
        /// Constructor for paginated data response.
        /// </summary>
        public ApiResponse((IEnumerable<T>, int) data, List<Error> errors = null)
        {
            Data = new PaginatedResponse<T>(data);
            Errors = errors;
        }

        /// <summary>
        /// Constructor for non-paginated data response.
        /// </summary>
        public ApiResponse(T data, List<Error> errors = null)
        {
            Data = data;
            Errors = errors;
        }

       
    }
}
