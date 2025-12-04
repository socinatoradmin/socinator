using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;

namespace YoutubeDominatorCore.YDFactories
{
    public class YdAccountCountFactory : IAccountCountFactory
    {
        private static YdAccountCountFactory _instance;

        private YdAccountCountFactory()
        {
        }

        public static YdAccountCountFactory Instance
            => _instance ?? (_instance = new YdAccountCountFactory());

        public string HeaderColumn1Value { get; set; } = "LangKeyChannelsCount".FromResourceDictionary();
        public bool HeaderColumn1Visiblity { get; set; } = true;
        public string HeaderColumn2Value { get; set; } = "LangKeyViewsCount".FromResourceDictionary();
        public bool HeaderColumn2Visiblity { get; set; } = false;
        public string HeaderColumn3Value { get; set; } = "LangKeySupportLinksCount".FromResourceDictionary();
        public bool HeaderColumn3Visiblity { get; set; } = false;
        public string HeaderColumn4Value { get; set; }
        public bool HeaderColumn4Visiblity { get; set; } = false;
        public string HeaderColumn5Value { get; set; }
        public bool HeaderColumn5Visiblity { get; set; } = false;
    }
}