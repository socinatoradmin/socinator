namespace DominatorHouseCore.Interfaces
{
    public interface ICompany
    {
        string CompanyId { get; set; }
        string CompanyUrl { get; set; }
        string CompanyName { get; set; }
        string TotalEmployees { get; set; }
        string Industry { get; set; }
        string Location { get; set; }
        string IsFollowed { get; set; }
    }
}