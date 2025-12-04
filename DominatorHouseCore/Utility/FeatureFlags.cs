#region

using System;
using System.Collections.Generic;
using System.Windows;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;

#endregion

namespace DominatorHouseCore.Utility
{
    public class FeatureFlags : Dictionary<string, bool>
    {
        public static FeatureFlags Instance;

        public static bool Check(string key, Action whenEnabled = null, Action whenDisabled = null)
        {
            if (!Instance.ContainsKey(key)) Instance[key] = false;

            var value = Instance[key];
            if (value)
                whenEnabled?.Invoke();
            else
                whenDisabled?.Invoke();
            return value;
        }

        public static void UpdateFeatures()
        {
            Instance = new FeatureFlags {{"SocinatorInitializer", true}};

            SocinatorInitialize.AvailableNetworks.ForEach(networks => { Instance.Add(networks.ToString(), true); });
        }

        public static bool Check(string key)
        {
            try
            {
                return Instance.ContainsKey(key);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool IsNetworkAvailable(SocialNetworks network)
        {
            try
            {
                return Instance.ContainsKey(network.ToString());
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static Visibility Check(SocialNetworks network)
        {
            try
            {
                return Instance.ContainsKey(network.ToString()) ? Visibility.Visible : Visibility.Collapsed;
            }
            catch (Exception)
            {
                return Visibility.Collapsed;
            }
        }
    }
}