using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDModel.CustomControlModel;
using System;
using System.Collections.ObjectModel;

namespace FaceDominatorCore.FDModel.FbEvents
{
    public class EventCreaterManagerModel : BindableBase, IEvent
    {
        public EventCreaterManagerModel()
        {
            Id = Utilities.GetGuid();
        }

        private string _id;
        public string Id
        {
            get
            {
                return _id;
            }
            set
            {
                if (value == _id)
                    return;
                SetProperty(ref _id, value);
            }
        }

        private string _eventId;
        public string EventId
        {
            get
            {
                return _eventId;
            }
            set
            {
                if (value == _eventId)
                    return;
                SetProperty(ref _eventId, value);
            }
        }

        private string _eventType = "Create Private Event";
        public string EventType
        {
            get { return _eventType; }
            set
            {
                if (_eventType == value)
                    return;
                SetProperty(ref _eventType, value);
            }
        }

        //private string _mediaPath;
        //public string MediaPath
        //{
        //    get
        //    { return _mediaPath; }
        //    set
        //    {
        //        if (value == _mediaPath)
        //            return;
        //        SetProperty(ref _mediaPath, value);
        //    }
        //}

        private string _eventName = string.Empty;
        public string EventName
        {
            get { return _eventName; }
            set
            {
                if (_eventName == value)
                    return;
                SetProperty(ref _eventName, value);
            }
        }

        private bool _isSelectLocation;
        public bool IsSelectLocation
        {
            get { return _isSelectLocation; }
            set
            {
                if (_isSelectLocation == value)
                    return;
                SetProperty(ref _isSelectLocation, value);
            }
        }

        private string _eventLocation;
        public string EventLocation
        {
            get { return _eventLocation; }
            set
            {
                if (_eventLocation == value)
                    return;
                SetProperty(ref _eventLocation, value);
            }
        }

        private string _eventDescription;

        public string EventDescription
        {
            get { return _eventDescription; }
            set
            {
                if (_eventDescription == value)
                    return;
                SetProperty(ref _eventDescription, value);
            }
        }

        private DateTime _eventStartDate = DateTime.Now.AddHours(3);
        public DateTime EventStartDate
        {
            get
            {
                return _eventStartDate;
            }
            set
            {
                if (value == _eventStartDate)
                    return;
                SetProperty(ref _eventStartDate, value);
            }
        }

        private DateTime _eventEndDate = DateTime.Now.AddHours(10);
        public DateTime EventEndDate
        {
            get
            {
                return _eventEndDate;
            }
            set
            {
                if (value == _eventEndDate)
                    return;
                SetProperty(ref _eventEndDate, value);
            }
        }

        private bool _isGuestCanInviteFriends = true;
        public bool IsGuestCanInviteFriends
        {
            get { return _isGuestCanInviteFriends; }
            set
            {
                if (_isGuestCanInviteFriends == value)
                    return;
                SetProperty(ref _isGuestCanInviteFriends, value);
            }
        }

        private bool _showGuestList = true;
        public bool IsShowGuestList
        {
            get { return _showGuestList; }
            set
            {
                if (_showGuestList == value)
                    return;
                SetProperty(ref _showGuestList, value);
            }
        }

        private bool _anyOneCanPostForAllPost = true;
        public bool IsAnyOneCanPostForAllPost
        {
            get { return _anyOneCanPostForAllPost; }
            set
            {
                if (_anyOneCanPostForAllPost == value)
                    return;
                SetProperty(ref _anyOneCanPostForAllPost, value);
            }
        }

        private bool _isPostMustApproved;
        public bool IsPostMustApproved
        {
            get { return _isPostMustApproved; }
            set
            {
                if (_isPostMustApproved == value)
                    return;
                SetProperty(ref _isPostMustApproved, value);
            }
        }

        private bool _isQuesOnMessanger;
        public bool IsQuesOnMessanger
        {
            get { return _isQuesOnMessanger; }
            set
            {
                if (_isQuesOnMessanger == value)
                    return;
                SetProperty(ref _isQuesOnMessanger, value);
            }
        }

        private FbMultiMediaModel _fbMultiMediaModel=new FbMultiMediaModel();

        public FbMultiMediaModel FbMultiMediaModel
        {
            get { return _fbMultiMediaModel; }
            set
            {
                if (_fbMultiMediaModel == value)
                    return;
                SetProperty(ref _fbMultiMediaModel, value);
            }
        }

        private string _mediaPath;

        public string MediaPath
        {
            get { return _mediaPath; }
            set
            {
                if (_mediaPath == value)
                    return;
                SetProperty(ref _mediaPath, value);
            }
        }

        private bool _isPrivatePostingVisibile;
        public bool IsPrivatePostingVisibile
        {
            get { return _isPrivatePostingVisibile; }
            set
            {
                if (_isPrivatePostingVisibile == value)
                    return;
                SetProperty(ref _isPrivatePostingVisibile, value);

            }
        }

        private bool _isPublicPostingVisibile;
        public bool IsPublicPostingVisibile
        {
            get { return _isPublicPostingVisibile; }
            set
            {
                if (_isPublicPostingVisibile == value)
                    return;
                SetProperty(ref _isPublicPostingVisibile, value);

            }
        }
        private string _textLength = "0/64";

        public string TextLength
        {
            get { return _textLength; }
            set
            {
                if (_textLength == value)
                    return;

                SetProperty(ref _textLength, value);
            }
        }


        private string _ownerId = string.Empty;

        public string OwnerId
        {
            get { return _ownerId; }
            set
            {
                if (_ownerId == value)
                    return;

                SetProperty(ref _ownerId, value);
            }
        }
    }
}