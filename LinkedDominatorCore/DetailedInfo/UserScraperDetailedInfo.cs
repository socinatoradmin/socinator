namespace LinkedDominatorCore.DetailedInfo
{
    public class UserScraperDetailedInfo
    {
        public bool IsVisiting { get; set; } = false;
        public string TitleCurrent { get; set; } = string.Empty;
        public string CompanyCurrent { get; set; } = string.Empty;
        public string CurrentCompanyUrl { get; set; } = string.Empty;
        public string CompanyDescription { get; set; } = string.Empty;
        public string CurrentCompanyWebsite { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string CompanyLocation { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string Industry { get; set; } = string.Empty;
        public string ConnectionType { get; set; } = string.Empty;
        public string Connection { get; set; } = string.Empty;
        public string Recommendation { get; set; } = string.Empty;
        public string Skill { get; set; } = string.Empty;
        public string Experience { get; set; } = string.Empty;
        public string EducationCollection { get; set; } = string.Empty;
        public string PastTitles { get; set; } = string.Empty;
        public string PastCompany { get; set; } = string.Empty;
        public string AccountEmail { get; set; } = string.Empty;
        public string AccountUserFullName { get; set; } = string.Empty;
        public string AccountUserProfileUrl { get; set; } = string.Empty;
        public string Flagship3ProfileViewBase { get; set; } = string.Empty;

        #region Personal

        public string ProfileType { get; set; } = string.Empty;
        public string ProfileUrl { get; set; } = string.Empty;
        public string ProfilePicUrl { get; set; } = string.Empty;
        public string Firstname { get; set; } = string.Empty;
        public string Lastname { get; set; } = string.Empty;
        public string HeadlineTitle { get; set; } = string.Empty;
        public string EmailId { get; set; } = string.Empty;
        public string PersonalPhoneNumber { get; set; } = string.Empty;
        public string PersonalWebsites { get; set; } = string.Empty;
        public string Birthdate { get; set; } = string.Empty;
        public string TwitterUserName { get; set; } = string.Empty;
        public string TwitterUrl { get; set; } = string.Empty;
        public string ConnectedTime { get; set; } = string.Empty;
        public string username { get; set; } = string.Empty;
        #endregion

        #region User Details Required for Engage Module Request

        public string PublicIdentifier { get; set; } = string.Empty;
        public string ProfileId { get; set; } = string.Empty;
        public string MemberId { get; set; } = string.Empty;
        public string TrackingId { get; set; } = string.Empty;
        public string Occupation { get; set; } = string.Empty;

        #endregion
    }
}