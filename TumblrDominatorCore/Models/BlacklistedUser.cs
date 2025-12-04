using System;
using System.ComponentModel;

namespace TumblrDominatorCore.Models
{
    public class BlacklistedUser : IEquatable<BlacklistedUser>
    {
        public BlacklistedUser(string pk, string username)
        {
            Pk = pk;
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
            if (Equals(this, other))
                return true;
            return Pk != null ? string.Equals(Pk, other.Pk) : string.Equals(Username, other.Username);
        }


        public static implicit operator BlacklistedUser(TumblrUser user)
        {
            return new BlacklistedUser(user.Pk, user.Username);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (this == obj)
                return true;
            return obj.GetType() == GetType() && Equals((BlacklistedUser)obj);
        }

        public override int GetHashCode()
        {
            return Pk == null ? Username.GetHashCode() : Pk.GetHashCode();
        }
    }
}