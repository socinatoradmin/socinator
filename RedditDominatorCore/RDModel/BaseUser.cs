using DominatorHouseCore.Interfaces;
using System;

namespace RedditDominatorCore.RDModel
{
    public class BaseUser : IEquatable<BaseUser>, IUser
    {
        public BaseUser(string id, string username)
        {
            Id = id;
            Username = username;
        }

        protected BaseUser()
        {
        }

        public string Id { get; set; }

        public bool Equals(BaseUser other)
        {
            if (other == null)
                return false;
            return Equals(other) || string.Equals(Id, other.Id);
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
            return obj.GetType() == GetType() && Equals((BaseUser)obj);
        }

        public override int GetHashCode()
        {
            // ReSharper disable once NonReadonlyMemberInGetHashCode
            return Id.GetHashCode();
        }
    }
}