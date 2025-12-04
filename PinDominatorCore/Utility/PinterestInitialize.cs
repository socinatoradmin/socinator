using System;
using System.Collections.Generic;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using PinDominatorCore.Interface;

namespace PinDominatorCore.Utility
{
    public class PinterestInitialize
    {
        private static Dictionary<ActivityType, IPdBaseFactory> PdRegisteredModules { get; } =
            new Dictionary<ActivityType, IPdBaseFactory>();

        public static void PdModulesRegister(ActivityType moduleName, IPdBaseFactory pdBaseFactory)
        {
            try
            {
                if (PdRegisteredModules.ContainsKey(moduleName))
                    return;

                PdRegisteredModules.Add(moduleName, pdBaseFactory);
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

        public static IPdBaseFactory GetModuleLibrary(ActivityType moduleName)
        {
            return PdRegisteredModules.ContainsKey(moduleName) ? PdRegisteredModules[moduleName] : null;
        }
    }
}