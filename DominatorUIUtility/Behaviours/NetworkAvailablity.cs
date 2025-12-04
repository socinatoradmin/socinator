using System.Windows;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;

namespace DominatorUIUtility.Behaviours
{
    public class NetworkAvailablity
    {
        #region Facebook

        public static readonly DependencyProperty FbElementVisibleProperty = DependencyProperty.RegisterAttached(
            "FbElementVisible", typeof(Visibility), typeof(NetworkAvailablity),
            new PropertyMetadata(FeatureFlags.Check(SocialNetworks.Facebook)));

        public static Visibility GetFbElementVisible(DependencyObject element)
        {
            return (Visibility) element.GetValue(FbElementVisibleProperty);
        }

        #endregion
    }
}