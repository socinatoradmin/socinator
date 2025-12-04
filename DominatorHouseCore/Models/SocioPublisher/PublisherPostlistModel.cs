#region

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using DominatorHouseCore.Enums.SocioPublisher;
using DominatorHouseCore.Models.SocioPublisher.Settings;
using DominatorHouseCore.Utility;
using ProtoBuf;

#endregion

namespace DominatorHouseCore.Models.SocioPublisher
{
    [Serializable]
    [ProtoContract]
    public class PublisherPostlistModel : BindableBase
    {
        #region Properties

        private string _postDescription = string.Empty;

        /// <summary>
        ///     To describe the post data
        /// </summary>
        [ProtoMember(1)]
        public string PostDescription
        {
            get => _postDescription;
            set
            {
                if (_postDescription == value)
                    return;

                _postDescription = value;
                OnPropertyChanged(nameof(PostDescription));
            }
        }


        private DateTime _createdTime;

        /// <summary>
        ///     To specify the post created date time
        /// </summary>
        [ProtoMember(2)]
        public DateTime CreatedTime
        {
            get => _createdTime;
            set
            {
                if (_createdTime == value)
                    return;
                _createdTime = value;
                OnPropertyChanged(nameof(CreatedTime));
            }
        }


        private PostSource _postSource = PostSource.NormalPost;

        /// <summary>
        ///     To specify from where the post comes
        /// </summary>
        [ProtoMember(3)]
        public PostSource PostSource
        {
            get => _postSource;
            set
            {
                if (_postSource == value)
                    return;
                _postSource = value;
                OnPropertyChanged(nameof(PostSource));
            }
        }


        private PostCategory _postCategory;

        /// <summary>
        ///     To specify the post category
        /// </summary>
        [ProtoMember(4)]
        public PostCategory PostCategory
        {
            get => _postCategory;
            set
            {
                if (_postCategory == value)
                    return;
                _postCategory = value;
                OnPropertyChanged(nameof(PostCategory));
            }
        }

        private string _campaignId = string.Empty;

        /// <summary>
        ///     To specify the post category
        /// </summary>
        [ProtoMember(5)]
        public string CampaignId
        {
            get => _campaignId;
            set
            {
                if (_campaignId == value)
                    return;
                _campaignId = value;
                OnPropertyChanged(nameof(CampaignId));
            }
        }


        private PostRunningStatus _postRunningStatus = PostRunningStatus.Active;

        /// <summary>
        ///     To specify the post running status whether active or completed
        /// </summary>
        [ProtoMember(6)]
        public PostRunningStatus PostRunningStatus
        {
            get => _postRunningStatus;
            set
            {
                if (_postRunningStatus == value)
                    return;
                _postRunningStatus = value;
                OnPropertyChanged(nameof(PostRunningStatus));
            }
        }

        private PostQueuedStatus _postQueuedStatus;

        /// <summary>
        ///     To specify the post queued status whether pending, draft or published
        /// </summary>
        [ProtoMember(7)]
        public PostQueuedStatus PostQueuedStatus
        {
            get => _postQueuedStatus;
            set
            {
                if (_postQueuedStatus == value)
                    return;
                _postQueuedStatus = value;
                OnPropertyChanged(nameof(PostQueuedStatus));
            }
        }


        private DateTime? _expiredTime;

        /// <summary>
        ///     To specify the post expired time
        /// </summary>
        [ProtoMember(8)]
        public DateTime? ExpiredTime
        {
            get => _expiredTime;
            set
            {
                if (_expiredTime == value)
                    return;
                _expiredTime = value;
                OnPropertyChanged(nameof(ExpiredTime));
            }
        }


        private ObservableCollection<string> _mediaList = new ObservableCollection<string>();

        /// <summary>
        ///     To hold the image or video file
        /// </summary>
        [ProtoMember(9)]
        public ObservableCollection<string> MediaList
        {
            get => _mediaList;
            set => SetProperty(ref _mediaList, value);
        }

        private string _postId = Utilities.GetGuid();

        /// <summary>
        ///     To specify the post id
        /// </summary>
        [ProtoMember(10)]
        public string PostId
        {
            get => _postId;
            set
            {
                if (_postId == value)
                    return;
                _postId = value;
                OnPropertyChanged(nameof(PostId));
            }
        }


        private bool _isPostlistSelected;

        /// <summary>
        ///     To specify the post list is selected or not
        /// </summary>
        [ProtoIgnore]
        public bool IsPostlistSelected
        {
            get => _isPostlistSelected;
            set
            {
                if (_isPostlistSelected == value)
                    return;
                _isPostlistSelected = value;
                OnPropertyChanged(nameof(IsPostlistSelected));
            }
        }

        private string _shareUrl = string.Empty;

        [ProtoMember(21)]
        public string ShareUrl
        {
            get => _shareUrl;
            set
            {
                _shareUrl = value;
                if (_shareUrl == value)
                    return;
                _shareUrl = value;
                OnPropertyChanged(nameof(ShareUrl));
            }
        }


        private bool _isFdSellPost;

        [ProtoMember(22)]
        public bool IsFdSellPost
        {
            get => _isFdSellPost;
            set
            {
                if (value == _isFdSellPost)
                    return;
                SetProperty(ref _isFdSellPost, value);
            }
        }

        private ObservableCollection<PublishedPostDetailsModel> _lstPublishedPostDetailsModels =
            new ObservableCollection<PublishedPostDetailsModel>();

        [ProtoMember(23)]
        public ObservableCollection<PublishedPostDetailsModel> LstPublishedPostDetailsModels
        {
            get => _lstPublishedPostDetailsModels;
            set
            {
                if (value == _lstPublishedPostDetailsModels)
                    return;
                SetProperty(ref _lstPublishedPostDetailsModels, value);
            }
        }


        private string _fetchedPostId = string.Empty;

        [ProtoMember(25)]
        public string FetchedPostIdOrUrl
        {
            get => _fetchedPostId;
            set
            {
                if (_fetchedPostId == value)
                    return;
                SetProperty(ref _fetchedPostId, value);
            }
        }


        private string _publishedTriedAndSuccessStatus = "00/00";

        [ProtoMember(26)]
        public string PublishedTriedAndSuccessStatus
        {
            get => _publishedTriedAndSuccessStatus;
            set
            {
                if (_publishedTriedAndSuccessStatus == value)
                    return;
                SetProperty(ref _publishedTriedAndSuccessStatus, value);
            }
        }

        private string _monitorFilePath = string.Empty;

        [ProtoMember(28)]
        public string MonitorFilePath
        {
            get => _monitorFilePath;
            set
            {
                if (_monitorFilePath == value)
                    return;
                SetProperty(ref _monitorFilePath, value);
            }
        }

        private bool _isSpinTax;

        [ProtoMember(30)]
        public bool IsSpinTax
        {
            get => _isSpinTax;
            set
            {
                if (value == _isSpinTax)
                    return;
                SetProperty(ref _isSpinTax, value);
            }
        }

        #region Postlist

        private string _currentMediaUrl = string.Empty;

        public string CurrentMediaUrl
        {
            get => _currentMediaUrl;
            set
            {
                if (_currentMediaUrl == value)
                    return;
                _currentMediaUrl = value;
                OnPropertyChanged(nameof(CurrentMediaUrl));
            }
        }


        private bool _nextImageEnable;

        public bool NextImageEnable
        {
            get => _nextImageEnable;
            set
            {
                if (_nextImageEnable == value)
                    return;
                _nextImageEnable = value;
                OnPropertyChanged(nameof(NextImageEnable));
            }
        }


        private bool _previousImageEnable;

        public bool PreviousImageEnable
        {
            get => _previousImageEnable;
            set
            {
                if (_previousImageEnable == value)
                    return;
                _previousImageEnable = value;
                OnPropertyChanged(nameof(PreviousImageEnable));
            }
        }


        private int _mediaCurrentPointer;

        public int MediaCurrentPointer
        {
            get => _mediaCurrentPointer;
            set
            {
                if (_mediaCurrentPointer == value)
                    return;
                _mediaCurrentPointer = value;
                OnPropertyChanged(nameof(MediaCurrentPointer));
            }
        }

        private int _imagePointer;

        public int ImagePointer
        {
            get => _imagePointer;
            set
            {
                if (_imagePointer == value)
                    return;
                _imagePointer = value;
                OnPropertyChanged(nameof(ImagePointer));
            }
        }

        private int _totalMediaCount;

        public int TotalMediaCount
        {
            get => _totalMediaCount;
            set
            {
                if (_totalMediaCount == value)
                    return;
                _totalMediaCount = value;
                OnPropertyChanged(nameof(TotalMediaCount));
            }
        }

        private bool _isPostListPresent = true;

        public bool IsPostListPresent
        {
            get => _isPostListPresent;
            set
            {
                if (_isPostListPresent == value)
                    return;
                _isPostListPresent = value;
                OnPropertyChanged(nameof(IsPostListPresent));
            }
        }

        public bool CanPostForNetwork { get; set; } = true;

        #endregion

        #region Settings

        private GeneralPostSettings _generalPostSettings;

        [ProtoMember(27)]
        public GeneralPostSettings GeneralPostSettings
        {
            get => _generalPostSettings;
            set
            {
                if (_generalPostSettings == value)
                    return;
                SetProperty(ref _generalPostSettings, value);
            }
        }


        private FdPostSettings _fdPostSettings = new FdPostSettings();

        [ProtoMember(11)]
        public FdPostSettings FdPostSettings
        {
            get => _fdPostSettings;
            set
            {
                if (_fdPostSettings == value)
                    return;
                SetProperty(ref _fdPostSettings, value);
            }
        }


        private GdPostSettings _gdPostSettings = new GdPostSettings();

        [ProtoMember(12)]
        public GdPostSettings GdPostSettings
        {
            get => _gdPostSettings;
            set
            {
                if (_gdPostSettings == value)
                    return;

                SetProperty(ref _gdPostSettings, value);
            }
        }


        private TdPostSettings _tdPostSettings = new TdPostSettings();

        [ProtoMember(13)]
        public TdPostSettings TdPostSettings
        {
            get => _tdPostSettings;
            set
            {
                if (_tdPostSettings == value)
                    return;

                SetProperty(ref _tdPostSettings, value);
            }
        }

        private LdPostSettings _ldPostSettings = new LdPostSettings();

        [ProtoMember(14)]
        public LdPostSettings LdPostSettings
        {
            get => _ldPostSettings;
            set
            {
                if (_ldPostSettings == value)
                    return;

                SetProperty(ref _ldPostSettings, value);
            }
        }

        private RedditPostSetting _redditPostSetting = new RedditPostSetting();

        [ProtoMember(24)]
        public RedditPostSetting RedditPostSetting
        {
            get => _redditPostSetting;
            set
            {
                if (_redditPostSetting == value)
                    return;
                SetProperty(ref _redditPostSetting, value);
            }
        }

        private TumblrPostSettings _tumblrPostSettings = new TumblrPostSettings();

        [ProtoMember(15)]
        public TumblrPostSettings TumblrPostSettings
        {
            get => _tumblrPostSettings;
            set
            {
                if (_tumblrPostSettings == value)
                    return;

                SetProperty(ref _tumblrPostSettings, value);
            }
        }
        private YTPostSettings _YTPostSettings = new YTPostSettings();

        [ProtoMember(42)]
        public YTPostSettings YTPostSettings
        {
            get => _YTPostSettings;
            set
            {
                if (_YTPostSettings == value)
                    return;

                SetProperty(ref _YTPostSettings, value);
            }
        }
        private string _fdSellProductTitle;

        [ProtoMember(16)]
        public string FdSellProductTitle
        {
            get => _fdSellProductTitle;
            set
            {
                if (value == _fdSellProductTitle)
                    return;
                SetProperty(ref _fdSellProductTitle, value);
            }
        }

        private double _fdSellPrice;

        [ProtoMember(17)]
        public double FdSellPrice
        {
            get => _fdSellPrice;
            set
            {
                if (Math.Abs(value - _fdSellPrice) < 0.00001)
                    return;
                SetProperty(ref _fdSellPrice, value);
            }
        }

        private string _fdSellLocation;

        [ProtoMember(18)]
        public string FdSellLocation
        {
            get => _fdSellLocation;
            set
            {
                if (value == _fdSellLocation)
                    return;
                SetProperty(ref _fdSellLocation, value);
            }
        }

        private string _publisherInstagramTitle;

        /// <summary>
        ///     Get-Set Title for post (for all the networks)
        /// </summary>
        [ProtoMember(19)]
        public string PublisherInstagramTitle
        {
            get => _publisherInstagramTitle;
            set
            {
                if (_publisherInstagramTitle == value)
                    return;
                _publisherInstagramTitle = value;
                OnPropertyChanged(nameof(PublisherInstagramTitle));
            }
        }

        private string _pdSourceUrl;
        private PublisherPostSettings _publisherPostSettings = new PublisherPostSettings();

        [ProtoMember(20)]
        public string PdSourceUrl
        {
            get => _pdSourceUrl;
            set
            {
                if (value == _pdSourceUrl)
                    return;
                SetProperty(ref _pdSourceUrl, value);
            }
        }

        [ProtoMember(29)]
        public PublisherPostSettings PublisherPostSettings
        {
            get => _publisherPostSettings;
            set
            {
                if (value == _publisherPostSettings)
                    return;
                SetProperty(ref _publisherPostSettings, value);
            }
        }

        private bool _isChangeHashOfMedia;

        [ProtoMember(35)]
        public bool IsChangeHashOfMedia
        {
            get => _isChangeHashOfMedia;
            set => SetProperty(ref _isChangeHashOfMedia, value);
        }

        private string _redditScrapedMediaType;

        [ProtoMember(36)]
        public string RedditScrapedMediaType
        {
            get => _redditScrapedMediaType;
            set
            {
                if (_redditScrapedMediaType == value)
                    return;
                _redditScrapedMediaType = value;
                OnPropertyChanged(nameof(RedditScrapedMediaType));
            }
        }

        private string _redditScrapedVideoUrl;

        [ProtoMember(37)]
        public string RedditScrapedVideoUrl
        {
            get => _redditScrapedVideoUrl;
            set
            {
                if (_redditScrapedVideoUrl == value)
                    return;
                _redditScrapedVideoUrl = value;
                OnPropertyChanged(nameof(RedditScrapedVideoUrl));
            }
        }
        private ScrapePostModel Scrape_PostModel = new ScrapePostModel();

        [ProtoMember(38)]
        public ScrapePostModel scrapePostModel
        {
            get => Scrape_PostModel;
            set
            {
                if (value == Scrape_PostModel)
                    return;
                SetProperty(ref Scrape_PostModel, value);
            }
        }
        private SharePostModel _sharePostModel = new SharePostModel();
        [ProtoMember(39)]
        public SharePostModel sharePostModel
        {
            get => _sharePostModel;
            set
            {
                if (value == _sharePostModel)
                    return;
                SetProperty(ref _sharePostModel, value);
            }
        }
        private PostDetailsModel postDetailsModel = new PostDetailsModel();
        [ProtoMember(40)]
        public PostDetailsModel PostDetailModel
        {
            get => postDetailsModel;
            set
            {
                if (value == postDetailsModel)
                    return;
                SetProperty(ref postDetailsModel, value);
            }
        }
        [NonSerialized] public List<SectionDetails> ListOfSections = new List<SectionDetails>();
        private string _fdCondition;

        [ProtoMember(41)]
        public string FdCondition
        {
            get => _fdCondition;
            set => SetProperty(ref _fdCondition, value);
        }
        #endregion

        #endregion

        #region Methods

        public void GenerateClonePostId()
        {
            PostId = Utilities.GetGuid();
            CreatedTime = DateTime.Now;
        }

        public void GenerateNewPostId()
        {
            PostId = Utilities.GetGuid();
        }

        /// <summary>
        ///     Update the posts navigation details based on media list
        /// </summary>
        public void InitializePostData()
        {
            try
            {
                // Check whether media list contains items or not
                IsPostListPresent = MediaList.Count > 0;

                // Update the process
                if (IsPostListPresent)
                {
                    ImagePointer = 0;
                    MediaCurrentPointer = 1;
                    var mediaUtilites = new MediaUtilites();

                    CurrentMediaUrl = mediaUtilites.GetThumbnail(MediaList[ImagePointer]);
                    TotalMediaCount = MediaList.Count;
                    NextImageEnable = TotalMediaCount - ImagePointer > -1;
                    PreviousImageEnable = ImagePointer > 0;
                }
                else
                {
                    ImagePointer = 0;
                    MediaCurrentPointer = 0;
                    CurrentMediaUrl = string.Empty;
                    TotalMediaCount = MediaList.Count;
                    NextImageEnable = false;
                    PreviousImageEnable = false;
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public void UpdateNavigationPointer()
        {
            NextImageEnable = TotalMediaCount - MediaCurrentPointer > 0;
            PreviousImageEnable = ImagePointer > 0;
        }

        #endregion
    }
}