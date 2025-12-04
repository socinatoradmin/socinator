using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;

namespace LinkedDominatorCore.Factories
{
    // ReSharper disable once InconsistentNaming
    public class LDAccountCountFactory : IAccountCountFactory
    {
        private static LDAccountCountFactory _instance;

        private LDAccountCountFactory()
        {
        }

        public static LDAccountCountFactory Instance
            => _instance ?? (_instance = new LDAccountCountFactory());

        public string HeaderColumn1Value { get; set; } = "LangKeyConnectionCount".FromResourceDictionary();
        public bool HeaderColumn1Visiblity { get; set; } = true;
        public string HeaderColumn2Value { get; set; } = "LangKeyLinkedinGroupCount".FromResourceDictionary();
        public bool HeaderColumn2Visiblity { get; set; } = true;
        public string HeaderColumn3Value { get; set; } = "LangKeyInvitationsSentCount".FromResourceDictionary();
        public bool HeaderColumn3Visiblity { get; set; } = true;
        public string HeaderColumn4Value { get; set; } = "LangKeyOwnPageCount".FromResourceDictionary();
        public bool HeaderColumn4Visiblity { get; set; } = true;
        public string HeaderColumn5Value { get; set; }
        public bool HeaderColumn5Visiblity { get; set; } = false;
    }
}