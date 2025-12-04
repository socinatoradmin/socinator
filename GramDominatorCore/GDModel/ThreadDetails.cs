using GramDominatorCore.GDUtility;

namespace GramDominatorCore.GDModel
{
    public class ThreadDetails
    {
        //public string ThreadId { get; set; }

        public InstagramUser InstagramUser { get; set; }

        //public bool Canonical { get; set; }

        public bool Named { get; set; }

        // public string ThreadTitle { get; set; }

        public bool Pending { get; set; }

        //  public string ThreadType { get; set; }

        public string ViewerId { get; set; }
    }
    public class ThreadIDDetails
    {
        public string ThreadId { get; set; }
        public ThreadIDDetails(string Response,bool IsWeb=false)
        {
            try
            {
                var handler = JsonJArrayHandler.GetInstance;
                var obj = handler.ParseJsonToJObject(Response);
                ThreadId = !IsWeb ? handler.GetJTokenValue(obj, "thread_id")
                    :handler.GetJTokenValue(obj, "messaging_thread_key");
            }
            catch { }
        }
        public ThreadIDDetails() { }
    }
}
