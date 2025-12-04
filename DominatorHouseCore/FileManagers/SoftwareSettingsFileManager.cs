#region

using System;
using System.IO;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using ProtoBuf;

#endregion

namespace DominatorHouseCore.FileManagers
{
    public interface ISoftwareSettingsFileManager
    {
        bool SaveSoftwareSettings(SoftwareSettingsModel softwareSetting);
        SoftwareSettingsModel GetSoftwareSettings();
    }

    public class SoftwareSettingsFileManager : ISoftwareSettingsFileManager
    {
        public bool SaveSoftwareSettings(SoftwareSettingsModel softwareSetting)
        {
            try
            {
                using (var stream = File.Create(ConstantVariable.GetOtherSoftwareSettingsFile()))
                {
                    Serializer.Serialize(stream, softwareSetting);
                    return true;
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return false;
            }
        }

        public SoftwareSettingsModel GetSoftwareSettings()
        {
            try
            {
                using (var stream = File.OpenRead(ConstantVariable.GetOtherSoftwareSettingsFile()))
                {
                    return Serializer.Deserialize<SoftwareSettingsModel>(stream);
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return new SoftwareSettingsModel();
        }
    }
}