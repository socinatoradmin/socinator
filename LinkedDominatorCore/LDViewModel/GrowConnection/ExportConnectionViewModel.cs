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
    public class ExportConnectionViewModel : BindableBase
    {
        private ExportConnectionModel _exportConnectionModel = new ExportConnectionModel();

        public ExportConnectionViewModel()
        {
            ExportConnectionModel.JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = Application.Current
                    .FindResource("LangKeyNumberOfConnectionsToExportPerJob")?.ToString(),
                ActivitiesPerHourDisplayName = Application.Current
                    .FindResource("LangKeyNumberOfConnectionsToExportPerHour")?.ToString(),
                ActivitiesPerDayDisplayName = Application.Current
                    .FindResource("LangKeyNumberOfConnectionsToExportPerDay")?.ToString(),
                ActivitiesPerWeekDisplayName = Application.Current
                    .FindResource("LangKeyNumberOfConnectionsToExportPerWeek")?.ToString(),
                IncreaseActivityDisplayName =
                    Application.Current.FindResource("LangKeyMaxConnectionsToExportPerDay")?.ToString(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };
            SaveCustomUserListCommand = new BaseCommand<object>(sender => true, SaveCustomUsers);
            SaveFileFormatCommand = new BaseCommand<object>(sender => true, SaveFileFormat);
        }

        public ExportConnectionModel ExportConnectionModel
        {
            get => _exportConnectionModel;
            set
            {
                if ((_exportConnectionModel == null) & (_exportConnectionModel == value))
                    return;
                SetProperty(ref _exportConnectionModel, value);
            }
        }

        public ExportConnectionModel Model => ExportConnectionModel;

        private void SaveCustomUsers(object sender)
        {
            try
            {
                if (ExportConnectionModel.UrlInput.Contains("\r\n"))
                {
                    ExportConnectionModel.UrlList =
                        Regex.Split(ExportConnectionModel.UrlInput, "\r\n").ToList();
                    GlobusLogHelper.log.Info("" + ExportConnectionModel.UrlList.Count +
                                             " profile urls saved sucessfully");
                }
                else
                {
                    try
                    {
                        ExportConnectionModel.UrlList = new List<string>() { ExportConnectionModel.UrlInput };
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

        private void SaveFileFormat(object sender)
        {
            try
            {
                if (!string.IsNullOrEmpty(ExportConnectionModel.FilenameFormat))
                    GlobusLogHelper.log.Info("filename format saved sucessfully");
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        #region Commands

        public ICommand SaveFileFormatCommand { get; set; }
        public ICommand SaveCustomUserListCommand { get; set; }

        #endregion
    }
}