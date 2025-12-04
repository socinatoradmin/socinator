using DominatorHouseCore;
using DominatorHouseCore.Enums;
using System;
using System.Collections.Generic;
using YoutubeDominatorCore.Interface;

namespace YoutubeDominatorCore.YDUtility
{
    public class YoutubeInitialize
    {
        private static Dictionary<ActivityType, IYdBaseFactory> YdRegisteredModules { get; } =
            new Dictionary<ActivityType, IYdBaseFactory>();

        public static void YdModulesRegister(ActivityType moduleName, IYdBaseFactory ydBaseFactory)
        {
            try
            {
                if (YdRegisteredModules.ContainsKey(moduleName))
                    return;

                YdRegisteredModules.Add(moduleName, ydBaseFactory);
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

        public static IYdBaseFactory GetModuleLibrary(ActivityType moduleName)
        {
            return YdRegisteredModules.ContainsKey(moduleName) ? YdRegisteredModules[moduleName] : null;
        }
    }
}