using System;
using System.Collections.Generic;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using TwtDominatorCore.Interface;

namespace TwtDominatorCore.TDUtility
{
    public class TDInitialise
    {
        private static Dictionary<ActivityType, ITDBaseFactory> TDRegisteredModules { get; } =
            new Dictionary<ActivityType, ITDBaseFactory>();

        public static void TDModulesRegister(ActivityType moduleName, ITDBaseFactory TdBaseFactory)
        {
            try
            {
                if (TDRegisteredModules.ContainsKey(moduleName))
                    return;

                TDRegisteredModules.Add(moduleName, TdBaseFactory);
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

        public static ITDBaseFactory GetModuleLibrary(ActivityType moduleName)
        {
            return TDRegisteredModules.ContainsKey(moduleName) ? TDRegisteredModules[moduleName] : null;
        }

        // checking array out of bound in TDRegisteredModules dictionary
        //try
        //{
        //    var demo = TDRegisteredModules.Select(x => x.Key).ToList();
        //}
        //catch (Exception ex)
        //{
        //}
    }
}