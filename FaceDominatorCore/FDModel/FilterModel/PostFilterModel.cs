using DominatorHouseCore.Utility;
using ProtoBuf;
using System.Collections.ObjectModel;

namespace FaceDominatorCore.FDModel.FilterModel
{
    public interface IPostFilter
    {
        /*bool IsFilterByReactionCountChkd { get; set; }

        bool IsFilterByLikeCountChkd { get; set; }

        bool IsFilterByShareCountChkd { get; set; }

        bool IsFilterByCommentCountChkd { get; set; }

        bool IsFilterByPostedDateTimeChkd { get; set; }

        bool IsFilterPostCategory { get; set; }

        bool IgnorePostImages { get; set; }

        bool IgnorePostVideos { get; set; }

        bool IgnoreNoMedia { get; set; }

        bool FilterByBlacklistedWhitelistedWordsInCaption { get; set; }

        bool FilterRestrictedPostCaptionList { get; set; }

        bool FilterAcceptedPostCaptionList { get; set; }

        string CaptionWhitelist { get; set; }

        string CaptionBlacklists { get; set; }*/

        RangeUtilities PostLikerCount { get; set; }

        //        RangeUtilities PostSharerCount { get; set; }
        //
        //        RangeUtilities PostCommentorCount { get; set; }

        RangeUtilities PostedDateTime { get; set; }

    }



    [ProtoContract]
    public class PostFilterModel : BindableBase, IPostFilter
    {


        private bool _isFilterByCommentCountChkd;

        [ProtoMember(1)]
        public bool IsFilterByCommentCountChkd
        {
            get
            {
                return _isFilterByCommentCountChkd;
            }
            set
            {
                SetProperty(ref _isFilterByCommentCountChkd, value);
            }
        }

        private bool _isFilterByLikeCountChkd;

        [ProtoMember(2)]
        public bool IsFilterByLikeCountChkd
        {
            get
            {
                return _isFilterByLikeCountChkd;
            }
            set
            {
                SetProperty(ref _isFilterByLikeCountChkd, value);
            }
        }

        private bool _isFilterByReactionCountChkd;

        [ProtoMember(3)]
        public bool IsFilterByReactionCountChkd
        {
            get
            {
                return _isFilterByReactionCountChkd;
            }
            set
            {
                SetProperty(ref _isFilterByReactionCountChkd, value);
            }
        }

        private bool _isFilterByShareCountChkd;

        [ProtoMember(4)]
        public bool IsFilterByShareCountChkd
        {
            get
            {
                return _isFilterByShareCountChkd;
            }
            set
            {
                SetProperty(ref _isFilterByShareCountChkd, value);
            }
        }

        private bool _isFilterByPostedDateTimeChkd;

        [ProtoMember(5)]
        public bool IsFilterByPostedDateTimeChkd
        {
            get
            {
                return _isFilterByPostedDateTimeChkd;
            }
            set
            {
                SetProperty(ref _isFilterByPostedDateTimeChkd, value);
            }
        }



        private RangeUtilities _postCommentorCount = new RangeUtilities(100, 200);

        [ProtoMember(6)]
        public RangeUtilities PostCommentorCount
        {
            get
            {
                return _postCommentorCount;
            }
            set
            {
                SetProperty(ref _postCommentorCount, value);
            }
        }

        private RangeUtilities _postSharerCount = new RangeUtilities(100, 200);

        [ProtoMember(7)]
        public RangeUtilities PostSharerCount
        {
            get
            {
                return _postSharerCount;
            }
            set
            {
                SetProperty(ref _postSharerCount, value);
            }
        }

        private RangeUtilities _postLikerCount = new RangeUtilities(100, 200);

        [ProtoMember(8)]
        public RangeUtilities PostLikerCount
        {
            get
            {
                return _postLikerCount;
            }
            set
            {
                SetProperty(ref _postLikerCount, value);
            }
        }

        private RangeUtilities _postedDateTime = new RangeUtilities(1, 5);

        [ProtoMember(9)]
        public RangeUtilities PostedDateTime
        {
            get
            {
                return _postedDateTime;
            }
            set
            {
                SetProperty(ref _postedDateTime, value);
            }
        }


        private bool _isFilterPostCategory;

        [ProtoMember(10)]
        public bool IsFilterPostCategory
        {
            get
            {
                return _isFilterPostCategory;
            }
            set
            {
                SetProperty(ref _isFilterPostCategory, value);
            }
        }


        private bool _ignorePostImages;

        [ProtoMember(11)]
        public bool IgnorePostImages
        {
            get
            {
                return _ignorePostImages;
            }
            set
            {
                SetProperty(ref _ignorePostImages, value);
            }
        }

        private bool _ignorePostVideos;

        [ProtoMember(12)]
        public bool IgnorePostVideos
        {
            get
            {
                return _ignorePostVideos;
            }
            set
            {
                SetProperty(ref _ignorePostVideos, value);
            }
        }

        private bool _ignoreNoMedia;

        [ProtoMember(13)]
        public bool IgnoreNoMedia
        {
            get
            {
                return _ignoreNoMedia;
            }
            set
            {
                SetProperty(ref _ignoreNoMedia, value);
            }
        }

        private bool _filterByBlacklistedWhitelistedWordsInCaption;

        [ProtoMember(14)]
        public bool FilterByBlacklistedWhitelistedWordsInCaption
        {
            get
            {
                return _filterByBlacklistedWhitelistedWordsInCaption;
            }
            set
            {
                SetProperty(ref _filterByBlacklistedWhitelistedWordsInCaption, value);
            }
        }


        private bool _filterAcceptedPostCaptionList;

        [ProtoMember(15)]
        public bool FilterAcceptedPostCaptionList
        {
            get
            {
                return _filterAcceptedPostCaptionList;
            }
            set
            {
                SetProperty(ref _filterAcceptedPostCaptionList, value);
            }
        }

        private bool _filterRestrictedPostCaptionList;

        [ProtoMember(16)]
        public bool FilterRestrictedPostCaptionList
        {
            get
            {
                return _filterRestrictedPostCaptionList;
            }
            set
            {
                SetProperty(ref _filterRestrictedPostCaptionList, value);
            }
        }


        private string _captionBlacklists;

        [ProtoMember(17)]
        public string CaptionBlacklists
        {
            get
            {
                return _captionBlacklists;
            }
            set
            {
                SetProperty(ref _captionBlacklists, value);
            }
        }

        private string _captionWhitelist;

        [ProtoMember(18)]
        public string CaptionWhitelist
        {
            get
            {
                return _captionWhitelist;
            }
            set
            {
                SetProperty(ref _captionWhitelist, value);
            }
        }

        [ProtoMember(19)]
        public ObservableCollection<string> RestrictedPostCaptionList { get; set; }
    }
}
