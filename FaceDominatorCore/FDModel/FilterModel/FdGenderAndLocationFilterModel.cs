using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDEnums;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace FaceDominatorCore.FDModel.FilterModel
{
    public interface IFdGenderAndLocationFilterModel
    {
        /*bool IsFilterByGender { get; set; }

        bool SelectMaleUser { get; set; }

        bool SelectFemaleUser { get; set; }

        bool IsLocationFilterChecked { get; set; }

        bool IsMutualFriendFilterChecked { get; set; }

        bool IsNoOfMutualFriendSmallerThan { get; set; }

        bool IsNoOfMutualFriend { get; set; }

        bool IsFriendOfFriend { get; set; }
*/


        string LocationText { get; set; }

        //        List<string> ListLocationUrl { get; set; }

        List<string> ListFriends { get; set; }

        //        string FriendsText { get; set; }
        //
        //        int TotalNoOfMutualFriend { get; set; }
        //
        //        int TotalNoOfMutualFriendSmallerThan { get; set; }
    }

    public class FdGenderAndLocationFilterModel : BindableBase, IFdGenderAndLocationFilterModel
    {
        private bool _isFilterByGender;

        [ProtoMember(1)]
        public bool IsFilterByGender
        {
            get
            {
                return _isFilterByGender;
            }
            set
            {
                SetProperty(ref _isFilterByGender, value);
            }
        }


        private bool _selectMaleUser;

        [ProtoMember(2)]
        public bool SelectMaleUser
        {
            get
            {
                return _selectMaleUser;
            }
            set
            {
                SetProperty(ref _selectMaleUser, value);
            }
        }

        private bool _selectFemaleUser;

        [ProtoMember(3)]
        public bool SelectFemaleUser
        {
            get
            {
                return _selectFemaleUser;
            }
            set
            {
                SetProperty(ref _selectFemaleUser, value);
            }
        }

        private bool _isLocationFilterChecked;

        [ProtoMember(4)]
        public bool IsLocationFilterChecked
        {
            get
            {
                return _isLocationFilterChecked;
            }
            set
            {
                SetProperty(ref _isLocationFilterChecked, value);
            }
        }

        private List<string> _listLocationUrl = new List<string>();

        [ProtoMember(5)]
        public List<string> ListLocationUrl
        {
            get
            {
                return _listLocationUrl;
            }
            set
            {
                SetProperty(ref _listLocationUrl, value);
            }
        }

        private string _locationText = string.Empty;

        [ProtoMember(6)]
        public string LocationText
        {
            get
            {
                return _locationText;
            }
            set
            {
                SetProperty(ref _locationText, value);
            }
        }


        private bool _isMutualFriendFilterChecked;

        [ProtoMember(7)]
        public bool IsMutualFriendFilterChecked
        {
            get
            {
                return _isMutualFriendFilterChecked;
            }
            set
            {
                SetProperty(ref _isMutualFriendFilterChecked, value);
            }
        }

        private bool _isNoOfMutualFriend;

        [ProtoMember(8)]
        public bool IsNoOfMutualFriend
        {
            get
            {
                return _isNoOfMutualFriend;
            }
            set
            {
                SetProperty(ref _isNoOfMutualFriend, value);
            }
        }

        private bool _isFriendOfFriend;

        [ProtoMember(9)]
        public bool IsFriendOfFriend
        {
            get
            {
                return _isFriendOfFriend;
            }
            set
            {
                SetProperty(ref _isFriendOfFriend, value);
            }
        }


        private List<string> _listFriends = new List<string>();

        [ProtoMember(10)]
        public List<string> ListFriends
        {
            get
            {
                return _listFriends;
            }
            set
            {
                SetProperty(ref _listFriends, value);
            }
        }


        /*private string _friendsText = string.Empty;

        [ProtoMember(11)]
        public string FriendsText
        {
            get
            {
                return _friendsText;
            }
            set
            {
                if (value == _friendsText)
                    return;
                SetProperty(ref _friendsText, value);
            }
        }*/



        private int _totalNoOfMutualFriend;

        [ProtoMember(12)]
        public int TotalNoOfMutualFriend
        {
            get
            {
                return _totalNoOfMutualFriend;
            }
            set
            {
                SetProperty(ref _totalNoOfMutualFriend, value);
            }
        }


        private bool _isNoOfMutualFriendSmallerThan;

        [ProtoMember(13)]
        public bool IsNoOfMutualFriendSmallerThan
        {
            get
            {
                return _isNoOfMutualFriendSmallerThan;
            }
            set
            {
                SetProperty(ref _isNoOfMutualFriendSmallerThan, value);
            }
        }


        private int _totalNoOfMutualFriendSmallerThan = 1;

        [ProtoMember(14)]
        public int TotalNoOfMutualFriendSmallerThan
        {
            get
            {
                return _totalNoOfMutualFriendSmallerThan;
            }
            set
            {
                SetProperty(ref _totalNoOfMutualFriendSmallerThan, value);
            }
        }


        private bool _isGroupFilterChecked;

        [ProtoMember(14)]
        public bool IsGroupFilterChecked
        {
            get
            {
                return _isGroupFilterChecked;
            }
            set
            {
                SetProperty(ref _isGroupFilterChecked, value);
            }
        }


        private bool _isGroupCategoryEnabled;

        [ProtoMember(15)]
        public bool IsGroupCategoryEnabled
        {
            get
            {
                return _isGroupCategoryEnabled;
            }
            set
            {
                SetProperty(ref _isGroupCategoryEnabled, value);
            }
        }

        private bool _isFriendsDropdownVisible;

        public bool IsFriendsDropdownVisible
        {
            get
            {
                return _isFriendsDropdownVisible;
            }
            set
            {
                SetProperty(ref _isFriendsDropdownVisible, value);
            }
        }

        private List<string> _objGroupMemberCategory = Enum.GetValues(typeof(GroupMemberCategory)).Cast<GroupMemberCategory>().ToList().Select(x => x.GetDescriptionAttr()).ToList();

        [ProtoMember(16)]
        // ReSharper disable once UnusedMember.Global
        public List<string> ObjGroupMemberCategory
        {
            get
            {
                if (IsFriendsDropdownVisible)
                    return _objGroupMemberCategory.Distinct().ToList();
                else
                {
                    List<String> list = _objGroupMemberCategory.Distinct().ToList();
                    list.RemoveAt(4);
                    return list;
                }

            }
            //set
            //{
            //    if (value == _objGroupMemberCategory)
            //        return;
            //    SetProperty(ref _objGroupMemberCategory, value);
            //}
        }

        private int _selectedGroupMemberCategory;

        [ProtoMember(16)]
        // ReSharper disable once UnusedMember.Global
        public int SelectedGroupMemberCategory
        {
            get
            {
                return _selectedGroupMemberCategory;
            }
            set
            {
                SetProperty(ref _selectedGroupMemberCategory, value);
            }
        }

        private bool _isTimeLimitEnabled;

        [ProtoMember(17)]
        public bool IsTimeLimitEnabled
        {
            get
            {
                return _isTimeLimitEnabled;
            }
            set
            {
                SetProperty(ref _isTimeLimitEnabled, value);
            }
        }
        private bool _isTimeLimitChecked;

        [ProtoMember(18)]
        public bool IsTimeLimitChecked
        {
            get
            {
                return _isTimeLimitChecked;
            }
            set
            {
                SetProperty(ref _isTimeLimitChecked, value);
            }
        }

        private int _timeLimitOfDays = 1;

        [ProtoMember(19)]
        public int TimeLimitOfDays
        {
            get
            {
                return _timeLimitOfDays;
            }
            set
            {
                SetProperty(ref _timeLimitOfDays, value);
            }
        }

        private bool _isMutualFriendsCountFilterSelected;

        public bool IsMutualFriendsCountFilterSelected
        {
            get { return _isMutualFriendsCountFilterSelected; }
            set
            {
                SetProperty(ref _isMutualFriendsCountFilterSelected, value);
            }
        }


        private List<LocationModel> _listLocationModel = new List<LocationModel>();
        [ProtoIgnore]
        public List<LocationModel> ListLocationModel
        {
            get { return _listLocationModel; }
            set
            {
                SetProperty(ref _listLocationModel, value);
            }
        }

        private ObservableCollection<string> _listCountry = new ObservableCollection<string>();
        [ProtoIgnore]
        public ObservableCollection<string> ListCountry
        {
            get { return _listCountry; }
            set
            {
                SetProperty(ref _listCountry, value);
            }
        }

        private ObservableCollection<string> _listSelectedCountry = new ObservableCollection<string>();
        [ProtoIgnore]
        public ObservableCollection<string> ListSelectedCountry
        {
            get { return _listSelectedCountry; }
            set
            {
                SetProperty(ref _listSelectedCountry, value);
            }
        }

        private List<KeyValuePair<string, string>> _listLocationUrlPair = new List<KeyValuePair<string, string>>();

        [ProtoMember(22)]
        public List<KeyValuePair<string, string>> ListLocationUrlPair
        {
            get
            {
                return _listLocationUrlPair;
            }
            set
            {
                SetProperty(ref _listLocationUrlPair, value);
            }
        }

    }
}
