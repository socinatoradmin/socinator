using System;
using System.ComponentModel;
using DominatorHouseCore.Interfaces;
using Newtonsoft.Json.Linq;
using DominatorHouseCore.Utility;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore;
using System.Text.RegularExpressions;
using System.Linq;

namespace GramDominatorCore.GDModel
{
    [Localizable(false)]
    public class UsernameInfoIgResponseHandler : GDLibrary.Response.IGResponseHandler
    {
        public UsernameInfoIgResponseHandler(IResponseParameter response,EditProfileModel editProfile = null)
            : base(response)
        {
            try
            {
                if (Success && response.Response.Contains("<!DOCTYPE html>"))
                {
                    var json = "{\"data\":" + Utilities.GetBetween(response.Response, "\"result\":{\"data\":", ",\"sequence_number\"");
                    if (!handler.IsValidJson(json))
                        return;
                    response.Response = json;
                }
                if (!Success)
                    return;
                if (response.Response.StartsWith("{\"data\":{\"user\":{\"biography\":") 
                    ||response.Response.StartsWith("{\"data\":{\"user\":{\"ai_agent_type\":null,\"biography\"")
                    ||response.Response.StartsWith("{\"data\":{\"user\":{\"friendship_status\"")
                    || response.Response.EndsWith("\"status\":\"ok\"}"))
                {
                    GetUserInfo(response.Response);
                    return;
                }
                if (!(editProfile is null) && response.Response.StartsWith("{\"form_data\":"))
                {
                    Success = checkProfileInfo(response.Response, editProfile);
                    return;
                }
                else
                {
                    JsonHandler json = new JsonHandler(response.Response);
                    GenderByCode = json.GetElementValue("form_data", "gender");
                    return;
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private bool checkProfileInfo(string response, EditProfileModel editProfile)
        {
            JsonHandler jsonHandler = new JsonHandler(response);
            try
            {
                if (response.StartsWith("{\"form_data\":"))
                {
                    var firstName = jsonHandler.GetElementValue("form_data", "first_name");
                    var lastName = jsonHandler.GetElementValue("form_data", "last_name");
                    var email = jsonHandler.GetElementValue("form_data", "email");
                    var userName = jsonHandler.GetElementValue("form_data", "username");
                    var phoneNumber = jsonHandler.GetElementValue("form_data", "phone_number");
                    var gender = jsonHandler.GetElementValue("form_data", "gender");
                    var birthday = jsonHandler.GetElementValue("form_data", "birthday");
                    var biography = jsonHandler.GetElementValue("form_data", "biography");
                    var external_url = jsonHandler.GetElementValue("form_data", "external_url");
                    editProfile.Bio = biography;
                    editProfile.Email = email;
                    editProfile.ExternalUrl = external_url;
                    editProfile.Fullname = firstName;
                    editProfile.PhoneNumber = phoneNumber;
                    if (gender == "1")
                        editProfile.IsMaleChecked = true;
                    else if (gender == "2")
                        editProfile.IsFemaleChecked = true;
                    else
                        editProfile.IsNonSpecifiedChecked = true;
                    return true; 
                }
                else
                {
                    if (bool.Parse(jsonHandler.GetElementValue("changed_profile")))
                        return true;
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog(ex.Message);
            }
            return false;
        }

        private void GetUserInfo(string response)
        {
            var jHandler = new JsonHandler(response);
            if (response.StartsWith("{\"data\":{\"user\":{\"ai_agent_type\":null,\"biography\""))
            {
                Username = jHandler.GetElementValue("data", "user", "username");
                instaUserDetails.Username = Username;
                instaUserDetails.IsFollowing = jHandler.GetElementValue("data","user","followed_by_viewer") == "True";
                Pk = jHandler.GetElementValue("data", "user", "id");
                instaUserDetails.Pk = Pk;
                ThreadId = jHandler.GetElementValue("data", "user", "eimu_id");
                ThreadId = string.IsNullOrEmpty(ThreadId) ? jHandler.GetElementValue("data", "user", "interop_messaging_user_fbid") :ThreadId;
                instaUserDetails.ThreadId = ThreadId;
                if (jHandler.GetElementValue("data", "user", "profile_pic_url_hd") != null)
                    HdImagePicUrlInfo = new InstaGramImage()
                    {
                        Width = 320,
                        Height = 320,
                        Url = jHandler.GetElementValue("data", "user", "profile_pic_url_hd")
                    };
                instaUserDetails.HdImagePicUrlInfo = HdImagePicUrlInfo;
                int followingCount = 0;
                Int32.TryParse(jHandler.GetElementValue("data", "user", "edge_follow", "count"), out followingCount);
                FollowingCount = followingCount;
                instaUserDetails.FollowingCount = FollowingCount;
                FullName = jHandler.GetElementValue("data", "user", "full_name");
                instaUserDetails.FullName = FullName;
                Biography = jHandler.GetElementValue("data", "user", "biography");
                instaUserDetails.Biography = Biography;
                var mediaCount = jHandler.GetElementValue("data", "user", "edge_owner_to_timeline_media", "count");
                int.TryParse(mediaCount, out int media_count);
                MediaCount = media_count;
                instaUserDetails.MediaCount = MediaCount;
                FollowerCount = int.Parse(jHandler.GetElementValue("data", "user", "edge_followed_by", "count") ?? "0");
                instaUserDetails.FollowerCount = FollowerCount;
                IsBusiness = bool.Parse(jHandler.GetElementValue("data", "user", "is_business_account"));
                instaUserDetails.IsBusiness = IsBusiness;
                bool.TryParse(jHandler.GetElementValue("data", "user", "is_verified"), out bool isVerified);
                IsVerified = isVerified;
                instaUserDetails.IsVerified = IsVerified;
                ProfilePicUrl = jHandler.GetElementValue("data", "user", "profile_pic_url");
                instaUserDetails.ProfilePicUrl = ProfilePicUrl;
                bool.TryParse(jHandler.GetElementValue("data", "user", "is_private"), out bool isPrivate);
                IsPrivate = isPrivate;
                instaUserDetails.IsPrivate = IsPrivate;
                ExternalUrl = jHandler.GetElementValue("data", "user", "external_url");
                instaUserDetails.ExternalUrl = ExternalUrl;
                PhoneNumber = jHandler.GetElementValue("data", "user", "business_phone_number");
                PhoneNumber = string.IsNullOrEmpty(PhoneNumber) ? Regex.Match(Biography,"(\\+[0-9])?\\d+")?.Value : PhoneNumber;
                instaUserDetails.PhoneNumber = !string.IsNullOrEmpty(PhoneNumber) ||PhoneNumber.Length < 10?"N/A":PhoneNumber;
                GenderByCode = "0";
                Email = jHandler.GetElementValue("data", "user", "business_email");
                Email = string.IsNullOrEmpty(Email) ?Regex.Match(Biography,"([a-z0-9A-Z.-]+)?@[a-z]+\\.[a-z]+")?.Value : Email;
                instaUserDetails.Email = string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Email.Split('@')?.FirstOrDefault()) ?"N/A":Email;
                BusinessCategory = jHandler.GetElementValue("data", "user", "business_category_name");
                instaUserDetails.BusinessCategory = BusinessCategory;
                GenderByCode = Gender;
                instaUserDetails.GenderByCode = GenderByCode;
                instaUserDetails.Gender = Gender;
            }else if (response.Contains("\"next_max_id\"") || response.Contains("\"auto_load_more_enabled\""))
            {
                instaUserDetails.Username = Username = jHandler.GetElementValue("user", "username");
                instaUserDetails.Pk = Pk = jHandler.GetElementValue("user", "pk");
                instaUserDetails.ProfilePicId = ProfilePicId = jHandler.GetElementValue("user", "profile_pic_id");
                instaUserDetails.ProfilePicUrl = ProfilePicUrl = jHandler.GetElementValue("user", "profile_pic_url");
                instaUserDetails.FullName = FullName = jHandler.GetElementValue("user", "full_name");
                bool.TryParse(jHandler.GetElementValue("user", "is_private"), out bool isPrivate);
                instaUserDetails.IsPrivate = IsPrivate = isPrivate;
                bool.TryParse(jHandler.GetElementValue("user", "is_active_on_text_post_app"), out bool canDm);
                instaUserDetails.CanMessage = CanMessage = canDm;
            }
            else
            {
                JToken jtoken1 = jHandler.GetJToken("data", "user", "usertags_count");
                jtoken1 = jtoken1 == null || !jtoken1.HasValues ? jHandler.GetJToken("data", "user", "friendship_status") :jtoken1;
                UsertagsCount = jtoken1 == null ? "0" : jtoken1.ToString();
                Username = jHandler.GetElementValue("data", "user", "username");
                instaUserDetails.Username = Username;
                Pk = jHandler.GetElementValue("data", "user", "id");
                instaUserDetails.Pk = Pk;
                ThreadId = jHandler.GetElementValue("data", "user", "eimu_id");
                ThreadId = string.IsNullOrEmpty(ThreadId) ? jHandler.GetElementValue("data", "user", "interop_messaging_user_fbid") : ThreadId;
                instaUserDetails.ThreadId = ThreadId;
                var HdProfile = jHandler.GetElementValue("data", "user", "profile_pic_url_hd");
                HdProfile = string.IsNullOrEmpty(HdProfile) ? jHandler.GetElementValue("data", "user", "hd_profile_pic_url_info","url") : HdProfile;
                if (HdProfile != null)
                    HdImagePicUrlInfo = new InstaGramImage()
                    {
                        Width = 320,
                        Height = 320,
                        Url = jHandler.GetElementValue("data", "user", "profile_pic_url_hd")
                    };
                instaUserDetails.HdImagePicUrlInfo = HdImagePicUrlInfo;
                int followingCount = 0;
                var followCount = jHandler.GetElementValue("data", "user", "edge_follow", "count");
                followCount = string.IsNullOrEmpty(followCount)? jHandler.GetElementValue("data", "user", "following_count"):followCount;
                Int32.TryParse(followCount, out followingCount);
                FollowingCount = followingCount;
                instaUserDetails.FollowingCount = FollowingCount;
                FullName = jHandler.GetElementValue("data", "user", "full_name");
                instaUserDetails.FullName = FullName;
                Biography = jHandler.GetElementValue("data", "user", "biography");
                instaUserDetails.Biography = Biography;
                var mediaCount = jHandler.GetElementValue("data", "user", "edge_owner_to_timeline_media", "count");
                mediaCount = string.IsNullOrEmpty(mediaCount)? jHandler.GetElementValue("data", "user", "media_count"):mediaCount;
                int.TryParse(mediaCount, out int media_count);
                MediaCount = media_count;
                instaUserDetails.MediaCount = MediaCount;
                followCount = jHandler.GetElementValue("data", "user", "edge_followed_by", "count");
                followCount = string.IsNullOrEmpty(followCount) ? jHandler.GetElementValue("data", "user", "follower_count") : followCount;
                FollowerCount = int.Parse(followCount ?? "0");
                instaUserDetails.FollowerCount = FollowerCount;
                bool.TryParse(jHandler.GetElementValue("data", "user", "is_business_account"), out bool IsBusinessAccount);
                if(!IsBusinessAccount)
                    bool.TryParse(jHandler.GetElementValue("data", "user", "is_business"), out IsBusinessAccount);
                IsBusiness = IsBusinessAccount;
                instaUserDetails.IsBusiness = IsBusiness;
                bool.TryParse(jHandler.GetElementValue("data", "user", "is_verified"), out bool isVerified);
                IsVerified = isVerified;
                instaUserDetails.IsVerified = IsVerified;
                ProfilePicUrl = jHandler.GetElementValue("data", "user", "profile_pic_url");
                instaUserDetails.ProfilePicUrl = ProfilePicUrl;
                bool.TryParse(jHandler.GetElementValue("data", "user", "is_private"), out bool isPrivate);
                IsPrivate = isPrivate;
                instaUserDetails.IsPrivate = IsPrivate;
                ExternalUrl = jHandler.GetElementValue("data", "user", "external_url");
                instaUserDetails.ExternalUrl = ExternalUrl;
                PhoneNumber = jHandler.GetElementValue("data", "user", "business_phone_number");
                if (string.IsNullOrEmpty(PhoneNumber))
                {
                    var tokens = jHandler.GetElementValue("data", "user", "bio_links");
                    var contact = handler.GetJArrayElement(tokens);
                    var numberToken = contact?.FirstOrDefault(x=>handler.GetJTokenValue(x, "title") == "WhatsApp");
                    PhoneNumber = handler.GetJTokenValue(numberToken, "url")?.Replace("http://wa.me/", "")?.Replace("/","");
                }
                instaUserDetails.PhoneNumber = PhoneNumber;
                GenderByCode = "0";
                Email = jHandler.GetElementValue("data", "user", "business_email");
                instaUserDetails.Email = Email;
                BusinessCategory = jHandler.GetElementValue("data", "user", "business_category_name");
                bool.TryParse(jHandler.GetElementValue("data", "user", "friendship_status", "following"), out bool isFollowing);
                instaUserDetails.IsFollowing = isFollowing;
                GenderByCode = Gender;
                instaUserDetails.GenderByCode = GenderByCode;
                instaUserDetails.Gender = Gender;
            }
            
        }

        public string Biography { get; set; }

        public string Email { get; set; }

        public string ExternalUrl { get; set; }

        public int FollowerCount { get; set; }

        public int FollowingCount { get; set; }

        public string FullName { get; set; }

        public string PublicPhoneCountryCode { get; set; }
        public string BusinessCategory { get; set; }

        public string Gender
        {
            get
            {
                string regexPatern = StringHelper.GetRegexPatern("\\p{L}+?(?=[^\\p{L}]|$)", FullName);
                if (string.IsNullOrWhiteSpace(regexPatern))
                    return "Unknown";
                return ConvertGenderType(DominatorHouseCore.Utility.GenderGuesser.GetGender(regexPatern));

            }
            set { }
        }
        public string GenderByCode { get; set; }
        private string ConvertGenderType(Gender gender)
        {
            switch (gender)
            {
                case DominatorHouseCore.Enums.Gender.Male:
                    return "Male";
                case DominatorHouseCore.Enums.Gender.Female:
                    return "Female";

                case DominatorHouseCore.Enums.Gender.Unisex:
                case DominatorHouseCore.Enums.Gender.Unknown:
                    return "Unknown";
                default:
                    throw new ArgumentOutOfRangeException(nameof(gender), gender, null);
            }
        }
        public bool HasAnonymousProfilePicture { get; set; }

        public bool IsBusiness { get; set; }

        public InstaGramImage HdImagePicUrlInfo { get; set; }

        public bool IsPrivate { get; set; }

        public bool IsVerified { get; set; }
        public bool CanMessage { get; set; } = true;
        public int MediaCount { get; set; }

        public string PhoneNumber { get; set; }

        public string Pk { get; set; }

        public string ProfilePicId { get; set; }

        public string ProfilePicUrl { get; set; }

        public string Username { get; set; }
        public string ThreadId { get; set; }

        public string UsertagsCount { get; set; }

        public string LocationId { get; set; }

        public InstagramUserDetails instaUserDetails = new InstagramUserDetails();

        public static implicit operator InstagramUser(UsernameInfoIgResponseHandler user)
        {
            if (user is null)
                return null;
            InstagramUser instagramUser = new InstagramUser(user.Pk, user.Username)
            {
                UserId = user.Pk,
                HasAnonymousProfilePicture = user.HasAnonymousProfilePicture,
                FullName = user.FullName
            };
            instagramUser.CanMessage = user.CanMessage;
            int num1 = user.IsPrivate ? 1 : 0;
            instagramUser.IsPrivate = num1 != 0;
            int num2 = user.IsVerified ? 1 : 0;
            instagramUser.IsVerified = num2 != 0;
            int num3 = user.IsBusiness ? 1 : 0;
            instagramUser.IsBusiness = num3 != 0;
            string profilePicUrl = user.ProfilePicUrl;
            instagramUser.ProfilePicUrl = profilePicUrl;
            instagramUser.IsFollowing = user.instaUserDetails.IsFollowing;
            return instagramUser;
        }
    }
}
