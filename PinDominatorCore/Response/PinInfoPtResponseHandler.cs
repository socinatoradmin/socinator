using System;
using System.Linq;
using System.Text.RegularExpressions;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using Newtonsoft.Json.Linq;
using PinDominatorCore.PDModel;
using PinDominatorCore.PDUtility;
using PinDominatorCore.Utility;

namespace PinDominatorCore.Response
{
    public class PinInfoPtResponseHandler : PdResponseHandler
    {
        public PinInfoPtResponseHandler(IResponseParameter response, string pinId) : base(response)
        {
            try
            {
                if (Success == false)
                    return;
                var JsonResponse = PdRequestHeaderDetails.GetJsonString(response.Response,true);
                var jObject = handler.ParseJsonToJObject(JsonResponse);
                JToken profileObject = null;
                try
                {
                    foreach (var jToken in handler.GetJTokenOfJToken(jObject,"pins"))
                    {
                        profileObject = jToken.First();
                        if (handler.GetJTokenValue(profileObject, "id").Equals(pinId)) break;
                    }
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }

                try
                {
                    profileObject = profileObject ??
                                    handler.GetJTokenOfJToken(
                                        handler.GetJTokenOfJToken(jObject,"resources", "data", "PinPageResource")?.First()?.First(),
                                        "data");
                }
                catch (Exception)
                {
                    profileObject = handler.GetJTokenOfJToken(handler.GetJTokenOfJToken(jObject,"resource_response","data"));
                    profileObject =profileObject==null || !profileObject.HasValues ? handler.GetJTokenOfJToken(handler.GetJTokenOfJToken(jObject, "initialReduxState", "pins")?.First()?.First()):profileObject;
                }

                if (profileObject != null)
                {
                    var pinData = handler.GetJTokenOfJToken(profileObject, "aggregated_pin_data");
                    var tryData = handler.GetJTokenOfJToken(pinData, "did_it_data");
                    var tryCount = handler.GetJTokenValue(tryData, "user_count");
                    int n;
                    int.TryParse(tryCount, out n);
                    TriedCount = n;
                    var commentCount = handler.GetJTokenValue(pinData, "comment_count");
                    commentCount = string.IsNullOrEmpty(commentCount) ?PdUtility.RemoveHtmlTags(Utilities.GetBetween(response.Response, "<h2 class=\"lH1 dyH iFc H2s bwj O2T zDA IZT\">", "comments"))?.Trim() : commentCount;
                    int.TryParse(Regex.Replace(commentCount,"[^0-9]",""), out n);
                    CommentCount = n;
                    Description = handler.GetJTokenValue(profileObject, "closeup_unified_description");
                    Description =string.IsNullOrEmpty(Description)? handler.GetJTokenValue(profileObject, "rich_metadata", "description"):Description;
                    Description = string.IsNullOrEmpty(Description) ?PdUtility.RemoveHtmlTags(Utilities.GetBetween(response.Response, "<div class=\"tBJ dyH iFc sAJ O2T zDA IZT swG CKL\"", "</span></div>")): Description;
                    if (string.IsNullOrEmpty(Description))
                        Description = handler.GetJTokenValue(profileObject, "description");
                    PinWebUrl = handler.GetJTokenValue(profileObject, "tracked_link");
                    PinWebUrl = string.IsNullOrEmpty(PinWebUrl) ?PdUtility.RemoveHtmlTags(Utilities.GetBetween(response.Response, "<a class=\"Wk9 xQ4 CCY S9z eEj iyn kVc e8F BG7\" href=\"", "\" rel=\"")): PinWebUrl;
                    PinName = handler.GetJTokenValue(profileObject, "title");
                    PinName = string.IsNullOrEmpty(PinName) ? handler.GetJTokenValue(profileObject, "closeup_unified_title") : PinName;
                    PinName = string.IsNullOrEmpty(PinName) ?PdUtility.RemoveHtmlTags(Utilities.GetBetween(response.Response, "<h1 class=\"lH1 dyH iFc H2s GTB O2T zDA IZT\">", "</h1>")): PinName;
                    var boardData = handler.GetJTokenOfJToken(profileObject, "board");
                    BoardId = handler.GetJTokenValue(boardData, "id");
                    BoardName = handler.GetJTokenValue(boardData, "name");

                    var userData = handler.GetJTokenOfJToken(profileObject, "origin_pinner");
                    userData = userData == null || !userData.HasValues ? handler.GetJTokenOfJToken(profileObject, "pinner") : userData;
                    UserName = handler.GetJTokenValue(userData, "username");
                    UserName = string.IsNullOrEmpty(UserName) ?PdUtility.RemoveHtmlTags(Utilities.GetBetween(response.Response, "<div class=\"tBJ dyH iFc j1A O2T zDA IZT H2s\">", "</div>")): UserName;
                    UserId = handler.GetJTokenValue(userData, "id");
                    UserId = string.IsNullOrEmpty(UserId) ?PdUtility.RemoveHtmlTags(Utilities.GetBetween(Utilities.GetBetween(response.Response, "<div data-test-id=\"creator-avatar\"", "<div data-test-id=\"creator-profile-name\""),"href=\"/","/\"")): UserId;
                    var isVideo = handler.GetJTokenOfJToken(profileObject, "videos");
                    if (isVideo.HasValues)
                    {
                        MediaType = MediaType.Video;
                        MediaString = handler.GetJTokenValue(profileObject, "videos", "video_list", "V_720P", "url");
                    }
                    else
                    {
                        MediaType = MediaType.Image;
                        MediaString = handler.GetJTokenValue(profileObject, "images", "564x", "url");
                    }
                    MediaString = string.IsNullOrEmpty(MediaString) ?PdUtility.RemoveHtmlTags(Utilities.GetBetween(Utilities.GetBetween(response.Response, "class=\"hCL kVc L4E MIw\"", "</div>"),"src=\"","\"")): MediaString;
                    AggregatedPinId = handler.GetJTokenValue(profileObject, "aggregated_pin_data", "id");
                    PublishDate = handler.GetJTokenValue(profileObject, "created_at");
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            PinId = pinId;
        }

        public string PinId { get; set; }
        public string Description { get; set; }
        public string MediaString { get; set; }
        public int CommentCount { get; set; }
        public string BoardName { get; set; }
        public string BoardId { get; set; }
        public string UserName { get; set; }
        public string PublishDate { get; set; }
        public string PinName { get; set; }
        public string UserId { get; set; }
        public string PinWebUrl { get; set; }
        public string Section { get; set; }
        public int TriedCount { get; set; }
        public string AggregatedPinId { get; set; }
        public MediaType MediaType { get; set; }

        public static implicit operator PinterestPin(PinInfoPtResponseHandler pin)
        {
            var pinterestPin = new PinterestPin
            {
                PinId = pin.PinId,
                Id = pin.PinId,
                PinWebUrl = pin.PinWebUrl,
                CommentCount = pin.CommentCount,
                Description = pin.Description,
                BoardName = pin.BoardName,
                BoardUrl = pin.BoardId,
                MediaType = pin.MediaType,
                User = {Username = pin.UserName, UserId = pin.UserId},
                PinName = pin.PinName,
                PublishDate = pin.PublishDate,
                MediaString = pin.MediaString,
                Section = pin.Section,
                NoOfTried = pin.TriedCount,
                AggregatedPinId =pin.AggregatedPinId
            };
            return pinterestPin;
        }
    }
}