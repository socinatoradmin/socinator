#region

using ProtoBuf;

#endregion

namespace DominatorHouseCore.Models
{
    [ProtoContract]
    public class ManageReportVideosContentModel : ManageCommentModel
    {
        public int ReportOption { get; set; }
        public int ReportSubOption { get; set; }
        public int VideoTimestampPercentage { get; set; } = 0;
        public bool IsSpinTax { get; set; }
        public string ReportText {  get; set; }
    }
}