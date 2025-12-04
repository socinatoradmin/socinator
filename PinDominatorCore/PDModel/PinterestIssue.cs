using PinDominatorCore.PDEnums;

namespace PinDominatorCore.PDModel
{
    public class PinterestIssue
    {
        public bool ChangeStatus { get; set; }

        public PinterestError Error { get; set; }

        public string Message { get; set; }

        public string Status { get; set; }
    }
}