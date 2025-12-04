#region

using System.ComponentModel;
using System.Globalization;
using System.Resources;

#endregion

namespace DominatorHouseCore.Utility
{
    internal class Resources
    {
        private static ResourceManager _resourceMan;

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        internal static ResourceManager ResourceManager =>
            _resourceMan ?? (_resourceMan =
                new ResourceManager("DominatorHouseCore.Resources", typeof(Resources).Assembly));

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        internal static CultureInfo Culture { get; set; }

        internal static string UserAccountEditPasswordNotValue =>
            ResourceManager.GetString(nameof(UserAccountEditPasswordNotValue), Culture);
    }
}