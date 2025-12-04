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
using LinkedDominatorCore.LDModel.GrowConnection;

namespace LinkedDominatorCore.LDViewModel.GrowConnection
{
    public class RemoveConnectionViewModel : BindableBase
    {
        private RemoveConnectionModel _RemoveConnectionModel = new RemoveConnectionModel();

        public RemoveConnectionViewModel()
        {
            RemoveConnectionModel.JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = Application.Current
                    .FindResource("LangKeyNumberOfConnectionsToRemovePerJob")?.ToString(),
                ActivitiesPerHourDisplayName = Application.Current
                    .FindResource("LangKeyNumberOfConnectionsToRemovePerHour")?.ToString(),
                ActivitiesPerDayDisplayName = Application.Current
                    .FindResource("LangKeyNumberOfConnectionsToRemovePerDay")?.ToString(),
                ActivitiesPerWeekDisplayName = Application.Current
                    .FindResource("LangKeyNumberOfConnectionsToRemovePerWeek")?.ToString(),
                IncreaseActivityDisplayName =
                    Application.Current.FindResource("LangKeyMaxConnectionsToRemovePerDay")?.ToString(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };
            SaveCustomUserListCommand = new BaseCommand<object>(sender => true, SaveCustomUsers);
        }

        public RemoveConnectionModel RemoveConnectionModel
        {
            get => _RemoveConnectionModel;
            set
            {
                if ((_RemoveConnectionModel == null) & (_RemoveConnectionModel == value))
                    return;
                SetProperty(ref _RemoveConnectionModel, value);
            }
        }

        public RemoveConnectionModel Model => RemoveConnectionModel;

        public ICommand SaveCustomUserListCommand { get; set; }

        private void SaveCustomUsers(object sender)
        {
            try
            {
                if (RemoveConnectionModel.UrlInput.Contains("\r\n"))
                {
                    RemoveConnectionModel.UrlList =
                        Regex.Split(RemoveConnectionModel.UrlInput, "\r\n").ToList();
                    GlobusLogHelper.log.Info("" + RemoveConnectionModel.UrlList.Count +
                                             " profile urls saved sucessfully");
                }
                else
                {
                    try
                    {
                        RemoveConnectionModel.UrlList = new List<string>() { RemoveConnectionModel.UrlInput };
                        GlobusLogHelper.log.Info("One profile url saved sucessfully");
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