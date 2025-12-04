using System;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;

namespace QuoraDominatorCore.Models
{
    public class QuoraUser : BaseUser, IUser
    {
        public QuoraUser()
        {
        }

        public QuoraUser(string pk, string username)
            : base(pk, username)
        {
        }

        public Gender Gender
        {
            get
            {
                var regexPattern = StringHelper.GetRegexPatern("\\p{L}+?(?=[^\\p{L}]|$)", FullName);
                if (string.IsNullOrWhiteSpace(regexPattern))
                    return Gender.Unknown;
                return ConvertGenderType(regexPattern.GetGender());
            }
        }

        public bool? HasAnonymousProfilePicture { get; set; }

        public bool IsBusiness { get; set; }

        public bool IsPrivate { get; set; }

        public bool IsVerified { get; set; }

        public int FollowingCount { get; set; }
        public int FollowerCount { get; set; }

        public int FollowedBack { get; set; }

        public string Formkey { get; set; }

        public string PostKey { get; set; }

        public string WindowId { get; set; }

        public string Uid { get; set; }

        public string Url { get; set; }

        public int NumberOfPost { get; set; }
        public int AnswerCount { get; set; }
        public int QuestionCount { get; set; }

        public string FullName { get; set; }

        public string ProfilePicUrl { get; set; }
        public string UserId { get; set; }

        private Gender ConvertGenderType(Gender gender)
        {
            switch (gender)
            {
                case Gender.Male:
                    return Gender.Male;
                case Gender.Female:
                    return Gender.Female;
                case Gender.Unisex:
                case Gender.Unknown:
                    return Gender.Unknown;
                default:
                    throw new ArgumentOutOfRangeException(nameof(gender), gender, null);
            }
        }
    }
}