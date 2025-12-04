using DominatorHouseCore.Interfaces;

namespace RedditDominatorCore.RDFactories
{
    public class RdAccountCountFactory : IAccountCountFactory
    {
        private static RdAccountCountFactory _instance;

        private RdAccountCountFactory()
        {
        }

        public static RdAccountCountFactory Instance
            => _instance ?? (_instance = new RdAccountCountFactory());

        public string HeaderColumn1Value { get; set; } = "Karma";
        public bool HeaderColumn1Visiblity { get; set; } = true;
        public string HeaderColumn2Value { get; set; } = "Communities Count";
        public bool HeaderColumn2Visiblity { get; set; } = true;
        public string HeaderColumn3Value { get; set; } = "Post Karma";
        public bool HeaderColumn3Visiblity { get; set; } = true;
        public string HeaderColumn4Value { get; set; } = "Comment Karma";
        public bool HeaderColumn4Visiblity { get; set; } = true;
        public string HeaderColumn5Value { get; set; }
        public bool HeaderColumn5Visiblity { get; set; } = false;
    }
}