using System;

namespace QuoraDominatorCore.Models
{
    public class BaseUser : IEquatable<BaseUser>
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

        public string Username { get; set; }

        public bool Equals(BaseUser other)
        {
            if (other == null)
                return false;
            if (!Equals(other))
                return string.Equals(Pk, other.Pk);
            return true;
        }

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
            return Pk.GetHashCode();
        }
    }
}