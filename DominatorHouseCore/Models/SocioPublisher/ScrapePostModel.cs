#region

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using DominatorHouseCore.Utility;
using ProtoBuf;

#endregion

namespace DominatorHouseCore.Models.SocioPublisher
{
    [Serializable]
    [ProtoContract]
    public class ScrapePostModel : BindableBase
    {
        /// <summary>
        ///     To specify facebook scrape post is checked
        /// </summary>
        private bool _isScrapeFacebookPost;

        [ProtoMember(1)]
        public bool IsScrapeFacebookPost
        {
            get => _isScrapeFacebookPost;
            set
            {
                if (_isScrapeFacebookPost == value)
                    return;
                _isScrapeFacebookPost = value;
                OnPropertyChanged(nameof(IsScrapeFacebookPost));
            }
        }

        /// <summary>
        ///     To specify scraping details of facebook
        /// </summary>
        private string _addFdPostSource = string.Empty;

        [ProtoMember(2)]
        public string AddFdPostSource
        {
            get => _addFdPostSource;
            set
            {
                if (_addFdPostSource == value)
                    return;
                _addFdPostSource = value;
                OnPropertyChanged(nameof(AddFdPostSource));
            }
        }

        /// <summary>
        ///     To specify pinterest scrape post is checked
        /// </summary>
        private bool _isScrapePinterestPost;

        [ProtoMember(3)]
        public bool IsScrapePinterestPost
        {
            get => _isScrapePinterestPost;
            set
            {
                if (_isScrapePinterestPost == value)
                    return;
                _isScrapePinterestPost = value;
                OnPropertyChanged(nameof(IsScrapePinterestPost));
            }
        }

        /// <summary>
        ///     To specify scraping details of pinterest
        /// </summary>
        private string _addPdPostSource = string.Empty;

        [ProtoMember(4)]
        public string AddPdPostSource
        {
            get => _addPdPostSource;
            set
            {
                if (_addPdPostSource == value)
                    return;
                _addPdPostSource = value;
                OnPropertyChanged(nameof(AddPdPostSource));
            }
        }

        /// <summary>
        ///     To specify twitter scrape post is checked
        /// </summary>
        private bool _isScrapeTwitterPost;

        [ProtoMember(5)]
        public bool IsScrapeTwitterPost
        {
            get => _isScrapeTwitterPost;
            set
            {
                if (_isScrapeTwitterPost == value)
                    return;
                _isScrapeTwitterPost = value;
                OnPropertyChanged(nameof(IsScrapeTwitterPost));
            }
        }

        /// <summary>
        ///     To specify scraping details of Twitter
        /// </summary>
        private string _addTdPostSource = string.Empty;

        [ProtoMember(6)]
        public string AddTdPostSource
        {
            get => _addTdPostSource;
            set
            {
                if (_addTdPostSource == value)
                    return;
                _addTdPostSource = value;
                OnPropertyChanged(nameof(AddTdPostSource));
            }
        }


        private int _scrapeCount = 1;

        [ProtoMember(7)]
        public int ScrapeCount
        {
            get => _scrapeCount;
            set
            {
                if (_scrapeCount == value)
                    return;
                SetProperty(ref _scrapeCount, value);
            }
        }


        private int _startScrapeOnXminute = 30;

        [ProtoMember(8)]
        public int StartScrapeOnXminute
        {
            get => _startScrapeOnXminute;
            set
            {
                if (_startScrapeOnXminute == value)
                    return;
                SetProperty(ref _startScrapeOnXminute, value);
            }
        }

        private bool _isOriginalPostDetails = true;

        [ProtoMember(9)]
        public bool IsOriginalPostDetails
        {
            get => _isOriginalPostDetails;
            set
            {
                if (value == _isOriginalPostDetails)
                    return;
                if (value)
                    IsOwnPostDetails = false;
                SetProperty(ref _isOriginalPostDetails, value);
            }
        }

        private bool _isOwnPostDetails;

        [ProtoMember(10)]
        public bool IsOwnPostDetails
        {
            get => _isOwnPostDetails;
            set
            {
                if (value == _isOwnPostDetails)
                    return;
                if (value)
                    IsOriginalPostDetails = false;
                SetProperty(ref _isOwnPostDetails, value);
            }
        }

        private ObservableCollection<string> _lstScrapedPostDetails = new ObservableCollection<string>();

        [ProtoMember(11)]
        public ObservableCollection<string> LstScrapedPostDetails
        {
            get => _lstScrapedPostDetails;
            set => SetProperty(ref _lstScrapedPostDetails, value);
        }

        private bool _isScrapePostOlderThanXXDays;

        [ProtoMember(12)]
        public bool IsScrapePostOlderThanXXDays
        {
            get => _isScrapePostOlderThanXXDays;
            set
            {
                if (value == _isScrapePostOlderThanXXDays)
                    return;
                SetProperty(ref _isScrapePostOlderThanXXDays, value);
            }
        }

        private RangeUtilities _doNotscrapePostOlderThanNDays = new RangeUtilities(2, 4);

        [ProtoMember(13)]
        public RangeUtilities DoNotScrapePostOlderThanNDays
        {
            get => _doNotscrapePostOlderThanNDays;
            set
            {
                if (value == _doNotscrapePostOlderThanNDays)
                    return;
                SetProperty(ref _doNotscrapePostOlderThanNDays, value);
            }
        }

        #region RedditPost Scaper 

        //To specify reddit scrape post is checked
        [ProtoMember(14)] private bool _isScrapeRedditPost;

        public bool IsScrapeRedditPost
        {
            get => _isScrapeRedditPost;
            set
            {
                if (_isScrapeRedditPost == value)
                    return;
                _isScrapeRedditPost = value;
                OnPropertyChanged(nameof(IsScrapeRedditPost));
            }
        }


        //To specify scraping details of reddit
        [ProtoMember(15)] private string _addRdPostSource;

        public string AddRdPostSource
        {
            get => _addRdPostSource;
            set
            {
                if (_addRdPostSource == value)
                    return;
                _addRdPostSource = value;
                OnPropertyChanged(nameof(AddRdPostSource));
            }
        }

        #endregion

        #region Google Image Scaper 

        //To specify google scrape image is checked
        [ProtoMember(16)] private bool _isScrapeGoogleImgaes;

        public bool IsScrapeGoogleImgaes
        {
            get => _isScrapeGoogleImgaes;
            set
            {
                if (_isScrapeGoogleImgaes == value)
                    return;
                _isScrapeGoogleImgaes = value;
                OnPropertyChanged(nameof(IsScrapeGoogleImgaes));
            }
        }


        //To specify scraping details of reddit
        [ProtoMember(17)] private string _addGooglePostSource;

        public string AddGooglePostSource
        {
            get => _addGooglePostSource;
            set
            {
                if (_addGooglePostSource == value)
                    return;
                _addGooglePostSource = value;
                OnPropertyChanged(nameof(AddGooglePostSource));
            }
        }


        /// <summary>
        ///     Is need to use file name as a description for multiple image posts
        /// </summary>
        private bool _isUseFileNameAsDescription = true;

        [ProtoMember(18)]
        public bool IsUseFileNameAsDescription
        {
            get => _isUseFileNameAsDescription;
            set
            {
                if (value == _isUseFileNameAsDescription)
                    return;
                SetProperty(ref _isUseFileNameAsDescription, value);
            }
        }

        private bool _isUploadPostDescription;

        [ProtoMember(19)]
        public bool IsUploadPostDescription
        {
            get => _isUploadPostDescription;
            set
            {
                if (value == _isUploadPostDescription)
                    return;
                SetProperty(ref _isUploadPostDescription, value);
            }
        }

        private List<string> _lstUploadPostDescription = new List<string>();

        [ProtoMember(20)]
        public List<string> LstUploadPostDescription
        {
            get => _lstUploadPostDescription;
            set => SetProperty(ref _lstUploadPostDescription, value);
        }

        private int _scrapeCountPerUrl = 1;

        [ProtoMember(21)]
        public int ScrapeCountPerUrl
        {
            get => _scrapeCountPerUrl;
            set
            {
                if (_scrapeCountPerUrl == value)
                    return;
                SetProperty(ref _scrapeCountPerUrl, value);
            }
        }


        private bool _ignoreTextOnlyPosts;

        [ProtoMember(22)]
        public bool IgnoreTextOnlyPosts
        {
            get => _ignoreTextOnlyPosts;
            set
            {
                if (_ignoreTextOnlyPosts == value)
                    return;
                SetProperty(ref _ignoreTextOnlyPosts, value);
            }
        }


        private bool _isScrapeHighQualityImages;

        [ProtoMember(23)]
        public bool IsScrapeHighQualityImages
        {
            get => _isScrapeHighQualityImages;
            set
            {
                if (_isScrapeHighQualityImages == value)
                    return;
                SetProperty(ref _isScrapeHighQualityImages, value);
            }
        }

        #endregion
    }
}