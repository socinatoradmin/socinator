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
    public class WithdrawConnectionRequestViewModel : BindableBase
    {
        private WithdrawConnectionRequestModel _WithdrawConnectionRequestModel = new WithdrawConnectionRequestModel();

        public WithdrawConnectionRequestViewModel()
        {
            WithdrawConnectionRequestModel.JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = Application.Current
                    .FindResource("LangKeyNumberOfConnectionsRequestsToWithdrawPerJob")?.ToString(),
                ActivitiesPerHourDisplayName = Application.Current
                    .FindResource("LangKeyNumberOfConnectionsRequestsToWithdrawPerHour")?.ToString(),
                ActivitiesPerDayDisplayName = Application.Current
                    .FindResource("LangKeyNumberOfConnectionsRequestsToWithdrawPerDay")?.ToString(),
                ActivitiesPerWeekDisplayName = Application.Current
                    .FindResource("LangKeyNumberOfConnectionsRequestsToWithdrawPerWeek")?.ToString(),
                IncreaseActivityDisplayName = Application.Current
                    .FindResource("LangKeyMaxConnectionRequestsToWithdrawPerDay")?.ToString(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };
            SaveCustomUserListCommand = new BaseCommand<object>(sender => true, SaveCustomUsers);
        }

        public WithdrawConnectionRequestModel WithdrawConnectionRequestModel
        {
            get => _WithdrawConnectionRequestModel;
            set
            {
                if ((_WithdrawConnectionRequestModel == null) & (_WithdrawConnectionRequestModel == value))
                    return;
                SetProperty(ref _WithdrawConnectionRequestModel, value);
            }
        }

        public WithdrawConnectionRequestModel Model => WithdrawConnectionRequestModel;
        public ICommand SaveCustomUserListCommand { get; set; }

        private void SaveCustomUsers(object sender)
        {
            try
            {
                if (WithdrawConnectionRequestModel.UrlInput.Contains("\r\n"))
                {
                    WithdrawConnectionRequestModel.UrlList =
                        Regex.Split(WithdrawConnectionRequestModel.UrlInput, "\r\n").ToList();
                    GlobusLogHelper.log.Info("" + WithdrawConnectionRequestModel.UrlList.Count +
                                             " profile urls saved sucessfully");
                }
                else
                {
                    try
                    {
                        WithdrawConnectionRequestModel.UrlList = new List<string>() { WithdrawConnectionRequestModel.UrlInput };
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