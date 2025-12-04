using DominatorHouseCore.Utility;
using ProtoBuf;
using System.Collections.Generic;

namespace FaceDominatorCore.FDModel.FilterModel
{
    [ProtoContract]

    public class FdCommentFilterModel : BindableBase
    {
        private bool _isCommentDateBetweenChecked;

        [ProtoMember(1)]
        public bool IsCommentDateBetweenChecked
        {
            get
            {
                return _isCommentDateBetweenChecked;
            }
            set
            {
                SetProperty(ref _isCommentDateBetweenChecked, value);
            }
        }

        private RangeUtilities _commentedBeforeDays = new RangeUtilities(0, 0);

        [ProtoMember(2)]
        public RangeUtilities CommentedBeforeDays
        {
            get
            {
                return _commentedBeforeDays;
            }
            set
            {
                SetProperty(ref _commentedBeforeDays, value);
            }
        }


        private bool _isCommentTextFilter;

        [ProtoMember(3)]
        public bool IsCommentTextFilter
        {
            get
            {
                return _isCommentTextFilter;
            }
            set
            {
                SetProperty(ref _isCommentTextFilter, value);
            }
        }


        private List<string> _listFilterText = new List<string>();

        [ProtoMember(4)]
        public List<string> ListFilterText
        {
            get
            {
                return _listFilterText;
            }
            set
            {
                SetProperty(ref _listFilterText, value);
            }
        }



        private string _filterText;

        [ProtoMember(5)]
        public string FilterText
        {
            get
            {
                return _filterText;
            }
            set
            {
                SetProperty(ref _filterText, value);
            }
        }

    }
}
