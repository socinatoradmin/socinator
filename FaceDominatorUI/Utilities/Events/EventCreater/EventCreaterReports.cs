using DominatorHouseCore.DatabaseHandler.FdTables.Campaigns;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDLibrary.DAL;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDModel.FbEvents;
using FaceDominatorCore.FdReports;
using FaceDominatorCore.Interface;
using MahApps.Metro.Controls.Dialogs;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace FaceDominatorUI.Utilities.Events.EventCreater
{
    internal class EventCreaterReports : IFdReportFactory
    {
        public static ObservableCollection<EventCreaterReportModel> InteractedEventModel =
            new ObservableCollection<EventCreaterReportModel>();

        public static List<string> Data = new List<string>();

        public static List<string> CampaignData = new List<string>();

        private readonly string _activityType = ActivityType.EventCreator.ToString();

        public ObservableCollection<QueryInfo> GetSavedQuery(ActivityType subModuleName, string activitySettings)
        {
            return JsonConvert.DeserializeObject<EventCreatorModel>(activitySettings).SavedQueries;
        }

        public ObservableCollection<object> GetCampaignsReport(ReportModel reportModel,
            List<KeyValuePair<string, string>> queryDetails, CampaignDetails campaignDetails)
        {
            var dataBase = new DbCampaignService(campaignDetails.CampaignId);

            InteractedEventModel.Clear();

            #region get data from InteractedUsers table and add to UnfollowerReportModel

            dataBase.GetAllInteractedData<InteractedEvents>().ForEach(
                report =>
                {
                    InteractedEventModel.Add(new EventCreaterReportModel
                    {
                        ActivityType = ActivityType.EventCreator.ToString(),
                        AccountEmail = report.AccountEmail,
                        EventType = report.EventType,
                        EventName = report.EventName,
                        EventUrl = FdConstants.FbHomeUrl + report.EventId,
                        EventStartDate = report.EventStartDate,
                        EventEndDate = report.EventEndDate,
                        EventLocation = report.EventLocation,
                        InteractionTimeStamp = report.InteractionDateTime
                    });
                });

            #endregion

            #region Generate Reports column with data

            //campaign.SelectedAccountList.ToList().ForEach(x =>
            //{
            reportModel.GridViewColumn =
                new ObservableCollection<GridViewColumnDescriptor>
                {
                    new GridViewColumnDescriptor {ColumnHeaderText = "ID", ColumnBindingText = "Id"},
                    new GridViewColumnDescriptor
                    {
                        ColumnHeaderText =
                            $"{"LangKeyAccount".FromResourceDictionary()} {"LangKeyEmail".FromResourceDictionary()}",
                        ColumnBindingText = "AccountEmail"
                    },
                    new GridViewColumnDescriptor
                    {
                        ColumnHeaderText = "LangKeyEventType".FromResourceDictionary(), ColumnBindingText = "EventType"
                    },
                    new GridViewColumnDescriptor
                    {
                        ColumnHeaderText = "LangKeyEventName".FromResourceDictionary(), ColumnBindingText = "EventName"
                    },
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "LangKeyEventUrl".FromResourceDictionary(), ColumnBindingText = "EventUrl"},
                    new GridViewColumnDescriptor
                    {
                        ColumnHeaderText = "LangKeyStartDate".FromResourceDictionary(),
                        ColumnBindingText = "EventStartDate"
                    },
                    new GridViewColumnDescriptor
                    {
                        ColumnHeaderText = "LangKeyEndDate".FromResourceDictionary(), ColumnBindingText = "EventEndDate"
                    },
                    new GridViewColumnDescriptor
                    {
                        ColumnHeaderText = "LangKeyEventLocation".FromResourceDictionary(),
                        ColumnBindingText = "EventLocation"
                    },
                    new GridViewColumnDescriptor
                    {
                        ColumnHeaderText = "LangKeyDate".FromResourceDictionary(),
                        ColumnBindingText = "InteractionTimeStamp"
                    }
                };
            //});
            // reportModel.ReportCollection = CollectionViewSource.GetDefaultView(InteractedUsersModel);

            #endregion

            return new ObservableCollection<object>(InteractedEventModel);
        }

        public void ExportReports(ActivityType activityType, string fileName, ReportType reportType)
        {
            var csvData = new List<string>();

            #region Campaign reports

            if (reportType == ReportType.Campaign)
            {
                Header =
                    "Account Email,Event Type,Event Name,Event Url,Event Start Date,Event End Date,Event Location,Date";

                InteractedEventModel.ToList().ForEach(report =>
                {
                    try
                    {
                        csvData.Add(report.AccountEmail + ","
                                                        + report.EventType + ","
                                                        + (string.IsNullOrEmpty(report.EventName)
                                                            ? "NA"
                                                            : report.EventName.Replace(",", " ")) + ","
                                                        + report.EventUrl + ","
                                                        + report.EventStartDate + ","
                                                        + report.EventEndDate + ","
                                                        + report.EventLocation + ","
                                                        + report.InteractionTimeStamp);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.StackTrace);
                    }
                });
            }

            #endregion

            #region Account reports

            if (reportType == ReportType.Account)
            {
                Header = "Event Type,Event Name,Event Url,Event Start Date,Event End Date,Event Location,Date";

                InteractedEventModel.ToList().ForEach(report =>
                {
                    try
                    {
                        csvData.Add(report.EventType + ","
                                                     + (string.IsNullOrEmpty(report.EventName)
                                                         ? "NA"
                                                         : report.EventName.Replace(",", " ")) + ","
                                                     + report.EventUrl + ","
                                                     + report.EventStartDate + ","
                                                     + report.EventEndDate + ","
                                                     + report.EventLocation + ","
                                                     + report.InteractionTimeStamp);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.StackTrace);
                    }
                });
            }

            #endregion

            if (csvData.Count == 0)
            {
                Dialog.ShowDialog(Application.Current.MainWindow,
                    "LangKeyWarning".FromResourceDictionary(), "LangKeyReportIsNotAvailable".FromResourceDictionary());
                return;
            }

            DominatorHouseCore.Utility.Utilities.ExportReports(fileName, Header, csvData);
        }

        public IList GetsAccountReport(DbAccountService dataBase)
        {
            IList reportDetails = dataBase
                .Get<DominatorHouseCore.DatabaseHandler.FdTables.Accounts.InteractedEvents>(x =>
                    x.ActivityType == _activityType).ToList();

            InteractedEventModel.Clear();

            var id = 1;

            foreach (DominatorHouseCore.DatabaseHandler.FdTables.Accounts.InteractedEvents report in reportDetails)
            {
                InteractedEventModel.Add(
                    new EventCreaterReportModel
                    {
                        Id = id,
                        ActivityType = ActivityType.EventCreator.ToString(),
                        EventType = report.EventType,
                        EventName = report.EventName,
                        EventUrl = report.EventUrl,
                        EventStartDate = report.EventStartDate,
                        EventEndDate = report.EventEndDate,
                        EventLocation = report.EventLocation,
                        InteractionTimeStamp = report.InteractionTimeStamp.EpochToDateTimeLocal()
                    }
                );
                id++;
            }

            return InteractedEventModel;
        }

        public string Header { get; set; }
    }
}