using System;
using System.Linq;
using System.Windows;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.GdQuery;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using GramDominatorCore.GDModel;
using System.Windows.Input;
using DominatorHouseCore;
using DominatorHouseCore.Command;

namespace GramDominatorCore.GDViewModel.InstaLikerCommenter
{
    public class LikeViewModel : BindableBase
    {

        public LikeViewModel()
        {
            //  LikeModel.ListQueryType = Enum.GetNames(typeof(GdPostQuery)).ToList();

            Enum.GetValues(typeof(GdPostQuery)).Cast<GdPostQuery>().ToList().ForEach(query =>
            {
                LikeModel.ListQueryType.Add(Application.Current.FindResource(query.GetDescriptionAttr())?.ToString());
            });
            LikeModel.ListQueryType.Remove("Own Liked Post");
            
            LikeModel.JobConfiguration = new JobConfiguration
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
        }

        #region Commands

        public ICommand AddQueryCommand { get; set; }
        public ICommand CustomFilterCommand { get; set; }

        #endregion


        #region Command Implemented Methods

        private void AddQuery(object sender)
        {
            try
            {
                var ModuleSettingsUserControl = sender as DominatorUIUtility.CustomControl.ModuleSettingsUserControl<LikeViewModel, LikeModel>;
                if (ModuleSettingsUserControl != null) ModuleSettingsUserControl.AddQuery(typeof(GdPostQuery), Model.ListQueryType);
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
                var ModuleSettingsUserControl = sender as DominatorUIUtility.CustomControl.ModuleSettingsUserControl<LikeViewModel, LikeModel>;
                if (ModuleSettingsUserControl != null) ModuleSettingsUserControl.CustomFilter();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        #endregion

        private LikeModel _likeModel = new LikeModel();

        public LikeModel LikeModel
        {
            get
            {
                return _likeModel;
            }
            set
            {
                if (_likeModel == null & _likeModel == value)
                    return;
                SetProperty(ref _likeModel, value);
            }
        }

        public LikeModel Model => LikeModel;


    }
}
