using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using DominatorHouseCore;
using DominatorHouseCore.Command;
using DominatorHouseCore.Enums;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using LinkedDominatorCore.LDModel;

namespace LinkedDominatorCore.LDViewModel.Group
{
    public class GroupUnJoinerViewModel : BindableBase
    {
        private GroupUnJoinerModel _GroupUnJoinerModel = new GroupUnJoinerModel();

        public GroupUnJoinerViewModel()
        {
            GroupUnJoinerModel.JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName =
                    Application.Current.FindResource("LangKeyNumberOfGroupUnjoinsPerJob")?.ToString(),
                ActivitiesPerHourDisplayName =
                    Application.Current.FindResource("LangKeyNumberOfGroupUnjoinsPerHour")?.ToString(),
                ActivitiesPerDayDisplayName =
                    Application.Current.FindResource("LangKeyNumberOfGroupUnjoinsPerDay")?.ToString(),
                ActivitiesPerWeekDisplayName =
                    Application.Current.FindResource("LangKeyNumberOfGroupUnjoinsPerWeek")?.ToString(),
                IncreaseActivityDisplayName =
                    Application.Current.FindResource("LangKeyMaxGroupUnjoinsPerDay")?.ToString(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };
            SaveCommand = new BaseCommand<object>(sender => true, Save);
        }

        public GroupUnJoinerModel GroupUnJoinerModel
        {
            get => _GroupUnJoinerModel;
            set
            {
                if ((_GroupUnJoinerModel == null) & (_GroupUnJoinerModel == value))
                    return;
                SetProperty(ref _GroupUnJoinerModel, value);
            }
        }

        public GroupUnJoinerModel Model => GroupUnJoinerModel;

        public ICommand SaveCommand { get; set; }

        public void Save(object sender)
        {
            try
            {
                if (GroupUnJoinerModel.UrlInput.Contains("\r\n"))
                {
                    GroupUnJoinerModel.UrlList =
                        Regex.Split(GroupUnJoinerModel.UrlInput, "\r\n").ToList();
                    GlobusLogHelper.log.Info(GroupUnJoinerModel.UrlList.Count + "Group urls saved sucessfully");
                }
                else
                {
                    try
                    {
                        GroupUnJoinerModel.UrlList = new List<string>{GroupUnJoinerModel.UrlInput};
                        GlobusLogHelper.log.Info("One Group url saved sucessfully");
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}