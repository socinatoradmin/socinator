#region

using System;
using System.Collections.Generic;
using System.Linq;
using DominatorHouseCore.Models;
using DominatorHouseCore.Models.Publisher.CampaignsAdvanceSetting;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.ViewModel;
using InstagramModel = DominatorHouseCore.Models.Publisher.CampaignsAdvanceSetting.InstagramModel;
using PinterestModel = DominatorHouseCore.Models.Publisher.CampaignsAdvanceSetting.PinterestModel;
using TumblrModel = DominatorHouseCore.Models.Publisher.CampaignsAdvanceSetting.TumblrModel;
using TwitterModel = DominatorHouseCore.Models.Publisher.CampaignsAdvanceSetting.TwitterModel;

#endregion

namespace DominatorHouseCore.Utility
{
    public interface ILockFileConfigProvider
    {
        R WithFile<T, R>(Func<string, R> act);
    }

    public class LockFileConfigProvider : ILockFileConfigProvider
    {
        private readonly Dictionary<Type, Tuple<object, Func<string>>> __lockAndFileByType =
            new Dictionary<Type, Tuple<object, Func<string>>>
            {
                {
                    typeof(CampaignDetails),
                    Tuple.Create(new object(), (Func<string>) ConstantVariable.GetIndexCampaignFile)
                },
                {
                    typeof(TemplateModel),
                    Tuple.Create(new object(), (Func<string>) ConstantVariable.GetTemplatesFile)
                },
                {
                    typeof(ProxyManagerModel),
                    Tuple.Create(new object(), (Func<string>) ConstantVariable.GetOtherProxyFile)
                },
                {
                    typeof(AddPostModel),
                    Tuple.Create(new object(), (Func<string>) ConstantVariable.GetOtherPostsFile)
                },
                {
                    typeof(Configuration),
                    Tuple.Create(new object(), (Func<string>) ConstantVariable.GetOtherConfigFile)
                },

                //Todo: Following line need to delete
                {
                    typeof(PublisherAccountDetails),
                    Tuple.Create(new object(), (Func<string>) ConstantVariable.GetPublisherFile)
                },

                {
                    typeof(PublisherPostlistModel),
                    Tuple.Create(new object(), (Func<string>) ConstantVariable.GetPublisherCreatePostlistFolder)
                },

                {
                    typeof(PublisherManageDestinationModel),
                    Tuple.Create(new object(), (Func<string>) ConstantVariable.GetPublisherDestinationsFile)
                },
                {
                    typeof(PublisherCreateDestinationModel),
                    Tuple.Create(new object(), (Func<string>) ConstantVariable.GetPublisherCreateDestinationsFolder)
                },
                {
                    typeof(PublisherPostlistSettingsModel),
                    Tuple.Create(new object(), (Func<string>) ConstantVariable.GetPublisherPostlistSettingsFile)
                },
                {
                    typeof(CampaignInteractionViewModel),
                    Tuple.Create(new object(), (Func<string>) ConstantVariable.GetConfigurationDir)
                },
                {
                    typeof(GlobalInteractionViewModel),
                    Tuple.Create(new object(), (Func<string>) ConstantVariable.GetConfigurationDir)
                },
                {
                    typeof(FacebookModel),
                    Tuple.Create(new object(), (Func<string>) ConstantVariable.GetPublisherOtherConfigDir)
                },
                {
                    typeof(GeneralModel),
                    Tuple.Create(new object(), (Func<string>) ConstantVariable.GetPublisherOtherConfigDir)
                },
                {
                    typeof(GooglePlusModel),
                    Tuple.Create(new object(), (Func<string>) ConstantVariable.GetPublisherOtherConfigDir)
                },
                {
                    typeof(InstagramModel),
                    Tuple.Create(new object(), (Func<string>) ConstantVariable.GetPublisherOtherConfigDir)
                },
                {
                    typeof(PinterestModel),
                    Tuple.Create(new object(), (Func<string>) ConstantVariable.GetPublisherOtherConfigDir)
                },
                {
                    typeof(TumblrModel),
                    Tuple.Create(new object(), (Func<string>) ConstantVariable.GetPublisherOtherConfigDir)
                },
                {
                    typeof(TwitterModel),
                    Tuple.Create(new object(), (Func<string>) ConstantVariable.GetPublisherOtherConfigDir)
                },
                {
                    typeof(object),
                    Tuple.Create(new object(), (Func<string>) ConstantVariable.GetIndexAccountFile)
                },
                {
                    typeof(ConfigFacebookModel),
                    Tuple.Create(new object(), (Func<string>) ConstantVariable.GetOtherFacebookSettingsFile)
                },
                {
                    typeof(DeletedCampaignTempModel),
                    Tuple.Create(new object(), (Func<string>) ConstantVariable.GetOtherTemp)
                }

            };

        /// <summary>
        ///     Do something while locking the file that is the repository for the corresponding class
        /// </summary>
        /// <typeparam name="T">subject</typeparam>
        /// <typeparam name="R">return type</typeparam>
        /// <param name="act">action to perform</param>
        /// <returns>repeats the action returned value</returns>
        public R WithFile<T, R>(Func<string, R> act)
        {
            // first, try the actual type
            if (!__lockAndFileByType.TryGetValue(typeof(T), out var typeConfig))
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