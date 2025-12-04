using System;
using DominatorHouseCore.Interfaces;

namespace TwtDominatorCore.TDModels
{
    public class BaseUser : IEquatable<BaseUser>, IUser
    {
        protected BaseUser()
        {
        }

        public bool Equals(BaseUser other)
        {
            if (other == null)
                return false;
            if (this != other)
                return string.Equals(UserId, other.UserId);
            return true;
        }

        public string FullName { get; set; }

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
                return Equals((BaseUser) obj);
            return false;
        }

        public override int GetHashCode()
        {
            return UserId.GetHashCode();
        }
    }
}