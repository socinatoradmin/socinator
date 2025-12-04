using TwtDominatorCore.TDEnums;

namespace TwtDominatorCore.TDModels
{
    public class TwitterIssue
    {
        public bool ChangeStatus { get; set; }

        public TwitterError Error { get; set; }

        public string Message { get; set; }

        public string Status { get; set; }
    }
}