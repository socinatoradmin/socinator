using DominatorHouseCore.Utility;
using GramDominatorCore.GDModel;
using GramDominatorCore.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GramDominatorCore.WebResponseHandler
{
   public  class UserInfoWeb
    {
        public UserInfoWeb(string response)
        {
            try
            {
                if (response == null)
                    return;
                //instagramUser = new InstagramUser();               
                //var jsonString = Utilities.GetBetween(response, ">window._sharedData =", ";</script>").Trim();
                //var jHand = new JsonHandler(jsonString);
                //var token = jHand.GetJToken("entry_data", "ProfilePage", 0, "graphql", "user");
                //string jsonData = token.ToString();
                //UserId= (jHand.GetJTokenValue(token, "id"));
                //instagramUser.UserId = UserId;
                //instagramUser.Pk = UserId;
                //Username = (jHand.GetJTokenValue(token, "username"));
                //instagramUser.Username = Username;               
                //var name = (jHand.GetJTokenValue(token, "full_name"));
                //if (!string.IsNullOrEmpty(name) && name != FullName)
                //    FullName = name;
                //instagramUser.FullName = FullName;
                //Followed=Convert.ToBoolean(jHand.GetJTokenValue(token, "follows_viewer"));

                //var description = (jHand.GetJTokenValue(token, "biography"));

                //if (!string.IsNullOrEmpty(description) && description != BioGraphy)
                //    BioGraphy = description;

                //if (string.IsNullOrEmpty(UserId))
                //    Error = "not found";
                
                //IsBusiness =Convert.ToBoolean((jHand.GetJTokenValue(token, "is_business_account")));
                //instagramUser.IsBusiness = IsBusiness;          
                // IsPrivate = Convert.ToBoolean((jHand.GetJTokenValue(token, "is_private")));
                //instagramUser.IsPrivate = IsPrivate;
                // IsVerified = Convert.ToBoolean((jHand.GetJTokenValue(token, "is_verified")));
                //instagramUser.IsVerified = IsVerified;
                //FollowerCount =Convert.ToInt32(token["edge_followed_by"]["count"]);
                //FollowingCount= Convert.ToInt32(token["edge_follow"]["count"]);
                //FeedCount = Convert.ToInt32(token["edge_owner_to_timeline_media"]["count"]);
               
                //ProfilePicUrl = (jHand.GetJTokenValue(token, "profile_pic_url"));
                //instagramUser.ProfilePicUrl = ProfilePicUrl;
                //Response =response;
            }
            catch (Exception )
            {
            }
        }



        public string Username = "";
        public string UserId = "";
        public string FullName = "";
        public string BioGraphy = "";
        public sbyte PrivateAccount = 0;
        public string ExternelURL = "";
        public string ProfilePicUrl = "";
        public bool Followed { get; set; }
        public bool Requested { get; set; }
        public string Error { get; set; }
        public bool HasAnonymousProfilePicture { get; set; }
        public bool IsBusiness { get; set; }
        public bool IsPrivate { get; set; }
        public bool IsVerified { get; set; }
        public string Response { get; set; }
        public InstagramUser instagramUser = new InstagramUser();
        public int FollowerCount { get; set; }
        public int FollowingCount { get; set; }
        public int FeedCount { get; set; }

    }
}
