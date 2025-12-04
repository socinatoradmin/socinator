using System;
using System.Linq;
using System.Windows;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.GdQuery;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using GramDominatorCore.GDModel;
using System.Windows.Input;
using DominatorHouseCore.Command;
using DominatorHouseCore;

namespace GramDominatorCore.GDViewModel.InstaLikerCommenter
{
    public class LikeCommentViewModel : BindableBase
    {

        public LikeCommentViewModel()
        {
            Enum.GetValues(typeof(GdPostQuery)).Cast<GdPostQuery>().ToList().ForEach(query =>
            {
                LikeCommentModel.ListQueryType.Add(Application.Current.FindResource(query.GetDescriptionAttr())?.ToString());
            });
          

            LikeCommentModel.JobConfiguration = new JobConfiguration
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

        }

        public ICommand AddQueryCommand { get; set; }

        private LikeCommentModel _likeCommentModel = new LikeCommentModel();

        private void AddQuery(object sender)
        {
            try
            {
                var ModuleSettingsUserControl = sender as DominatorUIUtility.CustomControl.ModuleSettingsUserControl<LikeCommentViewModel, LikeCommentModel>;
                if (ModuleSettingsUserControl != null) ModuleSettingsUserControl.AddQuery(typeof(GdPostQuery));
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
        public LikeCommentModel LikeCommentModel
        {
            get
            {
                return _likeCommentModel;
            }
            set
            {
                if (_likeCommentModel == null & _likeCommentModel == value)
                    return;
                SetProperty(ref _likeCommentModel, value);
            }
        }

        public LikeCommentModel Model => LikeCommentModel;


    }
}
