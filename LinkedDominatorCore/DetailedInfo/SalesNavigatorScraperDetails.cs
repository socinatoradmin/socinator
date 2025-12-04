namespace LinkedDominatorCore.DetailedInfo
{
    public class CurrentCompanyDetails
    {
        public string CurrentCompanyDescription = string.Empty;
        public string CurrentCompanyFoundingYear = string.Empty;
        public string CurrentCompanyHeadquarters = string.Empty;
        public string CurrentCompanyId = string.Empty;
        public string CurrentCompanyIndustryType = string.Empty;
        public string CurrentCompanyLocation = string.Empty;
        public string CurrentCompanyLogo = string.Empty;
        public string CurrentCompanySize = string.Empty;
        public string CurrentCompanyWebsite = string.Empty;
        public string Name = string.Empty;
    }

    public class SalesNavigatorScraperDetails : CurrentCompanyDetails
    {
        public string AccountUsed = string.Empty;
        public string ConnectionType = string.Empty;
        public string CurrentCompany = string.Empty;
        public string CurrentTitle = string.Empty;
        public string DetailsRequiredToSendConnectionRequest = string.Empty;
        public string Education = string.Empty;
        public string Email = string.Empty;
        public string Experience = string.Empty;
        public string FullName = string.Empty;
        public string HeadlineTitle = string.Empty;
        public string Industry = string.Empty;
        public bool IsCheckedOnlyDetailsRequiredToSendConnectionRequest = false;
        public bool IsVisiting = false;
        public string Location = string.Empty;
        public string MemberId = string.Empty;
        public string NumberOfConnections = string.Empty;
        public string NumberOfSharedConnections = string.Empty;
        public string PastCompanies = string.Empty;
        public string PastTitles = string.Empty;
        public string PhoneNumber = string.Empty;
        public string ProfilePicUrl = string.Empty;
        public string ProfileSummary = string.Empty;
        public string ProfileUrl = string.Empty;
        public string SalesNavigatorProfileUrl = string.Empty;
        public string Skills = string.Empty;

        public SalesNavigatorScraperDetails(string memberId)
        {
            MemberId = memberId;
        }
    }
}