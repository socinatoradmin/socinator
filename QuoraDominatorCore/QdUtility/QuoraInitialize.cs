using System;
using System.Collections.Generic;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using QuoraDominatorCore.Interface;

namespace QuoraDominatorCore.QdUtility
{
    public class QuoraInitialize
    {
        private static Dictionary<ActivityType, IQdBaseFactory> QdRegisteredModules { get; } =
            new Dictionary<ActivityType, IQdBaseFactory>();

        public static IQdBaseFactory GetModuleLibrary(ActivityType activityType)
        {
            return QdRegisteredModules.ContainsKey(activityType) ? QdRegisteredModules[activityType] : null;
        }

        public static void QdModulesRegister(ActivityType moduleName, IQdBaseFactory qdBaseFactory)
        {
            try
            {
                if (QdRegisteredModules.ContainsKey(moduleName))
                    return;

                QdRegisteredModules.Add(moduleName, qdBaseFactory);
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
    }
}