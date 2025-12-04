using DominatorHouseCore.ViewModel;
using System.Windows;
using System.Windows.Controls;
using Unity;

namespace DominatorHouse.Social
{
    public class TablifiedContentControl : UserControl
    {
        private static readonly Style TablifiedContentControlStyle = (Style)Application.Current.FindResource("TablifiedContentControlStyle");

        public TablifiedContentControl()
        {
            Style = TablifiedContentControlStyle;
        }
    }

    public class TablifiedContentControl<T> : TablifiedContentControl
        where T : ITabViewModel
    {
        public TablifiedContentControl(IUnityContainer container)
        {
            DataContext = container.Resolve<ITablifiedContentControlViewModel<T>>();
        }
    }
}
