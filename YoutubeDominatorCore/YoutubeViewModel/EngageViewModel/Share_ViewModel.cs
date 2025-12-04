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
    public class Share_ViewModel : BindableBase
    {
        #region Object creation logic
        private Share_Model _ShareModel = new Share_Model();


        public Share_Model ShareModel
        {
            get
            {
                return _ShareModel;
            }
            set
            {
                if (_ShareModel == null & _ShareModel == value)
                    return;
                SetProperty(ref _ShareModel, value);
            }
        }

        #endregion
        public Share_Model Model => ShareModel;
        public Share_ViewModel()
        {
            ShareModel.ListQueryType.Clear();
            Enum.GetValues(typeof(YdScraperParameters)).Cast<YdScraperParameters>().ToList().ForEach(query =>
            {
                ShareModel.ListQueryType.Add(Application.Current.FindResource(EnumUtility.GetDescriptionAttr(query)).ToString());
            });
            ShareModel.JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = "LangKeySharesPerJob".FromResourceDictionary(),
                ActivitiesPerHourDisplayName = "LangKeySharesPerHour".FromResourceDictionary(),
                ActivitiesPerDayDisplayName = "LangKeySharesPerDay".FromResourceDictionary(),
                ActivitiesPerWeekDisplayName = "LangKeySharesPerWeek".FromResourceDictionary(),
                IncreaseActivityDisplayName = "LangKeyMaximumSharesPerDay".FromResourceDictionary(),
                RunningTime = RunningTimes.DayWiseRunningTimes
            };
        }


    }
}
