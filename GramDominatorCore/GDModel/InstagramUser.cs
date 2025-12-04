using System;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;
using GramDominatorCore.GDEnums;
using System.Collections.Generic;

namespace GramDominatorCore.GDModel
{
    public class InstagramUser : BaseUser
    {
        public InstagramUser()
        {

        }

        public InstagramUser(string pk, string username)
            : base(pk, username)
        {
        }

        public InstagramGender Gender
        {
            get
            {
                string regexPatern = StringHelper.GetRegexPatern("\\p{L}+?(?=[^\\p{L}]|$)", FullName);
                if (string.IsNullOrWhiteSpace(regexPatern))
                    return InstagramGender.Unknown;
                return ConvertGenderType(regexPatern.GetGender());
            
            }
        }

        public bool? HasAnonymousProfilePicture { get; set; }
        public bool IsFollowing { get; set; }

        public bool IsBusiness { get; set; }
        public bool IsBlocking { get; set; }
        public bool CanMessage { get; set; } = true;
        public bool IsPrivate { get; set; }

        public bool OutgoingRequest { get; set; }

        public bool IsVerified { get; set; }

        public new string ProfilePicUrl { get; set; }

        public new string UserId { get; set; }
        public bool IsBestie {  get; set; }
        public bool IsOldProfile {  get; set; }
        private InstagramGender ConvertGenderType(Gender gender)
        {
            switch (gender)
            {
                case DominatorHouseCore.Enums.Gender.Male:
                    return InstagramGender.Male;
                case DominatorHouseCore.Enums.Gender.Female:
                    return InstagramGender.Female;

                case DominatorHouseCore.Enums.Gender.Unisex:
                case DominatorHouseCore.Enums.Gender.Unknown:
                    return InstagramGender.Unknown;
                default:
                    throw new ArgumentOutOfRangeException(nameof(gender), gender, null);
            }
        }
        public InstagramUserDetails UserDetails { get; set; } = new InstagramUserDetails();
        public List<UsersPostStory> UserStories { get; set; } = new List<UsersPostStory>();
    }
}
