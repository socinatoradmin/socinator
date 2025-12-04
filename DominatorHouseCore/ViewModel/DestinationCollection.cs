#region

using DominatorHouseCore.Enums;

#endregion

namespace DominatorHouseCore.ViewModel
{
    public class DestinationCollection
    {
        public int Id { get; set; }

        public string AccountName { get; set; }

        public SocialNetworks AccountNetwork { get; set; }

        public DestinationSelectionList GroupsCollection { get; set; }
    }
}