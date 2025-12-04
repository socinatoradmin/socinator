using System;
using System.Collections.Generic;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using GramDominatorCore.Interface;

namespace GramDominatorCore.Utility
{
    public class InstagramInitialize
    {
        private static Dictionary<ActivityType, IGdBaseFactory> GdRegisteredModules { get; } = new Dictionary<ActivityType, IGdBaseFactory>();

        public static void GdModulesRegister(ActivityType moduleName, IGdBaseFactory gdBaseFactory)
        {
            try
            {
                if (GdRegisteredModules.ContainsKey(moduleName))
                    return;

                GdRegisteredModules.Add(moduleName, gdBaseFactory);
            }
            catch (AggregateException)
            {

            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public static IGdBaseFactory GetModuleLibrary(ActivityType moduleName)
        {
            return GdRegisteredModules.ContainsKey(moduleName) ? GdRegisteredModules[moduleName] : null;
        }
    }
}
