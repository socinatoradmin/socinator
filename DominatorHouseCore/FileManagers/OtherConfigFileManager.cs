#region

using System;
using System.Collections.Generic;
using System.Linq;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using ProtoBuf;

#endregion

namespace DominatorHouseCore.FileManagers
{
    public interface IOtherConfigFileManager
    {
        bool SaveOtherConfig<T>(T configModel);
        T GetOtherConfig<T>() where T : class, new();
    }

    public class OtherConfigFileManager : IOtherConfigFileManager
    {
        private readonly IProtoBuffBase _protoBuffBase;

        private readonly IFileSystemProvider _fileSystemProvider;

        public OtherConfigFileManager(IProtoBuffBase protoBuffBase, IFileSystemProvider fileSystemProvider)
        {
            _protoBuffBase = protoBuffBase;
            _fileSystemProvider = fileSystemProvider;
        }

        public bool SaveOtherConfig<T>(T configModel)
        {
            try
            {
                return WithFile<T, bool>(file =>
                {
                    using (var stream = _fileSystemProvider.Create(file))
                    {
                        Serializer.Serialize(stream, configModel);
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

        public T GetOtherConfig<T>() where T : class, new()
        {
            var configModel = new T();
            try
            {
                WithFile<T, bool>(file =>
                {
                    if (_fileSystemProvider.Exists(file))
                        configModel = _protoBuffBase.Deserialize<T>(file);

                    return true;
                });
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return configModel;
        }

        public Dictionary<Type, Tuple<object, Func<string>>> __lockAndFileByType =
            new Dictionary<Type, Tuple<object, Func<string>>>
            {
                {
                    typeof(ConfigFacebookModel),
                    Tuple.Create(new object(), (Func<string>) ConstantVariable.GetOtherFacebookSettingsFile)
                },
                {
                    typeof(PinterestModel),
                    Tuple.Create(new object(), (Func<string>) ConstantVariable.GetOtherPinterestSettingsFile)
                },
                {
                    typeof(TumblrModel),
                    Tuple.Create(new object(), (Func<string>) ConstantVariable.GetOtherTumblrSettingsFile)
                },
                {
                    typeof(TwitterModel),
                    Tuple.Create(new object(), (Func<string>) ConstantVariable.GetOtherTwitterSettingsFile)
                },
                {
                    typeof(YoutubeModel),
                    Tuple.Create(new object(), (Func<string>) ConstantVariable.GetOtherYoutubeSettingsFile)
                },
                {
                    typeof(EmailNotificationsModel),
                    Tuple.Create(new object(), (Func<string>) ConstantVariable.GetOtherEmailNotificationFile)
                },
                {
                    typeof(InstagramModel),
                    Tuple.Create(new object(), (Func<string>) ConstantVariable.GetOtherInstagramSettingsFile)
                },
                {
                    typeof(EmbeddedBrowserSettingsModel),
                    Tuple.Create(new object(), (Func<string>) ConstantVariable.GetOtherEmbeddedBrowserSettingsFile)
                }
            };

        public R WithFile<T, R>(Func<string, R> act)
        {
            Tuple<object, Func<string>> typeConfig;
            // first, try the actual type
            if (!__lockAndFileByType.TryGetValue(typeof(T), out typeConfig))
            {
                // second, try to see if it's an assignable type
                var presentBaseClass = __lockAndFileByType.Keys.Except(new[] {typeof(object)}).FirstOrDefault(
                    candidateBase => candidateBase.IsAssignableFrom(typeof(T)));
                if (presentBaseClass == default(Type)) presentBaseClass = typeof(object);
                typeConfig = __lockAndFileByType[presentBaseClass];
            }

            lock (typeConfig.Item1)
            {
                return act(typeConfig.Item2());
            }
        }
    }
}