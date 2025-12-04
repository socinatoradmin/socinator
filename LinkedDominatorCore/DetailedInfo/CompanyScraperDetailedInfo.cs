namespace LinkedDominatorCore.DetailedInfo
{
    public class CompanyScraperDetailedInfo
    {
        #region Account Info Variables Initializations

        public string AccountEmail = string.Empty;
        public string AccountUserFullName = string.Empty;
        public string AccountUserProfileUrl = string.Empty;

        #endregion

        #region Company Info Variables Initializations

        public string CompanyName = string.Empty;
        public string CompanyId = string.Empty;
        public string CompanyUrl = string.Empty;
        public string CompanyLogoUrl = string.Empty;
        public string Industry = string.Empty;
        public string CompanyDescription { get; set; } = string.Empty;
        public string Specialties = string.Empty;
        public string CompanySize = string.Empty;
        public string Website { get; set; } = string.Empty;
        public string FoundationDate { get; set; } = string.Empty;
        public string Headquarter { get; set; } = string.Empty;
        public string OtherLocations { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Zipcode { get; set; } = string.Empty;
        public string AddressLine1 { get; set; } = string.Empty;
        public string AddressLine2 { get; set; } = string.Empty;
        public string IsFollowing { get; set; } = string.Empty;
        public string TotalEmployees { get; set; } = string.Empty;
        #endregion
    }

    public class Location
    {
        public string Country { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Zipcode { get; set; } = string.Empty;
        public string AddressLine1 { get; set; } = string.Empty;
        public string AddressLine2 { get; set; } = string.Empty;
    }
}