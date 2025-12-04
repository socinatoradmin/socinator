/*
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDModel.LikerCommentorModel;
using System;
using System.Linq;
using System.Windows;

namespace FaceDominatorCore.FDViewModel.LikerCommentorViewModel
{
    public class WebpageLikerCommentorViewModel:BindableBase
    {

        public WebpageLikerCommentorViewModel()
        {
            WebpageLikerCommentorModel.JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = Application.Current.FindResource("LangKeyNumberOfRequestPerJob")?.ToString(),
                ActivitiesPerHourDisplayName = Application.Current.FindResource("LangKeyNumberOfRequestPerHour")?.ToString(),
                ActivitiesPerDayDisplayName = Application.Current.FindResource("LangKeyNumberOfRequestPerDay")?.ToString(),
                ActivitiesPerWeekDisplayName = Application.Current.FindResource("LangKeyNumberOfRequestPerWeek")?.ToString(),
                IncreaseActivityDisplayName = Application.Current.FindResource("LangKeyMaxRequestPerDay")?.ToString(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };
        }

        public WebpageLikerCommentorModel Model => WebpageLikerCommentorModel;

        private WebpageLikerCommentorModel _webpageLikerCommentorModel = new WebpageLikerCommentorModel();

        public WebpageLikerCommentorModel WebpageLikerCommentorModel
        {
            get
            {
                return _webpageLikerCommentorModel;
            }
            set
            {
                if (_webpageLikerCommentorModel == null & _webpageLikerCommentorModel == value)
                    return;
                SetProperty(ref _webpageLikerCommentorModel, value);
            }
        }
    }
}
*/
