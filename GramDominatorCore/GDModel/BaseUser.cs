using System;
using DominatorHouseCore.Interfaces;

namespace GramDominatorCore.GDModel
{
    public class BaseUser : IEquatable<BaseUser> , IUser
    {
        public BaseUser(string pk, string username)
        {
            Pk = pk;
            Username = username;
        }

        // ReSharper disable once UnusedMember.Global
        protected BaseUser()
        {
        }

        public string FullName { get; set; }

        public string Pk { get;  set; }

        public string ProfilePicUrl { get; set; }

        public string UserId { get; set; }

        public string Username { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (this == obj)
                return true;
            if (obj.GetType() == GetType())
                return Equals((BaseUser)obj);
            return false;
        }

        public bool Equals(BaseUser other)
        {
            if (other == null)
                return false;
            if (this != other)
                return string.Equals(Pk, other.Pk);
            return true;
        }

        public override int GetHashCode()
        {
            return Pk.GetHashCode();
        }
    }
}
