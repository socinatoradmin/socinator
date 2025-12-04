using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;

namespace GramDominatorCore.GDFactories
{
    public class GDAccountCountFactory : IAccountCountFactory
    {
        private static GDAccountCountFactory _instance;

        public static GDAccountCountFactory Instance
            => _instance ?? (_instance = new GDAccountCountFactory());

        private GDAccountCountFactory() { }

        public string HeaderColumn1Value { get; set; } = "LangKeyFollowersCount".FromResourceDictionary();
        public bool HeaderColumn1Visiblity { get; set; } = true;
        public string HeaderColumn2Value { get; set; } = "LangKeyFollowingsCount".FromResourceDictionary();
        public bool HeaderColumn2Visiblity { get; set; } = true;
        public string HeaderColumn3Value { get; set; } = "LangKeyFeedCount".FromResourceDictionary();
        public bool HeaderColumn3Visiblity { get; set; } = true;
        public string HeaderColumn4Value { get; set; }
        public bool HeaderColumn4Visiblity { get; set; } = false;
        public string HeaderColumn5Value { get; set; }
        public bool HeaderColumn5Visiblity { get; set; } = false;
    }


}
