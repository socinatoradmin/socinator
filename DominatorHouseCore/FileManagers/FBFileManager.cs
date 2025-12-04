#region

using System;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using ProtoBuf;

#endregion

namespace DominatorHouseCore.FileManagers
{
    public interface IFBFileManager
    {
        bool SaveFacebookConfig(ConfigFacebookModel configFacebookModel);
        ConfigFacebookModel GetFacebookConfig();
    }

    public class FBFileManager : IFBFileManager
    {
        private readonly IProtoBuffBase _protoBuffBase;
        private readonly ILockFileConfigProvider _lockFileConfigProvider;
        private readonly IFileSystemProvider _fileSystemProvider;

        public FBFileManager(IProtoBuffBase protoBuffBase, ILockFileConfigProvider lockFileConfigProvider,
            IFileSystemProvider fileSystemProvider)
        {
            _protoBuffBase = protoBuffBase;
            _lockFileConfigProvider = lockFileConfigProvider;
            _fileSystemProvider = fileSystemProvider;
        }

        public bool SaveFacebookConfig(ConfigFacebookModel configFacebookModel)
        {
            try
            {
                return _lockFileConfigProvider.WithFile<ConfigFacebookModel, bool>(file =>
                {
                    using (var stream = _fileSystemProvider.Create(file))
                    {
                        Serializer.Serialize(stream, configFacebookModel);
                        return true;
                    }
                });
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return false;
        }

        public ConfigFacebookModel GetFacebookConfig()
        {
            var configFacebookModel = new ConfigFacebookModel();
            try
            {
                _lockFileConfigProvider.WithFile<ConfigFacebookModel, bool>(file =>
                {
                    if (_fileSystemProvider.Exists(file))
                        configFacebookModel = _protoBuffBase.Deserialize<ConfigFacebookModel>(file);
                    return true;
                });
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return configFacebookModel;
        }
    }
}