#region

using System.Collections.Generic;
using DominatorHouseCore.Enums;
using DominatorHouseCore.ViewModel.Common;

#endregion

namespace DominatorHouseCore.ViewModel
{
    public interface ISelectedNetworkViewModel : ISelectableViewModel<SocialNetworks?>
    {
    }

    public class SelectedNetworkViewModel : SelectableViewModel<SocialNetworks?>, ISelectedNetworkViewModel
    {
        public SelectedNetworkViewModel() : base(new List<SocialNetworks?>())
        {
        }
    }
}