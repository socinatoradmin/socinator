using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using DominatorHouseCore;
using DominatorHouseCore.Command;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.LdQuery;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Models.LinkedinModel;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using LinkedDominatorCore.LDModel.GrowConnection;
using System.Collections.ObjectModel;
using DominatorUIUtility.CustomControl.LinkedinCustomControl;
using LinkedDominatorCore.LDViewModel.Scraper;
using LinkedDominatorCore.LDUtility;

namespace LinkedDominatorCore.LDViewModel.GrowConnection
{
    public class ConnectionRequestViewModel : BindableBase
    {
        private ConnectionRequestModel _ConnectionRequestModel = new ConnectionRequestModel();

        public ConnectionRequestViewModel()
        {
            ConnectionRequestModel.ListQueryType.Clear();
            Enum.GetValues(typeof(LDGrowConnectionUserQueryParameters)).Cast<LDGrowConnectionUserQueryParameters>()
                .ForEach(QueryType =>
                {
                if (QueryType != LDGrowConnectionUserQueryParameters.PageUrl && QueryType != LDGrowConnectionUserQueryParameters.Email)
                        ConnectionRequestModel.ListQueryType.Add(QueryType.GetDescriptionAttr()
                            .FromResourceDictionary());
                });

            ConnectionRequestModel.JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = Application.Current
                    .FindResource("LangKeyNumberOfConnectionRequestsPerJob")?.ToString(),
                ActivitiesPerHourDisplayName = Application.Current
                    .FindResource("LangKeyNumberOfConnectionRequestsPerHour")?.ToString(),
                ActivitiesPerDayDisplayName = Application.Current
                    .FindResource("LangKeyNumberOfConnectionRequestsPerDay")?.ToString(),
                ActivitiesPerWeekDisplayName = Application.Current
                    .FindResource("LangKeyNumberOfConnectionRequestsPerWeek")?.ToString(),
                IncreaseActivityDisplayName =
                    Application.Current.FindResource("LangKeyMaxConnectionRequestsPerDay")?.ToString(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };
            AddQueryCommand = new BaseCommand<object>(sender => true, AddQuery);
            DeleteQueryCommand = new BaseCommand<object>(sender => true, DeleteQuery);
            DeleteMulipleCommand = new BaseCommand<object>(sender => true, DeleteMuliple);
            CustomFilterCommand = new BaseCommand<object>(sender => true, CustomFilter);
            SaveCommand = new BaseCommand<object>(sender => true, SavePersonalNote);
        }

        public ConnectionRequestModel ConnectionRequestModel
        {
            get => _ConnectionRequestModel;
            set
            {
                if ((_ConnectionRequestModel == null) & (_ConnectionRequestModel == value))
                    return;
                SetProperty(ref _ConnectionRequestModel, value);
            }
        }

        public ConnectionRequestModel Model => ConnectionRequestModel;

        public void AddQuery(object sender)
        {
            try
            {
                var ModuleSettingsUserControl =
                    sender as ModuleSettingsUserControl<ConnectionRequestViewModel, ConnectionRequestModel>;
                if (Utils.IsMultiContains(ModuleSettingsUserControl._queryControl.CurrentQuery.QueryType,
                    "Profile Url") && ModuleSettingsUserControl._queryControl.CurrentQuery.QueryValue.Contains("https://www.linkedin.com/sales/lead"))
                    new SearchUrlQueryHelper().AddQuery(ModuleSettingsUserControl._queryControl,
                        ModuleSettingsUserControl.CampaignName, ActivityType.SalesNavigatorUserScraper,
                        Model.SavedQueries);
                else
                    ModuleSettingsUserControl.AddQuery(typeof(LDGrowConnectionUserQueryParameters));
                
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public void DeleteQuery(object sender)
        {
            try
            {
                var currentQuery = sender as QueryInfo;
                if (Model.SavedQueries.Any(x => currentQuery != null && x.Id == currentQuery.Id))
                    Model.SavedQueries.Remove(currentQuery);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public void DeleteMuliple(object sender)
        {
            var selectedQuery = Model.SavedQueries.Where(x => x.IsQuerySelected).ToList();
            try
            {
                foreach (var currentQuery in selectedQuery)
                    try
                    {
                        if (Model.SavedQueries.Any(x => currentQuery != null && x.Id == currentQuery.Id))
                            Model.SavedQueries.Remove(currentQuery);
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void CustomFilter(object sender)
        {
            try
            {
                var ModuleSettingsUserControl =
                    sender as ModuleSettingsUserControl<ConnectionRequestViewModel, ConnectionRequestModel>;
                ModuleSettingsUserControl.CustomFilter();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void SavePersonalNote(object sender)
        {
            try
            {
                var PersonalNoteData = sender as ConnectionRequestPersonalNoteControl;
                if (string.IsNullOrEmpty(PersonalNoteData.PersonalNote.PersonalNoteText))
                {
                    Dialog.ShowDialog("Warning", "Please type some PersonalNote !!");
                    return;
                }
                if(Model.IsChkMultilineMessage)
                {
                    AddToPersonalNoteList(PersonalNoteData.PersonalNote, PersonalNoteData.PersonalNote.PersonalNoteText);
                    PersonalNoteData.PersonalNote = new ManagePersonalNoteModel();
                    ConnectionRequestModel.ManagePersonalNoteModel = PersonalNoteData.PersonalNote;
                }
                else
                {
                    var NoteList = PersonalNoteData.PersonalNote.PersonalNoteText.Split('\n').ToList();
                    NoteList = NoteList.Where(x => !string.IsNullOrEmpty(x.Trim())).Select(y => y.Trim()).ToList();
                    NoteList.ForEach(Note => { AddToPersonalNoteList(PersonalNoteData.PersonalNote, Note); });
                    PersonalNoteData.PersonalNote = new ManagePersonalNoteModel();
                    ConnectionRequestModel.ManagePersonalNoteModel = PersonalNoteData.PersonalNote;
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }


        }
        private void AddToPersonalNoteList(ManagePersonalNoteModel PersonalNOteModel, string PersonalText)
        {
            try
            {                
                    ConnectionRequestModel.LstDisplayManagePersonalNoteModel.Add(new ManagePersonalNoteModel
                    {
                        PersonalNoteText = PersonalText,                        
                    });
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        #region Commands

        public ICommand AddQueryCommand { get; set; }
        public ICommand CustomFilterCommand { get; set; }
        public ICommand DeleteQueryCommand { get; set; }
        public ICommand DeleteMulipleCommand { get; set; }
        public ICommand SaveCommand { get; set; }

        #endregion
    }
}