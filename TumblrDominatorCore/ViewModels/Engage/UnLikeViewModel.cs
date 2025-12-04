using DominatorHouseCore;
using DominatorHouseCore.Command;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Input;
using TumblrDominatorCore.Models;

namespace TumblrDominatorCore.ViewModels.Engage
{
    public class UnLikeViewModel : BindableBase
    {
        private UnLikeModel _unLikeModel = new UnLikeModel();

        public UnLikeViewModel()
        {
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

            UploadNotesCommand = new BaseCommand<object>(sender => true, CustomUser);
        }

        public ICommand UploadNotesCommand { get; set; }

        public UnLikeModel UnLikeModel
        {
            get => _unLikeModel;
            set
            {
                if ((_unLikeModel == null) & (_unLikeModel == value))
                    return;
                SetProperty(ref _unLikeModel, value);
            }
        }

        public UnLikeModel Model => UnLikeModel;

        private void CustomUser(object sender)
        {
            try
            {
                UnLikeModel.LstCustomPosts = Regex.Split(UnLikeModel.CustomPostsList, "\r\n").ToList();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}