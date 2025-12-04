using DominatorHouseCore;
using DominatorHouseCore.Enums;
using System;
using System.Collections.Generic;

namespace RedditDominatorCore.Utility
{
    public class RedditInitialize
    {
        private static Dictionary<ActivityType, IRdBaseFactory> RdRegisteredModules { get; } =
            new Dictionary<ActivityType, IRdBaseFactory>();

        public static void RdModulesRegister(ActivityType moduleName, IRdBaseFactory rdBaseFactory)
        {
            try
            {
                if (RdRegisteredModules.ContainsKey(moduleName))
                    return;

                RdRegisteredModules.Add(moduleName, rdBaseFactory);
            }
            catch (ArgumentException)
            {
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public static IRdBaseFactory GetModuleLibrary(ActivityType moduleName)
        {
            return RdRegisteredModules.ContainsKey(moduleName) ? RdRegisteredModules[moduleName] : null;
        }
    }
}