using System;
using DominatorHouseCore.Interfaces;
using GramDominatorCore.GDLibrary.Response;
using GramDominatorCore.GDModel;
using Newtonsoft.Json.Linq;

namespace GramDominatorCore.Response
{
    public class UsernameInfoResponse : IGResponseHandler
    {
        public UsernameInfoResponse(IResponseParameter response)
        : base(response)
        {
            if (!Success)
                return;
            JToken jtoken1 =RespJ["user"]["usertags_count"];
            UsertagsCount = jtoken1?.ToString();
            HasAnonymousProfilePicture = Convert.ToBoolean(RespJ["user"]["has_anonymous_profile_picture"].ToString());
            if (RespJ["user"]["hd_profile_pic_url_info"] != null)
                HdImagePicUrlInfo = new InstaGramImage()
                {
                    Width = Convert.ToInt32(RespJ["user"]["hd_profile_pic_url_info"]["width"].ToString()),
                    Height = Convert.ToInt32(RespJ["user"]["hd_profile_pic_url_info"]["height"].ToString()),
                    Url = RespJ["user"]["hd_profile_pic_url_info"]["url"].ToString()
                };
            JToken jtoken2 = RespJ["user"]["following_count"];
           FollowingCount = Convert.ToInt32(jtoken2?.ToString() ?? "0");
           FullName =  RespJ["user"]["full_name"].ToString();
           Biography = RespJ["user"]["biography"].ToString();
            JToken jtoken3 = RespJ["user"]["media_count"];
            MediaCount = Convert.ToInt32(jtoken3?.ToString() ?? "0");
            JToken jtoken4 =RespJ["user"]["follower_count"];
            FollowerCount = Convert.ToInt32(jtoken4?.ToString() ?? "0");
            Pk =RespJ["user"]["pk"].ToString();
            IsVerified = Convert.ToBoolean(RespJ["user"]["is_verified"].ToString());
            Username = RespJ["user"]["username"].ToString();
            ProfilePicUrl = RespJ["user"]["profile_pic_url"].ToString();
            JToken jtoken5 = RespJ["user"]["profile_pic_id"];
            ProfilePicId = jtoken5?.ToString();
            IsPrivate = Convert.ToBoolean(RespJ["user"]["is_private"].ToString());
            ExternalUrl = RespJ["user"]["external_url"].ToString();
            JToken jtoken6 = RespJ["user"]["phone_number"];
            PhoneNumber = jtoken6?.ToString();
            int result;
            Gender = RespJ["user"]["gender"] == null ? new int?() : (int.TryParse(RespJ["user"]["gender"].ToString(), out result) ? result : new int?());
            JToken jtoken7 = RespJ["user"]["email"];
           Email = jtoken7?.ToString();
        }

        public string Biography { get; }

        public string Email { get; }

        public string ExternalUrl { get; }

        public int FollowerCount { get; }

        public int FollowingCount { get; }

        public string FullName { get; }

        public int? Gender { get; }

        public bool HasAnonymousProfilePicture { get; }

        public InstaGramImage HdImagePicUrlInfo { get; }

        public bool IsPrivate { get; }

        public bool IsVerified { get; }

        public int MediaCount { get; }

        public string PhoneNumber { get; }

        public string Pk { get; }

        public string ProfilePicId { get; }

        public string ProfilePicUrl { get; }

        public string Username { get; }

        public string UsertagsCount { get; }

        public static implicit operator InstagramUser(UsernameInfoResponse user)
        {
            return new InstagramUser(user.Pk, user.Username)
            {
                HasAnonymousProfilePicture = user.HasAnonymousProfilePicture,
                FullName = user.FullName,
                IsPrivate = user.IsPrivate,
                IsVerified = user.IsVerified,
                ProfilePicUrl = user.ProfilePicUrl
            };
        }



    }

}
