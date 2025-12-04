using System;
using System.Linq;
using System.Windows;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Models.FacebookModels;
using DominatorHouseCore.Utility;
using Prism.Commands;
using Prism.Regions;

namespace DominatorUIUtility.ViewModel.Startup.ModuleConfig
{
    public interface IUnfriendViewModel
    {
        //int Count { get; set; }
        //int TypeCount { get; set; }
        //List<string> LstFilterText { get; set; }
        //string FilterText { get; set; }
        UnfriendOption UnfriendOptionModel { get; set; }
    }

    public class UnfriendViewModel : StartupBaseViewModel, IUnfriendViewModel
    {
        //public ICommand SaveCommandBinding { get; set; }

        //public ICommand SelectOptionCommandBinding { get; set; }

        //private void LoadTextBoxes()
        //{
        //    if (LstSelectedInput != null)
        //    {
        //        foreach (var str in LstSelectedInput)
        //        {
        //            InputText = InputText + str + "\r\n";
        //        }
        //    }
        //}
        //private void SelectOptionCommandExecute(object obj)
        //{

        //}
        //private void UserInputOnSaveExecute(object sender)
        //{

        //    if (!string.IsNullOrEmpty(InputText))
        //    {
        //        LstSelectedInput = Regex.Split(InputText, "\r\n").ToList();
        //    }
        //    else
        //    {
        //        Dialog.ShowDialog("Error", "There is no data to save.");
        //    }

        //}
        //private string _inputText;
        //public string InputText
        //{
        //    get { return _inputText; }
        //    set { SetProperty(ref _inputText, value); }
        //}
        //private IEnumerable<string> _lstSelectedInput;

        //public IEnumerable<string> LstSelectedInput
        //{
        //    get { return _lstSelectedInput; }
        //    set { SetProperty(ref _lstSelectedInput, value); }
        //}

        private UnfriendOption _unfriendOptionModel;

        public UnfriendViewModel(IRegionManager region) : base(region)
        {
            ViewModelToSave.Add(new ActivityConfig {Model = this, ActivityType = ActivityType.Unfriend});
            IsNonQuery = true;
            NextCommand = new DelegateCommand(UnfriendValidate);
            PreviousCommand = new DelegateCommand(NavigatePrevious);
            LoadedCommand = new DelegateCommand<string>(OnLoad);

            UnfriendOptionModel = new UnfriendOption
            {
                SourceDisplayName = Application.Current.FindResource("LangKeyUnfriendSource")?.ToString(),
                BySoftwareDisplayName = Application.Current.FindResource("LangKeyPeopleAddedBySoftware")?.ToString(),
                OutsideSoftwareDisplayName =
                    Application.Current.FindResource("LangKeyPeopleAddedOutsideSoftware")?.ToString()
            };

            //LoadTextBoxes();


            JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = "LangKeyNumberOfUnfriendPerJob".FromResourceDictionary(),
                ActivitiesPerHourDisplayName = "LangKeyNumberOfUnfriendPerHour".FromResourceDictionary(),
                ActivitiesPerDayDisplayName = "LangKeyNumberOfUnfriendPerDay".FromResourceDictionary(),
                ActivitiesPerWeekDisplayName = "LangKeyNumberOfUnfriendPerWeek".FromResourceDictionary(),
                IncreaseActivityDisplayName = "LangKeyMaxUnfriendPerDay".FromResourceDictionary(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };
            ListQueryType.Clear();
        }

        public UnfriendOption UnfriendOptionModel
        {
            get => _unfriendOptionModel;
            set => SetProperty(ref _unfriendOptionModel, value);
        }

        private void UnfriendValidate()
        {
            if (!UnfriendOptionModel.IsAddedThroughSoftware && !UnfriendOptionModel.IsAddedOutsideSoftware
                                                            && !UnfriendOptionModel.IsCustomUserList
                                                            && !UnfriendOptionModel.IsMutualFriends)
            {
                Dialog.ShowDialog("Error", "Please select atleast one source.");
                return;
            }

            if (UnfriendOptionModel.IsFilterApplied && UnfriendOptionModel.DaysBefore == 0 &&
                UnfriendOptionModel.HoursBefore == 0)
            {
                Dialog.ShowDialog("Error", "Please select valid source filter.");
                return;
            }

            NavigateNext();
        }

        //private bool _isAddedOutsideSoftware;
        //public bool IsAddedOutsideSoftware
        //{
        //    get
        //    {
        //        return _isAddedOutsideSoftware;
        //    }
        //    set
        //    {
        //        SetProperty(ref _isAddedOutsideSoftware, value);
        //    }
        //}

        //private bool _isFilterApplied;
        //public bool IsFilterApplied
        //{
        //    get
        //    {
        //        return _isFilterApplied;
        //    }

        //    set
        //    {
        //        SetProperty(ref _isFilterApplied, value);
        //    }
        //}

        //private int _daysBefore;
        //public int DaysBefore
        //{
        //    get
        //    {
        //        return _daysBefore;
        //    }
        //    set
        //    {
        //        SetProperty(ref _daysBefore, value);
        //    }
        //}


        //private int _hoursBefore;
        //public int HoursBefore
        //{
        //    get
        //    {
        //        return _hoursBefore;
        //    }

        //    set
        //    {
        //        SetProperty(ref _hoursBefore, value);
        //    }
        //}

        //private int _count;
        //public int Count
        //{
        //    get
        //    {
        //        return _count;
        //    }

        //    set
        //    {
        //        SetProperty(ref _count, value);
        //    }
        //}

        //int _typeCount;
        //public int TypeCount
        //{
        //    get
        //    {
        //        return _typeCount;
        //    }

        //    set
        //    {
        //        SetProperty(ref _typeCount, value);
        //    }
        //}

        //private string _filterText = string.Empty;
        //public string FilterText
        //{
        //    get
        //    {
        //        return _filterText;
        //    }

        //    set
        //    {
        //        SetProperty(ref _filterText, value);
        //    }
        //}
        //private List<string> _lstFilterText = new List<string>();
        //public List<string> LstFilterText
        //{
        //    get
        //    {
        //        return _lstFilterText;
        //    }

        //    set
        //    {
        //        SetProperty(ref _lstFilterText, value);
        //    }
        //}

        //private string _customUserText;
        //public string CustomUserText
        //{
        //    get
        //    {
        //        return _customUserText;
        //    }

        //    set
        //    {
        //        SetProperty(ref _customUserText, value);
        //    }
        //}

        //private List<string> _lstCustomUsers;
        //public List<string> LstCustomUsers
        //{
        //    get
        //    {
        //        return _lstCustomUsers;
        //    }

        //    set
        //    {
        //        SetProperty(ref _lstCustomUsers, value);
        //    }
        //}

        //private bool _isCustomUserList = false;
        //public bool IsCustomUserList
        //{
        //    get
        //    {
        //        return _isCustomUserList;
        //    }

        //    set
        //    {
        //        SetProperty(ref _isCustomUserList, value);
        //    }
        //}

        //private bool _isMutualFriends;
        //public bool IsMutualFriends
        //{
        //    get
        //    {
        //        return _isMutualFriends;
        //    }

        //    set
        //    {
        //        SetProperty(ref _isMutualFriends, value);
        //    }
        //}
    }
}