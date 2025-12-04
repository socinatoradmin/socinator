#region

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using DominatorHouseCore.Models.Publisher;
using DominatorHouseCore.Utility;
using ProtoBuf;

#endregion

namespace DominatorHouseCore.Models.SocioPublisher
{
    [ProtoContract]
    public class PublisherCreateCampaignModel : BindableBase
    {
        public PublisherCreateCampaignModel()
        {
            CampaignId = Utilities.GetGuid();
            JobConfigurations = new JobConfigurationModel();
        }

        /// <summary>
        ///     To Specify the campaign Id
        /// </summary>
        [ProtoMember(1)]
        public string CampaignId { get; set; }

        private string _campaignName = $"Campaign {DateTimeUtilities.GetEpochTime()}";

        // To specify the campaign name
        [ProtoMember(2)]
        public string CampaignName
        {
            get => _campaignName;
            set
            {
                if (_campaignName == value)
                    return;
                _campaignName = value;
                OnPropertyChanged(nameof(CampaignName));
            }
        }

        private PublisherCampaignStatus _campaignStatus = PublisherCampaignStatus.Active;

        // To specify the campaign status
        [ProtoMember(3)]
        public PublisherCampaignStatus CampaignStatus
        {
            get => _campaignStatus;
            set
            {
                if (_campaignStatus == value)
                    return;
                _campaignStatus = value;
                OnPropertyChanged(nameof(CampaignStatus));
            }
        }

        /// <summary>
        ///     To specify the Job Configuration Model
        /// </summary>
        private JobConfigurationModel _jobConfigurations;

        [ProtoMember(5)]
        public JobConfigurationModel JobConfigurations
        {
            get => _jobConfigurations;
            set
            {
                if (_jobConfigurations == value)
                    return;
                _jobConfigurations = value;
                OnPropertyChanged(nameof(JobConfigurations));
            }
        }

        /// <summary>
        ///     To specify the other configuration moodel
        /// </summary>
        private OtherConfigurationModel _otherConfiguration = new OtherConfigurationModel();

        [ProtoMember(6)]
        public OtherConfigurationModel OtherConfiguration
        {
            get => _otherConfiguration;
            set
            {
                if (_otherConfiguration == value)
                    return;
                _otherConfiguration = value;
                OnPropertyChanged(nameof(OtherConfiguration));
            }
        }

        /// <summary>
        ///     Post Details for the campaigns
        /// </summary>
        private PostDetailsModel _postDetailsModel = new PostDetailsModel();

        [ProtoMember(7)]
        public PostDetailsModel PostDetailsModel
        {
            get => _postDetailsModel;
            set
            {
                if (_postDetailsModel == value)
                    return;
                _postDetailsModel = value;
                OnPropertyChanged(nameof(PostDetailsModel));
            }
        }

        /// <summary>
        ///     To specify the destination which is related with campaign
        /// </summary>
        private ObservableCollection<string> _lstDestinationId = new ObservableCollection<string>();

        [ProtoMember(8)]
        public ObservableCollection<string> LstDestinationId
        {
            get => _lstDestinationId;
            set
            {
                if (_lstDestinationId == value)
                    return;
                _lstDestinationId = value;
                OnPropertyChanged(nameof(_lstDestinationId));
            }
        }

        /// <summary>
        ///     To specify the scarpe post details for a campaign
        /// </summary>
        private ScrapePostModel _scrapePostModel = new ScrapePostModel();

        [ProtoMember(9)]
        public ScrapePostModel ScrapePostModel
        {
            get => _scrapePostModel;
            set
            {
                if (_scrapePostModel == value)
                    return;
                _scrapePostModel = value;
                OnPropertyChanged(nameof(ScrapePostModel));
            }
        }

        /// <summary>
        ///     To specify the share post details for a campaign
        /// </summary>
        private SharePostModel _sharePostModel = new SharePostModel();

        [ProtoMember(10)]
        public SharePostModel SharePostModel
        {
            get => _sharePostModel;
            set
            {
                if (_sharePostModel == value)
                    return;
                _sharePostModel = value;
                OnPropertyChanged(nameof(SharePostModel));
            }
        }


        /// <summary>
        ///     Campaign Created date time
        /// </summary>
        [ProtoMember(11)]
        public DateTime CreatedDate { get; set; } = DateTime.Now;


        /// <summary>
        ///     Campaign Last modified date time
        /// </summary>
        [ProtoMember(16)]
        public DateTime UpdatedDate { get; set; } = DateTime.Now;

        //private PublisherMediaViewerModel _publisherMediaViewerModel = new PublisherMediaViewerModel();
        //[ProtoMember(12)]
        //public PublisherMediaViewerModel PublisherMediaViewerModel
        //{
        //    get
        //    {
        //        return _publisherMediaViewerModel;
        //    }
        //    set
        //    {
        //        if (_publisherMediaViewerModel == value)
        //            return;
        //        _publisherMediaViewerModel = value;
        //        OnPropertyChanged(nameof(PublisherMediaViewerModel));
        //    }
        //}

        /// <summary>
        ///     Rss Feed url collection for the campaign with default template
        /// </summary>
        private ObservableCollection<PublisherRssFeedModel> _lstFeedUrl =
            new ObservableCollection<PublisherRssFeedModel>();

        [ProtoMember(13)]
        public ObservableCollection<PublisherRssFeedModel> LstFeedUrl
        {
            get => _lstFeedUrl;
            set
            {
                if (value == _lstFeedUrl)
                    return;
                SetProperty(ref _lstFeedUrl, value);
            }
        }

        /// <summary>
        ///     Monitor Folder path collection for the campaign with default template
        /// </summary>
        private ObservableCollection<PublisherMonitorFolderModel> _lstFolderPath =
            new ObservableCollection<PublisherMonitorFolderModel>();

        [ProtoMember(14)]
        public ObservableCollection<PublisherMonitorFolderModel> LstFolderPath
        {
            get => _lstFolderPath;
            set
            {
                if (value == _lstFolderPath)
                    return;
                SetProperty(ref _lstFolderPath, value);
            }
        }


        /// <summary>
        ///     Post Collection details for multiple post details
        /// </summary>
        private ObservableCollection<PostDetailsModel> _lstPostDetailsModel =
            new ObservableCollection<PostDetailsModel>();

        [ProtoMember(15)]
        public ObservableCollection<PostDetailsModel> LstPostDetailsModels
        {
            get => _lstPostDetailsModel;
            set
            {
                if (value == _lstPostDetailsModel)
                    return;
                SetProperty(ref _lstPostDetailsModel, value);
            }
        }

        /// <summary>
        ///     Post Collection details for single post details
        /// </summary>
        [ProtoMember(17)]
        public ObservableCollection<PostDetailsModel> LstSinglePostCollection { get; set; } =
            new ObservableCollection<PostDetailsModel>();

        /// <summary>
        ///     Post collection which holds all direct post collection, where post may belongs to Draft, Pending, Published
        /// </summary>
        [ProtoIgnore]
        public ObservableCollection<PostDetailsModel> PostCollection { get; set; } =
            new ObservableCollection<PostDetailsModel>();

        /// <summary>
        ///     Post Collection details for Multiple Image post details
        /// </summary>
        [ProtoMember(19)]
        public ObservableCollection<PostDetailsModel> LstMultipleImagePostCollection { get; set; } =
            new ObservableCollection<PostDetailsModel>();

        private List<string> _lstUploadPostDescription = new List<string>();

        public List<string> LstUploadPostDescription
        {
            get => _lstUploadPostDescription;
            set => SetProperty(ref _lstUploadPostDescription, value);
        }
    }
}