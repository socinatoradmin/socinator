using DominatorHouseCore.Utility;
using FaceDominatorCore.FDEnums;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FaceDominatorCore.FDModel.FilterModel
{

    public interface IFdFanpageFilterModel
    {
        /*bool IsVerifiedPage { get; set; }

        bool IsLikedByMyFriends { get; set; }

        bool IsFanpageCategory { get; set; }

        List<string> ObjFanpageCategory { get; set; }

        string SelectedCategory { get; set; }

        RangeUtilities LikersBetWeen { get; set; }

        bool IsLikersRangeChecked { get; set; }*/
    }

    public class FdFanpageFilterModel : BindableBase, IFdFanpageFilterModel
    {

        private bool _isFanpageCategory;

        [ProtoMember(1)]
        public bool IsFanpageCategory
        {
            get { return _isFanpageCategory; }
            set
            {
                SetProperty(ref _isFanpageCategory, value);
            }
        }

        private bool _isLikedByMyFriends;

        [ProtoMember(2)]
        public bool IsLikedByMyFriends
        {
            get { return _isLikedByMyFriends; }
            set
            {
                SetProperty(ref _isLikedByMyFriends, value);
            }
        }


        private bool _isVerifiedPage;

        [ProtoMember(3)]
        public bool IsVerifiedPage
        {
            get { return _isVerifiedPage; }
            set
            {
                SetProperty(ref _isVerifiedPage, value);
            }
        }

        private List<string> _objFanpageCategory = Enum.GetNames(typeof(FanpageCategory)).ToList();

        [ProtoMember(4)]
        // ReSharper disable once UnusedMember.Global
        public List<string> ObjFanpageCategory
        {
            get { return _objFanpageCategory; }
            set
            {
                SetProperty(ref _objFanpageCategory, value);
            }
        }

        private string _selectedCategory = string.Empty;

        [ProtoMember(5)]
        public string SelectedCategory
        {
            get { return _selectedCategory; }
            set
            {
                SetProperty(ref _selectedCategory, value);
            }
        }


        private RangeUtilities _likersBetWeen = new RangeUtilities(500, 5000);

        [ProtoMember(5)]
        public RangeUtilities LikersBetWeen
        {
            get { return _likersBetWeen; }
            set
            {
                SetProperty(ref _likersBetWeen, value);
            }
        }

        private bool _isLikersRangeChecked;

        [ProtoMember(5)]
        public bool IsLikersRangeChecked
        {
            get { return _isLikersRangeChecked; }
            set
            {
                SetProperty(ref _isLikersRangeChecked, value);
            }
        }



    }
}
