#region

using System.Collections.Generic;
using DominatorHouseCore.Utility;
using ProtoBuf;

#endregion

namespace DominatorHouseCore.Models.Publisher
{
    [ProtoContract]
    public class OtherConfigurationModel : BindableBase
    {

        private bool _isYoutubeCommunityPost;
        [ProtoMember(1)]
        public bool IsYoutubeCommunityPost
        {
            get => _isYoutubeCommunityPost;
            set
            {
                if (_isYoutubeCommunityPost == value)
                    return;

                SetProperty(ref _isYoutubeCommunityPost, value);
            }
        }
        /// <summary>
        ///     To specify the whether to use the signature
        /// </summary>
        private bool _isEnableSignatureChecked;

        [ProtoMember(2)]
        public bool IsEnableSignatureChecked
        {
            get => _isEnableSignatureChecked;
            set
            {
                if (value == _isEnableSignatureChecked)
                    return;
                SetProperty(ref _isEnableSignatureChecked, value);
            }
        }

        /// <summary>
        ///     To specify the shorten url for link
        /// </summary>
        private bool _isShortenURLsChecked;

        [ProtoMember(3)]
        public bool IsShortenURLsChecked
        {
            get => _isShortenURLsChecked;
            set
            {
                if (value == _isShortenURLsChecked)
                    return;
                SetProperty(ref _isShortenURLsChecked, value);
            }
        }


        /// <summary>
        ///     To specify the publishing post with texts and link.Its only for Pinterest
        /// </summary>
        private bool _isAllowPublishingPinterestChecked;

        [ProtoMember(5)]
        public bool IsAllowPublishingPinterestChecked
        {
            get => _isAllowPublishingPinterestChecked;
            set
            {
                if (value == _isAllowPublishingPinterestChecked)
                    return;
                SetProperty(ref _isAllowPublishingPinterestChecked, value);
            }
        }

        /// <summary>
        ///     To publish as story instead of own wall. Its only for instagram
        /// </summary>
        private bool _isPostAsStoryChecked;

        [ProtoMember(6)]
        public bool IsPostAsStoryChecked
        {
            get => _isPostAsStoryChecked;
            set
            {
                if (value == _isPostAsStoryChecked)
                    return;
                SetProperty(ref _isPostAsStoryChecked, value);
            }
        }

        /// <summary>
        ///     Adding signature to your post descriptions
        /// </summary>
        private string _signatureText;

        [ProtoMember(10)]
        public string SignatureText
        {
            get => _signatureText;
            set
            {
                if (value == _signatureText)
                    return;
                SetProperty(ref _signatureText, value);
            }
        }

        private bool _isCheckedForApprovePost;

        [ProtoMember(11)]
        public bool IsCheckedForApprovePost
        {
            get => _isCheckedForApprovePost;
            set
            {
                if (value == _isCheckedForApprovePost)
                    return;
                SetProperty(ref _isCheckedForApprovePost, value);
            }
        }

        private bool _IsMentionUser;

        [ProtoMember(12)]
        public bool IsMentionUser
        {
            get => _IsMentionUser;
            set
            {
                if (value)
                    _IsMentionUser = false;

                SetProperty(ref _IsMentionUser, value);
            }
        }

        private string _MentionUserList;

        [ProtoMember(13)]
        public string MentionUserList
        {
            get => _MentionUserList;
            set
            {
                if (_MentionUserList == value)
                    return;

                SetProperty(ref _MentionUserList, value);
            }
        }

        #region Not Used

        


        private bool _isPostTextChecked;

        [ProtoMember(4)]
        public bool IsPostTextChecked
        {
            get => _isPostTextChecked;
            set
            {
                if (value == _isPostTextChecked)
                    return;
                SetProperty(ref _isPostTextChecked, value);
            }
        }


        private List<string> _makeImagesUniqueStatus = new List<string>();

        public List<string> MakeImagesUniqueStatus
        {
            get => _makeImagesUniqueStatus;
            set
            {
                if (value == _makeImagesUniqueStatus)
                    return;
                SetProperty(ref _makeImagesUniqueStatus, value);
            }
        }


        private bool _isEnableCustomTokensChecked;

        [ProtoMember(9)]
        public bool IsEnableCustomTokensChecked
        {
            get => _isEnableCustomTokensChecked;
            set
            {
                if (value == _isEnableCustomTokensChecked)
                    return;
                SetProperty(ref _isEnableCustomTokensChecked, value);
            }
        }


        private bool _isEnableWatermarkChecked;

        [ProtoMember(8)]
        public bool IsEnableWatermarkChecked
        {
            get => _isEnableWatermarkChecked;
            set
            {
                if (value == _isEnableWatermarkChecked)
                    return;
                SetProperty(ref _isEnableWatermarkChecked, value);
            }
        }


        private bool _isMakeImagesUniqueChecked;

        [ProtoMember(7)]
        public bool IsMakeImagesUniqueChecked
        {
            get => _isMakeImagesUniqueChecked;
            set
            {
                if (value == _isMakeImagesUniqueChecked)
                    return;
                SetProperty(ref _isMakeImagesUniqueChecked, value);
            }
        }

        #endregion
    }
}