
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;

namespace PinDominatorCore.PDFactories
{
    public class PdAccountCountFactory : IAccountCountFactory
    {
        private static PdAccountCountFactory _instance;

        private PdAccountCountFactory()
        {
        }

        public static PdAccountCountFactory Instance
            => _instance ?? (_instance = new PdAccountCountFactory());

        public string HeaderColumn5Value { get; set; }= "LangKeyPinterestAccountType".FromResourceDictionary();
        public bool HeaderColumn5Visiblity { get; set; } = true;

        public string HeaderColumn1Value { get; set; } = "LangKeyFollowersCount".FromResourceDictionary();
        public bool HeaderColumn1Visiblity { get; set; } = true;
        public string HeaderColumn2Value { get; set; } = "LangKeyFollowingsCount".FromResourceDictionary();
        public bool HeaderColumn2Visiblity { get; set; } = true;
        public string HeaderColumn3Value { get; set; } = "LangKeyBoardCount".FromResourceDictionary();
        public bool HeaderColumn3Visiblity { get; set; } = true;
        public string HeaderColumn4Value { get; set; } = "LangKeyPinCount".FromResourceDictionary();
        public bool HeaderColumn4Visiblity { get; set; } = true;
    }
}