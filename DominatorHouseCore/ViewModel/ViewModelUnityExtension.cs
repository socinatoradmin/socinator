#region

using DominatorHouseCore.ViewModel.DashboardVms;
using Unity;
using Unity.Extension;

#endregion

namespace DominatorHouseCore.ViewModel
{
    public class ViewModelUnityExtension : UnityContainerExtension
    {
        protected override void Initialize()
        {
            Container.RegisterSingleton<IDashboardViewModel, RevisionHistoryViewModel>("RevisionHistory");
            Container.RegisterSingleton<ISelectedNetworkViewModel, SelectedNetworkViewModel>();
            Container.RegisterSingleton<ILogViewModel, LogViewModel>();
        }
    }
}