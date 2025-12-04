using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Input;
using DominatorHouseCore;
using DominatorHouseCore.Command;
using DominatorHouseCore.Enums;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using Prism.Commands;
using Prism.Regions;

namespace DominatorUIUtility.ViewModel.Startup.ModuleConfig
{
    public interface IBlockUserViewModel
    {
        string UrlInput { get; set; }
        ICommand SaveCustomUserListCommand { get; set; }
        List<string> UrlList { get; set; }
    }

    public class BlockUserViewModel : StartupBaseViewModel, IBlockUserViewModel
    {
        private string _UrlInput;
        private List<string> _urlList;

        public BlockUserViewModel(IRegionManager region) : base(region)
        {
            ViewModelToSave.Add(new ActivityConfig {Model = this, ActivityType = ActivityType.BlockUser});
            SaveCustomUserListCommand = new BaseCommand<object>(sender => true, SaveCustomUsers);
            NextCommand = new DelegateCommand(ValidateAndNevigate);
            PreviousCommand = new DelegateCommand(NavigatePrevious);
            LoadedCommand = new DelegateCommand<string>(OnLoad);
            IsNonQuery = true;
            JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = "LangKeyNumberOfUsersToBlockPerJob".FromResourceDictionary(),
                ActivitiesPerHourDisplayName = "LangKeyNumberOfUsersToBlockPerHour".FromResourceDictionary(),
                ActivitiesPerDayDisplayName = "LangKeyNumberOfUsersToBlockPerDay".FromResourceDictionary(),
                ActivitiesPerWeekDisplayName = "LangKeyNumberOfUsersToBlockPerWeek".FromResourceDictionary(),
                IncreaseActivityDisplayName = "LangKeyMaxUsersToBlockPerDay".FromResourceDictionary(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };
        }

        public string UrlInput
        {
            get => _UrlInput;
            set => SetProperty(ref _UrlInput, value);
        }

        public ICommand SaveCustomUserListCommand { get; set; }

        public List<string> UrlList
        {
            get => _urlList;
            set => SetProperty(ref _urlList, value);
        }

        private void ValidateAndNevigate()
        {
            if (string.IsNullOrEmpty(UrlInput?.Trim()))
            {
                Dialog.ShowDialog("Error", "please add profile url(s).");
                return;
            }

            NavigateNext();
        }

        private void SaveCustomUsers(object sender)
        {
            try
            {
                if (UrlInput.Contains("\r\n"))
                {
                    UrlList =
                        Regex.Split(UrlInput, "\r\n").Where(x => !string.IsNullOrEmpty(x.Trim())).Distinct().ToList();
                    GlobusLogHelper.log.Info("" + UrlList.Count + " profile urls saved sucessfully");
                }
                else
                {
                    UrlList = new List<string> {UrlInput};
                    GlobusLogHelper.log.Info("One profile url saved sucessfully");
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}