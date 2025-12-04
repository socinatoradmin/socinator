namespace FaceDominatorCore.FDLibrary.FdClassLibrary
{
    public class NewsFeedPagination
    {
        public string Pagelet { get; set; }

        public string AjaxToken { get; set; }

        public int ScrollCount { get; set; } = 0;

        public int ClientStoriesCount { get; set; } = 0;

        public double SectionId { get; set; }
    }
}
