#region

using DominatorHouseCore.Utility;
using ProtoBuf;

#endregion

namespace DominatorHouseCore.Models.Publisher.CampaignsAdvanceSetting
{
    [ProtoContract]
    public class PinterestModel : BindableBase
    {
        private bool _isEnableCampaignSourceURL;

        [ProtoMember(1)]
        public bool IsEnableCampaignSourceURL
        {
            get => _isEnableCampaignSourceURL;
            set
            {
                if (_isEnableCampaignSourceURL == value)
                    return;
                SetProperty(ref _isEnableCampaignSourceURL, value);
            }
        }

        #region Common

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

        #endregion

        private string _sourceURL;

        [ProtoMember(12)]
        public string SourceURL
        {
            get => _sourceURL;
            set
            {
                if (_sourceURL == value)
                    return;
                SetProperty(ref _sourceURL, value);
            }
        }

        private int _sourceURLPercentage;

        [ProtoMember(13)]
        public int SourceURLPercentage
        {
            get => _sourceURLPercentage;
            set
            {
                if (_sourceURLPercentage == value)
                    return;
                SetProperty(ref _sourceURLPercentage, value);
            }
        }

        private bool _isOverwritePostSourceURL;

        [ProtoMember(14)]
        public bool IsOverwritePostSourceURL
        {
            get => _isOverwritePostSourceURL;
            set
            {
                if (_isOverwritePostSourceURL == value)
                    return;
                SetProperty(ref _isOverwritePostSourceURL, value);
            }
        }

        private bool _isDeletePinsThatHaveBellow;

        [ProtoMember(15)]
        public bool IsDeletePinsThatHaveBellow
        {
            get => _isDeletePinsThatHaveBellow;
            set
            {
                if (_isDeletePinsThatHaveBellow == value)
                    return;
                SetProperty(ref _isDeletePinsThatHaveBellow, value);
            }
        }

        private int _numberOfLike;

        [ProtoMember(16)]
        public int NumberOfLike
        {
            get => _numberOfLike;
            set
            {
                if (_numberOfLike == value)
                    return;
                SetProperty(ref _numberOfLike, value);
            }
        }

        private int _numberOfRepinsCheckIn;

        [ProtoMember(17)]
        public int NumberOfRepinsCheckIn
        {
            get => _numberOfRepinsCheckIn;
            set
            {
                if (_numberOfRepinsCheckIn == value)
                    return;
                SetProperty(ref _numberOfRepinsCheckIn, value);
            }
        }

        private int _numberOfDayAfterRepins;

        [ProtoMember(18)]
        public int NumberOfDayAfterRepins
        {
            get => _numberOfDayAfterRepins;
            set
            {
                if (_numberOfDayAfterRepins == value)
                    return;
                SetProperty(ref _numberOfDayAfterRepins, value);
            }
        }

        [ProtoMember(19)] public string CampaignId { get; set; }
        private string _hashtagsFromList1;

        [ProtoMember(20)]
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

        [ProtoMember(21)]
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

        public PinterestModel Clone()
        {
            return (PinterestModel) MemberwiseClone();
        }
    }
}