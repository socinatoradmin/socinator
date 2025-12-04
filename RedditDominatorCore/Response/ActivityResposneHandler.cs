namespace RedditDominatorCore.Response
{
    public class ActivityResposneHandler
    {
        #region Instance Creation.
        private static readonly object _lock = new object();
        private static volatile ActivityResposneHandler Instance;
        public static ActivityResposneHandler GetInstance
        {
            get
            {
                if (Instance == null)
                {
                    lock (_lock)
                    {
                        if (Instance == null)
                            Instance = new ActivityResposneHandler();
                    }
                }
                Instance.ResponseMessage = string.Empty;
                Instance.Status = false;
                return Instance;
            }
        }
        #endregion
        #region Property.
        public string ResponseMessage { get; set; } = string.Empty;
        public bool Status { get; set; } = true;
        #endregion
    }
}
