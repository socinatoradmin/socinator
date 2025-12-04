using DominatorHouseCore.Enums.YdQuery;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using YoutubeDominatorCore.YoutubeModel;

namespace YoutubeDominatorCore.YoutubeViewModel
{
    public class DeleteComment_ViewModel : BindableBase
    {
        #region Object creation logic
        private DeleteCommentModel _DeleteCommentModel = new DeleteCommentModel();


        public DeleteCommentModel DeleteCommentModel
        {
            get
            {
                return _DeleteCommentModel;
            }
            set
            {
                if (_DeleteCommentModel == null & _DeleteCommentModel == value)
                    return;
                SetProperty(ref _DeleteCommentModel, value);
            }
        }

        #endregion
        public DeleteCommentModel Model => DeleteCommentModel;
        public DeleteComment_ViewModel()
        {
            DeleteCommentModel.ListQueryType.Clear();

            Enum.GetValues(typeof(YdScraperParameters)).Cast<YdScraperParameters>().ToList().ForEach(query =>
            {
                DeleteCommentModel.ListQueryType.Add(Application.Current.FindResource(EnumUtility.GetDescriptionAttr(query)).ToString());
            });
            DeleteCommentModel.JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = "LangKeyDeleteCommentsPerJob".FromResourceDictionary(),
                ActivitiesPerHourDisplayName = "LangKeyDeleteCommentsPerHour".FromResourceDictionary(),
                ActivitiesPerDayDisplayName = "LangKeyDeleteCommentsPerDay".FromResourceDictionary(),
                ActivitiesPerWeekDisplayName = "LangKeyDeleteCommentsPerWeek".FromResourceDictionary(),
                IncreaseActivityDisplayName = "LangKeyMaximumDeleteCommentsPerDay".FromResourceDictionary(),
                RunningTime = RunningTimes.DayWiseRunningTimes
            };
        }


    }
}
