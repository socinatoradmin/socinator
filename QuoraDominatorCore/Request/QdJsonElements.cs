using Newtonsoft.Json;
using System.Collections.Generic;

namespace QuoraDominatorCore.Request
{
    public class QdJsonElements
    {
        [JsonProperty(PropertyName = "formkey")]
        public string Formkey { get; set; }

        [JsonProperty(PropertyName = "postkey")]
        public string Postkey { get; set; }

        [JsonProperty(PropertyName = "window_id")]
        public string WindowId { get; set; }

        [JsonProperty(PropertyName = "parent_domid")]
        public string ParentDomId { get; set; }

        [JsonProperty(PropertyName = "parent_cid")]
        public string ParentCid { get; set; }
        [JsonProperty(PropertyName = "args")] public string[] Args { get; set; }

        [JsonProperty(PropertyName = "kwargs")]
        public QdJsonElements Kwargs { get; set; }

        [JsonProperty(PropertyName = "extra_data")]
        public QdJsonElements ExtraData { get; set; }
        [JsonProperty(PropertyName = "kind")] public string Kind { get; set; }

        [JsonProperty(PropertyName = "nid")] public string NID { get; set; }

        [JsonProperty(PropertyName = "paged_list_parent_cid")]
        public string PageListParentCid { get; set; }

   

        [JsonProperty(PropertyName = "filter_hashes")]
        public string[] FilterHashes { get; set; }

        [JsonProperty(PropertyName = "force_cid")]
        public string ForceCid { get; set; }

        [JsonProperty(PropertyName = "hashes")]
        public string[] Hashes { get; set; }

        [JsonProperty(PropertyName = "has_more")]
        public bool? HasMore { get; set; }

        [JsonProperty(PropertyName = "serialized_component")]
        public string SerializedComponent { get; set; }

        [JsonProperty(PropertyName = "auto_paged")]
        public bool? AutoPaged { get; set; }

        [JsonProperty(PropertyName = "enable_mobile_hide_content")]
        public bool? EnableMobileHideContent { get; set; }

        [JsonProperty(PropertyName = "new_page")]
        public bool? NewPage { get; set; } //

        [JsonProperty(PropertyName = "not_auto_paged")]
        public bool? NotAutoPaged { get; set; } //has_animation

        [JsonProperty(PropertyName = "auto_paged_offset")]
        public int? AutoPagedOffset { get; set; }

        [JsonProperty(PropertyName = "loading_text")]
        public string LoadingText { get; set; }

        [JsonProperty(PropertyName = "error_text")]
        public string ErrorText { get; set; }
    }
    public class Embed
    {
        [JsonProperty(PropertyName ="url")]
        public string Url { get; set; }
    }
    public class Modifiers
    {
        [JsonProperty(PropertyName ="image")]
        public string Image { get; set; }
        [JsonProperty(PropertyName ="embed")]
        public Embed Embed { get; set; }
    }
    public class Spans
    {
        [JsonProperty(PropertyName = "modifiers")]
        public Modifiers Modifiers { get; set; }
        [JsonProperty(PropertyName = "text")]
        public string Text { get; set; }
    }
    public class PostBody
    {
        [JsonProperty(PropertyName = "sections")]
        public List<PostBodyJsonElement> Sections { get; set; }
    }
    public class PostBodyJsonElement
    {
        [JsonProperty(PropertyName ="type")]
        public string Type { get; set; }
        [JsonProperty(PropertyName = "indent")]
        public int Indent { get; set; }
        [JsonProperty(PropertyName = "quoted")]
        public bool Quoted { get; set; }
        [JsonProperty(PropertyName = "is_rtl")]
        public bool IsRtl { get; set; }
        [JsonProperty(PropertyName = "spans")]
        public List<Spans> Spans { get; set; }
    }
}