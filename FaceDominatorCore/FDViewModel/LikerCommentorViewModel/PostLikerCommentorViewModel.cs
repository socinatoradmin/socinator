using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDModel.LikerCommentorModel;
using System;
using System.Linq;
using System.Windows;

namespace FaceDominatorCore.FDViewModel.LikerCommentorViewModel
{
    public class PostLikerCommentorViewModel : BindableBase
    {

        public PostLikerCommentorViewModel()
        {


            PostLikerCommentorModel.JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = Application.Current.FindResource("LangKeyNumberOfLikesAndCommentsPerJob")?.ToString(),
                ActivitiesPerHourDisplayName = Application.Current.FindResource("LangKeyNumberOfLikesAndCommentsPerHour")?.ToString(),
                ActivitiesPerDayDisplayName = Application.Current.FindResource("LangKeyNumberOfLikesAndCommentsPerDay")?.ToString(),
                ActivitiesPerWeekDisplayName = Application.Current.FindResource("LangKeyNumberOfLikesAndCommentsPerWeek")?.ToString(),
                IncreaseActivityDisplayName = Application.Current.FindResource("LangKeyMaxLikesAndCommentsPerDay")?.ToString(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };
        }

        public PostLikerCommentorModel Model => PostLikerCommentorModel;

        private PostLikerCommentorModel _postLikerCommentorModel = new PostLikerCommentorModel();

        public PostLikerCommentorModel PostLikerCommentorModel
        {
            get
            {
                return _postLikerCommentorModel;
            }
            set
            {
                if (_postLikerCommentorModel == null & _postLikerCommentorModel == value)
                    return;
                SetProperty(ref _postLikerCommentorModel, value);
            }
        }
    }


}
