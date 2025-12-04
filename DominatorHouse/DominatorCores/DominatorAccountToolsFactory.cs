
using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
using System.Collections.Generic;
using System.Windows.Controls;

// ReSharper disable once IdentifierTypo
namespace Socinator.DominatorCores
{
    public class DominatorAccountToolsFactory : IAccountToolsFactory
    {
        private static DominatorAccountToolsFactory _instance;

        public static DominatorAccountToolsFactory Instance
            => _instance ?? (_instance = new DominatorAccountToolsFactory());

        private DominatorAccountToolsFactory() { }

        public UserControl GetStartupToolsView()
            => new UserControl();

        public IEnumerable<ActivityType> GetImportantActivityTypes()
        {
            return new List<ActivityType>();
        }

        public IEnumerable<ActivityType> GetOtherActivityTypes()
        {
            return new List<ActivityType>();
        }

        public string RecentlySelectedAccount { get; set; } = string.Empty;
    }
}