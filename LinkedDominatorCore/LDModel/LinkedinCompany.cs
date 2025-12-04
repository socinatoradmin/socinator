using DominatorHouseCore.Interfaces;

namespace LinkedDominatorCore.LDModel
{
    public class LinkedinCompany : ICompany
    {
        public LinkedinCompany(string companyId)
        {
            CompanyId = companyId;
            CompanyUrl = $"https://www.linkedin.com/company/{companyId}";
        }

        public string CompanyName { get; set; }
        public string TotalEmployees { get; set; }
        public string Industry { get; set; }
        public string Location { get; set; }
        public string IsFollowed { get; set; }

        public string LogoUrl { get; set; }

        public string CompanyId { get; set; }

        public string CompanyUrl { get; set; }
        //Note Check DB model For InteractedCompanies and add other Properties as Reqired
    }
}