#region

using System;
using System.Collections.ObjectModel;
using DominatorHouseCore.Utility;
using ProtoBuf;

#endregion

namespace DominatorHouseCore.Models.SocioPublisher
{
    [Serializable]
    [ProtoContract]
    public class PublisherMediaViewerModel : BindableBase, IDisposable
    {
        private ObservableCollection<string> _mediaList = new ObservableCollection<string>();
        private int _currentItem;
        private int _totalItem;
        private string _mediaUrl = string.Empty;
        private bool _isEnableGoNext;
        private bool _isEnablePreviousNext;
        private bool _isPostDetailsPresent;

        /// <summary>
        ///     To specify the all media items
        /// </summary>
        [ProtoMember(1)]
        public ObservableCollection<string> MediaList
        {
            get => _mediaList;
            set
            {
                if (_mediaList == value)
                    return;
                _mediaList = value;
                if (_mediaList.Count <= 0) return;
                TotalItem = _mediaList.Count;
                CurrentItem = 1;
                MediaUrl = _mediaList[CurrentItem - 1];
                IsEnablePreviousNext = false;
                IsEnableGoNext = _mediaList.Count > 1;
                IsPostDetailsPresent = true;
            }
        }

        /// <summary>
        ///     To specify the current item index with respective to media lists
        /// </summary>
        public int CurrentItem
        {
            get => _currentItem;
            set
            {
                if (_currentItem == value)
                    return;
                _currentItem = value;
                OnPropertyChanged(nameof(CurrentItem));
            }
        }

        /// <summary>
        ///     To specify the total item index with respective to media lists
        /// </summary>
        public int TotalItem
        {
            get => _totalItem;
            set
            {
                if (_totalItem == value)
                    return;
                _totalItem = value;
                OnPropertyChanged(nameof(TotalItem));
            }
        }

        /// <summary>
        ///     To hold current media item
        /// </summary>
        public string MediaUrl
        {
            get => _mediaUrl;
            set
            {
                if (_mediaUrl.Equals(value))
                    return;
                _mediaUrl = value;
                OnPropertyChanged(nameof(MediaUrl));
            }
        }

        /// <summary>
        ///     To specify whether button enable for go to next
        /// </summary>
        public bool IsEnableGoNext
        {
            get => _isEnableGoNext;
            set
            {
                if (_isEnableGoNext == value)
                    return;
                _isEnableGoNext = value;
                OnPropertyChanged(nameof(IsEnableGoNext));
            }
        }

        /// <summary>
        ///     To specify whether button enable for go to previous
        /// </summary>
        public bool IsEnablePreviousNext
        {
            get => _isEnablePreviousNext;
            set
            {
                if (_isEnablePreviousNext == value)
                    return;
                _isEnablePreviousNext = value;
                OnPropertyChanged(nameof(IsEnablePreviousNext));
            }
        }

        /// <summary>
        ///     To Specify whether post detail are present or not
        /// </summary>
        public bool IsPostDetailsPresent
        {
            get => _isPostDetailsPresent;
            set
            {
                if (_isPostDetailsPresent == value)
                    return;
                _isPostDetailsPresent = value;
                OnPropertyChanged(nameof(IsPostDetailsPresent));
            }
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}