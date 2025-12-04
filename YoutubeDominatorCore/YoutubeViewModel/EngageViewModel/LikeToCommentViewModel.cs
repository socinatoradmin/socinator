using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoutubeDominatorCore.YoutubeModel;
using DominatorHouseCore.Enums.YdQuery;
using System.Windows;
using DominatorHouseCore.Enums;

namespace YoutubeDominatorCore.YoutubeViewModel
{
    public class LikeToCommentViewModel : BindableBase 
    {
        #region Object creation logic
        private LikeToCommentModel _LikeToCommentModel = new LikeToCommentModel();
        

        public LikeToCommentModel LikeToCommentModel
        {
            get
            {
                return _LikeToCommentModel;
            }
            set
            {
                if (_LikeToCommentModel == null & _LikeToCommentModel == value)
                    return;
                SetProperty(ref _LikeToCommentModel, value);
            }
        }

        #endregion
        public LikeToCommentModel Model => LikeToCommentModel;
        public LikeToCommentViewModel()
        {
            LikeToCommentModel.ListQueryType.Clear();
            Enum.GetValues(typeof(YdScraperParameters)).Cast<YdScraperParameters>().ToList().ForEach(query =>
            {
                LikeToCommentModel.ListQueryType.Add(Application.Current.FindResource(EnumUtility.GetDescriptionAttr(query)).ToString());
            });
            
            LikeToCommentModel.JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = "LangKeyLikeToCommentsPerJob".FromResourceDictionary(),
                ActivitiesPerHourDisplayName = "LangKeyLikeToCommentsPerHour".FromResourceDictionary(),
                ActivitiesPerDayDisplayName = "LangKeyLikeToCommentsPerDay".FromResourceDictionary(),
                ActivitiesPerWeekDisplayName = "LangKeyLikeToCommentsPerWeek".FromResourceDictionary(),
                IncreaseActivityDisplayName = "LangKeyMaximumLikeToCommentsPerDay".FromResourceDictionary(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };
    }


    }
}
