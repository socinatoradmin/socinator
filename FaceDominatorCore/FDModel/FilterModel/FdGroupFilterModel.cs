using DominatorHouseCore.Utility;
using ProtoBuf;

namespace FaceDominatorCore.FDModel.FilterModel
{

    public interface IFdGroupFilterModel
    {
        /*bool IsGroupTypeChecked { get; set; }

        bool IsPublicGroup { get; set; }

        bool IsPrivateGroup { get; set; }

        bool IsGroupsJoinedByMyFriends { get; set; }

        bool IsSkipJoinedGroups { get; set; }

        bool IsGroupsBetweenChecked { get; set; }

        RangeUtilities MemberCountBetween { get; set; }*/



    }

    public class FdGroupFilterModel : BindableBase, IFdGroupFilterModel
    {

        private bool _isGroupTypeChecked;

        [ProtoMember(1)]
        public bool IsGroupTypeChecked
        {
            get { return _isGroupTypeChecked; }
            set
            {
                SetProperty(ref _isGroupTypeChecked, value);
            }
        }

        private bool _isPublicGroup;

        [ProtoMember(2)]
        public bool IsPublicGroup
        {
            get { return _isPublicGroup; }
            set
            {
                SetProperty(ref _isPublicGroup, value);
            }
        }

        private bool _isPrivateGroup;

        [ProtoMember(3)]
        public bool IsPrivateGroup
        {
            get { return _isPrivateGroup; }
            set
            {
                SetProperty(ref _isPrivateGroup, value);
            }
        }

        private bool _isGroupsJoinedByMyFriends;

        [ProtoMember(4)]
        public bool IsGroupsJoinedByMyFriends
        {
            get { return _isGroupsJoinedByMyFriends; }
            set
            {
                SetProperty(ref _isGroupsJoinedByMyFriends, value);
            }
        }

        private bool _isSkipJoinedGroups;

        [ProtoMember(5)]
        public bool IsSkipJoinedGroups
        {
            get { return _isSkipJoinedGroups; }
            set
            {
                SetProperty(ref _isSkipJoinedGroups, value);
            }
        }

        private bool _isGroupsBetweenChecked;

        [ProtoMember(6)]
        public bool IsGroupsBetweenChecked
        {
            get { return _isGroupsBetweenChecked; }
            set
            {
                SetProperty(ref _isGroupsBetweenChecked, value);
            }
        }

        private RangeUtilities _memberCountBetween = new RangeUtilities();

        [ProtoMember(7)]
        public RangeUtilities MemberCountBetween
        {
            get { return _memberCountBetween; }
            set
            {
                SetProperty(ref _memberCountBetween, value);
            }
        }


        private bool _isUniqueGroupsChecked;

        [ProtoMember(8)]
        public bool IsUniqueGroupsChecked
        {
            get { return _isUniqueGroupsChecked; }
            set
            {
                SetProperty(ref _isUniqueGroupsChecked, value);
            }
        }


    }
}
