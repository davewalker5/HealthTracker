namespace HealthTracker.Mvc.Models
{
    public abstract class PaginatedViewModelBase<T> where T : class
    {
        public IEnumerable<T> Entities { get; private set; }
        public int ResultsCount { get; set; }
        public int PageNumber { get; set; }
        public bool PreviousEnabled { get; private set; }
        public bool NextEnabled { get; private set; }
        public string Action { get; set; }
        public bool HasNoMatchingResults { get; set; }

        /// <summary>
        /// Set the list of entities to be exposed by this view model
        /// </summary>
        /// <param name="entities"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        public void SetEntities(IEnumerable<T> entities, int pageNumber, int pageSize)
        {
            var count = entities?.Count() ?? 0;
            Entities = entities;
            PageNumber = pageNumber;
            HasNoMatchingResults = count == 0;
            SetPreviousNextEnabled(count, pageNumber, pageSize);
        }

        /// <summary>
        /// Set the "previous/next" button enabled flags according to the
        /// following logic, where SZ is the page size:
        ///
        ///         Page    Previous    Next
        /// Count   Number  Enabled     Enabled
        ///
        /// 0       -       No          No
        /// = SZ    1       No          Yes   
        /// < SZ    1       No          No
        /// = SZ    > 1     Yes         Yes
        /// < SZ    > 1     Yes         No
        /// 
        /// </summary>
        /// <param name="count"/>
        /// <param name="pageNumber"/>
        /// <param name="pageSize"/>
        /// <returns></returns>
        protected void SetPreviousNextEnabled(int count, int pageNumber, int pageSize)
        {
            PreviousEnabled = (pageNumber > 1);
            NextEnabled = (count == pageSize);
        }
    }
}
