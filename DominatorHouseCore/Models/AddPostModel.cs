#region

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using DominatorHouseCore.Models.Publisher;
using DominatorHouseCore.Utility;
using ProtoBuf;

#endregion

namespace DominatorHouseCore.Models
{
    [ProtoContract]
    public class AddPostModel : BindableBase
    {
        private ICollectionView _postsDetailCollection;

        public ICollectionView PostsDetailCollection
        {
            get => _postsDetailCollection;
            set
            {
                if (_postsDetailCollection != null && value == _postsDetailCollection)
                    return;
                SetProperty(ref _postsDetailCollection, value);
            }
        }

        private string _messeges;

        [ProtoMember(1)]
        public string Messeges
        {
            get => _messeges;
            set
            {
                if (value == _messeges)
                    return;
                SetProperty(ref _messeges, value);
            }
        }

        private List<string> _photoUrl = new List<string>();

        [ProtoMember(2)]
        public List<string> PhotoUrl
        {
            get => _photoUrl;
            set
            {
                if (value == _photoUrl)
                    return;
                SetProperty(ref _photoUrl, value);
            }
        }

        private List<string> _videoUrl = new List<string>();

        [ProtoMember(3)]
        public List<string> VideoUrl
        {
            get => _videoUrl;
            set
            {
                if (value == _videoUrl)
                    return;
                SetProperty(ref _videoUrl, value);
            }
        }

        private string _titles;

        [ProtoMember(4)]
        public string Titles
        {
            get => _titles;
            set
            {
                if (value == _titles)
                    return;
                SetProperty(ref _titles, value);
            }
        }

        private bool _isUseFacebookSellPostChecked;

        [ProtoMember(5)]
        public bool IsUseFacebookSellPostChecked
        {
            get => _isUseFacebookSellPostChecked;
            set
            {
                if (value == _isUseFacebookSellPostChecked)
                    return;
                SetProperty(ref _isUseFacebookSellPostChecked, value);
            }
        }

        private bool _isUseInstagramStoryPollChecked;

        [ProtoMember(6)]
        public bool IsUseInstagramStoryPollChecked
        {
            get => _isUseInstagramStoryPollChecked;
            set
            {
                if (value == _isUseInstagramStoryPollChecked)
                    return;
                SetProperty(ref _isUseInstagramStoryPollChecked, value);
            }
        }


        private string _campaign;

        [ProtoMember(31)]
        public string Campaign
        {
            get => _campaign;
            set
            {
                if (value == _campaign)
                    return;
                SetProperty(ref _campaign, value);
            }
        }

        private string _importedText;

        [ProtoMember(41)]
        public string ImportedText
        {
            get => _importedText;
            set
            {
                if (value == _importedText)
                    return;
                SetProperty(ref _importedText, value);
            }
        }

        private DateTime _campaignStartDate;

        [ProtoMember(42)]
        public DateTime CampaignStartDate
        {
            get => _campaignStartDate;
            set
            {
                if (value == _campaignStartDate)
                    return;
                SetProperty(ref _campaignStartDate, value);
            }
        }

        private DateTime _campaignEndDate;

        [ProtoMember(43)]
        public DateTime CampaignEndDate
        {
            get => _campaignEndDate;
            set
            {
                if (value == _campaignEndDate)
                    return;
                SetProperty(ref _campaignEndDate, value);
            }
        }

        private int _serialNo;

        [ProtoMember(44)]
        public int SerialNo
        {
            get => _serialNo;
            set
            {
                if (value == _serialNo)
                    return;
                SetProperty(ref _serialNo, value);
            }
        }

        private string _finished = "0/0";

        [ProtoMember(45)]
        public string Finished
        {
            get => _finished;
            set
            {
                if (value == _finished)
                    return;
                SetProperty(ref _finished, value);
            }
        }

        private string _succeessfull = "0/0";

        [ProtoMember(46)]
        public string Succeessfull
        {
            get => _succeessfull;
            set
            {
                if (value == _succeessfull)
                    return;
                SetProperty(ref _succeessfull, value);
            }
        }

        private string _status = "Active";

        [ProtoMember(47)]
        public string Status
        {
            get => _status;
            set
            {
                if (value == _status)
                    return;
                SetProperty(ref _status, value);
            }
        }

        private int _deleted;

        [ProtoMember(48)]
        public int Deleted
        {
            get => _deleted;
            set
            {
                if (value == _deleted)
                    return;
                SetProperty(ref _deleted, value);
            }
        }


        private bool _isExpireDateChecked;

        [ProtoMember(50)]
        public bool IsExpireDateChecked
        {
            get => _isExpireDateChecked;
            set
            {
                if (value == _isExpireDateChecked)
                    return;
                SetProperty(ref _isExpireDateChecked, value);
            }
        }

        private DateTime _expireDate;

        [ProtoMember(51)]
        public DateTime ExpireDate
        {
            get => _expireDate;
            set
            {
                if (value == _expireDate)
                    return;
                SetProperty(ref _expireDate, value);
            }
        }

        private bool _isEnableReAddPostChecked;

        [ProtoMember(52)]
        public bool IsEnableReAddPostChecked
        {
            get => _isEnableReAddPostChecked;
            set
            {
                if (value == _isEnableReAddPostChecked)
                    return;
                SetProperty(ref _isEnableReAddPostChecked, value);
            }
        }

        private int _times;

        [ProtoMember(53)]
        public int Times
        {
            get => _times;
            set
            {
                if (value == _times)
                    return;
                SetProperty(ref _times, value);
            }
        }

        private bool _isGeneralChecked;

        [ProtoMember(54)]
        public bool IsGeneralChecked
        {
            get => _isGeneralChecked;
            set
            {
                if (value == _isGeneralChecked)
                    return;
                SetProperty(ref _isGeneralChecked, value);
            }
        }

        private bool _isJobChecked;

        [ProtoMember(55)]
        public bool IsJobChecked
        {
            get => _isJobChecked;
            set
            {
                if (value == _isJobChecked)
                    return;
                SetProperty(ref _isJobChecked, value);
            }
        }

        private string _tumblrTags;

        [ProtoMember(56)]
        public string TumblrTags
        {
            get => _tumblrTags;
            set
            {
                if (value == _tumblrTags)
                    return;
                SetProperty(ref _tumblrTags, value);
            }
        }

        private string _descriptionUsedForFacebook;

        [ProtoMember(57)]
        public string DescriptionUsedForFacebook
        {
            get => _descriptionUsedForFacebook;
            set
            {
                if (value == _descriptionUsedForFacebook)
                    return;
                SetProperty(ref _descriptionUsedForFacebook, value);
            }
        }

        private bool _isPostAsPartOfStoryChecked;

        [ProtoMember(58)]
        public bool IsPostAsPartOfStoryChecked
        {
            get => _isPostAsPartOfStoryChecked;
            set
            {
                if (value == _isPostAsPartOfStoryChecked)
                    return;
                SetProperty(ref _isPostAsPartOfStoryChecked, value);
            }
        }

        private bool _isDeletePostAfterChecked;

        [ProtoMember(59)]
        public bool IsDeletePostAfterChecked
        {
            get => _isDeletePostAfterChecked;
            set
            {
                if (value == _isDeletePostAfterChecked)
                    return;
                SetProperty(ref _isDeletePostAfterChecked, value);
            }
        }

        private int _hours;

        [ProtoMember(60)]
        public int Hours
        {
            get => _hours;
            set
            {
                if (value == _hours)
                    return;
                SetProperty(ref _hours, value);
            }
        }

        private bool _isArchivePostChecked;

        [ProtoMember(61)]
        public bool IsArchivePostChecked
        {
            get => _isArchivePostChecked;
            set
            {
                if (value == _isArchivePostChecked)
                    return;
                SetProperty(ref _isArchivePostChecked, value);
            }
        }

        private bool _isRepostusingGeoLocationChecked;

        [ProtoMember(62)]
        public bool IsRepostusingGeoLocationChecked
        {
            get => _isRepostusingGeoLocationChecked;
            set
            {
                if (value == _isRepostusingGeoLocationChecked)
                    return;
                SetProperty(ref _isRepostusingGeoLocationChecked, value);
            }
        }

        private bool _isTagSpecificUsersChecked;

        [ProtoMember(63)]
        public bool IsTagSpecificUsersChecked
        {
            get => _isTagSpecificUsersChecked;
            set
            {
                if (value == _isTagSpecificUsersChecked)
                    return;
                SetProperty(ref _isTagSpecificUsersChecked, value);
            }
        }

        private bool _isChooseVideoCoverChecked;

        [ProtoMember(64)]
        public bool IsChooseVideoCoverChecked
        {
            get => _isChooseVideoCoverChecked;
            set
            {
                if (value == _isChooseVideoCoverChecked)
                    return;
                SetProperty(ref _isChooseVideoCoverChecked, value);
            }
        }

        private Campaign _campaignDetails = new Campaign();

        [ProtoMember(65)]
        public Campaign CampaignDetails
        {
            get => _campaignDetails;
            set
            {
                if (value == _campaignDetails)
                    return;
                SetProperty(ref _campaignDetails, value);
            }
        }

        private PostStatus _postStatus = new PostStatus();

        [ProtoMember(66)]
        public PostStatus PostStatus
        {
            get => _postStatus;
            set
            {
                if (value == _postStatus)
                    return;
                SetProperty(ref _postStatus, value);
            }
        }

        private JobConfigurationModel _jobConfigurations = new JobConfigurationModel();

        [ProtoMember(67)]
        public JobConfigurationModel JobConfigurations
        {
            get => _jobConfigurations;
            set
            {
                if (value == _jobConfigurations)
                    return;
                SetProperty(ref _jobConfigurations, value);
            }
        }

        private OtherConfigurationModel _otherConfiguration = new OtherConfigurationModel();

        [ProtoMember(68)]
        public OtherConfigurationModel OtherConfiguration
        {
            get => _otherConfiguration;
            set
            {
                if (value == _otherConfiguration)
                    return;
                SetProperty(ref _otherConfiguration, value);
            }
        }

        private LocationDetails _locationDetails = new LocationDetails();

        public LocationDetails LocationDetails
        {
            get => _locationDetails;
            set
            {
                if (value == _locationDetails)
                    return;
                SetProperty(ref _locationDetails, value);
            }
        }

        private ICollectionView _locationDetailsCollection;

        public ICollectionView LocationDetailsCollection
        {
            get => _locationDetailsCollection;
            set
            {
                if (value == _locationDetailsCollection)
                    return;
                SetProperty(ref _locationDetailsCollection, value);
            }
        }

        private int _imageCount;

        public int ImageCount
        {
            get => _imageCount;
            set
            {
                if (value == _imageCount)
                    return;
                SetProperty(ref _imageCount, value);
            }
        }

        private int _videoCount;

        public int VideoCount
        {
            get => _videoCount;
            set
            {
                if (value == _videoCount)
                    return;
                SetProperty(ref _videoCount, value);
            }
        }

        private ObservableCollectionBase<string> _lstMediaSources = new ObservableCollectionBase<string>();

        public ObservableCollectionBase<string> LstMediaSources
        {
            get => _lstMediaSources;
            set
            {
                if (value == _lstMediaSources)
                    return;
                SetProperty(ref _lstMediaSources, value);
            }
        }
    }


    [ProtoContract]
    public class TimeSpanHelper : BindableBase
    {
        private TimeSpan _startTime;

        [ProtoMember(1)]
        public TimeSpan StartTime
        {
            get => _startTime;
            set
            {
                if (value == _startTime)
                    return;
                SetProperty(ref _startTime, value);
            }
        }

        private TimeSpan _endTime;

        [ProtoMember(2)]
        public TimeSpan EndTime
        {
            get => _endTime;
            set
            {
                if (value == _endTime)
                    return;
                SetProperty(ref _endTime, value);
            }
        }

        private TimeSpan _midTime;

        [ProtoMember(3)]
        public TimeSpan MidTime
        {
            get => _midTime;
            set
            {
                if (value == _midTime)
                    return;
                SetProperty(ref _midTime, value);
            }
        }
    }


    [ProtoContract]
    public class LocationDetails : BindableBase
    {
        private int _locationId = 1;

        [ProtoMember(1)]
        public int LocationId
        {
            get => _locationId;
            set
            {
                if (value == _locationId)
                    return;
                SetProperty(ref _locationId, value);
            }
        }

        private string _locationName = "xyz";

        [ProtoMember(2)]
        public string LocationName
        {
            get => _locationName;
            set
            {
                if (value == _locationName)
                    return;
                SetProperty(ref _locationName, value);
            }
        }

        private string _locationAddress = "abc";

        [ProtoMember(3)]
        public string LocationAddress
        {
            get => _locationAddress;
            set
            {
                if (value == _locationAddress)
                    return;
                SetProperty(ref _locationAddress, value);
            }
        }
    }


    [ProtoContract]
    public class Campaign : BindableBase
    {
        private ObservableCollection<Campaign> _lstCampaign = new ObservableCollection<Campaign>();

        public ObservableCollection<Campaign> LstCampaign
        {
            get => _lstCampaign;
            set
            {
                if (_lstCampaign != null && value == _lstCampaign)
                    return;
                SetProperty(ref _lstCampaign, value);
            }
        }

        private ObservableCollection<string> _lstStatus = new ObservableCollection<string>();

        public ObservableCollection<string> LstStatus
        {
            get => _lstStatus;
            set
            {
                if (_lstStatus != null && value == _lstStatus)
                    return;
                SetProperty(ref _lstStatus, value);
            }
        }

        private string _selectedAccount;

        [ProtoMember(1)]
        public string SelectedAccount
        {
            get => _selectedAccount;
            set
            {
                if (value == _selectedAccount)
                    return;
                SetProperty(ref _selectedAccount, value);
            }
        }

        private string _selectedStatus = Application.Current.FindResource("LangKeyAll").ToString();

        [ProtoMember(2)]
        public string SelectedStatus
        {
            get => _selectedStatus;
            set
            {
                if (value == _selectedStatus)
                    return;
                SetProperty(ref _selectedStatus, value);
            }
        }

        private string _campaignName = "Campaign - [ " + DateTime.Now + " ]";

        [ProtoMember(3)]
        public string CampaignName
        {
            get => _campaignName;
            set
            {
                if (value == _campaignName)
                    return;
                SetProperty(ref _campaignName, value);
            }
        }

        private string _status = "Active";

        [ProtoMember(4)]
        public string Status
        {
            get => _status;
            set
            {
                if (value == _status)
                    return;
                SetProperty(ref _status, value);
            }
        }

        private DateTime _campaignCreatedDate;

        [ProtoMember(5)]
        public DateTime CampaignCreatedDate
        {
            get => _campaignCreatedDate;
            set
            {
                if (value == _campaignCreatedDate)
                    return;
                SetProperty(ref _campaignCreatedDate, value);
            }
        }
    }


    [ProtoContract]
    public class PostStatus : BindableBase
    {
        private int _pendingCount;

        [ProtoMember(1)]
        public int PendingCount
        {
            get => _pendingCount;
            set
            {
                if (value == _pendingCount)
                    return;
                SetProperty(ref _pendingCount, value);
            }
        }

        private int _draftCount;

        [ProtoMember(2)]
        public int DraftCount
        {
            get => _draftCount;
            set
            {
                if (value == _draftCount)
                    return;
                SetProperty(ref _draftCount, value);
            }
        }

        private int _publishedCount;

        [ProtoMember(3)]
        public int PublishedCount
        {
            get => _publishedCount;
            set
            {
                if (value == _publishedCount)
                    return;
                SetProperty(ref _publishedCount, value);
            }
        }
    }
}