#region

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using DominatorHouseCore.Utility;
using ProtoBuf;

#endregion

namespace DominatorHouseCore.Models
{
    [ProtoContract]
    public class ManageMessagesModel : BindableBase
    {
        public ManageMessagesModel()
        {
            MessageId = Utilities.GetGuid();
        }

        private string _messageId;

        public string MessageId
        {
            get => _messageId;
            set
            {
                if (value == _messageId)
                    return;
                SetProperty(ref _messageId, value);
            }
        }

        private string _messagesText;

        public string MessagesText
        {
            get => _messagesText;
            set
            {
                if (value == _messagesText)
                    return;
                SetProperty(ref _messagesText, value);
            }
        }

        public string _MediaPath = string.Empty;
        private ObservableCollection<QueryContent> _lstQueries = new ObservableCollection<QueryContent>();
        private ObservableCollection<QueryContent> _selectedQuery = new ObservableCollection<QueryContent>();

        public string MediaPath
        {
            get => _MediaPath;
            set
            {
                if (value == _MediaPath)
                    return;
                SetProperty(ref _MediaPath, value);
            }
        }
        private Visibility _ShowMediaControl = Visibility.Visible;

        public Visibility ShowMediaControl
        {
            get=> _ShowMediaControl;
            set=>SetProperty(ref _ShowMediaControl, value);
        }
        private ObservableCollection<MessageMediaInfo> _Medias = new ObservableCollection<MessageMediaInfo>();
        public ObservableCollection<MessageMediaInfo> Medias
        {
            get=> _Medias;
            set=>SetProperty(ref _Medias, value);
        }
        public ObservableCollection<QueryContent> SelectedQuery
        {
            get => _selectedQuery;
            set
            {
                if (value == _selectedQuery)
                    return;
                SetProperty(ref _selectedQuery, value);
            }
        }

        [ProtoMember(1)]
        public ObservableCollection<QueryContent> LstQueries
        {
            get => _lstQueries;
            set
            {
                if (value == _lstQueries)
                    return;
                SetProperty(ref _lstQueries, value);
            }
        }
    }
}