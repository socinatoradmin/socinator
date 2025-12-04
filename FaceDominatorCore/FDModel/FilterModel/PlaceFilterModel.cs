using DominatorHouseCore.Utility;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace FaceDominatorCore.FDModel.FilterModel
{

    public interface IFdPlaceFilterModel
    {
        /*bool IsVerifiedPage { get; set; }

        bool IsLikedByMyFriends { get; set; }

        bool IsFanpageCategory { get; set; }

        List<string> ObjFanpageCategory { get; set; }

        string SelectedCategory { get; set; }

        RangeUtilities LikersBetWeen { get; set; }

        bool IsLikersRangeChecked { get; set; }*/
    }

    public class FdPlaceFilterModel : BindableBase, IFdPlaceFilterModel
    {

        private bool _isPriceRangeChecked;

        [ProtoMember(1)]
        public bool IsPriceRangeChecked
        {
            get { return _isPriceRangeChecked; }
            set
            {
                SetProperty(ref _isPriceRangeChecked, value);
            }
        }

        private bool _isOpenNowChecked;

        [ProtoMember(2)]
        public bool IsOpenNowChecked
        {
            get { return _isOpenNowChecked; }
            set
            {
                SetProperty(ref _isOpenNowChecked, value);
            }
        }




        private bool _isVisitedByFriendsChecked;

        [ProtoMember(3)]
        public bool IsVisitedByFriendsChecked
        {
            get { return _isVisitedByFriendsChecked; }
            set
            {
                SetProperty(ref _isVisitedByFriendsChecked, value);
            }
        }

        private List<string> _priceRangeCategory = Enum.GetNames(typeof(PlacePriceRange)).ToList();

        [ProtoMember(4)]
        // ReSharper disable once UnusedMember.Global
        public List<string> PriceRangeCategory
        {
            get { return _priceRangeCategory.Distinct().ToList(); }
            set
            {
                SetProperty(ref _priceRangeCategory, value);
            }
        }

        private string _selectedPriceRange = string.Empty;

        [ProtoMember(5)]
        public string SelectedPriceRange
        {
            get { return _selectedPriceRange; }
            set
            {
                SetProperty(ref _selectedPriceRange, value);
            }
        }
        private bool _isdeliveryChecked;
        [ProtoMember(6)]
        public bool IsDeliveryChecked
        {
            get { return _isdeliveryChecked; }
            set
            {
                SetProperty(ref _isdeliveryChecked, value);
            }
        }

        private bool _istakeawayChecked;

        [ProtoMember(7)]
        public bool IsTakeAwayChecked
        {
            get { return _istakeawayChecked; }
            set
            {
                SetProperty(ref _istakeawayChecked, value);
            }
        }

    }


    public enum PlacePriceRange
    {
        [Description("£")]
        AnyPrice = 0,
        [Description("£")]
        Low = 1,
        [Description("££")]
        Medium = 2,
        [Description("£££")]
        High = 3,
        [Description("££££")]
        VeryHigh = 4
    }
}
