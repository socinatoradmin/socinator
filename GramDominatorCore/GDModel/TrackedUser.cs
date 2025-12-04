using System;
using DominatorHouseCore.Enums;

namespace GramDominatorCore.GDModel
{
    public class TrackedUser : IEquatable<TrackedUser>
    {
       // public TrackedUser()
       // {
       // }

       // public TrackedUser(string pk, string username)
       // {
        //    Pk = pk;
       //     Username = username;
      //  }

//public string FilterArgument { get; set; }

    
//public ScrapingParameters? FilterType { get; set; }

        //public short FilterTypeSql
        //{
        //    get
        //    {
        //        ScrapingParameters? filterType = FilterType;
        //        return (filterType.HasValue ? (short)filterType.GetValueOrDefault() : new short?()) ?? (short)-1;
        //    }
        //    set
        //    {
        //        FilterType = (int)value == -1 ? new ScrapingParameters?() : (ScrapingParameters)value;
        //    }
        //}

       // public bool FollowedBack { get; set; }

       // public int FollowedBackDate { get; set; }

        public int InteractionDate { get; set; }

        public ActivityType ActivityType { get; set; }

    
        public string Pk { get;  }

        public string Username { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (this == obj)
                return true;
            if (obj.GetType() == GetType())
                return Equals((TrackedUser)obj);
            return false;
        }

        public bool Equals(TrackedUser other)
        {
            if (other == null)
                return false;
            if (!Equals(other))
                return string.Equals(Pk, other.Pk);
            return true;
        }

        public override int GetHashCode()
        {
            return Pk.GetHashCode();
        }
    }
}
