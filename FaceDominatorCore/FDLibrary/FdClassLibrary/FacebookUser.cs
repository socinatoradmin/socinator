using DominatorHouseCore.Interfaces;
using System;
using System.Globalization;

namespace FaceDominatorCore.FDLibrary.FdClassLibrary
{
    public class FacebookUser : IUser
    {

        public string Familyname { get; set; } = string.Empty;

        public string Currentcity { get; set; } = string.Empty;

        public string Hometown { get; set; } = string.Empty;

        public string Gender { get; set; } = string.Empty;

        public string ProfileUrl { get; set; } = string.Empty;

        public string DateOfBirth { get; set; } = string.Empty;

        public string University { get; set; } = string.Empty;

        public string WorkPlace { get; set; } = string.Empty;

        public string ContactNo { get; set; }

        public string UserId { get; set; }
        public string ProfileId { get; set; }

        public string Username { get; set; }

        public string FullName { get; set; }

        public string ProfilePicUrl
        {
            get;
            set;
        }

        public string IsAlreadyFriend { get; set; } = string.Empty;

        public string InteractionDate { get; set; } = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss",
            CultureInfo.InvariantCulture);

        public string OtherDetails { get; set; } = string.Empty;

        public bool IsFriendAccount { get; set; } = false;


        public string Email { get; set; } = string.Empty;

        public string ScrapedProfileUrl { get; set; } = string.Empty;

        public InviteStatus InviteStatus { get; set; }


        public string RelationShip { get; set; }

        public string Address { get; set; }

        public string AddressMapUrl { get; set; }

        public string NeighBorHood { get; set; }

        public string Age { get; set; }

        public string ClassName { get; set; }

        public string QueryType { get; set; }

        public bool IsPrivateUser { get; set; }
        public bool IsverifiedUser { get; set; }
        public bool IsAllDetailsScrapped { get; set; }

        public string CanSendFriendRequest { get; set; } = string.Empty;
        public string CanFollow { get; set; } = string.Empty;

        public bool HasMutualFriends { get; set; } = false;

        public string WebsiteUrl { get; set; }

        public string Bio { get; set; }

        public string PhoneNumber { get; set; }

        public bool CanSendMessage { get; set; }
    }

    public enum InviteStatus
    {
        Liked = 1,
        Notinvited = 2,
        Pending = 3
    }
}
