namespace DominatorHouseCore.Process
{
    public class JobProcessResult
    {
        /// <summary>
        ///     true if the running process with queries/non-queries is completed at some time
        /// </summary>
        public bool IsProcessCompleted { get; set; }

        /// <summary>
        ///     true if an action(like, comment etc) is succeeded
        /// </summary>
        public bool IsProcessSuceessfull { get; set; }

        /// <summary>
        ///     Set true if the query/non-query result have no more result
        /// </summary>
        public bool HasNoResult { get; set; }

        /// <summary>
        ///     get/set a paginationToken or a continuetionToken for getting more results(data)
        /// </summary>
        public string maxId { get; set; }
        public int FailedCount {  get; set; }
        public string RankToken { get; set; }
        public string SecondMaxID {  get; set; }
    }
}