#region

using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;

#endregion

namespace DominatorHouseCore.BusinessLogic.Factories
{
    public class DominatorAccountCountFactory : IAccountCountFactory
    {
        private static DominatorAccountCountFactory _instance;

        public static DominatorAccountCountFactory Instance
            => _instance ?? (_instance = new DominatorAccountCountFactory());

        private DominatorAccountCountFactory()
        {
        }

        public string HeaderColumn1Value { get; set; } = "LangKeyFriendshipCount".FromResourceDictionary();
        public bool HeaderColumn1Visiblity { get; set; } = true;
        public string HeaderColumn2Value { get; set; } = string.Empty;
        public bool HeaderColumn2Visiblity { get; set; } = false;
        public string HeaderColumn3Value { get; set; } = string.Empty;
        public bool HeaderColumn3Visiblity { get; set; } = false;
        public string HeaderColumn4Value { get; set; } = string.Empty;
        public bool HeaderColumn4Visiblity { get; set; } = false;
        public string HeaderColumn5Value { get; set; }
        public bool HeaderColumn5Visiblity { get; set; } = false;
    }
}