using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDEnums;
using FaceDominatorCore.FDModel.CustomControlModel;
using FaceDominatorCore.Interface;
using ProtoBuf;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace FaceDominatorCore.FDModel.FilterModel
{
    [ProtoContract]
    public class LikerCommentorConfigModel:BindableBase, ILikerCommentorConfig
    {
        private bool _isLikeTypeFilterChkd=true;

        [ProtoMember(1)]
        public bool IsLikeTypeFilterChkd
        {
            get
            {
                return _isLikeTypeFilterChkd;
            }
            set
            {
                if (value == _isLikeTypeFilterChkd)
                    return;
                SetProperty(ref _isLikeTypeFilterChkd, value);
            }
        }




        private bool _isHahaFilterChkd;

        [ProtoMember(2)]
        // ReSharper disable once IdentifierTypo
        public bool IsHahaFilterChkd
        {
            get
            {
                return _isHahaFilterChkd;
            }
            set
            {
                if (value == _isHahaFilterChkd)
                    return;
                SetProperty(ref _isHahaFilterChkd, value);
            }
        }



        private bool _isLikeFilterChkd = true;

        [ProtoMember(3)]
        // ReSharper disable once IdentifierTypo
        public bool IsLikeFilterChkd
        {
            get
            {
                return _isLikeFilterChkd;
            }
            set
            {
                if (value == _isLikeFilterChkd)
                    return;
                SetProperty(ref _isLikeFilterChkd, value);
            }
        }

        private bool _isLoveFilterChkd;

        [ProtoMember(4)]
        // ReSharper disable once UnusedMember.Global
        public bool IsLoveFilterChkd
        {
            get
            {
                return _isLoveFilterChkd;
            }
            set
            {
                if (value == _isLoveFilterChkd)
                    return;
                SetProperty(ref _isLoveFilterChkd, value);
            }
        }

        private bool _isWowFilterChkd;

        [ProtoMember(5)]
        // ReSharper disable once IdentifierTypo
        public bool IsWowFilterChkd
        {
            get
            {
                return _isWowFilterChkd;
            }
            set
            {
                if (value == _isWowFilterChkd)
                    return;
                SetProperty(ref _isWowFilterChkd, value);
            }
        }

        private bool _isSadFilterChkd;

        [ProtoMember(6)]
        // ReSharper disable once UnusedMember.Global
        public bool IsSadFilterChkd
        {
            get
            {
                return _isSadFilterChkd;
            }
            set
            {
                if (value == _isSadFilterChkd)
                    return;
                SetProperty(ref _isSadFilterChkd, value);
            }
        }

        private bool _isAngryFilterChkd;

        [ProtoMember(7)]
        // ReSharper disable once UnusedMember.Global
        public bool IsAngryFilterChkd
        {
            get
            {
                return _isAngryFilterChkd;
            }
            set
            {
                if (value == _isAngryFilterChkd)
                    return;
                SetProperty(ref _isAngryFilterChkd, value);
            }
        }

        private bool _isCommentFilterChecked=true;

        [ProtoMember(8)]
        public bool IsCommentFilterChecked
        {
            get
            {
                return _isCommentFilterChecked;
            }
            set
            {
                if (value == _isCommentFilterChecked)
                    return;
                SetProperty(ref _isCommentFilterChecked, value);
            }
        }

        private ObservableCollectionBase<ManageCustomCommentsModel> _savedComments = new ObservableCollectionBase<ManageCustomCommentsModel>();

        [ProtoMember(3)]
        public ObservableCollectionBase<ManageCustomCommentsModel> SavedComments
        {
            get { return _savedComments; }
            set
            {
                if (value == _savedComments)
                    return;
                SetProperty(ref _savedComments, value);
            }
        }

        private ManageCustomCommentsModel _currentCommment = new ManageCustomCommentsModel();

        [ProtoMember(4)]
        public ManageCustomCommentsModel CurrentCommment
        {
            get { return _currentCommment; }
            set
            {
                if (value == _currentCommment)
                    return;
                SetProperty(ref _currentCommment, value);
            }
        }


        [ProtoMember(5)]
        public ObservableCollection<ManageCommentModel> LstManageCommentModel { get; set; } = new ObservableCollection<ManageCommentModel>();


        [ProtoMember(6)]

        public ManageCommentModel ManageCommentModel { get; set; } = new ManageCommentModel();

        private List<ReactionType> _lstReactionType = new List<ReactionType>();

        [ProtoMember(7)]

        public List<ReactionType> ListReactionType
        {
            get { return _lstReactionType; }
            set
            {
                if (value == _lstReactionType)
                    return;
                SetProperty(ref _lstReactionType, value);
            }
        }

        private bool _isSpintaxChecked;

        [ProtoMember(14)]
        public bool IsSpintaxChecked
        {
            get { return _isSpintaxChecked; }
            set
            {
                if (value == _isSpintaxChecked)
                    return;
                SetProperty(ref _isSpintaxChecked, value);
            }
        }

        private bool _isSplitOnNextLine;
        public bool IsSplitOnNextLine
        {
            get { return _isSplitOnNextLine; }
            set
            {
                if (value == IsSplitOnNextLine)
                    return;
                SetProperty(ref _isSplitOnNextLine, value);
            }
        }

    }
}
