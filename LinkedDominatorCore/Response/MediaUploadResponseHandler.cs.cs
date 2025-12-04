using System.Collections.Generic;

namespace LinkedDominatorCore.Response
{
    public class MediaUploadResponseHandler
    {
        public UploadType Type {  get; set; } = UploadType.SINGLE;
        public string SingleUploadUrl {  get; set; }
        public string StatusUrl {  get; set; }
        public List<MultiPartData> MultiUploadData { get; set; }=new List<MultiPartData>();
    }
    public class MultiPartData
    {
        public string UploadUrl { get; set; }
        public string ContentType {  get; set; }
        public string type {  get; set; }
        public int lastByte {  get; set; }
        public int firstByte {  get; set; }
        public long urlExpire {  get; set; }
    }
    public enum UploadType
    {
        MULTIPART = 0,
        SINGLE = 1
    }
}
