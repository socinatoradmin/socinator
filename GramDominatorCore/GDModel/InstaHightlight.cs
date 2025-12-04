using DominatorHouseCore.Utility;
using System.Windows.Input;

namespace GramDominatorCore.GDModel
{
    public class InstaHightlight:BindableBase
    {
        private ICommand _downloadStoryCommand;
        public ICommand DownloadStoryCommand
        {
            get => _downloadStoryCommand;
            set => SetProperty(ref _downloadStoryCommand, value);
        }
        private string _id;
        public string Id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }
        private string _title;
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }
        private string _coverUrl;
        public string CoverUrl
        {
            get => _coverUrl;
            set => SetProperty(ref _coverUrl, value);
        }
        private string _Username;
        public string Username
        {
            get => _Username;
            set => SetProperty(ref _Username, value);
        }
    }
    public class HeighlightDetails
    {
        public string MediaUrl { get; set; }
        public string VideoUrl { get; set; }
        public string Type { get; set; }
        public string Created { get; set; }
    }
}
