using System;
using System.ComponentModel;

namespace QuoraDominatorCore.Models
{
    public class BlacklistedUser : IEquatable<BlacklistedUser>
    {
        public BlacklistedUser(string pk, string username)
        {
            Pk = pk;
            Username = username;
        }

        public BlacklistedUser(string username)
        {
            Username = username;
        }

        public BlacklistedUser()
        {
        }

        [ReadOnly(true)] public string Pk { get; }


        [ReadOnly(true)] public string Username { get; }

        public bool Equals(BlacklistedUser other)
        {
            if (other == null)
                return false;
            if (this == other)
                return true;
            if (Pk != null)
                return string.Equals(Pk, other.Pk);
            return string.Equals(Username, other.Username);
        }

        public static implicit operator BlacklistedUser(QuoraUser user)
        {
            return new BlacklistedUser(user.Pk, user.Username);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (this == obj)
                return true;
            if (obj.GetType() == GetType())
                return Equals((BlacklistedUser) obj);
            return false;
        }

        public override int GetHashCode()
        {
            if (Pk == null)
                return Username.GetHashCode();
            return Pk.GetHashCode();
        }
    }
}