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

namespace YoutubeDominatorCore.YoutubeViewModel
{
    public class ReplyToCommentViewModel : BindableBase 
    {
        #region Object creation logic
        private ReplyToCommentModel _ReplyToCommentModel = new ReplyToCommentModel();
        

        public ReplyToCommentModel ReplyToCommentModel
        {
            get
            {
                return _ReplyToCommentModel;
            }
            set
            {
                if (_ReplyToCommentModel == null & _ReplyToCommentModel == value)
                    return;
                SetProperty(ref _ReplyToCommentModel, value);
            }
        }

        #endregion
        public ReplyToCommentModel Model => ReplyToCommentModel;
        public ReplyToCommentViewModel()
        {
            ReplyToCommentModel.ListQueryType.Clear();
            Enum.GetValues(typeof(YdScraperParameters)).Cast<YdScraperParameters>().ToList().ForEach(query =>
            {
                ReplyToCommentModel.ListQueryType.Add(Application.Current.FindResource(EnumUtility.GetDescriptionAttr(query)).ToString());
            });
            
            ReplyToCommentModel.JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = "YdLangReplyToCommentsPerJob".FromResourceDictionary(),
                ActivitiesPerHourDisplayName = "YdLangReplyToCommentsPerHour".FromResourceDictionary(),
                ActivitiesPerDayDisplayName = "YdLangReplyToCommentsPerDay".FromResourceDictionary(),
                ActivitiesPerWeekDisplayName = "YdLangReplyToCommentsPerWeek".FromResourceDictionary(),
                IncreaseActivityDisplayName = "YdLangMaxReplyToCommentsDay".FromResourceDictionary(),
                RunningTime = RunningTimes.DayWiseRunningTimes
            };
    }


    }
}
