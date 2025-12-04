namespace TwtDominatorCore.TDModels
{
    public class MessageId
    {
        public int ResendId { get; set; }

        public int MediaDataField => ResendId - 1;

        public int MediaUploadId { get; set; }
    }
}