using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;

namespace TwtDominatorCore.TDFactories
{
    public class TdAccountCountFactory : IAccountCountFactory
    {
        private static TdAccountCountFactory _instance;

        private TdAccountCountFactory()
        {
        }

        public static TdAccountCountFactory Instance => _instance ?? (_instance = new TdAccountCountFactory());

        public string HeaderColumn1Value { get; set; } = "LangKeyFollowersCount".FromResourceDictionary();
        public bool HeaderColumn1Visiblity { get; set; } = true;
        public string HeaderColumn2Value { get; set; } = "LangKeyFollowingsCount".FromResourceDictionary();
        public bool HeaderColumn2Visiblity { get; set; } = true;
        public string HeaderColumn3Value { get; set; } = "LangKeyTweetCount".FromResourceDictionary();
        public bool HeaderColumn3Visiblity { get; set; } = true;
        public string HeaderColumn4Value { get; set; }
        public bool HeaderColumn4Visiblity { get; set; } = false;
        public string HeaderColumn5Value { get; set; }
        public bool HeaderColumn5Visiblity { get; set; } = false;
    }
}