using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.GdQuery;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using GramDominatorCore.GDModel;
using DominatorHouseCore.Command;
using DominatorHouseCore;
using System.Collections.ObjectModel;
using static GramDominatorCore.GDModel.UserScraperModel;
using System.Windows.Controls;

namespace GramDominatorCore.GDViewModel.InstaScraper
{
    public class UserScraperViewModel : BindableBase
    {
        public UserScraperViewModel()
        {
            // UserScraperModel.ListQueryType = Enum.GetNames(typeof(GdUserQuery)).ToList();

            Enum.GetValues(typeof(GdUserQuery)).Cast<GdUserQuery>().ToList().ForEach(query =>
            {
                UserScraperModel.ListQueryType.Add(Application.Current.FindResource(query.GetDescriptionAttr())?.ToString());
            });
            UserScraperModel.ListQueryType.Remove("Own Followers");
            UserScraperModel.ListQueryType.Remove("Own Followings");
            UserScraperModel.ListQueryType.Remove("Scrape users whom we messaged");
            try
            {
                UserScraperModel.ListUserRequiredData = new ObservableCollection<UserRequiredData>()
                {
                    new UserRequiredData {ItemName="All", IsSelected=false },
                    new UserRequiredData {ItemName="Profile Picture Url", IsSelected=false },
                    new UserRequiredData {ItemName="User Name", IsSelected=false },
                    new UserRequiredData {ItemName="User ID", IsSelected=false },
                    new UserRequiredData {ItemName="User Full Name", IsSelected=false },
                    new UserRequiredData {ItemName="Is Followed Already", IsSelected=false },
                    new UserRequiredData {ItemName="Post Count", IsSelected=false },
                    new UserRequiredData {ItemName="Follower Count", IsSelected=false },
                    new UserRequiredData {ItemName="Following Count", IsSelected=false },
                    new UserRequiredData {ItemName="Email Id",IsSelected=false },
                     new UserRequiredData {ItemName="Contact Number",IsSelected=false },
                    new UserRequiredData {ItemName="Engagement Rate", IsSelected=false },
                    new UserRequiredData {ItemName="Comment count",IsSelected=false } ,
                    new UserRequiredData {ItemName="Like count",IsSelected=false },
                    new UserRequiredData {ItemName="Biography",IsSelected=false },
                    new UserRequiredData {ItemName="Business Acocunts",IsSelected=false },
                    new UserRequiredData {ItemName="Business Category",IsSelected=false },
                 };
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            UserScraperModel.JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = Application.Current.FindResource("LangKeyNumberOfUsersPerJob")?.ToString(),
                ActivitiesPerHourDisplayName = Application.Current.FindResource("LangKeyNumberOfUsersPerHour")?.ToString(),
                ActivitiesPerDayDisplayName = Application.Current.FindResource("LangKeyNumberOfUsersPerDay")?.ToString(),
                ActivitiesPerWeekDisplayName = Application.Current.FindResource("LangKeyNumberOfUsersPerWeek")?.ToString(),
                IncreaseActivityDisplayName = Application.Current.FindResource("LangKeyMaxUsersPerDay")?.ToString(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };
            AddQueryCommand = new BaseCommand<object>((sender) => true, AddQuery);
            CustomFilterCommand = new BaseCommand<object>((sender) => true, CustomFilter);
            CheckAllRequiredDataCommand = new BaseCommand<object>((sender) => true, CheckAllReqData);

        }

        public ICommand CheckAllRequiredDataCommand { get; set; }
        public ICommand AddQueryCommand { get; set; }
        public ICommand CustomFilterCommand { get; set; }


        private void CustomFilter(object sender)
        {
            try
            {
                var ModuleSettingsUserControl = sender as DominatorUIUtility.CustomControl.ModuleSettingsUserControl<UserScraperViewModel, UserScraperModel>;
                if (ModuleSettingsUserControl != null) ModuleSettingsUserControl.CustomFilter();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public void CheckAllReqData(object sender)
        {
            try
            {
                CheckUncheckAll(sender, ((System.Windows.Controls.Primitives.ToggleButton)sender).IsChecked == true);
            }
            catch (Exception)
            {
                // ignored
            }
        }

        private void CheckUncheckAll(object sender, bool IsChecked)
        {
            try
            {
                if (((UserRequiredData)(sender as CheckBox)?.DataContext).ItemName == "All")
                {
                    UserScraperModel.ListUserRequiredData.ToList().Select(query => { query.IsSelected = IsChecked; return query; }).ToList();
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void AddQuery(object sender)
        {
            try
            {
                var ModuleSettingsUserControl = sender as DominatorUIUtility.CustomControl.ModuleSettingsUserControl<UserScraperViewModel, UserScraperModel>;
                if (ModuleSettingsUserControl != null) ModuleSettingsUserControl.AddQuery(typeof(GdUserQuery), Model.ListQueryType);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private UserScraperModel _userScraperModel = new UserScraperModel();
        // private UserScraperModel _userScraperModel;
        public UserScraperModel UserScraperModel
        {
            get
            {
                return _userScraperModel;
            }
            set
            {
                if (_userScraperModel == null & _userScraperModel == value)
                    return;
                SetProperty(ref _userScraperModel, value);
            }
        }

        public UserScraperModel Model => UserScraperModel;

    }
}
