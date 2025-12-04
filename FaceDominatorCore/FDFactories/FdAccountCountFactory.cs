using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;

namespace FaceDominatorCore.FDFactories
{
    public class FdAccountCountFactory : IAccountCountFactory
    {
        private static FdAccountCountFactory _instance;

        public static FdAccountCountFactory Instance
            => _instance ?? (_instance = new FdAccountCountFactory());

        private FdAccountCountFactory() { }

        public string HeaderColumn1Value { get; set; } = "LangKeyFriendCount".FromResourceDictionary();

        public bool HeaderColumn1Visiblity { get; set; } = true;

        public string HeaderColumn2Value { get; set; } = "LangKeyGroupCount".FromResourceDictionary();

        public bool HeaderColumn2Visiblity { get; set; } = true;

        public string HeaderColumn3Value { get; set; } = "LangKeyPageCount".FromResourceDictionary();

        public bool HeaderColumn3Visiblity { get; set; } = true;

        public string HeaderColumn4Value { get; set; }

        public bool HeaderColumn4Visiblity { get; set; } = false;
        public string HeaderColumn5Value { get; set; }
        public bool HeaderColumn5Visiblity { get; set; } = false;
    }
}
