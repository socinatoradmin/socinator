using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DominatorHouseCore.Command;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.GdQuery;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using System.Collections.ObjectModel;
using GramDominatorCore.GDModel;
using DominatorHouseCore;
using static GramDominatorCore.GDModel.DownloadPhotosModel;

namespace GramDominatorCore.GDViewModel.InstaScraper
{
    public class DownloadPhotosViewModel : BindableBase
    {
        public DownloadPhotosViewModel()
        {
            //  DownloadPhotosModel.ListQueryType = Enum.GetNames(typeof(GdPostQuery)).ToList();

            Enum.GetValues(typeof(GdPostQuery)).Cast<GdPostQuery>().ToList().ForEach(query =>
            {
                DownloadPhotosModel.ListQueryType.Add(Application.Current.FindResource(query.GetDescriptionAttr())?.ToString());
            });

            DownloadedPathCommand = new BaseCommand<object>(DownloadedPathCanExecute, DownloadedPathExecute);

            DownloadPhotosModel.JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = Application.Current.FindResource("LangKeyNumberOfMediaPerJob")?.ToString(),
                ActivitiesPerHourDisplayName = Application.Current.FindResource("LangKeyNumberOfMediaPerHour")?.ToString(),
                ActivitiesPerDayDisplayName = Application.Current.FindResource("LangKeyNumberOfMediaPerDay")?.ToString(),
                ActivitiesPerWeekDisplayName = Application.Current.FindResource("LangKeyNumberOfMediaPerWeek")?.ToString(),
                IncreaseActivityDisplayName = Application.Current.FindResource("LangKeyMaxMediaPerDay")?.ToString(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };
            try
            {
                DownloadPhotosModel.ListUserRequiredData = new ObservableCollection<PostRequiredData>()
                {
                    new PostRequiredData {ItemName="All", IsSelected=false },
                    new PostRequiredData {ItemName="Likes", IsSelected=false },
                    new PostRequiredData {ItemName="Comments", IsSelected=false },
                    new PostRequiredData {ItemName="date & Time", IsSelected=false },
                    new PostRequiredData {ItemName="Location", IsSelected=false },
                     new PostRequiredData {ItemName="Post Url", IsSelected=false },
                };
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }           
            AddQueryCommand = new BaseCommand<object>((sender) => true, AddQuery);
            CustomFilterCommand = new BaseCommand<object>((sender) => true, CustomFilter);
            CheckAllRequiredDataCommand = new BaseCommand<object>((sender) => true, CheckAllReqData);
        }


        public ICommand AddQueryCommand { get; set; }
        public ICommand CustomFilterCommand { get; set; }

        public ICommand CheckAllRequiredDataCommand { get; set; }
        public ICommand DownloadedPathCommand{ get;set;}

        private void CustomFilter(object sender)
        {
            try
            {
                var ModuleSettingsUserControl = sender as DominatorUIUtility.CustomControl.ModuleSettingsUserControl<DownloadPhotosViewModel, DownloadPhotosModel>;
                if (ModuleSettingsUserControl != null) ModuleSettingsUserControl.CustomFilter();
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
                var ModuleSettingsUserControl = sender as DominatorUIUtility.CustomControl.ModuleSettingsUserControl<DownloadPhotosViewModel, DownloadPhotosModel>;
                if (ModuleSettingsUserControl != null) ModuleSettingsUserControl.AddQuery(typeof(GdUserQuery));
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

       

        private bool DownloadedPathCanExecute(object sender)
        {
            return true;
        }

        private void DownloadedPathExecute(object sender)
        {
            DownloadPhotosModel.DownloadedFolderPath = FileUtilities.GetExportPath();
        }

        private DownloadPhotosModel _downloadPhotosModel = new DownloadPhotosModel();

        public DownloadPhotosModel DownloadPhotosModel
        {
            get
            {
                return _downloadPhotosModel;
            }
            set
            {
                if (_downloadPhotosModel == null & _downloadPhotosModel == value)
                    return;
                SetProperty(ref _downloadPhotosModel, value);
            }
        }

        public DownloadPhotosModel Model => DownloadPhotosModel;
        private void CheckUncheckAll(object sender, bool IsChecked)
        {
            try
            {
                if (((PostRequiredData)(sender as CheckBox)?.DataContext)?.ItemName == "All")
                {
                   DownloadPhotosModel.ListUserRequiredData.ToList().Select(query => { query.IsSelected = IsChecked; return query; }).ToList();
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public void CheckAllReqData(object sender)
        {
            CheckUncheckAll(sender, ((System.Windows.Controls.Primitives.ToggleButton) sender).IsChecked == true);
        }


    }
}
