#region

using DominatorHouseCore.Utility;
using ProtoBuf;

#endregion

namespace DominatorHouseCore.Models.Publisher.CampaignsAdvanceSetting
{
    [ProtoContract]
    public class TwitterModel : BindableBase
    {
        private bool _isEnableAutomaticHashTags;

        [ProtoMember(2)]
        public bool IsEnableAutomaticHashTags
        {
            get => _isEnableAutomaticHashTags;
            set
            {
                if (_isEnableAutomaticHashTags == value)
                    return;
                SetProperty(ref _isEnableAutomaticHashTags, value);
            }
        }

        private int _maxHashtagsPerPost;

        [ProtoMember(3)]
        public int MaxHashtagsPerPost
        {
            get => _maxHashtagsPerPost;
            set
            {
                if (_maxHashtagsPerPost == value)
                    return;
                SetProperty(ref _maxHashtagsPerPost, value);
            }
        }

        private string _hashWords;

        [ProtoMember(4)]
        public string HashWords
        {
            get => _hashWords;
            set
            {
                if (_hashWords == value)
                    return;
                SetProperty(ref _hashWords, value);
            }
        }

        private int _minimumWordLength;

        [ProtoMember(5)]
        public int MinimumWordLength
        {
            get => _minimumWordLength;
            set
            {
                if (_minimumWordLength == value)
                    return;
                SetProperty(ref _minimumWordLength, value);
            }
        }

        private int _replaceProbability;

        [ProtoMember(6)]
        public int ReplaceProbability
        {
            get => _replaceProbability;
            set
            {
                if (_replaceProbability == value)
                    return;
                SetProperty(ref _replaceProbability, value);
            }
        }

        private bool _isEnableDynamicHashTags;

        [ProtoMember(7)]
        public bool IsEnableDynamicHashTags
        {
            get => _isEnableDynamicHashTags;
            set
            {
                if (_isEnableDynamicHashTags == value)
                    return;
                SetProperty(ref _isEnableDynamicHashTags, value);
            }
        }

        private bool _isAddHashTagEvenIfAlreadyHastags;

        [ProtoMember(8)]
        public bool IsAddHashTagEvenIfAlreadyHastags
        {
            get => _isAddHashTagEvenIfAlreadyHastags;
            set
            {
                if (_isAddHashTagEvenIfAlreadyHastags == value)
                    return;
                SetProperty(ref _isAddHashTagEvenIfAlreadyHastags, value);
            }
        }

        private RangeUtilities _maxHashtagsPerPostRange = new RangeUtilities();

        [ProtoMember(9)]
        public RangeUtilities MaxHashtagsPerPostRange
        {
            get => _maxHashtagsPerPostRange;
            set
            {
                if (_maxHashtagsPerPostRange == value)
                    return;
                SetProperty(ref _maxHashtagsPerPostRange, value);
            }
        }

        private int _pickPercentHashTag;

        [ProtoMember(10)]
        public int PickPercentHashTag
        {
            get => _pickPercentHashTag;
            set
            {
                if (_pickPercentHashTag == value)
                    return;
                SetProperty(ref _pickPercentHashTag, value);
            }
        }

        private int _pickPercentFromList;

        [ProtoMember(11)]
        public int PickPercentFromList
        {
            get => _pickPercentFromList;
            set
            {
                if (_pickPercentFromList == value)
                    return;
                SetProperty(ref _pickPercentFromList, value);
            }
        }

        private bool _isDeletePostAfter;

        [ProtoMember(12)]
        public bool IsDeletePostAfter
        {
            get => _isDeletePostAfter;
            set
            {
                if (_isDeletePostAfter == value)
                    return;
                SetProperty(ref _isDeletePostAfter, value);
            }
        }

        private RangeUtilities _deletePostAfter = new RangeUtilities();

        [ProtoMember(13)]
        public RangeUtilities DeletePostAfter
        {
            get => _deletePostAfter;
            set
            {
                if (_deletePostAfter == value)
                    return;
                SetProperty(ref _deletePostAfter, value);
            }
        }

        [ProtoMember(14)] public string CampaignId { get; set; }
        private string _hashtagsFromList1;

        [ProtoMember(15)]
        public string HashtagsFromList1
        {
            get => _hashtagsFromList1;
            set
            {
                if (_hashtagsFromList1 == value)
                    return;
                SetProperty(ref _hashtagsFromList1, value);
            }
        }

        private string _hashtagsFromList2;

        [ProtoMember(16)]
        public string HashtagsFromList2
        {
            get => _hashtagsFromList2;
            set
            {
                if (_hashtagsFromList2 == value)
                    return;
                SetProperty(ref _hashtagsFromList2, value);
            }
        }

        public TwitterModel Clone()
        {
            return (TwitterModel) MemberwiseClone();
        }
    }
}