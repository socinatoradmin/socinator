using DominatorHouseCore.Utility;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace GramDominatorCore.GDModel
{
    public class StoryCollection: BindableBase
    {
        public string OtherProfile { get; set; }

        private string _profileUrl;
        public string ProfileUrl
        {
            get => _profileUrl;
            set => SetProperty(ref _profileUrl, value);
        }
        private bool _isPrivate;
        public bool IsPrivate
        {
            get => _isPrivate;
            set => SetProperty(ref _isPrivate, value);
        }
        private string id;
        public string Id
        {
            get => id;
            set => SetProperty(ref id, value);
        }
        private string _ProfilePic;
        public string ProfilePic
        {
            get => _ProfilePic;
            set => SetProperty(ref _ProfilePic, value);
        }
        public string _Username;
        public string Username
        {
            get => _Username;
            set => SetProperty(ref _Username, value);
        }
        private int _PostCount;
        public int PostCount
        {
            get => _PostCount;
            set => SetProperty(ref _PostCount, value);
        }
        private int _FollowerCount;
        public int FollowerCount
        {
            get => _FollowerCount;
            set => SetProperty(ref _FollowerCount, value);
        }
        private int _FollowingCount;
        public int FollowingCount
        {
            get => _FollowingCount;
            set => SetProperty(ref _FollowingCount, value);
        }
        private string _FullName;
        public string FullName
        {
            get => _FullName;
            set => SetProperty(ref _FullName, value);
        }
        private string _Caption="N/A";
        public string Caption
        {
            get => _Caption;
            set => SetProperty(ref _Caption, value);
        }
        private ObservableCollection<StoriesMedia> _Stories=new ObservableCollection<StoriesMedia>();
        public ObservableCollection<StoriesMedia> Stories
        {
            get => _Stories;
            set => SetProperty(ref _Stories, value);
        }
        private ObservableCollection<InstaHightlight> _Highlights = new ObservableCollection<InstaHightlight>();
        public ObservableCollection<InstaHightlight> Highlights
        {
            get => _Highlights;
            set => SetProperty(ref _Highlights, value);
        }
    }
    public class StoriesMedia:BindableBase
    {
        public string Username { get; set; }
        private ICommand _downloadStoryCommand;
        public ICommand DownloadStoryCommand
        {
            get => _downloadStoryCommand;
            set => SetProperty(ref _downloadStoryCommand, value);
        }

        private string _StoryUrl;
        public string StoryUrl
        {
            get => _StoryUrl;
            set => SetProperty(ref _StoryUrl, value);
        }
        private string _StoryDate;
        public string StoryDate
        {
            get => _StoryDate;
            set => SetProperty(ref _StoryDate, value);
        }
        private string _videoUrl;
        public string VideoUrl
        {
            get => _videoUrl;
            set => SetProperty(ref _videoUrl, value);
        }
        private string _type;
        public string Type
        {
            get => _type;
            set => SetProperty(ref _type, value);
        }
        private int _likeCount;
        public int LikeCount
        {
            get => _likeCount;
            set => SetProperty(ref _likeCount, value);
        }
        private bool _isVideo;
        public bool IsVideo
        {
            get => _isVideo;
            set => SetProperty(ref _isVideo, value);
        }
        private string _id;
        public string Id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }
        private int _commentCount;
        public int CommentCount
        {
            get => _commentCount;
            set => SetProperty(ref _commentCount, value);
        }
        private string _createdAt;
        public string CreatedAt
        {
            get => _createdAt;
            set => SetProperty(ref _createdAt, value);
        }
        private string _caption;
        public string Caption
        {
            get => _caption;
            set => SetProperty(ref _caption, value);
        }
    }
}
