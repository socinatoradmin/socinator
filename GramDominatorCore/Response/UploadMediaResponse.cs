using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
using GramDominatorCore.GDLibrary.Response;
using System;
using System.Linq;

namespace GramDominatorCore.Response
{
    public class UploadMediaResponse : IGResponseHandler
    {
        public UploadMediaResponse(IResponseParameter response,string ProfileId="",bool IsStory=false) : base(response)
        {
            if (!Success)
                return;
            try
            {
                var jsonObject = handler.ParseJsonToJObject(response.Response);
                ErrorMessage = handler.GetJTokenValue(jsonObject, "feedback_message");
                if (!string.IsNullOrEmpty(ErrorMessage)
                    ||(!string.IsNullOrEmpty(response?.Response) && response.Response.Contains("\"status\":\"fail\"")))
                {
                    Success = false;
                    return;
                }
                if (IsStory)
                {
                    var token = handler.GetJTokenOfJToken(jsonObject, "media");
                    MediaId = handler.GetJTokenValue(token, "pk");
                    PostId = handler.GetJTokenValue(token, "id");
                    Code = handler.GetJTokenValue(token, "code");
                    MediaType = (MediaType)Enum.Parse(typeof(MediaType), handler.GetJTokenValue(token, "media_type"));
                    var username = handler.GetJTokenValue(token, "user", "username");
                    StoryUrl = $"https://www.instagram.com/stories/{username}/{MediaId}/";
                    if (MediaType == MediaType.Video)
                    {
                        double.TryParse(handler.GetJTokenValue(token, "video_duration"), out double videoDuration);
                        VideoDuration = videoDuration;
                    }
                    int.TryParse(handler.GetJTokenValue(token, "taken_at"), out int takenAt);
                    TakenAt = takenAt;
                }
                else
                {
                    var SharedNodes = handler.GetJArrayElement(handler.GetJTokenValue(jsonObject, "ranked_recipients"));
                    if (SharedNodes != null && SharedNodes.Count > 0)
                    {
                        var CurrentSharedNode = SharedNodes.FirstOrDefault(x => x.ToString().Contains(ProfileId));
                        if (CurrentSharedNode is null)
                            CurrentSharedNode = SharedNodes.FirstOrDefault();
                        Code = handler.GetJTokenValue(CurrentSharedNode, "user", "interop_messaging_user_fbid");
                        Success = !string.IsNullOrEmpty(Code);
                        return;
                    }
                    var token = handler.GetJTokenOfJToken(jsonObject, "media");
                    MediaId = handler.GetJTokenValue(token, "pk");
                    PostId = handler.GetJTokenValue(token, "id");
                    Code = handler.GetJTokenValue(token, "code");
                    MediaType = (MediaType)Enum.Parse(typeof(MediaType), handler.GetJTokenValue(token, "media_type"));

                    if (MediaType == MediaType.Video)
                    {
                        double.TryParse(handler.GetJTokenValue(token, "video_duration"), out double videoDuration);
                        VideoDuration = videoDuration;
                    }
                    int.TryParse(handler.GetJTokenValue(token, "taken_at"), out int takenAt);
                    TakenAt = takenAt;
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
        public string ErrorMessage { get; set; }
        public string MediaId { get; set; }

        public string PostId { get; set; }

        public string Code { get; set; }

        public MediaType MediaType { get; set; }

        public int TakenAt { get; set; }
        public string StoryUrl { get; set; }
        public double VideoDuration { get; set; }
    }
}
