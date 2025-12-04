namespace QuoraDominatorCore.Models
{
    public class UsersWithInfo : QuoraUser
    {
        public UsersWithInfo()
        {
        }

        public UsersWithInfo(string pk, string username)
            : base(pk, username)
        {
        }

        public UsersWithInfo(QuoraUser user)
            : base(user.Pk, user.Username)
        {
            FullName = user.FullName;
            HasAnonymousProfilePicture = user.HasAnonymousProfilePicture;
            IsPrivate = user.IsPrivate;
            IsVerified = user.IsVerified;
            ProfilePicUrl = user.ProfilePicUrl;
        }
    }
}