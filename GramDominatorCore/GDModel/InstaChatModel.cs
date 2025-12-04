//using System;
//using System.Collections.Generic;
//using System.Linq;
//using DominatorHouseCore.Interfaces;
//using DominatorHouseCore.Models;
//using DominatorHouseCore.Utility;
//using GramDominatorCore.GDEnums;
//using ProtoBuf;

//namespace GramDominatorCore.GDModel
//{
//    public interface IInstaChatModel
//    {

//        #region IInstaChatModel

//        //int IncreaseMessageCount { get; set; }

//        //int MessageCountUntil { get; set; }

//        //RangeUtilities MessageBetweenJobs { get; set; }

//        //RangeUtilities MessageMaxBetween { get; set; }

//        //bool IsChkStopMessageToolWhenReachChecked { get; set; }

//        //bool IsChkMessageToolGetsTemporaryBlockedChecked { get; set; }

//        //RangeUtilities IncreaseEachDayMessage { get; set; }

//        #endregion

//    }

//    [ProtoContract]
//    public class InstaChatModel : ModuleSetting, IInstaChatModel, IGeneralSettings
//    {
        
//        #region Variables

//        private int _increaseMessageCount;
//        private int _messageCountUntil;
//        private RangeUtilities _messageBetweenJobs= new RangeUtilities();
//        private RangeUtilities _messageMaxBetween = new RangeUtilities();
//        private bool _isChkStopMessageToolWhenReachChecked;
//        private bool _isChkMessageToolGetsTemporaryBlockedChecked;
//        private RangeUtilities _increaseEachDayMessage = new RangeUtilities();
//        private string _uploadMessage;
//        private List<string> _lstMessage = new List<string>();
//        private string _message;

//        #endregion        

//        public class QueryTypeWithTitle
//        {
//            public string QueryType { get; set; }
//            public string QueryDisplayName()
//            {
//                var value = (Enums.UserQueryParameters)Enum.Parse(typeof(Enums.UserQueryParameters), QueryType);

//                string description = value.GetDescriptionAttr().FromResourceDictionary();
//                return description;
//            }

//            //public QueryTypeWithTitle(string queryType)
//            //{
//            //    QueryType = queryType;
//            //}

//            public override string ToString()
//            {
//                return QueryDisplayName();
//            }
//        }


//        public List<string> ListQueryType { get; set; }


//        //public InstaChatModel()
//        //{
//        //    ListQueryType = Enum.GetNames(typeof(Enums.UserQueryParameters)).ToList();
//        //    ListQueryType.Remove("HashtagPost");
//        //    ListQueryType.Remove("LocationPosts");
//        //    ListQueryType.Remove("CustomPhotos");

//        //}

//        [ProtoMember(2)]
//        public override UserFilterModel UserFilterModel { get; set; } = new UserFilterModel();
        
//        [ProtoMember(3)]
//        JobConfiguration IGeneralSettings.JobConfiguration { get; set; }


//        #region IInstaChatModel


//        [ProtoMember(4)]
//        public int IncreaseMessageCount
//        {
//            get
//            {
//                return _increaseMessageCount;
//            }
//            set
//            {
//                if (value == _increaseMessageCount)
//                    return;
//                SetProperty(ref _increaseMessageCount, value);
//            }
//        }

//        [ProtoMember(5)]
//        public int MessageCountUntil
//        {
//            get
//            {
//                return _messageCountUntil;
//            }
//            set
//            {
//                if (value == _messageCountUntil)
//                    return;
//                SetProperty(ref _messageCountUntil, value);
//            }
//        }

//        [ProtoMember(6)]
//        public RangeUtilities MessageBetweenJobs
//        {
//            get
//            {
//                return _messageBetweenJobs;
//            }
//            set
//            {
//                if (value == _messageBetweenJobs)
//                    return;
//                SetProperty(ref _messageBetweenJobs, value);
//            }
//        }

//        [ProtoMember(7)]
//        public RangeUtilities MessageMaxBetween
//        {
//            get { return _messageMaxBetween; }

//            set
//            {
//                if (value == _messageMaxBetween)
//                    return;
//                SetProperty(ref _messageMaxBetween, value);
//            }
//        }
        

//        [ProtoMember(8)]
//        public bool IsChkStopMessageToolWhenReachChecked
//        {
//            get
//            {
//                return _isChkStopMessageToolWhenReachChecked;
//            }

//            set
//            {
//                if (value == _isChkStopMessageToolWhenReachChecked)
//                    return;
//                SetProperty(ref _isChkStopMessageToolWhenReachChecked, value);
//            }
//        }
        

//        [ProtoMember(9)]
//        public bool IsChkMessageToolGetsTemporaryBlockedChecked
//        {
//            get
//            {
//                return _isChkMessageToolGetsTemporaryBlockedChecked;
//            }

//            set
//            {
//                if (value == _isChkMessageToolGetsTemporaryBlockedChecked)
//                    return;
//                SetProperty(ref _isChkMessageToolGetsTemporaryBlockedChecked, value);
//            }
//        }


//        [ProtoMember(10)]
//        public RangeUtilities IncreaseEachDayMessage
//        {
//            get
//            {
//                return _increaseEachDayMessage;
//            }

//            set
//            {
//                if (value == _increaseEachDayMessage)
//                    return;
//                SetProperty(ref _increaseEachDayMessage, value);
//            }
//        }
        
//        [ProtoMember(11)]
//        public string UploadMessage
//        {
//            get
//            {
//                return _uploadMessage;
//            }
//            set
//            {
//                if (_uploadMessage == value)
//                    return;
//                SetProperty(ref _uploadMessage, value);
//            }
//        }
        
//        [ProtoMember(12)]
//        public List<string> LstMessage
//        {
//            get
//            {
//                return _lstMessage;
//            }
//            set
//            {
//                if (value == _lstMessage)
//                    return;
//                SetProperty(ref _lstMessage, value);
//            }
//        }
        
//        [ProtoIgnore]
//        public string Message
//        {
//            get { return _message; }
//            set
//            {
//                if (_message == value)
//                    return;
//                SetProperty(ref _message, value);
//            }
//        }

//        #endregion

//    }
//}
