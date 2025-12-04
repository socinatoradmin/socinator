using DominatorHouseCore.Interfaces;

namespace TumblrDominatorCore.TumblrFactory
{
    public class TumblrAccountCountFactory : IAccountCountFactory
    {
        private static TumblrAccountCountFactory _instance;

        private TumblrAccountCountFactory()
        {
        }

        public static TumblrAccountCountFactory Instance
            => _instance ?? (_instance = new TumblrAccountCountFactory());

        public string HeaderColumn1Value { get; set; } = "Follower Count";
        public bool HeaderColumn1Visiblity { get; set; } = true;
        public string HeaderColumn2Value { get; set; } = "Post Count";
        public bool HeaderColumn2Visiblity { get; set; } = true;
        public string HeaderColumn3Value { get; set; } = "Blog Count";
        public bool HeaderColumn3Visiblity { get; set; } = true;
        public string HeaderColumn4Value { get; set; } = "Following Count";
        public bool HeaderColumn4Visiblity { get; set; } = true;
        public string HeaderColumn5Value { get; set; }
        public bool HeaderColumn5Visiblity { get; set; } = false;
    }
}