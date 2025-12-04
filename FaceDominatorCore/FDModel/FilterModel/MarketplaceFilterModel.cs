using DominatorHouseCore.Utility;
using FaceDominatorCore.FDEnums;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FaceDominatorCore.FDModel.FilterModel
{


    public interface IMarketplaceFilterModel
    {
        /*bool IsVerifiedPage { get; set; }

        bool IsLikedByMyFriends { get; set; }

        bool IsFanpageCategory { get; set; }

        List<string> ObjFanpageCategory { get; set; }

        string SelectedCategory { get; set; }

        RangeUtilities LikersBetWeen { get; set; }

        bool IsLikersRangeChecked { get; set; }*/
    }

    public class MarketplaceFilterModel : BindableBase, IMarketplaceFilterModel
    {

        private bool _isLocationFilterChecked;

        [ProtoMember(1)]
        public bool IsLocationFilterChecked
        {
            get { return _isLocationFilterChecked; }
            set
            {
                SetProperty(ref _isLocationFilterChecked, value);
            }
        }

        private bool _isCategoryChecked;

        [ProtoMember(2)]
        public bool IsCategoryChecked
        {
            get { return _isCategoryChecked; }
            set
            {
                SetProperty(ref _isCategoryChecked, value);
            }
        }



        private bool _isSubCategoryChecked;

        [ProtoMember(3)]
        public bool IsSubCategoryChecked
        {
            get { return _isSubCategoryChecked; }
            set
            {
                SetProperty(ref _isSubCategoryChecked, value);
            }
        }


        private bool _isLocationDistanceChecked;

        [ProtoMember(4)]
        public bool IsLocationDistanceChecked
        {
            get { return _isLocationDistanceChecked; }
            set
            {
                SetProperty(ref _isLocationDistanceChecked, value);
            }
        }


        private bool _isFreeListingsChecked;

        [ProtoMember(5)]
        public bool IsFreeListingsChecked
        {
            get { return _isFreeListingsChecked; }
            set
            {
                SetProperty(ref _isFreeListingsChecked, value);
            }
        }


        private bool _isPriceRangeChecked;

        [ProtoMember(6)]
        public bool IsPriceRangeChecked
        {
            get { return _isPriceRangeChecked; }
            set
            {
                SetProperty(ref _isPriceRangeChecked, value);
            }
        }


        private bool _isSortByChecked;

        [ProtoMember(7)]
        public bool IsSortByChecked
        {
            get { return _isSortByChecked; }
            set
            {
                SetProperty(ref _isSortByChecked, value);
            }
        }



        private List<string> _marketplaceSortOption = Enum.GetNames(typeof(MarketplaceSortByOptions)).ToList();

        [ProtoMember(8)]
        // ReSharper disable once UnusedMember.Global
        public List<string> MarketplaceSortOption
        {
            get { return _marketplaceSortOption; }
            set
            {
                SetProperty(ref _marketplaceSortOption, value);
            }
        }

        private string _selectedSortOption = (Enum.GetNames(typeof(MarketplaceSortByOptions)).ToList())[0];

        [ProtoMember(9)]
        public string SelectedSortOption
        {
            get { return _selectedSortOption; }
            set
            {
                SetProperty(ref _selectedSortOption, value);
            }
        }





        private List<string> _marketplaceMainCategory = Enum.GetNames(typeof(MarketplaceCategory)).ToList();

        [ProtoMember(10)]
        // ReSharper disable once UnusedMember.Global
        public List<string> MarketplaceMainCategory
        {
            get { return _marketplaceMainCategory; }
            set
            {
                SetProperty(ref _marketplaceMainCategory, value);
            }
        }

        private string _selectedMainCategory = (Enum.GetNames(typeof(MarketplaceCategory)).ToList())[0];

        [ProtoMember(11)]
        public string SelectedMainCategory
        {
            get { return _selectedMainCategory; }
            set
            {
                SetProperty(ref _selectedMainCategory, value);
            }
        }


        private List<string> _marketplaceLocationDistance = new List<string>();

        [ProtoMember(12)]
        // ReSharper disable once UnusedMember.Global
        public List<string> MarketplaceLocationDistance
        {
            get { return _marketplaceLocationDistance; }
            set
            {
                SetProperty(ref _marketplaceLocationDistance, value);
            }
        }

        private string _selectedLocationDistance;

        [ProtoMember(13)]
        public string SelectedLocationDistance
        {
            get { return _selectedLocationDistance; }
            set
            {
                SetProperty(ref _selectedLocationDistance, value);
            }
        }



        private RangeUtilities _priceBetween = new RangeUtilities(0, 2000000000);

        [ProtoMember(14)]
        public RangeUtilities PriceBetween
        {
            get { return _priceBetween; }
            set
            {
                SetProperty(ref _priceBetween, value);
            }
        }

        private List<string> _lstLocation = new List<string>();

        [ProtoMember(15)]
        public List<string> LstLocation
        {
            get { return _lstLocation; }
            set
            {
                SetProperty(ref _lstLocation, value);
            }
        }

        private string _locationText = string.Empty;

        [ProtoMember(16)]
        public string LocationText
        {
            get { return _locationText; }
            set
            {
                SetProperty(ref _locationText, value);
            }
        }


        private bool _isSqureFeetBetweenChecked;

        [ProtoMember(17)]
        public bool IsSqureFeetBetweenChecked
        {
            get { return _isSqureFeetBetweenChecked; }
            set
            {
                SetProperty(ref _isSqureFeetBetweenChecked, value);
            }
        }


        private RangeUtilities _noOfSquraeFeet = new RangeUtilities(0, 2000);

        [ProtoMember(18)]
        public RangeUtilities NoOfSquraeFeet
        {
            get { return _noOfSquraeFeet; }
            set
            {
                SetProperty(ref _noOfSquraeFeet, value);
            }
        }


        private bool _isNoOfBedroomsChecked;

        [ProtoMember(19)]
        public bool IsNoOfBedroomsChecked
        {
            get { return _isNoOfBedroomsChecked; }
            set
            {
                SetProperty(ref _isNoOfBedroomsChecked, value);
            }
        }


        private RangeUtilities _noOfBedrooms = new RangeUtilities(0, 2000);

        [ProtoMember(20)]
        public RangeUtilities NoOfBedrooms
        {
            get { return _noOfBedrooms; }
            set
            {
                SetProperty(ref _noOfBedrooms, value);
            }
        }

        private bool _isNoOfBathroomsChecked;

        [ProtoMember(21)]
        public bool IsNoOfBathroomsChecked
        {
            get { return _isNoOfBathroomsChecked; }
            set
            {
                SetProperty(ref _isNoOfBathroomsChecked, value);
            }
        }


        private RangeUtilities _noOfBaths = new RangeUtilities(0, 2000);

        [ProtoMember(22)]
        public RangeUtilities NoOfBaths
        {
            get { return _noOfBaths; }
            set
            {
                SetProperty(ref _noOfBaths, value);
            }
        }


        private bool _isPropertyTypeChecked;

        [ProtoMember(23)]
        public bool IsPropertyTypeChecked
        {
            get { return _isPropertyTypeChecked; }
            set
            {
                SetProperty(ref _isPropertyTypeChecked, value);
            }
        }


        private List<string> _marketplacePropertyTypeToRent = Enum.GetNames(typeof(MarketplacePropertyType)).ToList();

        [ProtoMember(24)]
        // ReSharper disable once UnusedMember.Global
        public List<string> MarketplacePropertyTypeToRent
        {
            get { return _marketplacePropertyTypeToRent; }
            set
            {
                SetProperty(ref _marketplacePropertyTypeToRent, value);
            }
        }

        private string _selectedPropertyTypeToRent = (Enum.GetNames(typeof(MarketplacePropertyType)).ToList())[0];

        [ProtoMember(25)]
        public string SelectedPropertyTypeToRent
        {
            get { return _selectedPropertyTypeToRent; }
            set
            {
                SetProperty(ref _selectedPropertyTypeToRent, value);
            }
        }
    }
}
