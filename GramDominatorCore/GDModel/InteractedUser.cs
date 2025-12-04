using System;

namespace GramDominatorCore.GDModel
{
    public class InteractedUser
    {
        public int Id { get; set; }

        public string Query { get; set; }

        public string QueryType { get; set; }

       // public int FollowedBack { get; set; }

       // public DateTime FollowedBackDate { get; set; }

        public DateTime Date { get; set; }

        public string ActivityType { get; set; }

        public string Username { get; set; }

        public string InteractedUsername { get; set; }

      //  public string DirectMessage { get; set; }

        public string InteractedUserId { get; set; }

        public TimeSpan Time { get; set; }

        public string IsPrivate { get; set; }

//public string IsBusiness  { get; set; }

       // public string IsVerified { get; set; }

       // public string IsProfilePicAvailable { get; set; }

        public string ProfilePicUrl{ get; set; }
    }
}
