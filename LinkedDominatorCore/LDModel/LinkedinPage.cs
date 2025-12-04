using DominatorHouseCore.Interfaces;

namespace LinkedDominatorCore.LDModel
{
    public class LinkedinPage : IPage
    {
        public LinkedinPage()
        {
        }

        public LinkedinPage(string pageId)
        {
            PageId = pageId;
            PageUrl = $"https://www.linkedin.com/company/{PageId}";
        }

        public string PageId { get; set; }

        public string PageName { get; set; }

        public string PageUrl { get; set; }
        public string UniversalPageName { get; set; }

        public string FollowerCount { get; set; }

        public string StaffCount { get; set; }
        public string IsFollowed { get; set; }

        public string FanPageID { get; set; }

        public string FanPageName { get; set; }

        public string FanPageUrl { get; set; }
    }
}