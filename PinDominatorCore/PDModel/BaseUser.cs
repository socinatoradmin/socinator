using DominatorHouseCore.Interfaces;

namespace PinDominatorCore.PDModel
{
    public class BaseUser : IUser
    {
        public BaseUser(string pk, string username)
        {
            Pk = pk;
            Username = username;
        }

        protected BaseUser()
        {
        }

        public string Pk { get; }

        public string UserId { get; set; }
        public string FullName { get; set; }
        public string Username { get; set; }
        public string ProfilePicUrl { get; set; }
        public override int GetHashCode()
        {
            return Pk.GetHashCode();
        }
    }
}