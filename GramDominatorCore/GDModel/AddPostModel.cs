//using DominatorHouseCore.Utility;
//using ProtoBuf;
//using System;
//using System.Collections.Generic;
//using System.Collections.ObjectModel;
//using System.ComponentModel;
//using System.Windows;

//namespace GramDominatorCore.GDModel
//{
//    [ProtoContract]
//    public class AddPostModel : BindableBase
//    {
//        private ICollectionView _postsDetailCollection;


//        public ICollectionView PostsDetailCollection
//        {
//            get { return _postsDetailCollection; }
//            set
//            {
//                if (_postsDetailCollection != null && value == _postsDetailCollection)
//                    return;
//                SetProperty(ref _postsDetailCollection, value);
//            }
//        }

//        private string  _messeges;
//        [ProtoMember(1)]
//        public string Messeges
//        {
//            get
//            {
//                return _messeges;
//            }
//            set
//            {
//                if (value == _messeges)
//                    return;
//                SetProperty(ref _messeges, value);
//            }
//        }
//        private List<string> _photoUrl=new List<string>();
//        [ProtoMember(2)]
//        public List<string> PhotoUrl
//        {
//            get
//            {
//                return _photoUrl;
//            }
//            set
//            {
//                if (value == _photoUrl)
//                    return;
//                SetProperty(ref _photoUrl, value);
//             }
//        }

//        private List<string> _videoUrl=new List<string>();
//        [ProtoMember(3)]
//        public List<string> VideoUrl
//        {
//            get
//            {
//                return _videoUrl;
//            }
//            set
//            {
//                if (value == _videoUrl)
//                    return;
//                SetProperty(ref _videoUrl, value);
             
//            }
//        }
//        private string _titles;
//        [ProtoMember(4)]
//        public string Titles
//        {
//            get
//            {
//                return _titles;
//            }
//            set
//            {
//                if (value == _titles)
//                    return;
//                SetProperty(ref _titles, value);
               
//            }
//        }
//        private bool _isUseFacebookSellPostChecked;
//        [ProtoMember(5)]
//        public bool IsUseFacebookSellPostChecked
//        {
//            get
//            {
//                return _isUseFacebookSellPostChecked;
//            }
//            set
//            {
//                if (value == _isUseFacebookSellPostChecked)
//                    return;
//                SetProperty(ref _isUseFacebookSellPostChecked, value);
                
//            }
//        }
//        private bool _isUseInstagramStoryPollChecked;
//        [ProtoMember(6)]
//        public bool IsUseInstagramStoryPollChecked
//        {
//            get
//            {
//                return _isUseInstagramStoryPollChecked;
//            }
//            set
//            {
//                if (value == _isUseInstagramStoryPollChecked)
//                    return;
//                SetProperty(ref _isUseInstagramStoryPollChecked, value);
               
//            }
//        }

//        private int _maxPost;
//        [ProtoMember(7)]
//        public int MaxPost
//        {
//            get
//            {
//                return _maxPost;
//            }
//            set
//            {
//                if (value == _maxPost)
//                    return;
//                SetProperty(ref _maxPost, value);
              
//            }
//        }
//        private TimeSpan _startTime;
//        [ProtoMember(8)]
//        public TimeSpan StartTime
//        {
//            get
//            {
//                return _startTime;
//            }
//            set
//            {
//                if (value == _startTime)
//                    return;
//                SetProperty(ref _startTime, value);
//            }
//        }
//        private TimeSpan _endTime;
//        [ProtoMember(9)]
//        public TimeSpan EndTime
//        {
//            get
//            {
//                return _endTime;
//            }
//            set
//            {
//                if (value == _endTime)
//                    return;
//                SetProperty(ref _endTime, value);
               
//            }
//        }
//        private bool _isSpecifyPostingIntervalChecked;
//        [ProtoMember(10)]
//        public bool IsSpecifyPostingIntervalChecked
//        {
//            get
//            {
//                return _isSpecifyPostingIntervalChecked;
//            }
//            set
//            {
//                if (value == _isSpecifyPostingIntervalChecked)
//                    return;
//                SetProperty(ref _isSpecifyPostingIntervalChecked, value);
//            }
//        }
//        private bool _isRandomizePublishingTimerChecked;
//        [ProtoMember(11)]
//        public bool IsRandomizePublishingTimerChecked
//        {
//            get
//            {
//                return _isRandomizePublishingTimerChecked;
//            }
//            set
//            {
//                if (value == _isRandomizePublishingTimerChecked)
//                    return;
//                SetProperty(ref _isRandomizePublishingTimerChecked, value);
               
//            }
//        }
//        private bool _isRandomizeNumberOfPostsChecked;
//        [ProtoMember(12)]
//        public bool IsRandomizeNumberOfPostsChecked
//        {
//            get
//            {
//                return _isRandomizeNumberOfPostsChecked;
//            }
//            set
//            {
//                if (value == _isRandomizeNumberOfPostsChecked)
//                    return;
//                SetProperty(ref _isRandomizeNumberOfPostsChecked, value);
//            }
//        }
//        private RangeUtilities  _postBetween=new RangeUtilities();
//        [ProtoMember(13)]
//        public RangeUtilities PostBetween
//        {
//            get
//            {
//                return _postBetween;
//            }
//            set
//            {
//                if (value == _postBetween)
//                    return;
//                SetProperty(ref _postBetween, value);
//            }
//        }
//        private RangeUtilities _increaseEachDay = new RangeUtilities();
//        [ProtoMember(14)]
//        public RangeUtilities IncreaseEachDay
//        {
//            get
//            {
//                return _increaseEachDay;
//            }
//            set
//            {
//                if (value == _increaseEachDay)
//                    return;
//                SetProperty(ref _increaseEachDay, value);
//            }
//        }
//        private bool _isPublishPostOnDestinationsChecked;
//        [ProtoMember(15)]
//        public bool IsPublishPostOnDestinationsChecked
//        {
//            get
//            {
//                return _isPublishPostOnDestinationsChecked;
//            }
//            set
//            {
//                if (value == _isPublishPostOnDestinationsChecked)
//                    return;
//                SetProperty(ref _isPublishPostOnDestinationsChecked, value);
                
//            }
//        }
//        private bool _isAddRandomSleepTimeWhilePublishingChecked;
//        [ProtoMember(16)]
//        public bool IsAddRandomSleepTimeWhilePublishingChecked
//        {
//            get
//            {
//                return _isAddRandomSleepTimeWhilePublishingChecked;
//            }
//            set
//            {
//                if (value == _isAddRandomSleepTimeWhilePublishingChecked)
//                    return;
//                SetProperty(ref _isAddRandomSleepTimeWhilePublishingChecked, value);
               
//            }
//        }
//        private bool _isSleepBetweenChecked;
//        [ProtoMember(17)]
//        public bool IsSleepBetweenChecked
//        {
//            get
//            {
//                return _isSleepBetweenChecked;
//            }
//            set
//            {
//                if (value == _isSleepBetweenChecked)
//                    return;
//                SetProperty(ref _isSleepBetweenChecked, value);
//            }
//        }
//        private RangeUtilities _sleepBetween = new RangeUtilities();
//        [ProtoMember(18)]
//        public RangeUtilities SleepBetween
//        {
//            get
//            {
//                return _sleepBetween;
//            }
//            set
//            {
//                if (value == _sleepBetween)
//                    return;
//                SetProperty(ref _sleepBetween, value);
//            }
//        }
//        private RangeUtilities _sendingBetween;
//        [ProtoMember(19)]
//        public RangeUtilities SendingBetween
//        {
//            get
//            {
//                return _sendingBetween;
//            }
//            set
//            {
//                if (value == _sendingBetween)
//                    return;
//                SetProperty(ref _sendingBetween, value);
//            }
//        }
//        private bool _isCampaignHasStartDateChecked;
//        [ProtoMember(20)]
//        public bool IsCampaignHasStartDateChecked
//        {
//            get
//            {
//                return _isCampaignHasStartDateChecked;
//            }
//            set
//            {
//                if (value == _isCampaignHasStartDateChecked)
//                    return;
//                SetProperty(ref _isCampaignHasStartDateChecked, value);
//            }
//        }
//        private bool _isCampaignHasEndDateChecked;
//        [ProtoMember(21)]
//        public bool IsCampaignHasEndDateChecked
//        {
//            get
//            {
//                return _isCampaignHasEndDateChecked;
//            }
//            set
//            {
//                if (value == _isCampaignHasEndDateChecked)
//                    return;
//                SetProperty(ref _isCampaignHasEndDateChecked, value);
//            }
//        }
//        private int _waitAround;
//        [ProtoMember(22)]
//        public int WaitAround
//        {
//            get
//            {
//                return _waitAround;
//            }
//            set
//            {
//                if (value == _waitAround)
//                    return;
//                SetProperty(ref _waitAround, value);

//            }
//        }
//        private bool _isSundayChecked;
//        [ProtoMember(23)]
//        public bool IsSundayChecked
//        {
//            get
//            {
//                return _isSundayChecked;
//            }
//            set
//            {
//                if (value == _isSundayChecked)
//                    return;
//                SetProperty(ref _isSundayChecked, value);
//            }
//        }
//        private bool _isMondayChecked;
//        [ProtoMember(24)]
//        public bool IsMondayChecked
//        {
//            get
//            {
//                return _isMondayChecked;
//            }
//            set
//            {
//                if (value == _isMondayChecked)
//                    return;
//                SetProperty(ref _isMondayChecked, value);
//            }
//        }
//        private bool _isTuesdayChecked;
//        [ProtoMember(25)]
//        public bool IsTuesdayChecked
//        {
//            get
//            {
//                return _isTuesdayChecked;
//            }
//            set
//            {
//                if (value == _isTuesdayChecked)
//                    return;
//                SetProperty(ref _isTuesdayChecked, value);
//            }
//        }
//        private bool _isWednesdayChecked;
//        [ProtoMember(26)]
//        public bool IsWednesdayChecked
//        {
//            get
//            {
//                return _isWednesdayChecked;
//            }
//            set
//            {
//                if (value == _isWednesdayChecked)
//                    return;
//                SetProperty(ref _isWednesdayChecked, value);
//            }
//        }
//        private bool _isThursdayChecked;
//        [ProtoMember(27)]
//        public bool IsThursdayChecked
//        {
//            get
//            {
//                return _isThursdayChecked;
//            }
//            set
//            {
//                if (value == _isThursdayChecked)
//                    return;
//                SetProperty(ref _isThursdayChecked, value);
//            }
//        }
//        private bool _isFridayChecked;
//        [ProtoMember(28)]
//        public bool IsFridayChecked
//        {
//            get
//            {
//                return _isFridayChecked;
//            }
//            set
//            {
//                if (value == _isFridayChecked)
//                    return;
//                SetProperty(ref _isFridayChecked, value);
//            }
//        }
//        private bool _isSaturdayChecked;
//        [ProtoMember(29)]
//        public bool IsSaturdayChecked
//        {
//            get
//            {
//                return _isSaturdayChecked;
//            }
//            set
//            {
//                if (value == _isSaturdayChecked)
//                    return;
//                SetProperty(ref _isSaturdayChecked, value);
//            }
//        }
//        private bool _isRotateDayChecked;
//        [ProtoMember(30)]
//        public bool IsRotateDayChecked
//        {
//            get
//            {
//                return _isRotateDayChecked;
//            }
//            set
//            {
//                if (value == _isRotateDayChecked)
//                    return;
//                SetProperty(ref _isRotateDayChecked, value);
//            }
//        }
//        private string _campaign;
//        [ProtoMember(31)]
//        public string Campaign
//        {
//            get
//            {
//                return _campaign;
//            }
//            set
//            {
//                if (value == _campaign)
//                    return;
//                SetProperty(ref _campaign, value);
//            }
//        }
//        private string _campaigntTag;
//        [ProtoMember(32)]
//        public string CampaignTag
//        {
//            get
//            {
//                return _campaigntTag;
//            }
//            set
//            {
//                if (value == _campaigntTag)
//                    return;
//                SetProperty(ref _campaigntTag, value);
//            }
//        }
//        private bool _isEnableSignatureChecked;
//        [ProtoMember(33)]
//        public bool IsEnableSignatureChecked
//        {
//            get
//            {
//                return _isEnableSignatureChecked;
//            }
//            set
//            {
//                if (value == _isEnableSignatureChecked)
//                    return;
//                SetProperty(ref _isEnableSignatureChecked, value);
//            }
//        }
//        private bool _isShortenURLsChecked;
//        [ProtoMember(34)]
//        public bool IsShortenURLsChecked
//        {
//            get
//            {
//                return _isShortenURLsChecked;
//            }
//            set
//            {
//                if (value == _isShortenURLsChecked)
//                    return;
//                SetProperty(ref _isShortenURLsChecked, value);
//            }
//        }
//        private bool _isPostTextChecked;
//        [ProtoMember(35)]
//        public bool IsPostTextChecked
//        {
//            get
//            {
//                return _isPostTextChecked;
//            }
//            set
//            {
//                if (value == _isPostTextChecked)
//                    return;
//                SetProperty(ref _isPostTextChecked, value);
//            }
//        }
//        private bool _isAllowPublishingPinterestChecked;
//        [ProtoMember(36)]
//        public bool IsAllowPublishingPinterestChecked
//        {
//            get
//            {
//                return _isAllowPublishingPinterestChecked;
//            }
//            set
//            {
//                if (value == _isAllowPublishingPinterestChecked)
//                    return;
//                SetProperty(ref _isAllowPublishingPinterestChecked, value);
//            }
//        }
//        private bool _isPostAsStoryChecked;
//        [ProtoMember(37)]
//        public bool IsPostAsStoryChecked
//        {
//            get
//            {
//                return _isPostAsStoryChecked;
//            }
//            set
//            {
//                if (value == _isPostAsStoryChecked)
//                    return;
//                SetProperty(ref _isPostAsStoryChecked, value);
//            }
//        }
//        private bool _isMakeImagesUniqueChecked;
//        [ProtoMember(38)]
//        public bool IsMakeImagesUniqueChecked
//        {
//            get
//            {
//                return _isMakeImagesUniqueChecked;
//            }
//            set
//            {
//                if (value == _isMakeImagesUniqueChecked)
//                    return;
//                SetProperty(ref _isMakeImagesUniqueChecked, value);
//            }
//        }
//        private bool _isEnableWatermarkChecked;
//        [ProtoMember(39)]
//        public bool IsEnableWatermarkChecked
//        {
//            get
//            {
//                return _isEnableWatermarkChecked;
//            }
//            set
//            {
//                if (value == _isEnableWatermarkChecked)
//                    return;
//                SetProperty(ref _isEnableWatermarkChecked, value);
//            }
//        }

//        private bool _isEnableCustomTokensChecked;
//        [ProtoMember(40)]
//        public bool IsEnableCustomTokensChecked
//        {
//            get
//            {
//                return _isEnableCustomTokensChecked;
//            }
//            set
//            {
//                if (value == _isEnableCustomTokensChecked)
//                    return;
//                SetProperty(ref _isEnableCustomTokensChecked, value);
//            }
//        }
//        private string _importedText;
//        [ProtoMember(41)]
//        public string ImportedText
//        {
//            get
//            {
//                return _importedText;
//            }
//            set
//            {
//                if (value == _importedText)
//                    return;
//                SetProperty(ref _importedText, value);
//            }
//        }
//        private DateTime _campaignStartDate;
//        [ProtoMember(42)]
//        public DateTime CampaignStartDate
//        {
//            get
//            {
//                return _campaignStartDate;
//            }
//            set
//            {
//                if (value == _campaignStartDate)
//                    return;
//                SetProperty(ref _campaignStartDate, value);
//            }
//        }
//        private DateTime _campaignEndDate;
//        [ProtoMember(43)]
//        public DateTime CampaignEndDate
//        {
//            get
//            {
//                return _campaignEndDate;
//            }
//            set
//            {
//                if (value == _campaignEndDate)
//                    return;
//                SetProperty(ref _campaignEndDate, value);
//            }
//        }
//        private int _serialNo;
//        [ProtoMember(44)]
//        public int SerialNo
//        {
//            get
//            {
//                return _serialNo;
//            }
//            set
//            {
//                if (value == _serialNo)
//                    return;
//                SetProperty(ref _serialNo, value);
//            }
//        }
//        private string _finished="0/0";
//        [ProtoMember(45)]
//        public string Finished
//        {
//            get
//            {
//                return _finished;
//            }
//            set
//            {
//                if (value == _finished)
//                    return;
//                SetProperty(ref _finished, value);
//            }
//        }
//        private string _succeessfull="0/0";
//        [ProtoMember(46)]
//        public string Succeessfull
//        {
//            get
//            {
//                return _succeessfull;
//            }
//            set
//            {
//                if (value == _succeessfull)
//                    return;
//                SetProperty(ref _succeessfull, value);
//            }
//        }
//        private string _status= "Active";
//        [ProtoMember(47)]
//        public string Status
//        {
//            get
//            {
//                return _status;
//            }
//            set
//            {
//                if (value == _status)
//                    return;
//                SetProperty(ref _status, value);
//            }
//        }
//        private int _deleted;
//        [ProtoMember(48)]
//        public int Deleted
//        {
//            get
//            {
//                return _deleted;
//            }
//            set
//            {
//                if (value == _deleted)
//                    return;
//                SetProperty(ref _deleted, value);
//            }
//        }

//        private ObservableCollection<lstTimeSpan> _lstTimer=new ObservableCollection<lstTimeSpan>();
//        [ProtoMember(49)]
//        public ObservableCollection<lstTimeSpan> LstTimer
//        {
//            get
//            {
//                return _lstTimer;
//            }
//            set
//            {
//                if (value == _lstTimer)
//                    return;
//                SetProperty(ref _lstTimer, value);
//            }
//        }
//        private bool _isExpireDateChecked;
//        [ProtoMember(50)]
//        public bool IsExpireDateChecked
//        {
//            get
//            {
//                return _isExpireDateChecked;
//            }
//            set
//            {
//                if (value == _isExpireDateChecked)
//                    return;
//                SetProperty(ref _isExpireDateChecked, value);
//            }
//        }
//        private DateTime _expireDate;
//        [ProtoMember(51)]
//        public DateTime ExpireDate
//        {
//            get
//            {
//                return _expireDate;
//            }
//            set
//            {
//                if (value == _expireDate)
//                    return;
//                SetProperty(ref _expireDate, value);
//            }
//        }
//        private bool _isEnableReAddPostChecked;
//        [ProtoMember(52)]
//        public bool IsEnableReAddPostChecked
//        {
//            get
//            {
//                return _isEnableReAddPostChecked;
//            }
//            set
//            {
//                if (value == _isEnableReAddPostChecked)
//                    return;
//                SetProperty(ref _isEnableReAddPostChecked, value);
//            }
//        }
//        private int _times;
//        [ProtoMember(53)]
//        public int Times
//        {
//            get
//            {
//                return _times;
//            }
//            set
//            {
//                if (value == _times)
//                    return;
//                SetProperty(ref _times, value);
//            }
//        }
//        private bool _isGeneralChecked;
//        [ProtoMember(54)]
//        public bool IsGeneralChecked
//        {
//            get
//            {
//                return _isGeneralChecked;
//            }
//            set
//            {
//                if (value == _isGeneralChecked)
//                    return;
//                SetProperty(ref _isGeneralChecked, value);
//            }
//        }
//        private bool _isJobChecked;
//        [ProtoMember(55)]
//        public bool IsJobChecked
//        {
//            get
//            {
//                return _isJobChecked;
//            }
//            set
//            {
//                if (value == _isJobChecked)
//                    return;
//                SetProperty(ref _isJobChecked, value);
//            }
//        }
//        private string _tumblrTags;
//        [ProtoMember(56)]
//        public string TumblrTags
//        {
//            get
//            {
//                return _tumblrTags;
//            }
//            set
//            {
//                if (value == _tumblrTags)
//                    return;
//                SetProperty(ref _tumblrTags, value);
//            }
//        }
//        private string _descriptionUsedForFacebook;
//        [ProtoMember(57)]
//        public string DescriptionUsedForFacebook
//        {
//            get
//            {
//                return _descriptionUsedForFacebook;
//            }
//            set
//            {
//                if (value == _descriptionUsedForFacebook)
//                    return;
//                SetProperty(ref _descriptionUsedForFacebook, value);
//            }
//        }
//        private bool _isPostAsPartOfStoryChecked;
//        [ProtoMember(58)]
//        public bool IsPostAsPartOfStoryChecked
//        {
//            get
//            {
//                return _isPostAsPartOfStoryChecked;
//            }
//            set
//            {
//                if (value == _isPostAsPartOfStoryChecked)
//                    return;
//                SetProperty(ref _isPostAsPartOfStoryChecked, value);
//            }
//        }
//        private bool _isDeletePostAfterChecked;
//        [ProtoMember(59)]
//        public bool IsDeletePostAfterChecked
//        {
//            get
//            {
//                return _isDeletePostAfterChecked;
//            }
//            set
//            {
//                if (value == _isDeletePostAfterChecked)
//                    return;
//                SetProperty(ref _isDeletePostAfterChecked, value);
//            }
//        }
//        private int _hours;
//        [ProtoMember(60)]
//        public int Hours
//        {
//            get
//            {
//                return _hours;
//            }
//            set
//            {
//                if (value == _hours)
//                    return;
//                SetProperty(ref _hours, value);
//            }
//        }

//        private bool _isArchivePostChecked;
//        [ProtoMember(61)]
//        public bool IsArchivePostChecked
//        {
//            get
//            {
//                return _isArchivePostChecked;
//            }
//            set
//            {
//                if (value == _isArchivePostChecked)
//                    return;
//                SetProperty(ref _isArchivePostChecked, value);
//            }
//        }
//        private bool _isRepostusingGeoLocationChecked;
//        [ProtoMember(62)]
//        public bool IsRepostusingGeoLocationChecked
//        {
//            get
//            {
//                return _isRepostusingGeoLocationChecked;
//            }
//            set
//            {
//                if (value == _isRepostusingGeoLocationChecked)
//                    return;
//                SetProperty(ref _isRepostusingGeoLocationChecked, value);
//            }
//        }
//        private bool _isTagSpecificUsersChecked;
//        [ProtoMember(63)]
//        public bool IsTagSpecificUsersChecked
//        {
//            get
//            {
//                return _isTagSpecificUsersChecked;
//            }
//            set
//            {
//                if (value == _isTagSpecificUsersChecked)
//                    return;
//                SetProperty(ref _isTagSpecificUsersChecked, value);
//            }
//        }
//        private bool _isChooseVideoCoverChecked;
//        [ProtoMember(64)]
//        public bool IsChooseVideoCoverChecked
//        {
//            get
//            {
//                return _isChooseVideoCoverChecked;
//            }
//            set
//            {
//                if (value == _isChooseVideoCoverChecked)
//                    return;
//                SetProperty(ref _isChooseVideoCoverChecked, value);
//            }
//        }

//        private Campaign _campaignDetails = new Campaign();
//        [ProtoMember(65)]
//        public Campaign CampaignDetails
//        {
//            get
//            {
//                return _campaignDetails;
//            }
//            set
//            {
//                if (value == _campaignDetails)
//                    return;
//                SetProperty(ref _campaignDetails, value);
//            }
//        }

//        private PostStatus _postStatus = new PostStatus();
//        [ProtoMember(66)]
//        public PostStatus PostStatus
//        {
//            get
//            {
//                return _postStatus;
//            }
//            set
//            {
//                if (value == _postStatus)
//                    return;
//                SetProperty(ref _postStatus, value);
//            }
//        }

//        private LocationDetails _locationDetails=new LocationDetails();
//        public LocationDetails LocationDetails
//        {
//            get
//            {
//                return _locationDetails;
//            }
//            set
//            {
//                if (value == _locationDetails)
//                    return;
//                SetProperty(ref _locationDetails, value);
//            }
//        }

//        private ICollectionView _locationDetailsCollection;
//        public ICollectionView LocationDetailsCollection
//        {
//            get
//            {
//                return _locationDetailsCollection;
//            }
//            set
//            {
//                if (value == _locationDetailsCollection)
//                    return;
//                SetProperty(ref _locationDetailsCollection, value);
//            }
//        }

      

//    }

//    [ProtoContract]
//    public class lstTimeSpan : BindableBase
//    {
//        private TimeSpan _timeSpan;
//        [ProtoMember(1)]
//        public TimeSpan TimeSpan
//        {
//            get { return _timeSpan; }
//            set
//            {
//                if (value == _timeSpan)
//                    return;
//                SetProperty(ref _timeSpan, value);
//            }
//        }
//    }

//    [ProtoContract]
//    public class LocationDetails : BindableBase
//    {
//        private int _locationId=1;
//        [ProtoMember(1)]
//        public int LocationId
//        {
//            get { return _locationId; }
//            set
//            {
//                if (value == _locationId)
//                    return;
//                SetProperty(ref _locationId, value);
//            }
//        }
//        private string _locationName="xyz";
//        [ProtoMember(2)]
//        public string LocationName
//        {
//            get { return _locationName; }
//            set
//            {
//                if (value == _locationName)
//                    return;
//                SetProperty(ref _locationName, value);
//            }
//        }
//        private string _locationAddress="abc";
//        [ProtoMember(3)]
//        public string LocationAddress
//        {
//            get { return _locationAddress; }
//            set
//            {
//                if (value == _locationAddress)
//                    return;
//                SetProperty(ref _locationAddress, value);
//            }
//        }
//    }

//    [ProtoContract]
//    public class Campaign:BindableBase
//    {
//        private ObservableCollection<string> _lstCampaign = new ObservableCollection<string>();
    
//        public ObservableCollection<string> LstCampaign
//        {
//            get
//            {
//                return _lstCampaign;
//            }
//            set
//            {
//                if (_lstCampaign != null && value == _lstCampaign)
//                    return;
//                SetProperty(ref _lstCampaign, value);
//            }
//        }
//        private ObservableCollection<string> _lstStatus = new ObservableCollection<string>();
   
//        public ObservableCollection<string> LstStatus
//        {
//            get { return _lstStatus; }
//            set
//            {
//                if (_lstStatus != null && value == _lstStatus)
//                    return;
//                SetProperty(ref _lstStatus, value);
//            }
//        }

//        private string _selectedCampaignName;
//        [ProtoMember(1)]
//        public string SelectedCampaignName
//        {
//            get
//            {
//                return _selectedCampaignName;
//            }
//            set
//            {
//                if (value == _selectedCampaignName)
//                    return;
//                SetProperty(ref _selectedCampaignName, value);
//            }
//        }
//        private string _selectedStatus = Application.Current.FindResource("LangKeyAll")?.ToString();
//        [ProtoMember(2)]
//        public string SelectedStatus
//        {
//            get
//            {
//                return _selectedStatus;
//            }
//            set
//            {
//                if (value == _selectedStatus)
//                    return;
//                SetProperty(ref _selectedStatus, value);
//            }
//        }
//        private string _campaignName="Campaign - [ "+ DateTime.Now+" ]";
//        [ProtoMember(3)]
//        public string CampaignName
//        {
//            get
//            {
//                return _campaignName;
//            }
//            set
//            {
//                if (value == _campaignName)
//                    return;
//                SetProperty(ref _campaignName, value);
//            }
//        }
       
//        private string _status= "Active";
//        [ProtoMember(4)]
//        public string Status
//        {
//            get
//            {
//                return _status;
//            }
//            set
//            {
//                if (value == _status)
//                    return;
//                SetProperty(ref _status, value);
//            }
//        }
       
      
//    }

//    [ProtoContract]
//    public class PostStatus : BindableBase
//    {
//        private int _pendingCount;
//        [ProtoMember(1)]
//        public int PendingCount
//        {
//            get
//            {
//                return _pendingCount;
//            }
//            set
//            {
//                if (value == _pendingCount)
//                    return;
//                SetProperty(ref _pendingCount, value);
//            }
//        }
//        private int _draftCount;
//        [ProtoMember(2)]
//        public int DraftCount
//        {
//            get
//            {
//                return _draftCount;
//            }
//            set
//            {
//                if (value == _draftCount)
//                    return;
//                SetProperty(ref _draftCount, value);
//            }
//        }
//        private int _publishedCount;
//        [ProtoMember(3)]
//        public int PublishedCount
//        {
//            get
//            {
//                return _publishedCount;
//            }
//            set
//            {
//                if (value == _publishedCount)
//                    return;
//                SetProperty(ref _publishedCount, value);
//            }
//        }
//    }
//}
