using System;
using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using Newtonsoft.Json.Linq;
using QuoraDominatorCore.Models;
using QuoraDominatorCore.QdUtility;

namespace QuoraDominatorCore.Response
{
    public class UserInfoResponseHandler : QuoraResponseHandler
    {
        private readonly bool _hasAnonymousProfilePicture = true;
        public int AnsweredLast;
        public string Biography;
        public int FollowedBack;
        public int FollowerCount;
        public int FollowingCount;
        public int FollowRatio;
        public string Formkey;
        public string FullName;
        public bool IsVerified;
        public string Location;
        public string PostKey;
        public string Profilephotourl;
        public string StudiedAt;
        public int TotalAnswerView;
        public int UserAnswerCount;
        public int UserBlogsCount; //Blogs are nothing but spaces
        public int UserEditsCount;
        public string Url;
        public string UserId;
        public string Username;
        public int UserPostsCount;
        public int UserQuestionsCount;
        public int UserTopicsCount;
        public string WindowId;
        public string WorkAt;
        public bool IsFollowing;
        public bool ViewerCanMessage;
        public bool ViewerIsFollowing;

        public UserInfoResponseHandler(IResponseParameter response,bool IsBrowser=true) : base(response)
        {
            if (!Success)
                return;
            
            try
            {
                var JsonString= IsBrowser ? QdConstants.GetJsonForAllTypePosts(response.Response.Replace("\\\"","\"").Replace("\\\"", "\""), "user") :response?.Response;
                var jObject=jsonHandler.ParseJsonToJObject(JsonString);
                jObject = jsonHandler.GetJTokenOfJToken(jObject, "data", "user") as JObject;
                int.TryParse(jsonHandler.GetJTokenValue(jObject, "followingCount"), out FollowingCount);
                int.TryParse(jsonHandler.GetJTokenValue(jObject, "followerCount"), out FollowerCount);
                int.TryParse(jsonHandler.GetJTokenValue(jObject, "numProfileQuestions"), out UserQuestionsCount);
                int.TryParse(jsonHandler.GetJTokenValue(jObject, "numPublicAnswers"), out UserAnswerCount);
                int.TryParse(jsonHandler.GetJTokenValue(jObject, "postsCount"), out UserPostsCount);
                int.TryParse(jsonHandler.GetJTokenValue(jObject, "allTimePublicAnswerViews"), out TotalAnswerView);
                StudiedAt=jsonHandler.GetJTokenValue(jObject, "schoolCredentials", 0, "school", "name");
                Location=jsonHandler.GetJTokenValue(jObject, "locationCredentials", 0, "location", "name");
                WorkAt=jsonHandler.GetJTokenValue(jObject, "workCredentials", 0, "company", "name");
                Username = jsonHandler.GetJTokenValue(jObject, "profileUrl")?.Replace("/profile/","")?.Replace("\\u00f8", "ø");
                Profilephotourl = jsonHandler.GetJTokenValue(jObject, "profileImageUrl");
                _hasAnonymousProfilePicture = !Profilephotourl.Contains("-4-images.new_grid.profile_default.png-26-688c79556f251aa0.png");
                UserId = jsonHandler.GetJTokenValue(jObject, "uid");
                bool.TryParse(jsonHandler.GetJTokenValue(jObject, "isVerified"),out IsVerified);
                bool.TryParse(jsonHandler.GetJTokenValue(jObject, "viewerCanMessage"),out ViewerCanMessage);
                var follow=jsonHandler.GetJTokenValue(jObject, "viewerIsFollowedBy");
                FollowedBack = follow.Contains("True") ? 1 : 0;
                bool.TryParse(jsonHandler.GetJTokenValue(jObject, "viewerIsFollowing"),out IsFollowing);
                if (UserAnswerCount > 0)
                {
                    long.TryParse(jsonHandler.GetJTokenValue(jObject, "combinedProfileFeedConnection", "edges", 0, "node", "answer", "creationTime"), out long creationTime);
                    var utcDateTime = (creationTime/1000).EpochToDateTimeUtc();
                    TimeZoneInfo istTimeZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
                    DateTime ISTDateTime = TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, istTimeZone);
                    AnsweredLast = (ISTDateTime != null) ? QdUtilities.GetDateDifferenceFromTimeStamp(ISTDateTime) : 0;
                }
                var BioData = jsonHandler.GetJTokenValue(jObject, "descriptionQtextDocument", "legacyJson");
                Biography = Utilities.GetBetween(BioData, "text\": ", ", \"modifiers");
                Formkey = Utilities.GetBetween(response.Response, "\"formkey\": \"", "\", \"");
                PostKey = Utilities.GetBetween(response.Response, "\"postkey\": \"", "\"");
                WindowId = Utilities.GetBetween(response.Response, "\"windowId\": \"", "\"");
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public static implicit operator QuoraUser(UserInfoResponseHandler user)
        {
            var quoraUser =
                new QuoraUser
                {
                    HasAnonymousProfilePicture = user._hasAnonymousProfilePicture,
                    FullName = user.FullName,
                    FollowerCount = user.FollowerCount,
                    FollowingCount = user.FollowingCount,
                    AnswerCount = user.UserAnswerCount,
                    QuestionCount = user.UserQuestionsCount
                };
            quoraUser.HasAnonymousProfilePicture = user._hasAnonymousProfilePicture;
            quoraUser.FollowedBack = user.FollowedBack;
            var num2 = user.IsVerified ? 1 : 0;
            quoraUser.IsVerified = num2 != 0;
            quoraUser.ProfilePicUrl=user.Profilephotourl;
            quoraUser.Username = user.Username;
            quoraUser.Formkey = user.Formkey;
            quoraUser.PostKey = user.PostKey;
            quoraUser.WindowId = user.WindowId;
            quoraUser.Url = user.Url;
            quoraUser.UserId = user.UserId;
            quoraUser.NumberOfPost = user.UserPostsCount;
            return quoraUser;
        }
    }
}