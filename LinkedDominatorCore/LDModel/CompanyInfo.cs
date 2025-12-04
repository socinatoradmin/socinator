namespace LinkedDominatorCore.LDModel
{
    public class CompanyInfo
    {
        public string PastTitles { get; set; } = string.Empty;
        public string CompanyResponse {  get; set; } = string.Empty;
        public string Experience {  get; set; } = string.Empty;
        public CurrentAndPastCompanyInfo CurrentAndPastCompanyInfo { get; set; } = new CurrentAndPastCompanyInfo();
    }
    public class CurrentAndPastCompanyInfo
    {
        public string TitleCurrent { get; set; } = string.Empty;
        public string CurrentCompany {  get; set; } = string.Empty;
        public string CurrentCompanyUrl {  get; set; } = string.Empty;
        public string PastCompany {  get; set; } = string.Empty;
        public string PastCompanyUrl { get;set; } = string.Empty;
        public string CurrentCompanyWebsite {  get; set; } = string.Empty;
    }
}
