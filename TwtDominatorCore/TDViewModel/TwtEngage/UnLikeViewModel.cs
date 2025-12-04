using System;
using System.Linq;
using System.Windows.Input;
using DominatorHouseCore.Command;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using TwtDominatorCore.TDModels;

namespace TwtDominatorCore.TDViewModel.TwtEngage
{
    public class UnLikeViewModel : BindableBase
    {
        private UnLikeModel _unlikeModel = new UnLikeModel();

        public UnLikeViewModel()
        {
            UnLikeModel.ListQueryType.Clear();
            // Load job configuration values
            UnLikeModel.JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = "LangKeyNumberOfUnLikesPerJob".FromResourceDictionary(),
                ActivitiesPerHourDisplayName = "LangKeyNumberOfUnLikesPerHour".FromResourceDictionary(),
                ActivitiesPerDayDisplayName = "LangKeyNumberOfUnLikesPerDay".FromResourceDictionary(),
                ActivitiesPerWeekDisplayName = "LangKeyNumberOfUnLikesPerWeek".FromResourceDictionary(),
                IncreaseActivityDisplayName = "LangKeyMaxUnLikesPerDay".FromResourceDictionary(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };


            StoreModuleSettingsCommand = new BaseCommand<object>(StoreCampaignCanExecute, StoreCampaignExecute);
            CreateCampaignCommand = new BaseCommand<object>(CreateCampaignCanExecute, CreateCampaignExecute);
            AddSearchQueryCommand = new BaseCommand<object>(AddSearchQueryCanExecute, AddSearchQueryExecute);
            SplitInputToListCommand = new BaseCommand<object>(sender => true, SplitInputToListExecute);
        }

        public ICommand SplitInputToListCommand { get; set; }

        public UnLikeModel UnLikeModel
        {
            get => _unlikeModel;
            set
            {
                if ((_unlikeModel == null) & (_unlikeModel == value))
                    return;
                SetProperty(ref _unlikeModel, value);
            }
        }

        public UnLikeModel Model => UnLikeModel;

        public ICommand StoreModuleSettingsCommand { get; set; }
        public ICommand CreateCampaignCommand { get; set; }

        public ICommand AddSearchQueryCommand { get; set; }

        private void CreateCampaignExecute(object obj)
        {
        }

        private bool CreateCampaignCanExecute(object arg)
        {
            return true;
        }

        private void AddSearchQueryExecute(object obj)
        {
            var UnLikerSearchControl = obj as SearchQueryControl;

            if (UnLikerSearchControl != null && string.IsNullOrEmpty(UnLikerSearchControl.CurrentQuery.QueryValue))
            {
                UnLikerSearchControl.QueryCollection.ForEach(query =>
                {
                    var currentQuery = UnLikerSearchControl.CurrentQuery.Clone() as QueryInfo;

                    if (currentQuery == null) return;

                    currentQuery.QueryValue = query;

                    currentQuery.QueryTypeDisplayName
                        = currentQuery.QueryType.ToString();

                    currentQuery.QueryPriority
                        = UnLikeModel.SavedQueries.Count + 1;

                    UnLikeModel.SavedQueries.Add(currentQuery);
                });
            }
            else
            {
                if (UnLikerSearchControl == null) return;
                UnLikerSearchControl.CurrentQuery.QueryTypeDisplayName
                    = UnLikerSearchControl.CurrentQuery.QueryType;
                var currentQuery = UnLikerSearchControl.CurrentQuery.Clone() as QueryInfo;
                if (currentQuery == null) return;
                currentQuery.QueryPriority = UnLikeModel.SavedQueries.Count + 1;
                UnLikeModel.SavedQueries.Add(currentQuery);
                UnLikerSearchControl.CurrentQuery = new QueryInfo();
            }
        }

        private bool AddSearchQueryCanExecute(object arg)
        {
            return true;
        }

        private bool StoreCampaignCanExecute(object sender)
        {
            return true;
        }

        private void StoreCampaignExecute(object sender)
        {
        }

        private void SplitInputToListExecute(object sender)
        {
        }
    }
}