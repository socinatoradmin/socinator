using DominatorHouseCore;
using DominatorHouseCore.Command;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.GdQuery;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using GramDominatorCore.GDModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
namespace GramDominatorCore.GDViewModel.InstaScraper
{
    public class CommentScraperViewModel: BindableBase
    {
        public CommentScraperViewModel()
        {
            Enum.GetValues(typeof(GdPostQuery)).Cast<GdPostQuery>().ToList().ForEach(query =>
            {
                CommentScraperModel.ListQueryType.Add(Application.Current.FindResource(query.GetDescriptionAttr())?.ToString());
            });
            
            CommentScraperModel.JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = Application.Current.FindResource("LangKeyNumberOfMediaPerJob")?.ToString(),
                ActivitiesPerHourDisplayName = Application.Current.FindResource("LangKeyNumberOfMediaPerHour")?.ToString(),
                ActivitiesPerDayDisplayName = Application.Current.FindResource("LangKeyNumberOfMediaPerDay")?.ToString(),
                ActivitiesPerWeekDisplayName = Application.Current.FindResource("LangKeyNumberOfMediaPerWeek")?.ToString(),
                IncreaseActivityDisplayName = Application.Current.FindResource("LangKeyMaxMediaPerDay")?.ToString(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };
            AddQueryCommand = new BaseCommand<object>((sender) => true, AddQuery);
        }


        public ICommand AddQueryCommand { get; set; }
        public ICommand DownloadedPathCommand
        {
            get;
            set;
        }
        private void AddQuery(object sender)
        {
            try
            {
                var ModuleSettingsUserControl = sender as DominatorUIUtility.CustomControl.ModuleSettingsUserControl<CommentScraperViewModel, CommentScraperModel>;
                if (ModuleSettingsUserControl != null) ModuleSettingsUserControl.AddQuery(typeof(GdPostQuery));
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private CommentScraperModel _CommentScraperModel = new CommentScraperModel();

        public CommentScraperModel CommentScraperModel
        {
            get
            {
                return _CommentScraperModel;
            }
            set
            {
                if (_CommentScraperModel == null & _CommentScraperModel == value)
                    return;
                SetProperty(ref _CommentScraperModel, value);
            }
        }

        public CommentScraperModel Model => CommentScraperModel;

    }
}
