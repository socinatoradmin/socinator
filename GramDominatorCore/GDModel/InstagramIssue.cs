using GramDominatorCore.GDEnums;

namespace GramDominatorCore.GDModel
{
    public class InstagramIssue
    {
        public bool ChangeStatus { get; set; }

        public InstagramError? Error { get; set; }

        public HealthState? Health { get; set; }

        public string Message { get; set; }

        public string Status { get; set; }

        public string ApiPath { get; set; }

    }
}
