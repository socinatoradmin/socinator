using DominatorHouseCore;
using DominatorHouseCore.Command;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.FdQuery;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDModel.LikerCommentorModel;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using BindableBase = DominatorHouseCore.Utility.BindableBase;
using EnumUtility = DominatorHouseCore.Utility.EnumUtility;

namespace FaceDominatorCore.FDViewModel.LikerCommentorViewModel
{
    public class WebPostLikeCommentViewModel : BindableBase
    {
        public WebPostLikeCommentViewModel()
        {
            WebPostCommentLikerModel.ListQueryType.Clear();

            Enum.GetValues(typeof(WebCommentLikerParameter)).Cast<WebCommentLikerParameter>().ToList().ForEach(query =>
            {
                WebPostCommentLikerModel.ListQueryType.Add(Application.Current.FindResource(EnumUtility.GetDescriptionAttr(query)).ToString());
            });

            WebPostCommentLikerModel.JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = Application.Current.FindResource("LangKeyNumberOfLikesPerJob")?.ToString(),
                ActivitiesPerHourDisplayName = Application.Current.FindResource("LangKeyNumberOfLikesPerHour")?.ToString(),
                ActivitiesPerDayDisplayName = Application.Current.FindResource("LangKeyNumberOfLikesPerDay")?.ToString(),
                ActivitiesPerWeekDisplayName = Application.Current.FindResource("LangKeyNumberOfLikesPerWeek")?.ToString(),
                IncreaseActivityDisplayName = Application.Current.FindResource("LangKeyMaxLikesPerDay")?.ToString(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };

            AddQueryCommand = new BaseCommand<object>((sender) => true, AddQuery);
            CustomFilterCommand = new BaseCommand<object>((sender) => true, CustomFilter);
            DeleteQueryCommand = new BaseCommand<object>((sender) => true, DeleteQuery);
            DeleteMulipleCommand = new BaseCommand<object>((sender) => true, DeleteMuliple);
        }

        #region Commands

        public ICommand AddQueryCommand { get; set; }
        public ICommand CustomFilterCommand { get; set; }
        public ICommand DeleteQueryCommand { get; set; }
        public ICommand DeleteMulipleCommand { get; set; }

        #endregion

        #region Methods

        private void CustomFilter(object sender)
        {
            try
            {
                var ModuleSettingsUserControl = sender as DominatorUIUtility.CustomControl.ModuleSettingsUserControl<WebPostLikeCommentViewModel, WebPostCommentLikerModel>;
                ModuleSettingsUserControl.CustomFilter();
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
                var ModuleSettingsUserControl = sender as DominatorUIUtility.CustomControl.ModuleSettingsUserControl<WebPostLikeCommentViewModel, WebPostCommentLikerModel>;

                var listQuery = ModuleSettingsUserControl._queryControl.CurrentQuery.QueryValue.Split(',').Where(x => !string.IsNullOrEmpty(x.Trim())).Distinct();
                listQuery.ForEach(z =>
                {
                    if (!WebPostCommentLikerModel.LikerCommentorConfigModel.ManageCommentModel.LstQueries.Any(x =>
                        x.Content.QueryValue == z &&
                        x.Content.QueryType == ModuleSettingsUserControl._queryControl.CurrentQuery.QueryType))
                    {

                        WebPostCommentLikerModel.LikerCommentorConfigModel.ManageCommentModel.LstQueries.Add(new QueryContent
                        {
                            Content = new QueryInfo
                            {
                                QueryValue = z,
                                QueryType = ModuleSettingsUserControl._queryControl.CurrentQuery.QueryType

                            }
                        });
                    }
                });

                if (string.IsNullOrEmpty(ModuleSettingsUserControl._queryControl.CurrentQuery.QueryValue) &&
                    ModuleSettingsUserControl._queryControl.QueryCollection.Count != 0)
                {
                    ModuleSettingsUserControl._queryControl.QueryCollection.ForEach(query =>
                    {
                        var currentQuery = ModuleSettingsUserControl._queryControl.CurrentQuery.Clone() as QueryInfo;

                        if (currentQuery == null) return;

                        currentQuery.QueryTypeDisplayName = currentQuery.QueryType;

                        if (!WebPostCommentLikerModel.LikerCommentorConfigModel.ManageCommentModel.LstQueries.Any(x =>
                            x.Content.QueryValue == query &&
                            x.Content.QueryType == currentQuery.QueryTypeDisplayName))
                        {


                            WebPostCommentLikerModel.LikerCommentorConfigModel.ManageCommentModel.LstQueries.Add(new QueryContent
                            {
                                Content = new QueryInfo
                                {
                                    QueryValue = query,
                                    QueryType = currentQuery.QueryTypeDisplayName

                                }
                            });
                        }
                    });
                }
                ModuleSettingsUserControl.AddQuery(typeof(WebCommentLikerParameter));

            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
        private void DeleteQuery(object sender)
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

        private void DeleteMuliple(object sender)
        {
            var selectedQuery = Model.SavedQueries.Where(x => x.IsQuerySelected).ToList();
            try
            {
                foreach (var currentQuery in selectedQuery)
                {
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
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

        }

        #endregion

        public WebPostCommentLikerModel Model => WebPostCommentLikerModel;

        private WebPostCommentLikerModel _webPostCommentLikerLikerModel = new WebPostCommentLikerModel();

        public WebPostCommentLikerModel WebPostCommentLikerModel
        {
            get
            {
                return _webPostCommentLikerLikerModel;
            }
            set
            {
                if (_webPostCommentLikerLikerModel == null & _webPostCommentLikerLikerModel == value)
                    return;
                SetProperty(ref _webPostCommentLikerLikerModel, value);
            }
        }
    }
}

