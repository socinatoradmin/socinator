using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using PinDominatorCore.PDModel;
using PinDominatorCore.Utility;
using System;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace PinDominatorCore.PDUtility
{
    public class PdConstants
    {
        #region Url End Points.
        public static string Https = "https://";
        public static string CreateBoardSectionResource = "/BoardSectionResource/create/";
        public static string ConversationsResource = "/resource/ConversationsResource/get/";
        public static string Ngjs = "/_ngjs";
        public static string CheckLoginStatusString = "Accounts and more options";
        public static string UserFollowersResourceGet = "/UserFollowersResource/get/";

        public static string UserFollowingsResourceGet = "/UserFollowingResource/get/";

        public static string BaseSearchResourceGet = "/BaseSearchResource/get/";

        public static string ContactRequestsResourceGet = "/_ngjs/resource/ContactRequestsResource/get/";

        public static string BoardInvitesResourceGet = "/_ngjs/resource/BoardInvitesResource/get/";

        public static string ContactRequestsCountGetResource = "/_ngjs/resource/ContactRequestsCountGetResource/get/";

        public static string ContactRequestAcceptResourceUpdate = "/resource/ContactRequestAcceptResource/update/";

        public static string BoardInviteAcceptResourceUpdate = "/resource/BoardInviteResource/update/";

        public static string BoardEmailInviteResourceCreate = "/resource/BoardInviteResource/create/";

        public static string BoardFollowersResourceGet = "/BoardFollowersResource/get/";

        public static string BoardFeedResourceGet = "/BoardFeedResource/get/";

        public static string BoardsResourceGet = "/BoardsResource/get/";

        public static string UserPinsResourceGet = "/UserPinsResource/get/";

        public static string AggregatedActivityFeedResourceCreate = "/AggregatedActivityFeedResource/get/";

        public static string ConversationsResourceGet = "/_ngjs/resource/ConversationsResource/get/";

        public static string UserFollowResourceCreate = "/UserFollowResource/create/";

        public static string BoardFollowResourceCreate = "/BoardFollowResource/create/";

        public static string UserFollowersResourceCreate = "/UserSessionResource/create/";

        public static string BoardsResourceCreate = "/BoardResource/create/";

        public static string RepinResourceCreate = "/_ngjs/resource/RepinResource/create/";

        public static string PinResourceCreate = "/PinResource/create/";

        public static string AggregatedCommentResourceCreate = "/_ngjs/resource/AggregatedCommentResource/create/";

        public static string AggregatedCommentLikeResourceCreate = "/AggregatedCommentLikeResource/create/";

        public static string DidItImageUploadResourceCreate = "/resource/DidItImageUploadResource/create/";

        public static string DidItActivityResourceCreate = "/resource/DidItActivityResource/create/";

        public static string ConversationsResourceCreate = "/_ngjs/resource/ConversationsResource/create/";

        public static string UserResetPasswordResourceCreate = "/resource/UserResetPasswordResource/create/";

        public static string UserFollowResourceDelete = "/_ngjs/resource/UserFollowResource/delete/";

        public static string PinResourceUpdate = "/resource/PinResource/update/";

        public static string BoardFollowResourceDelete = "/BoardFollowResource/delete/";

        public static string PinResourceDelete = "/PinResource/delete/";

        public static string Resource = "/resource";

        public static string Following = "/following";

        public static string Followers = "/followers";

        public static string Pin = "/pin";

        public static string UploadImage = "/upload-image/?img=";
        public static string UserRegisterCrete = "/UserRegisterResource/create/";

        //Select gender post url at create account time
        public static string UserSettingResourceupdate = "/UserSettingsResource/update/";

        public static string UserStateResourcecreate = "/UserStateResource/create/ ";

        public static string OrientationContaxtResource = "/OrientationContextResource/create/";

        public static string ConversationContaxtResource = "/resource/ConversationsResource/create/";
        #endregion

        #region Json Data.
        public static string ImageUrl = "\"image_url\":\"";

        public static string ObjectId = "\"object_id_str\":\"";

        public static string ApplicationJsonDoubleInitialStateSingle = "<script type=\"application/json\" id='initial-state'>";

        public static string ApplicationJsonDoubleInitialStateDouble = "<script type=\"application/json\" id=\"initial-state\">";

        public static string InitialStateDoubleApplicationJsonDouble = "<script id=\"initial-state\" type=\"application/json\">";

        public static string ApplicationJsonDoubleJsInitSingle = "<script type=\"application/json\" id='jsInit1'>";

        public static string ApplicationJsonDoubleJsInitDouble = "<script type=\"application/json\" id=\"jsInit1\">";

        public static string WindowJsonString = "window.__INITIAL_STATE__ = ";

        public static string Script = "</script>";
        public static string ScriptDouble = "</script><script";
        public static string UpdatedJsonPWSData = "script id=\"__PWS_DATA__\" type=\"application/json\">";
        public static string NewJsonPwsData = "script id=\"__PWS_INITIAL_PROPS__\" type=\"application/json\">";
        public static string PDsourceUrl = "source_url=%2F";

        public static string UserName = "\"username\": \"";

        public static string RequestIdentifier = "{\"request_identifier";

        public static string Name = "\"name\":\"";

        public static string BoardCount = "\"board_count\":";

        public static string BookMark = "\"bookmarks\":[\"";

        public static string Id = "\"id\": \"";
        public static string GetPaginationID(string PaginationToken) => string.IsNullOrEmpty(PaginationToken) ?
            $",\"bookmarks\":[\"{PaginationToken}\"]" : string.Empty;
        #endregion

        #region Others.
        public static string EncodeString(string Response, bool UrlEncode = true, bool HtmlEncode = false) =>
            string.IsNullOrEmpty(Response) ? Response : UrlEncode && HtmlEncode ? WebUtility.HtmlEncode(WebUtility.UrlEncode(Response)) :
            UrlEncode ? WebUtility.UrlEncode(Response) : WebUtility.HtmlEncode(Response);
        public static string AccountReport = "AccountReport";

        public static string DownloadImagePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

        public static string UserAgent = "Mozilla/5.0 (Windows NT 6.3; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/81.0.4044.122 Safari/537.36";
        public static string GetTicks => DateTime.Now.Ticks.ToString();
        public static bool IsBusinessAccount(DominatorAccountModel accountModel) => accountModel == null ? false : accountModel.DisplayColumnValue11 == "Normal Mode" ? false : true;
        #endregion

        #region Post Data.
        public static string GetaccountcreaterPostData(CreateAccountInfo info, string browsecountry,string firstname)
       => "{\"options\":{\"container\":\"home_page\",\"email\":\"" + info.Email + "\",\"password\":\"" + info.Password + "\",\"age\":\"" + info.Age + "\",\"country\":\"" + browsecountry + "\",\"signupSource\":\"homePage\",\"first_name\":\"" + firstname + "\",\"last_name\":\"\",\"hybridTier\":\"open\",\"page\":\"home\",\"no_fetch_context_on_resource\":false},\"context\":{}}";

        
        public static string GetCustomBoardFollowPostData(string board,string boardId)
        {
            return
                $"source_url=/{board}/&data={{\"options\":{{\"board_id\":\"{boardId}\"}},\"context\":{{}}}}";
        }
        public static string GetUploadMediaPostData(string BoardId,
            string MediaSignature, string Description = "", string DestinationLink = "", string Title = "",string SectionId="")
        {
            return
                $"source_url=/pin-builder/&data={{\"options\":{{\"field_set_key\":" +
                $"\"create_success\",\"skip_pin_create_log\":true{GetSectionData(sectionId:SectionId,IsVideo:false)},\"board_id\":" +
                $"\"{BoardId}\",\"image_signature\":\"{MediaSignature}\",\"description\"" +
                $":\"{Description}\",\"link\":\"{DestinationLink}\",\"title\":\"{Title}\",\"method\"" +
                $":\"uploaded\",\"upload_metric\":{{\"source\":\"pinner_upload_standalone\"}}" +
                $",\"user_mention_tags\":[]}},\"context\":{{}}}}";
        }
        public static string GetPinDeletePostData(string PinID, string ClientTrackingParams)
        {
            return
                $"source_url=%2Fpin%2F{PinID}%2F&data=%7B%22options%22%3A%7B%22id%22%3A%22{PinID}%22%2C%22client_tracking_params%22%3A%22{ClientTrackingParams}%22%7D%2C%22context%22%3A%7B%7D%7D";
        }
        public static string GetRepinPostData(string PinID, string ClientTrackingParams, string Description, string Link, string BoardID, string Title, bool IsBuyablePin = false, bool IsRemovable = false,string SectionId="",string storyPinId="")
        {
            return
                $"source_url=%2Fpin%2F{PinID}%2F&data="+WebUtility.UrlEncode($"{{\"options\":{{\"carousel_slot_index\":0,\"clientTrackingParams\":\"{ClientTrackingParams}\",\"description\":\"{Description}\",\"is_buyable_pin\":\"{IsBuyablePin}\",\"is_promoted\":\"{IsRemovable}\",\"is_removable\":\"{IsRemovable}\",\"link\":\"{Link}\",\"pin_id\":\"{PinID}\",\"title\":\"{Title}\",\"board_id\":\"{BoardID}\"{GetSectionData(SectionId, IsVideo: false)},\"commerce_data\":\"{{\\\"story_pin_id\\\":\\\"{storyPinId}\\\"}}\"}},\"context\":{{}}}}");
        }
        public static string GetPinCreatePostData { get; set; } = "source_url=%2Fpin-builder%2F&data=%7B%22options%22%3A%7B%22type%22%3A%22pinimage%22%7D%2C%22context%22%3A%7B%7D%7D";
        public static string GetThumnailUploadPostData => $"source_url=/pin-creation-tool/&data={{\"options\":{{\"url\":\"/v3/media/uploads/register/batch/\",\"data\":{{\"media_info_list\":\"[{{\\\"id\\\":\\\"{Guid.NewGuid().ToString()}\\\",\\\"media_type\\\":\\\"image-story-pin\\\"}},{{\\\"id\\\":\\\"{Guid.NewGuid().ToString()}\\\",\\\"media_type\\\":\\\"image-story-pin\\\"}}]\"}},\"context\":{{}}}}}}";
        public static string GetIdeaPinUploadParameterPostData(string MediaPath) =>
            $"source_url=/pin-creation-tool/&data={{\"options\":{{\"url\":\"/v3/media/uploads/register/batch/\",\"data\":{{\"media_info_list\":\"[{{\\\"id\\\":\\\"{Guid.NewGuid().ToString()}\\\",\\\"media_type\\\":\\\"video-story-pin\\\",\\\"upload_aux_data\\\":{{\\\"clips\\\":[{{\\\"durationMs\\\":{PdUtility.GetVideoDuration(MediaPath)},\\\"isFromImage\\\":false,\\\"startTimestampMs\\\":-1}}]}}}}]\"}},\"context\":{{}}}}}}";
            //$"source_url=/idea-pin-builder/&data={{\"options\":{{\"url\":\"/v3/media/uploads/register/batch/\",\"data\":{{\"media_info_list\":\"[{{\\\"id\\\":\\\"{Guid.NewGuid().ToString()}\\\",\\\"media_type\\\":\\\"image-story-pin\\\"}},{{\\\"id\\\":\\\"{Guid.NewGuid().ToString()}\\\",\\\"media_type\\\":\\\"image-story-pin\\\"}}]\"}}}},\"context\":{{}}}}";
        public static string GetSectionCreatePostData(string profileId,string boardName,string boardId,string sectionName) => $"source_url=/{profileId}/{boardName}/&data={{\"options\":{{\"board_id\":\"{boardId}\",\"initial_pins\":[],\"name\":\"{sectionName}\",\"name_source\":0}},\"context\":{{}}}}";
        public static string GetBookMarkData(string paginationToken) => $",\"bookmarks\":[\"{paginationToken}\"]";
        public static string GetSectionData(string sectionId,bool IsVideo=false)=>string.IsNullOrEmpty(sectionId) ?string.Empty:IsVideo? $",\"board_section_id\":\"{sectionId}\"" : $",\"section\":\"{sectionId}\"";
        public static string GetEditPinPostBody(string PinID,string Title,string Description,string BoardID,string SectionID,string WebsiteUrl,string CommentEnabled,string RecommendationEnbaled) => $"source_url=%2Fpin%2F{PinID}%2F&data=" + WebUtility.UrlEncode($"{{\"options\":{{\"alt_text\":\"Fun Time\",\"board_id\":\"{BoardID}\"{GetSectionData(SectionID,true)},\"description\":\"{Description}\",\"disable_comments\":{CommentEnabled},\"disable_did_it\":false,\"id\":\"{PinID}\",\"link\":\"{WebsiteUrl}\",\"shopping_rec_disabled\":{RecommendationEnbaled},\"title\":\"{Title}\",\"user_mention_tags\":\"[]\"}},\"context\":{{}}}}");
        public static string GetPinCommentPostBody(string PinID, string ObjectId, string CommentText)
            => $"source_url=%2Fpin%2F{PinID}%2F&data={WebUtility.UrlEncode($"{{\"options\":{{\"force\":false,\"objectId\":\"{ObjectId}\",\"pinId\":\"{PinID}\",\"tags\":\"[]\",\"text\":\"{CommentText}\"}},\"context\":{{}}}}")}";
        #endregion

        #region APIs.
        public static string GetUserProfileAPI(string user,string Domain)
        {
            var userId = !string.IsNullOrEmpty(user) && user.Contains("/")? user.Split('/')[3]:user;
            return $"https://{Domain}/resource/UserResource/get/?source_url=%2F{userId}%2F&data=%7B%22options%22%3A%7B%22username%22%3A%22{userId}%22%2C%22field_set_key%22%3A%22profile%22%7D%2C%22context%22%3A%7B%7D%7D&_={GetTicks}";
        }
        public static string GetPostUploadAPI { get; set; } = "https://pinterest-media-upload.s3.amazonaws.com/";
        public static string GetThumbnailUploadAPI => "https://u.pinimg.com/";
        public static string GetPinCreateAPI(string domain)
        {
            return $"https://{domain}/resource/VIPResource/create/";
        }
        public static string GetUserFollowingAPI(string domain,string UserName,string bookmark="")
        {
            return
                $"https://{domain}/resource/UserFollowingResource/get/?source_url=/{UserName}/&data={{\"options\":{{\"page_size\":50,\"username\":\"{UserName}\"{bookmark}}},\"context\":{{}}}}&_={GetTicks}";
        }
        
        public static string GetPinDetailsAPI(string PinID,string Domain)
        {
            return
                $"https://{Domain}/resource/PinResource/get/?source_url=%2Fpin%2F{PinID}%2F&data=%7B%22options%22%3A%7B%22id%22%3A%22{PinID}%22%2C%22field_set_key%22%3A%22auth_web_main_pin%22%2C%22noCache%22%3Atrue%2C%22fetch_visual_search_objects%22%3Atrue%7D%2C%22context%22%3A%7B%7D%7D&_={GetTicks}";
        }
        
        public static string GetUserDetailsAPI(string userName,string Domain) => //$"https://{Domain}/resource/UserResource/get/?source_url=%2F{Uri.EscapeDataString(userName)}%2F_saved%2F&data=%7B%22options%22%3A%7B%22username%22%3A%22{Uri.EscapeDataString(userName)}%22%2C%22field_set_key%22%3A%22profile%22%2C%22no_fetch_context_on_resource%22%3Afalse%7D%2C%22context%22%3A%7B%7D%7D&_={GetTicks}";
            $"https://{Domain}/resource/UserResource/get/?source_url=/{userName}/&data={{\"options\":{{\"username\":\"{userName}\",\"field_set_key\":\"profile\"}},\"context\":{{}}}}&_={GetTicks}";
        public static string GetPinCommentAPI(string PinID,string AggregatedPinId,string Domain,string bookMark="") => $"https://{Domain}/resource/UnifiedCommentsResource/get/?source_url=/pin/{PinID}/&data={{\"options\":{{\"aggregated_pin_id\":\"{AggregatedPinId}\",\"comment_featured_ids\":\"\",\"page_size\":5,\"redux_normalize_feed\":true{bookMark}}},\"context\":{{}}}}&_={GetTicks}";
        public static string GetMessageInvitationAPI(string domain, string profileId) => $"{Https}{domain}/resource/ContactRequestsResource/get/?source_url=%2F{profileId}%2F&data=%7B%22options%22%3A%7B%7D%2C%22context%22%3A%7B%7D%7D&_={DateTime.Now.Ticks.ToString()}";
        public static string GetUserPinDetailsAPI(string ProfileId,string Domain,string bookMark="") => $"https://{Domain}/resource/UserPinsResource/get/?source_url=/{ProfileId}/pins/&data={{\"options\":{{\"is_own_profile_pins\":false,\"username\":\"{ProfileId}\",\"field_set_key\":\"partner_grid_item\",\"pin_filter\":null,\"bookmarks\":[\"{bookMark}\"]}},\"context\":{{}}}}&_={GetTicks}";
        public static string GetBoardSectionAPI(string ProfileId, string BoardName, string BoardId,string Domain) => $"https://{Domain}/resource/BoardSectionsResource/get/?source_url=/{ProfileId}/{BoardName}/&data={{\"options\":{{\"board_id\":\"{BoardId}\",\"redux_normalize_feed\":true}},\"context\":{{}}}}&_={GetTicks}";
        public static string GetBoardPinsAPI(string ProfileId,string BoardName,string BoardId,string Domain,string PaginationToken="") => $"https://{Domain}/resource/BoardFeedResource/get/?source_url=/{ProfileId}/{BoardName}/&data={{\"options\":{{\"board_id\":\"{BoardId}\",\"board_url\":\"/{ProfileId}/{BoardName}/\",\"currentFilter\":-1,\"field_set_key\":\"react_grid_pin\",\"filter_section_pins\":true,\"sort\":\"default\",\"layout\":\"default\",\"page_size\":25,\"redux_normalize_feed\":true{GetPaginationID(PaginationToken)}}},\"context\":{{}}}}&_={GetTicks}";
        public static string GetVideoUploadAPI => "https://pinterest-media-upload.s3-accelerate.amazonaws.com/";
        public static string GetVideoDetailsAPI(string UserId,string Domain) => $"https://{Domain}/resource/StoryPinDraftsResource/get/?source_url=/idea-pin-builder/&data={{\"options\":{{\"page_size\":50,\"redux_normalize_feed\":true,\"userId\":\"{UserId}\",\"orbacSubjectId\":\"\"}},\"context\":{{}}}}&_={GetTicks}";
        public static string GetIdeaPinCreateAPI(string Domain) => $"https://{Domain}/resource/StoryPinResource/create/";
        public static string GetVideoUploadParameterAPI(string Domain) => $"https://{Domain}/resource/ApiResource/create/";
        public static string GetBaseUrl(string Domain) => $"https://{Domain}/";
        public static string GetHomePageUrl(bool IsBusinessAccount,string Domain) => IsBusinessAccount ? GetBaseUrl(Domain)+"homefeed/" :GetBaseUrl(Domain);
        public static string GetBoardDetailsAPI(string Domain, string UserName, string BoardName) => $"https://{Domain}/resource/BoardResource/get/?source_url=%2F{UserName}%2F{BoardName}%2F&data=%7B%22options%22%3A%7B%22username%22%3A%22{UserName}%22%2C%22slug%22%3A%22{BoardName}%22%2C%22field_set_key%22%3A%22detailed%22%2C%22no_fetch_context_on_resource%22%3Afalse%7D%2C%22context%22%3A%7B%7D%7D&_={GetTicks}";
        public static string GetAcceptMessageRequestBody(string RequestID, string ProfileId) => $"source_url=%2F{ProfileId}%2F&data=%7B%22options%22%3A%7B%22url%22%3A%22%2Fv3%2Fcontact_requests%2F{RequestID}%2Faccept%2F%22%2C%22data%22%3A%7B%22fields%22%3A%5B%22conversation.board()%22%2C%22conversation.emails%22%2C%22conversation.id%22%2C%22conversation.last_message()%22%2C%22conversation.name%22%2C%22conversation.type%22%2C%22conversation.unread%22%2C%22conversation.users()%22%2C%22conversation.created_at%22%2C%22conversationmessage.board()%22%2C%22conversationmessage.created_at%22%2C%22conversationmessage.created_ms%22%2C%22conversationmessage.event_type%22%2C%22conversationmessage.id%22%2C%22conversationmessage.pin()%22%2C%22conversationmessage.sender()%22%2C%22conversationmessage.text%22%2C%22conversationmessage.type%22%2C%22conversationmessage.user()%22%2C%22board.id%22%2C%22board.image_cover_url%22%2C%22board.image_thumbnail_url%22%2C%22board.name%22%2C%22board.url%22%2C%22pin.id%22%2C%22pin.images%5B236x%2C136x136%5D%22%2C%22pin.dominant_color%22%2C%22pin.grid_description%22%2C%22pin.category%22%2C%22user.first_name%22%2C%22user.full_name%22%2C%22user.id%22%2C%22user.image_large_url%22%2C%22user.is_default_image%22%2C%22user.username%22%5D%7D%7D%2C%22context%22%3A%7B%7D%7D";
        #endregion

        #region Pin Post Response.
        public static string InvalidImage { get; set; } = "This image type is invalid - please make sure the image is natively .jpg or .png";
        public static string WrongImageFormat { get; set; } = "Your upload failed because it is in the wrong format.";
        public static string WrongImageFormat1 { get; set; } = "Your upload failed because it's the wrong format.";
        public static string TooSmallImage { get; set; } = "Hmm...this image is too small. Make sure your images are at least 200 x 300 pixels.";
        public static string TooSmallImage1 { get; set; } = "This image is too small. Make sure your images are at least 200 x 300 pixels.";
        #endregion

        #region Pinterest Login.
        public static string CheckExistAPI(string Domain, string email) => $"https://{Domain}/resource/ApiResource/get/?source_url=%2F&data=%7B%22options%22%3A%7B%22url%22%3A%22%2Fv3%2Fregister%2Fexists%2F%22%2C%22data%22%3A%7B%22email%22%3A%22{WebUtility.UrlEncode(email)}%22%7D%7D%2C%22context%22%3A%7B%7D%7D&_={GetTicks}";
        public static string CheckValidAPI(string Domain, string email) => $"https://{Domain}/resource/ApiResource/get/?source_url=%2Fcreate-personal&data=%7B%22options%22%3A%7B%22url%22%3A%22%2Fv3%2Femail%2Fvalidation%2F%22%2C%22data%22%3A%7B%22email%22%3A%22{WebUtility.UrlEncode(email)}%22%7D%7D%2C%22context%22%3A%7B%7D%7D&_={GetTicks}";
        public static string SignUpAPI => "https://accounts.pinterest.com/v3/register/email/handshake/";
        public static string SignUpBody(string email,string password,int age) => $"email={WebUtility.UrlEncode(email)}&username=&password={WebUtility.UrlEncode(password)}&first_name={GetFirstname(email)}&last_name=&country=IN&locale=en-GB&&birthday={GetDOB(age)}&recaptcha_v3_token=default";

        private static long GetDOB(int age)
        {
            try
            {
                var currentTime = DateTime.Now;
                var birthdatetime = currentTime.AddYears(-age);
                return birthdatetime.GetCurrentEpochTimeSeconds();
            }
            catch { return DateTime.Now.GetCurrentEpochTimeSeconds(); }
        }

        public static object GetFirstname(string email)
        {
            try
            {
                if (string.IsNullOrEmpty(email))
                    return "Pinterest User";
                var name = email.Split('@')?.ToList()?.FirstOrDefault()?.ToString();
                if (!string.IsNullOrEmpty(name))
                    return char.ToUpper(name[0])+name.Substring(1).ToLower();
                return email.Split('@')?.ToList()?.FirstOrDefault();
            }
            catch { return "Pinterest User"; }
        }

        public static string HandShakeAPI(string Domain) => $"https://{Domain}/resource/HandshakeSessionResource/create/";
        public static string HandShakeBody(string TokenID) => $"source_url=%2F&data=%7B%22options%22%3A%7B%22token%22%3A%22{TokenID}%22%2C%22isRegistration%22%3Atrue%7D%2C%22context%22%3A%7B%7D%7D";
        public static string LoginAPI => "https://accounts.pinterest.com/v3/login/handshake/";
        public static string LoginBody(string username,string password) => $"username_or_email={WebUtility.UrlEncode(username)}&password={WebUtility.UrlEncode(password)}";
        public static string AccountSetupAPI(string Domain) => $"https://{Domain}/resource/UserStateResource/create/";
        public static byte[] AccountSetupBody(string option, int value) => Encoding.UTF8.GetBytes($"source_url=%2F&data=%7B%22options%22%3A%7B%22state%22%3A%22{option}%22%2C%22value%22%3A{value}%7D%2C%22context%22%3A%7B%7D%7D");
        #endregion
    }
}