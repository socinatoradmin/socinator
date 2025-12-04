using DominatorHouseCore.Utility;

namespace YoutubeDominatorCore.YoutubeModels
{
    public class SearchVideoFilterModel : BindableBase
    {
        private string _duration = "None";

        private string _sortBy = "Relevance";
        private string _uploadDate = "None";

        public string UploadDate
        {
            get => _uploadDate;
            set => SetProperty(ref _uploadDate, value);
        }

        public string Duration
        {
            get => _duration;
            set => SetProperty(ref _duration, value);
        }

        public string SortBy
        {
            get => _sortBy;
            set => SetProperty(ref _sortBy, value);
        }
    }
}