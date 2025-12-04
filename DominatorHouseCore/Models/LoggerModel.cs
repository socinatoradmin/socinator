#region

using System;
using DominatorHouseCore.Utility;

#endregion

namespace DominatorHouseCore.Models
{
    public class LoggerModel : BindableBase
    {
        private string _dateTime;

        public string DateTime
        {
            get => _dateTime;
            set
            {
                if (value == _dateTime)
                    return;
                SetProperty(ref _dateTime, value);
            }
        }

        private string _network;

        public string Network
        {
            get => _network;
            set
            {
                if (_network != null && value == _network)
                    return;
                SetProperty(ref _network, value);
            }
        }

        private string _activityType;

        public string ActivityType
        {
            get => _activityType;
            set
            {
                if (_activityType != null && value == _activityType)
                    return;
                SetProperty(ref _activityType, value);
            }
        }

        private string _accountCampaign;

        public string AccountCampaign
        {
            get => _accountCampaign;
            set
            {
                if (_accountCampaign != null && value == _accountCampaign)
                    return;
                SetProperty(ref _accountCampaign, value);
            }
        }

        private string _message;

        public string Message
        {
            get => _message;
            set
            {
                if (_message != null && value == _message)
                    return;
                SetProperty(ref _message, value);
            }
        }

        private string _logType = "Info";

        public string LogType
        {
            get => _logType;
            set
            {
                if (_logType != null && value == _logType)
                    return;
                SetProperty(ref _logType, value);
            }
        }

        private string _messageCode;

        public string MessageCode
        {
            get => _messageCode;
            set
            {
                if (_messageCode != null && value == _messageCode)
                    return;
                SetProperty(ref _messageCode, value);
            }
        }
    }
}