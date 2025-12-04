using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace QuoraDominatorCore.QdUtility
{
    public class QdCreatePostPostData
    {
        [JsonProperty(PropertyName = "queryName")]
        public string queryName { get; set; }
        [JsonProperty(PropertyName = "variables")]
        public CreatePostVariable variables { get; set; }
        [JsonProperty(PropertyName = "extensions")]
        public Extensions extensions { get; set; }
    }
    public class Extensions
    {
        [JsonProperty(PropertyName = "hash")]
        public string hash { get; set; }
    }

    public class CreatePostVariable
    {
        [JsonProperty(PropertyName = "tribeId")]
        public int tribeId { get; set; }
        [JsonProperty(PropertyName = "title")]
        public string title { get; set; }
        [JsonProperty(PropertyName = "content")]
        public string content { get; set; }
        [JsonProperty(PropertyName = "isAutoSaved")]
        public bool isAutoSaved { get; set; }
        [JsonProperty(PropertyName = "isNullspacePost")]
        public bool isNullspacePost { get; set; }
    }
    public class QdUploadPostPostData
    {
        [JsonProperty(PropertyName = "queryName")]
        public string queryName { get; set; }
        [JsonProperty(PropertyName = "variables")]
        public UploadPostVariable variables { get; set; }
        [JsonProperty(PropertyName = "extensions")]
        public Extensions extensions { get; set; }
    }
    public class UploadPostVariable
    {
        [JsonProperty(PropertyName = "tribeId")]
        public int tribeId { get; set; }
        [JsonProperty(PropertyName = "title")]
        public string title { get; set; }
        [JsonProperty(PropertyName = "content")]
        public string content { get; set; }
        [JsonProperty(PropertyName = "shouldQueue")]
        public bool shouldQueue { get; set; }
        [JsonProperty(PropertyName = "credentialId")]
        public string credentialId { get; set; }
        [JsonProperty(PropertyName = "accessOption")]
        public string accessOption { get; set; }
        [JsonProperty(PropertyName = "captcha")]
        public string captcha { get; set; }
    }
}
