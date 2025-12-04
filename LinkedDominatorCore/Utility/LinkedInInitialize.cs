using System;
using System.Collections.Generic;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using LinkedDominatorCore.Interfaces;

namespace LinkedDominatorCore.Utility
{
    public class LinkedInInitialize
    {
        private static Dictionary<ActivityType, ILdBaseFactory> LdRegisteredModules { get; } =
            new Dictionary<ActivityType, ILdBaseFactory>();

        public static void LdModulesRegister(ActivityType moduleName, ILdBaseFactory LdBaseFactory)
        {
            try
            {
                if (LdRegisteredModules.ContainsKey(moduleName))
                    return;

                LdRegisteredModules.Add(moduleName, LdBaseFactory);
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

        public static ILdBaseFactory GetModuleLibrary(ActivityType moduleName)
        {
            return LdRegisteredModules.ContainsKey(moduleName) ? LdRegisteredModules[moduleName] : null;
        }
    }
}