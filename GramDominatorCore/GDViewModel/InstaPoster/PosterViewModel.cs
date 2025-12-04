using System.Windows;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using GramDominatorCore.GDModel;

namespace GramDominatorCore.GDViewModel.InstaPoster
{
    public class PosterViewModel : BindableBase
    {
        public PosterViewModel()
        {
            PosterModel.JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = Application.Current.FindResource("LangKeyNumberOfRepostPerJob")?.ToString(),
                ActivitiesPerHourDisplayName = Application.Current.FindResource("LangKeyNumberOfRepostPerHour")?.ToString(),
                ActivitiesPerDayDisplayName = Application.Current.FindResource("LangKeyNumberOfRepostPerDay")?.ToString(),
                ActivitiesPerWeekDisplayName = Application.Current.FindResource("LangKeyNumberOfRepostPerWeek")?.ToString(),
                RunningTime = RunningTimes.DayWiseRunningTimes
            };

        }
        private PosterModel _posterModel = new PosterModel();

        public PosterModel PosterModel
        {
            get
            {
                return _posterModel;
            }
            set
            {
                if (_posterModel == null & _posterModel == value)
                    return;
                SetProperty(ref _posterModel, value);
            }
        }
        public PosterModel Model => PosterModel;

    }
}
