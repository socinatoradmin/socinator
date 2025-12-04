#region

using DominatorHouseCore.Utility;
using ProtoBuf;

#endregion

namespace DominatorHouseCore.Models.FacebookModels
{
    public interface IInviterOptions
    {
//        bool IsSendInvitationInMessanger { get; set; }
//
//        bool IsSendInvitationWithNote { get; set; }
//
//        bool IsInviteWithNoteOptionVisible { get; set; }
//
//        bool IsInviteWithNoteOptionVisibleEvent { get; set; }

        string Note { get; set; }

//        bool IsReinvite { get; set; }
//
//        int SendMessageAfterDays { get; set; }
    }


    public class InviterOptions : BindableBase, IInviterOptions
    {
        private bool _isSendInvitationInMessanger;

        [ProtoMember(1)]
        public bool IsSendInvitationInMessanger
        {
            get => _isSendInvitationInMessanger;
            set
            {
                if (value == _isSendInvitationInMessanger)
                    return;
                SetProperty(ref _isSendInvitationInMessanger, value);
            }
        }

        private bool _isSendInvitationWithNote;

        [ProtoMember(2)]
        public bool IsSendInvitationWithNote
        {
            get => _isSendInvitationWithNote;
            set
            {
                if (value == _isSendInvitationWithNote)
                    return;
                SetProperty(ref _isSendInvitationWithNote, value);
            }
        }


        private string _note = ConstantVariable.PageInviterNote;

        [ProtoMember(3)]
        public string Note
        {
            get => _note;
            set
            {
                if (value == _note)
                    return;
                SetProperty(ref _note, value);
            }
        }


        private int _sendMessageAfterDays;

        [ProtoMember(4)]
        public int SendMessageAfterDays
        {
            get => _sendMessageAfterDays;
            set
            {
                if (value == _sendMessageAfterDays)
                    return;
                SetProperty(ref _sendMessageAfterDays, value);
            }
        }


        private bool _isReinvite;

        [ProtoMember(5)]
        public bool IsReinvite
        {
            get => _isReinvite;
            set
            {
                if (value == _isReinvite)
                    return;
                SetProperty(ref _isReinvite, value);
            }
        }

        private bool _isInviteWithNoteOptionVisible = true;

        [ProtoMember(6)]
        public bool IsInviteWithNoteOptionVisible
        {
            get => _isInviteWithNoteOptionVisible;
            set
            {
                if (value == _isInviteWithNoteOptionVisible)
                    return;
                SetProperty(ref _isInviteWithNoteOptionVisible, value);
            }
        }

        private bool _isInviteWithNoteOptionVisibleEvent;

        [ProtoMember(7)]
        public bool IsInviteWithNoteOptionVisibleEvent
        {
            get => _isInviteWithNoteOptionVisibleEvent;
            set
            {
                if (value == _isInviteWithNoteOptionVisibleEvent)
                    return;
                SetProperty(ref _isInviteWithNoteOptionVisibleEvent, value);
            }
        }


        /*private bool _isGroupMember;

        [ProtoMember(8)]
        public bool IsGroupMember
        {
            get { return _isGroupMember; }
            set
            {
                if (value == _isGroupMember)
                    return;
                SetProperty(ref _isGroupMember, value);
            }
        }*/
    }
}