namespace PinDominatorCore.PDModel
{
    public class UserInfoDetailed : PinterestUser
    {
        public UserInfoDetailed()
        {
        }

        public UserInfoDetailed(string pk, string username)
            : base(pk, username)
        {
        }

        public UserInfoDetailed(PinterestUser user)
            : base(user.Pk, user.Username)
        {
            FullName = user.FullName;
            HasProfilePic = user.HasProfilePic;
            ProfilePicUrl = user.ProfilePicUrl;
        }

        public int Followers { get; set; }

        public int Followings { get; set; }
    }
}