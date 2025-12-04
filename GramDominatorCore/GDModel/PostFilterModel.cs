using System.Collections.ObjectModel;
using DominatorHouseCore.Utility;
using GramDominatorCore.Settings;
using ProtoBuf;
using IPostFilter = GramDominatorCore.Interface.IPostFilter;

namespace GramDominatorCore.GDModel
{
    [ProtoContract]
    public class PostFilterModel : BindableBase, IPostFilter
    {


        private int _minimumPostCaptionChars = 10;
        [ProtoMember(1)]
        public int MinimumPostCaptionChars
        {
            get
            {
                return _minimumPostCaptionChars;
            }
            set
            {
                if (value == _minimumPostCaptionChars)
                    return;
                SetProperty(ref _minimumPostCaptionChars, value);
            }
        }


        private bool _filterRestrictedPostCaptionList = true;
        [ProtoMember(2)]
        public bool FilterRestrictedPostCaptionList
        {
            get
            {
                return _filterRestrictedPostCaptionList;
            }
            set
            {
                if (value == _filterRestrictedPostCaptionList)
                    return;
                SetProperty(ref _filterRestrictedPostCaptionList, value);
            }
        }


        private ObservableCollection<string> _restrictedPostCaptionList = new ObservableCollection<string>();
        [ProtoMember(3)]
        public ObservableCollection<string> RestrictedPostCaptionList
        {
            get
            {
                return _restrictedPostCaptionList;
            }
            set
            {
                if (value == _restrictedPostCaptionList)
                    return;
                SetProperty(ref _restrictedPostCaptionList, value);
            }
        }


        private bool _filterComments;
        [ProtoMember(4)]
        public bool FilterComments
        {
            get
            {
                return _filterComments;
            }
            set
            {
                if (value == _filterComments)
                    return;
                SetProperty(ref _filterComments, value);
            }
        }


        private RangeUtilities _commentsCountRange = new RangeUtilities(10, 20);
        [ProtoMember(5)]
        public RangeUtilities CommentsCountRange
        {
            get
            {
                return _commentsCountRange;
            }
            set
            {
                if (value == _commentsCountRange)
                    return;
                SetProperty(ref _commentsCountRange, value);
            }
        }


        private bool _filterLikes;
        [ProtoMember(6)]
        public bool FilterLikes
        {
            get
            {
                return _filterLikes;
            }
            set
            {
                if (value == _filterLikes)
                    return;
                SetProperty(ref _filterLikes, value);
            }
        }


        private RangeUtilities _likesCountRange = new RangeUtilities(10, 20);
        [ProtoMember(7)]
        public RangeUtilities LikesCountRange
        {
            get
            {
                return _likesCountRange;
            }
            set
            {
                if (value == _likesCountRange)
                    return;
                SetProperty(ref _likesCountRange, value);
            }
        }


        private bool _filterPostAge;
        [ProtoMember(8)]
        public bool FilterPostAge
        {
            get
            {
                return _filterPostAge;
            }
            set
            {
                if (value == _filterPostAge)
                    return;
                SetProperty(ref _filterPostAge, value);
            }
        }



        private int _maxPostAge;
        [ProtoMember(9)]
        public int MaxPostAge
        {
            get
            {
                return _maxPostAge;
            }
            set
            {
                if (value == _maxPostAge)
                    return;
                SetProperty(ref _maxPostAge, value);
            }
        }




        private PostCategory _postCategory = new PostCategory();
        [ProtoMember(10)]
        public PostCategory PostCategory
        {
            get
            {
                return _postCategory;
            }
            set
            {
                if (value == _postCategory)
                    return;
                SetProperty(ref _postCategory, value);
            }
        }




        //private bool _ignoreAdPost;
        //[ProtoMember(11)]
        //public bool IgnoreAdPost
        //{
        //    get
        //    {
        //        return _ignoreAdPost;
        //    }
        //    set
        //    {
        //        if (value == _ignoreAdPost)
        //            return;
        //        SetProperty(ref _ignoreAdPost, value);
        //    }
        //}




        //private bool _ignoreTopPost;
        //[ProtoMember(12)]
        //public bool IgnoreTopPost
        //{
        //    get
        //    {
        //        return _ignoreTopPost;
        //    }
        //    set
        //    {
        //        if (value == _ignoreTopPost)
        //            return;
        //        SetProperty(ref _ignoreTopPost, value);
        //    }
        //}


        //private bool _ignoreOwnPosts;
        //[ProtoMember(12)]
        //public bool IgnoreOwnPosts
        //{
        //    get
        //    {
        //        return _ignoreOwnPosts;
        //    }
        //    set
        //    {
        //        if (value == _ignoreOwnPosts)
        //            return;
        //        SetProperty(ref _ignoreTopPost, value);
        //    }
        //}





        



        private bool _filterAcceptedPostCaptionList;
        [ProtoMember(13)]
        public bool FilterAcceptedPostCaptionList
        {
            get
            {
                return _filterAcceptedPostCaptionList;
            }
            set
            {
                if (value == _filterAcceptedPostCaptionList)
                    return;
                SetProperty(ref _filterAcceptedPostCaptionList, value);
            }
        }



        private ObservableCollection<string> _acceptedPostCaptionList = new ObservableCollection<string>();

        [ProtoMember(14)]
        public ObservableCollection<string> AcceptedPostCaptionList
        {
            get
            {
                return _acceptedPostCaptionList;
            }
            set
            {
                if (value == _acceptedPostCaptionList)
                    return;
                SetProperty(ref _acceptedPostCaptionList, value);
            }
        }

        

        private string _captionBlacklists;

        [ProtoMember(15)]
        public string CaptionBlacklists
        {
            get
            {
                return _captionBlacklists;
            }
            set
            {
                if (value == _captionBlacklists)
                    return;
                SetProperty(ref _captionBlacklists, value);
            }
        }
        private string _captionWhitelist;

        [ProtoMember(16)]
        public string CaptionWhitelist
        {
            get
            {
                return _captionWhitelist;
            }
            set
            {
                if (value == _captionWhitelist)
                    return;
                SetProperty(ref _captionWhitelist, value);
            }
        }


        //private bool _ignoreLikedPost;
        //[ProtoMember(11)]
        //public bool IgnoreLikedPost
        //{
        //    get
        //    {
        //        return _ignoreLikedPost;
        //    }
        //    set
        //    {
        //        if (value == _ignoreLikedPost)
        //            return;
        //        SetProperty(ref _ignoreLikedPost, value);
        //    }
        //}


        //private bool _ignoreCommentedPosts;
        //[ProtoMember(11)]
        //public bool IgnoreCommentedPosts
        //{
        //    get
        //    {
        //        return _ignoreCommentedPosts;
        //    }
        //    set
        //    {
        //        if (value == _ignoreCommentedPosts)
        //            return;
        //        SetProperty(ref _ignoreCommentedPosts, value);
        //    }
        //}


        private bool _filterCharsLenghInCaption;
        [ProtoMember(17)]
        public bool FilterCharsLenghInCaption
        {
            get
            {
                return _filterCharsLenghInCaption;
            }
            set
            {
                if (value == _filterCharsLenghInCaption)
                    return;
                SetProperty(ref _filterCharsLenghInCaption, value);
            }
        }

        private bool _filterByBlacklistedWhitelistedWordsInCaption;
        [ProtoMember(19)]
        public bool FilterByBlacklistedWhitelistedWordsInCaption
        {
            get
            {
                return _filterByBlacklistedWhitelistedWordsInCaption;
            }
            set
            {
                if (value == _filterByBlacklistedWhitelistedWordsInCaption)
                    return;
                SetProperty(ref _filterByBlacklistedWhitelistedWordsInCaption, value);
            }
        }

        private bool _filterLastPostAge;
        [ProtoMember(20)]
        public bool FilterLastPostAge
        {
            get
            {
                return _filterLastPostAge;
            }
            set
            {
                if (value == _filterLastPostAge)
                    return;
                SetProperty(ref _filterLastPostAge, value);
            }
        }
        private int _maxLastPostAge;
        [ProtoMember(21)]
        public int MaxLastPostAge
        {
            get
            {
                return _maxLastPostAge;
            }
            set
            {
                if (value == _maxLastPostAge)
                    return;
                SetProperty(ref _maxLastPostAge, value);
            }
        }

       
        private bool _filterBeforePostAge;
        [ProtoMember(22)]
        public bool FilterBeforePostAge
        {
            get
            {
                return _filterBeforePostAge;
            }
            set
            {
                if (value == _filterBeforePostAge)
                    return;
                SetProperty(ref _filterBeforePostAge, value);
            }
        }
    }
}