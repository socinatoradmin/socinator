using QuoraDominatorCore.Enums;

namespace QuoraDominatorCore.Models
{
    public class QuoraIssue
    {
        public bool ChangeStatus { get; set; }

        public QuoraError? Error { get; set; }

        public HealthState? Health { get; set; }

        public string Message { get; set; }

        public string Status { get; set; }
    }
}