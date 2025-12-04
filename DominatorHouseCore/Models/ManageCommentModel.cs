#region

using System.Collections.ObjectModel;
using DominatorHouseCore.Utility;
using ProtoBuf;

#endregion

namespace DominatorHouseCore.Models
{
    [ProtoContract]
    public class ManageCommentModel : BindableBase
    {
        public ManageCommentModel()
        {
            CommentId = Utilities.GetGuid();
        }

        private string _commentId;

        public string CommentId
        {
            get => _commentId;
            set
            {
                if (value == _commentId)
                    return;
                SetProperty(ref _commentId, value);
            }
        }

        private string _commentText;

        public string CommentText
        {
            get => _commentText;
            set
            {
                if (value == _commentText)
                    return;
                SetProperty(ref _commentText, value);
            }
        }

        private string _filterText;

        public string FilterText
        {
            get => _filterText;
            set
            {
                if (value == _filterText)
                    return;
                SetProperty(ref _filterText, value);
            }
        }

        private ObservableCollection<QueryContent> _selectedQuery = new ObservableCollection<QueryContent>();

        public ObservableCollection<QueryContent> SelectedQuery
        {
            get => _selectedQuery;
            set => SetProperty(ref _selectedQuery, value);
        }

        private ObservableCollection<QueryContent> _lstQueries = new ObservableCollection<QueryContent>();

        public ObservableCollection<QueryContent> LstQueries
        {
            get => _lstQueries;
            set => SetProperty(ref _lstQueries, value);
        }

        public string _mediaPath = string.Empty;
        public string MediaPath
        {
            get => _mediaPath;
            set
            {
                if (value == _mediaPath)
                    return;
                SetProperty(ref _mediaPath, value);
            }
        }

        private ObservableCollection<string> _mediaList = new ObservableCollection<string>();
        public ObservableCollection<string> MediaList
        {
            get => _mediaList;
            set
            {
                if (value == _mediaList)
                    return;
                SetProperty(ref _mediaList, value);
            }
        }
    }

    [ProtoContract]
    public class QueryContent : BindableBase
    {
        private QueryInfo _content;

        /// <summary>
        ///     Provide the content
        /// </summary>
        [ProtoMember(1)]
        public QueryInfo Content
        {
            get => _content;
            set
            {
                if (_content != null && value == _content)
                    return;
                SetProperty(ref _content, value);
            }
        }


        private bool _isContentSelected;

        /// <summary>
        ///     IsContentSelected is used to give the status whether the content is selected or not
        /// </summary>
        [ProtoMember(2)]
        public bool IsContentSelected
        {
            get => _isContentSelected;
            set
            {
                if (value == _isContentSelected)
                    return;
                SetProperty(ref _isContentSelected, value);
            }
        }
    }
}