using DominatorHouseCore.Utility;
using System.Collections.Generic;
using System.Linq;

namespace TumblrDominatorCore.Models
{
    public class SearchFilterModel : BindableBase
    {
        public SearchFilterModel()
        {
            _timeList = _timeList ?? new List<string> { "All Time", "Last year", "Last 6 months", "Last month", "Last week", "Today" };
            _postTypeList = _postTypeList ?? new List<string> { "All posts", "Text", "Photo", "GIF", "Quote", "Link", "Chat", "Audio", "Video", "Ask", "Poll" };
            SelectedTime = SelectedTime ?? _timeList.FirstOrDefault();
            SelectedPostType = SelectedPostType ?? _postTypeList.FirstOrDefault();
            _isCheckTop = true;
        }
        private bool _isCheckTop;
        private bool _isCheckLatest;
        private string _selectedTime;
        private string _selectedPostType;
        private List<string> _timeList;
        private List<string> _postTypeList;
        public bool IsCheckTop
        {
            get => _isCheckTop;
            set => SetProperty(ref _isCheckTop, value);
        }
        public bool IsCheckLatest
        {
            get => _isCheckLatest;
            set => SetProperty(ref _isCheckLatest, value);
        }
        public string SelectedTime
        {
            get => _selectedTime;
            set => SetProperty(ref _selectedTime, value);
        }
        public string SelectedPostType
        {
            get => _selectedPostType;
            set => SetProperty(ref _selectedPostType, value);
        }
        public List<string> TimeList
        {
            get => _timeList?.Distinct()?.ToList();
            set => SetProperty(ref _timeList, value);
        }
        public List<string> PostTypeList
        {
            get => _postTypeList?.Distinct()?.ToList();
            set => SetProperty(ref _postTypeList, value);
        }
    }
}
