#region

using System.Windows;

#endregion

namespace DominatorHouseCore.AppResources
{
    public interface IApplicationResourceProvider
    {
        string GetStringResource(string resourceKey);
    }

    public class ApplicationResourceProvider : IApplicationResourceProvider
    {
        public const string LangKeyAccountsActivity = "LangKeyAccountsActivity";
        public const string LangKeyPublisher = "LangKeyPublisher";
        public const string LangKeyAccountsManager = "LangKeyAccountsManager";
        public const string LangKeySociopublisher = "LangKeySociopublisher";
        public const string LangKeyCampaigns = "LangKeyCampaigns";

        public string GetStringResource(string resourceKey)
        {
            try
            {
                return Application.Current?.FindResource(resourceKey)?.ToString();
            }
            catch (ResourceReferenceKeyNotFoundException)
            {
                return string.Empty;
            }
        }
    }
}