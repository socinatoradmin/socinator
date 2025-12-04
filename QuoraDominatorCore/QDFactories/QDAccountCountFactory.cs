using DominatorHouseCore.Interfaces;

namespace QuoraDominatorCore.QDFactories
{
    public class QdAccountCountFactory : IAccountCountFactory
    {
        private static QdAccountCountFactory _instance;

        private QdAccountCountFactory()
        {
        }

        public static QdAccountCountFactory Instance
            => _instance ?? (_instance = new QdAccountCountFactory());

        public string HeaderColumn1Value { get; set; } = "Follower Count";
        public bool HeaderColumn1Visiblity { get; set; } = true;
        public string HeaderColumn2Value { get; set; } = "Following Count";
        public bool HeaderColumn2Visiblity { get; set; } = true;
        public string HeaderColumn3Value { get; set; } = "Post Count";
        public bool HeaderColumn3Visiblity { get; set; } = true;
        public string HeaderColumn4Value { get; set; }
        public bool HeaderColumn4Visiblity { get; set; } = false;
        public string HeaderColumn5Value { get; set; }
        public bool HeaderColumn5Visiblity { get; set; } = false;
    }
}