namespace CommonLibrary.Pagination
{
    /// <summary>
    /// Provides functionality for filtering, paginating, and sorting grid data.
    /// </summary>
    /// <typeparam name="TFilter">The type used for filtering data.</typeparam>
    public class GridDataFilter<TFilter> where TFilter : class
    {
        private int _pageIndex = 1; // Default page index value
        private string _sort = "ID DESC"; // Default sorting order

        /// <summary>
        /// The filter data to be applied to the grid.
        /// </summary>
        public TFilter FilterData { get; set; }

        /// <summary>
        /// The page index for pagination. Defaults to 1 if set to 0.
        /// </summary>
        public int PageIndex
        {
            get => _pageIndex;
            set => _pageIndex = value == 0 ? 1 : value; // If set to 0, default to 1
        }

        /// <summary>
        /// The number of records per page.
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// The sorting order for the data, e.g., "PropertyName ASC" or "PropertyName DESC".
        /// </summary>
        public string Sort
        {
            get => _sort;
            set
            {
                // Validate if the sort string ends with " ASC" or " DESC"
                if (string.IsNullOrWhiteSpace(value) ||
                    (!value.EndsWith(" ASC", StringComparison.OrdinalIgnoreCase) &&
                     !value.EndsWith(" DESC", StringComparison.OrdinalIgnoreCase)))
                {
                    _sort = "ID DESC"; // Default sort order
                }
                else
                {
                    _sort = value.Trim();
                }
            }
        }
    }
}
