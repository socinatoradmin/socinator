#region

using System.Linq;
using DominatorHouseCore.ViewModel.Common;
using Unity;

#endregion

namespace DominatorHouseCore.ViewModel
{
    public interface ITablifiedContentControlViewModel<T>
        where T : ITabViewModel
    {
    }

    public class TablifiedContentControlViewModel<T> : SelectableViewModel<T>, ITablifiedContentControlViewModel<T>
        where T : ITabViewModel
    {
        public TablifiedContentControlViewModel(IUnityContainer container) : base(container.ResolveAll<T>(),
            container.ResolveAll<T>().FirstOrDefault())
        {
        }
    }
}