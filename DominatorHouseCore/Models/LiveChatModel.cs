#region

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;
using ProtoBuf;

#endregion

namespace DominatorHouseCore.Models
{
    [ProtoContract]
    public class LiveChatModel : BindableBase
    {
        private ObservableCollection<SenderDetails> _lstSender = new ObservableCollection<SenderDetails>();

        [ProtoMember(1)]
        public ObservableCollection<SenderDetails> LstSender
        {
            get => _lstSender;
            set
            {
                if (value == _lstSender)
                    return;
                SetProperty(ref _lstSender, value);
            }
        }


        private Dictionary<string, ObservableCollection<ChatDetails>> _accountChatDetails =
            new Dictionary<string, ObservableCollection<ChatDetails>>();

        [ProtoMember(2)]
        public Dictionary<string, ObservableCollection<ChatDetails>> AccountChatDetails
        {
            get => _accountChatDetails;
            set
            {
                if (value == _accountChatDetails)
                    return;
                SetProperty(ref _accountChatDetails, value);
            }
        }


        private ObservableCollection<ChatDetails> _lstChat = new ObservableCollection<ChatDetails>();

        public ObservableCollection<ChatDetails> LstChat
        {
            get => _lstChat;
            set
            {
                if (value == _lstChat)
                    return;
                SetProperty(ref _lstChat, value);
            }
        }

        private ObservableCollection<string> _lstImages = new ObservableCollection<string>();

        public ObservableCollection<string> LstImages
        {
            get => _lstImages;
            set
            {
                if (value == _lstImages)
                    return;
                SetProperty(ref _lstImages, value);
            }
        }

        private int _imageCount;

        public int ImageCount
        {
            get => _imageCount;
            set
            {
                if (value == _imageCount)
                    return;
                SetProperty(ref _imageCount, value);
            }
        }


        private SenderDetails _senderDetails = new SenderDetails();

        [ProtoMember(3)]
        public SenderDetails SenderDetails
        {
            get => _senderDetails;
            set
            {
                if (value == _senderDetails)
                    return;
                SetProperty(ref _senderDetails, value);
            }
        }


        [ProtoMember(4)] public string SenderDetailsCursorId { get; set; }

        public DominatorAccountModel DominatorAccountModel { get; set; } = new DominatorAccountModel();


        private ObservableCollection<string> _accountNames = new ObservableCollection<string>();

        [ProtoIgnore]
        public ObservableCollection<string> AccountNames
        {
            get => _accountNames;
            set => SetProperty(ref _accountNames, value);
        }


        private string _selectedAccount = string.Empty;

        public string SelectedAccount
        {
            get => _selectedAccount;
            set
            {
                if (_selectedAccount == value)
                    return;
                SetProperty(ref _selectedAccount, value);
            }
        }


        private string _textMessage = string.Empty;

        public string TextMessage
        {
            get => _textMessage;
            set
            {
                if (_textMessage == value)
                    return;
                SetProperty(ref _textMessage, value);
            }
        }
    }


    [ProtoContract]
    public class SenderDetails : BindableBase
    {
        private string _senderName;

        [ProtoMember(1)]
        public string SenderName
        {
            get => _senderName;
            set
            {
                if (value == _senderName)
                    return;
                SetProperty(ref _senderName, value);
            }
        }

        private string _senderImage;

        [ProtoMember(2)]
        public string SenderImage
        {
            get => _senderImage;
            set
            {
                if (value == _senderImage)
                    return;
                SetProperty(ref _senderImage, value);
            }
        }

        private string _lastMessegedate;

        [ProtoMember(3)]
        public string LastMessegedate
        {
            get => _lastMessegedate;
            set
            {
                if (value == _lastMessegedate)
                    return;
                SetProperty(ref _lastMessegedate, value);
                try
                {
                    if (_lastMessegedate.Length > 12)
                        LastMessegeDateTime = long.Parse(_lastMessegedate.Split('.')[0]).EpochToDateTimeLocal();
                    else
                        LastMessegeDateTime = int.Parse(_lastMessegedate.Split('.')[0]).EpochToDateTimeLocal();
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            }
        }

        private string _lastMesseges;

        [ProtoMember(4)]
        public string LastMesseges
        {
            get => _lastMesseges;
            set
            {
                if (value == _lastMesseges)
                    return;
                SetProperty(ref _lastMesseges, value);
            }
        }

        [ProtoMember(5)] public string ThreadId { get; set; }

        [ProtoMember(6)] public string LastMessageOwnerId { get; set; }

        [ProtoMember(7)] public string SenderId { get; set; }

        [ProtoMember(8)] public string AccountId { get; set; } = string.Empty;

        public bool MoreAvailableMax { get; set; }

        public bool MoreAvailableMin { get; set; }

        public string NextMaxId { get; set; }
        public string NextMinId { get; set; }

        private DateTime _lastMessegeDateTime;

        [ProtoMember(9)]
        public DateTime LastMessegeDateTime
        {
            get => _lastMessegeDateTime;
            set
            {
                if (value == _lastMessegeDateTime)
                    return;
                SetProperty(ref _lastMessegeDateTime, value);
            }
        }
    }

    [ProtoContract]
    public class ChatDetails : BindableBase
    {
        private string _sender;

        [ProtoMember(1)]
        public string Sender
        {
            get => _sender;
            set
            {
                if (value == _sender)
                    return;
                SetProperty(ref _sender, value);
            }
        }

        private string _messeges;

        [ProtoMember(2)]
        public string Messeges
        {
            get => _messeges;
            set
            {
                if (value == _messeges)
                    return;
                SetProperty(ref _messeges, value);
            }
        }


        private string _type;

        [ProtoMember(3)]
        public string Type
        {
            get => _type;
            set
            {
                if (value == _type)
                    return;
                SetProperty(ref _type, value);
            }
        }

        private string _time;

        [ProtoMember(4)]
        public string Time
        {
            get => _time;
            set
            {
                if (value == _time)
                    return;
                SetProperty(ref _time, value);
                try
                {
                    if (Utilities.GetIntegerOnlyString(Time) != Time)
                        MessageTime = DateTime.Parse(Time);
                    else
                        MessageTime = long.Parse(Time).EpochToDateTimeLocal();
                }
                catch (Exception)
                {
                }
            }
        }

        [ProtoMember(5)] public string MessegesId { get; set; }

        [ProtoMember(6)] public string SenderId { get; set; }


        private bool _IsRecieved;

        public bool IsRecieved
        {
            get => _IsRecieved;
            set
            {
                if (value == _IsRecieved) return;
                _IsRecieved = value;
                OnPropertyChanged();
            }
        }

        private string _clientContext;

        public string ClientContext
        {
            get => _clientContext;
            set
            {
                if (value == _clientContext) return;
                _clientContext = value;
                OnPropertyChanged();
            }
        }

        private ChatMessageType _messegeType;

        [ProtoMember(7)]
        public ChatMessageType MessegeType
        {
            get => _messegeType;
            set
            {
                if (value == _messegeType)
                    return;
                SetProperty(ref _messegeType, value);
            }
        }


        private ObservableCollection<string> _listMediaUrls;

        [ProtoMember(8)]
        public ObservableCollection<string> ListMediaUrls
        {
            get => _listMediaUrls;
            set
            {
                if (value == _listMediaUrls)
                    return;
                SetProperty(ref _listMediaUrls, value);
            }
        }

        private DateTime _messageTime;

        [ProtoMember(9)]
        public DateTime MessageTime
        {
            get => _messageTime;
            set => SetProperty(ref _messageTime, value);
        }
    }


    public class AllignmentConvertor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null && (bool) value ? HorizontalAlignment.Right : HorizontalAlignment.Left;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null && (HorizontalAlignment) value == HorizontalAlignment.Right;
        }
    }
}