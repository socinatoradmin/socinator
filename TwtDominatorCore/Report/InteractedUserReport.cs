using System;

namespace TwtDominatorCore.Report
{
    public class InteractedUserReport
    {
        public int SlNo { get; set; }

        public string SinAccUsername { get; set; }

        public string QueryType { get; set; }

        public string QueryValue { get; set; }


        public string UserName { get; set; }

        public string UserId { get; set; }

        public string UserFullName { get; set; }

        public string FollowStatus { get; set; }

        public string FollowBackStatus { get; set; }


        public int FollowersCount { get; set; }

        public int FollowingsCount { get; set; }

        public int TweetsCount { get; set; }

        public string MessageText { get; set; }
        public string MediaPath { get; set; }


        public int LikesCount { get; set; }

        public string ProfilePicture { get; set; }

        public string Privacy { get; set; }

        public string Verified { get; set; }


        public string ProfilePicUrl { get; set; }


        public string JoinedDate { get; set; }

        public string Location { get; set; }

        public string Website { get; set; }

        public string Bio { get; set; }

        /// <summary>
        ///     Describes wheather the activity is done in Activity process or after activity process
        /// </summary>
        public string ProcessType { get; set; }

        public DateTime InteractionDate { get; set; }
    }
}