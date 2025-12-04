using DominatorHouseCore.Interfaces;
using GramDominatorCore.GDLibrary.Response;
using GramDominatorCore.GDModel;
using System.Collections.Generic;

namespace GramDominatorCore.Response
{
    public class FriendShipPendingResponseHandler : IGResponseHandler
    {
        public FriendShipPendingResponseHandler(IResponseParameter response) : base(response)
        {
            if (!Success)
                return;
            var users = handler.GetJArrayElement(handler.GetJTokenValue(RespJ, "users"));
            if(users != null && users.HasValues)
            {
                foreach( var user in users)
                {
                    bool.TryParse(handler.GetJTokenValue(user, "is_verified"), out bool isVerified);
                    bool.TryParse(handler.GetJTokenValue(user, "is_private"), out bool isPrivate);
                    bool.TryParse(handler.GetJTokenValue(user, "has_anonymous_profile_picture"), out bool hasAnonymous);
                    UserList.Add(new InstagramUser
                    {
                        Pk = handler.GetJTokenValue(user,"pk"),
                        UserId = handler.GetJTokenValue(user,"pk"),
                        Username = handler.GetJTokenValue(user, "username"),
                        FullName = handler.GetJTokenValue(user, "full_name"),
                        IsPrivate = isPrivate,
                        ProfilePicUrl = handler.GetJTokenValue(user, "profile_pic_url"),
                        IsVerified = isVerified,
                        HasAnonymousProfilePicture = hasAnonymous
                    });
                }
            }
        }
       public string MaxId { get; set; }
       public  List<InstagramUser> UserList = new List<InstagramUser>();
    }
}
