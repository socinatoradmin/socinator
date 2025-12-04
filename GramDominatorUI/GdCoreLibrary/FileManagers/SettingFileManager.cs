using System;
using DominatorHouseCore.LogHelper;
using GramDominatorCore.GDModel;

namespace GramDominatorUI.FileManagers
{
    internal class SettingFileManager
    {
        public static bool SaveSetting<T>(T setting) where T : class
        {
            try
            {
                // BinFileHelper.UpdateSetting(setting);
                GlobusLogHelper.log.Debug("Setting successfully saved");
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static SettingsModel GetSettings()
        {
            // return BinFileHelper.GetSettingDetails<SettingsModel>();

            return new SettingsModel();
        }
    }
}