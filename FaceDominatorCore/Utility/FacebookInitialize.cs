using DominatorHouseCore;
using DominatorHouseCore.Enums;
using FaceDominatorCore.Interface;
using System;
using System.Collections.Generic;

namespace FaceDominatorCore.Utility
{
    public class FacebookInitialize
    {
        private static Dictionary<ActivityType, IFdBaseFactory> FdRegisteredModules { get; } = new Dictionary<ActivityType, IFdBaseFactory>();

        public static void FdModulesRegister(ActivityType moduleName, IFdBaseFactory fdBaseFactory)
        {
            try
            {
                if (FdRegisteredModules.ContainsKey(moduleName))
                    return;

                FdRegisteredModules.Add(moduleName, fdBaseFactory);
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

        public static IFdBaseFactory GetModuleLibrary(ActivityType moduleName)
        {
            return FdRegisteredModules.ContainsKey(moduleName) ? FdRegisteredModules[moduleName] : null;
        }


    }




}