using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;
using System;

namespace GramDominatorCore.GDModel
{
    public class InstagramUserDetails
    {
        public string Biography { get; set; }

        public string Email { get; set; }
        public bool IsFollowing {  get; set; }
        public string ChatID {  get; set; }
        public string BusinessCategory { get; set; }

        public string ExternalUrl { get; set; }

        public int FollowerCount { get; set; }

        public int FollowingCount { get; set; }

        public string FullName { get; set; }

        public string PublicPhoneCountryCode { get; set; }

        public string Gender
        {
            get
            {
                string regexPatern = StringHelper.GetRegexPatern("\\p{L}+?(?=[^\\p{L}]|$)", FullName);
                if (string.IsNullOrWhiteSpace(regexPatern))
                    return "Unknown";
                return ConvertGenderType(regexPatern.GetGender());

            }
            set { }
        }
        public string GenderByCode { get; set; }
        private string ConvertGenderType(Gender gender)
        {
            switch (gender)
            {
                case DominatorHouseCore.Enums.Gender.Male:
                    return "Male";
                case DominatorHouseCore.Enums.Gender.Female:
                    return "Female";

                case DominatorHouseCore.Enums.Gender.Unisex:
                case DominatorHouseCore.Enums.Gender.Unknown:
                    return "Unknown";
                default:
                    throw new ArgumentOutOfRangeException(nameof(gender), gender, null);
            }
        }
        public bool? HasAnonymousProfilePicture { get; set; }

        public bool IsBusiness { get; set; }

        public InstaGramImage HdImagePicUrlInfo { get; set; }

        public bool IsPrivate { get; set; }

        public bool IsVerified { get; set; }

        public bool CanMessage { get; set; } = true;

        public int MediaCount { get; set; }

        public string PhoneNumber { get; set; }

        public string Pk { get; set; }

        public string ProfilePicId { get; set; }

        public string ProfilePicUrl { get; set; }

        public string Username { get; set; }

        public string UsertagsCount { get; set; }

        public string LocationId { get; set; }
        public string ThreadId { get; set; }
    }
}
