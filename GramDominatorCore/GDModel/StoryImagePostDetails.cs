using Newtonsoft.Json;

namespace GramDominatorCore.GDModel
{
    public class StoryImagePostDetails
    {
        public string upload_id { get; set; }
        public string image_compression { get; set; }
        public string retry_context { get; set; }
        public string media_type { get; set; }

        public string original_photo_pdq_hash { get; set; }

        public string[] xsharing_user_ids { get; set; }
    }

    public class ImageAlbumPostDetails
    {
        public string upload_id { get; set; }
        public string image_compression { get; set; }
        public string retry_context { get; set; }
        public string media_type { get; set; }

        public string is_sidecar { get; set; }

        public string[] xsharing_user_ids { get; set; }
    }
    public class AlbumVideoImagePostDetails
    {
        public string upload_id { get; set; }
        public string image_compression { get; set; }
        public string retry_context { get; set; }
        public string media_type { get; set; }

        public string is_sidecar { get; set; }
        public string original_photo_pdq_hash { get; set; }

        public string[] xsharing_user_ids { get; set; }
    }

    public class StoryVideoFeedDetails
    {
        public string upload_media_height { get; set; }

        public string[] xsharing_user_ids { get; set; }
        public string upload_media_width { get; set; }
        public string for_direct_story { get; set; }
        public string upload_media_duration_ms { get; set; }
        public string upload_id { get; set; }
        public string for_album { get; set; }
        public string retry_context { get; set; }

        public string media_type { get; set; }
    }

    public class VideoPostDetails
    {
        public string upload_id { get; set; }
        public string retry_context { get; set; }
        public string media_type { get; set; }
        public string[] xsharing_user_ids { get; set; }
        public string upload_media_duration_ms { get; set; }
        public string upload_media_height{ get; set; }
        public string upload_media_width { get; set; }
        public string is_fmp4 { get; set; }
        public string content_tags { get; set; }
        public string extract_cover_frame { get; set; }
        public string is_clips_video { get; set; }

    }

    public class WebVideoPostDetails
    {
        [JsonProperty("client-passthrough")]
        public string clientpassthrough { get; set; }
        public string is_clips_video { get; set; }
        public string is_sidecar { get; set; }
        public int media_type { get; set; }
        public bool for_album { get; set; }
        public string video_format { get; set; }
        public string upload_id { get; set; }
        public int upload_media_duration_ms { get; set; }
        public int upload_media_height { get; set; }
        public int upload_media_width { get; set; }
        public object video_transform { get; set; }
        public VideoEditParams video_edit_params { get; set; }
    }

    public class VideoEditParams
    {
        public int crop_height { get; set; }
        public int crop_width { get; set; }
        public int crop_x1 { get; set; }
        public int crop_y1 { get; set; }
        public bool mute { get; set; }
        public double trim_end { get; set; }
        public int trim_start { get; set; }
    }

    public class AlbumVideoPostDetails
    {
        public string upload_id { get; set; }
        public string retry_context { get; set; }
        public string media_type { get; set; }
        public string[] xsharing_user_ids { get; set; }
        public string upload_media_duration_ms { get; set; }
        public string upload_media_height { get; set; }
        public string upload_media_width { get; set; }
        public string is_sidecar { get; set; }
    }

    public class SendVideoMessage
    {
        public string upload_id { get; set; }
        public string retry_context { get; set; }
        public string media_type { get; set; }
        public string[] xsharing_user_ids { get; set; }
        public string upload_media_duration_ms { get; set; }
        public string upload_media_height { get; set; }
        public string upload_media_width { get; set; }
        public string direct_v2 { get; set; }
    }

    public class SendVideoThumbnailMessage
    {
        public string upload_id { get; set; }
        public string retry_context { get; set; }
        public string media_type { get; set; }
        public string[] xsharing_user_ids { get; set; }
        public string image_compression { get; set; }
    }

    public class ProfilePicture
    {
        public string upload_id { get; set; }
        public string media_type { get; set; }
        public string image_compression { get; set; }
    }
}
