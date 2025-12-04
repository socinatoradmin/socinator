using DominatorHouseCore;
using DominatorHouseCore.Enums;
using System;
using System.Collections.Generic;
using TumblrDominatorCore.Interface;

namespace TumblrDominatorCore.TmblrUtility
{
    public class TumblrInitialize
    {
        private static Dictionary<ActivityType, ITumblrBaseFactory> TumblrRegisteredModules { get; } =
            new Dictionary<ActivityType, ITumblrBaseFactory>();

        public static void TumblrModulesRegister(ActivityType moduleName, ITumblrBaseFactory tumblrBaseFactory)
        {
            try
            {
                if (TumblrRegisteredModules.ContainsKey(moduleName))
                    return;

                TumblrRegisteredModules.Add(moduleName, tumblrBaseFactory);
            }
            catch (ArgumentException ex)
            {
                ex.DebugLog();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public static ITumblrBaseFactory GetModuleLibrary(ActivityType moduleName)
        {
            return TumblrRegisteredModules.ContainsKey(moduleName) ? TumblrRegisteredModules[moduleName] : null;
        }
    }
}