#region

using System;
using System.IO;
using CommonServiceLocator;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using ProtoBuf;

#endregion

namespace DominatorHouseCore.FileManagers
{
    public class SocinatorKeyHelper
    {
        public static FatalErrorHandler Key;

        public static bool SaveKey(FatalErrorHandler keyDetails)
        {
            try
            {
                using (var stream = File.Create(ConstantVariable.GetConfigurationKey()))
                {
                    Key = keyDetails;
                    if (keyDetails != null && (!string.IsNullOrEmpty(keyDetails?.FatalErrorMessage) && keyDetails.FatalErrorMessage.Contains("SOC")))
                        keyDetails.FatalErrorMessage = AesDecryption.EncryptKey(keyDetails.FatalErrorMessage);
                    Serializer.Serialize(stream, keyDetails);
                    return true;
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return false;
            }
        }

        public static void InitilizeKey()
        {
            try
            {
                var genericFileManager = InstanceProvider.GetInstance<IGenericFileManager>();
                var keyDetails = genericFileManager.GetModel<FatalErrorHandler>(ConstantVariable.GetConfigurationKey());
                if (keyDetails != null && (!string.IsNullOrEmpty(keyDetails?.FatalErrorMessage) && !keyDetails.FatalErrorMessage.Contains("SOC")))
                    keyDetails.FatalErrorMessage = AesDecryption.DecryptKey(keyDetails.FatalErrorMessage);
                Key = keyDetails;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}