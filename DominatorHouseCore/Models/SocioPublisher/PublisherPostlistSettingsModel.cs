#region

using DominatorHouseCore.Utility;
using ProtoBuf;

#endregion

namespace DominatorHouseCore.Models.SocioPublisher
{
    [ProtoContract]
    public class PublisherPostlistSettingsModel : BindableBase
    {
        private bool _isRemoveFromDraft;
        private bool _isReaddPublishedPosts;
        private bool _isSendPublishedPostToBump;

        /// <summary>
        ///     If its checked, the post items should be removed from draft post list while adding to pending post list,
        ///     Otherwise it should be present draft also and pending list also
        /// </summary>
        [ProtoMember(1)]
        public bool IsRemoveFromDraft
        {
            get => _isRemoveFromDraft;
            set
            {
                if (_isRemoveFromDraft == value)
                    return;
                _isRemoveFromDraft = value;
                OnPropertyChanged(nameof(IsRemoveFromDraft));
            }
        }

        /// <summary>
        ///     If its checked, The post item need to add in pending post list again after published successfully
        ///     Note: Only for particular campaigns post list
        /// </summary>
        [ProtoMember(2)]
        public bool IsReaddPublishedPosts
        {
            get => _isReaddPublishedPosts;
            set
            {
                if (_isReaddPublishedPosts == value)
                    return;
                _isReaddPublishedPosts = value;
                OnPropertyChanged(nameof(IsReaddPublishedPosts));
            }
        }

        /// <summary>
        ///     If its selected, send the published post items to bump features
        /// </summary>
        [ProtoMember(3)]
        public bool IsSendPublishedPostToBump
        {
            get => _isSendPublishedPostToBump;
            set
            {
                if (_isSendPublishedPostToBump == value)
                    return;
                _isSendPublishedPostToBump = value;
                OnPropertyChanged(nameof(IsSendPublishedPostToBump));
            }
        }


        private string _campaignId;

        /// <summary>
        ///     To specify the campaign id for identify all above settings are belongs to which campaign.
        /// </summary>
        [ProtoMember(4)]
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
    }
}