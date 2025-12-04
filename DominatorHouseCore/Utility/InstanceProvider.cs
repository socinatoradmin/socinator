using CommonServiceLocator;
using DominatorHouseCore.DatabaseHandler.Utility;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using System.Collections.Generic;
using Unity;
using Unity.Resolution;

namespace DominatorHouseCore.Utility
{
    public static class InstanceProvider
    {
        public static IServiceLocator Current => ServiceLocator.Current;
        public static T GetInstance<T>()
        {
            return Current.GetInstance<T>();
        }
        public static T GetInstance<T>(string key)
        {
            return Current.GetInstance<T>(key);
        }
        public static IEnumerable<T> GetAllInstance<T>()
        {
            return Current.GetAllInstances<T>();
        }
        public static IDbOperations ResolveAccountDbOperations(string accountId,SocialNetworks networks)
        {
            return Current.GetInstance<IUnityContainer>().Resolve<IDbOperations>(
                new ParameterOverride("id", accountId),
                new ParameterOverride("networks", networks),
                new ParameterOverride("type", ConstantVariable.GetAccountDb));
        }
        public static T ResolveWithDominatorAccount<T>(DominatorAccountModel model)
        {
            return Current.GetInstance<IUnityContainer>()
                .Resolve<T>(new ParameterOverride("dominatorAccountModel", model));
        }
        public static IDbOperations ResolveCampaignDbOperations(string campaignId,SocialNetworks networks)
        {
            return Current.GetInstance<IUnityContainer>().Resolve<IDbOperations>(
                new ParameterOverride("id", campaignId),
                new ParameterOverride("networks", networks),
                new ParameterOverride("type", ConstantVariable.GetCampaignDb));
        }
        public static T ResolveWithAccountFactory<T>(string KeyID, string Name = "")
        {
            var accountFactory = Current.GetInstance<IAccountScopeFactory>();
            return string.IsNullOrEmpty(Name) ? accountFactory[KeyID].Resolve<T>() : accountFactory[KeyID].Resolve<T>(Name);
        }
        public static ModuleConfiguration ResolveModuleConfiguration(string AccountId, ActivityType activity)
        {
            var iJobActivityConfigurationManager = Current.GetInstance<IJobActivityConfigurationManager>();
            return iJobActivityConfigurationManager[AccountId, activity];
        }
    }
}
