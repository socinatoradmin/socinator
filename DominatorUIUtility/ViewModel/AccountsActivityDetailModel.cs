using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;

namespace DominatorUIUtility.ViewModel
{
    public class AccountsActivityDetailModel : BindableBase
    {
        private string _showMoreButtonText = "LangKeyMore".FromResourceDictionary();
        public string AccountName { get; set; }

        public string AccountId { get; set; }

        public SocialNetworks AccountNetwork { get; set; }

        public ObservableCollection<ActivityDetailsModel> ActivityDetailsCollections { get; set; }

        public string ShowMoreButtonText
        {
            get => _showMoreButtonText;
            set => SetProperty(ref _showMoreButtonText, value);
        }
    }

    public class ActivityDetailsModel : BindableBase
    {
        private string _accountId = string.Empty;
        private string _activityTitle;

        private bool _status;

        private ActivityType _title;

        public string AccountId
        {
            get => _accountId;
            set
            {
                if (_accountId == value)
                    return;
                SetProperty(ref _accountId, value);
            }
        }

        public ActivityType Title
        {
            get => _title;
            set
            {
                if (_title == value)
                    return;
                SetProperty(ref _title, value);
            }
        }

        public string ActivityTitle
        {
            get
            {
                _activityTitle = Regex.Replace(Title.ToString(), "(\\B[A-Z])", " $1");
                return _activityTitle;
            }
            set
            {
                if (_activityTitle == value)
                    return;
                SetProperty(ref _activityTitle, value);
            }
        }

        public bool Status
        {
            get => _status;
            set
            {
                if (_status == value)
                    return;
                SetProperty(ref _status, value);
            }
        }
    }
}