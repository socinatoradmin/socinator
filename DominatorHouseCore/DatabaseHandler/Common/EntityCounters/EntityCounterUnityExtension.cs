#region

using Unity;
using Unity.Extension;

#endregion

namespace DominatorHouseCore.DatabaseHandler.Common.EntityCounters
{
    public class EntityCounterUnityExtension : UnityContainerExtension
    {
        protected override void Initialize()
        {
            Container.AddNewExtension<EntityCounterFunctionRedditRegisterUnityExtension>();
            Container.AddNewExtension<EntityCounterFunctionTwitterRegisterUnityExtension>();
            Container.AddNewExtension<EntityCounterFunctionQuoraRegisterUnityExtension>();
            Container.AddNewExtension<EntityCounterFunctionFaceBookRegisterUnityExtension>();
            Container.AddNewExtension<EntityCounterFunctionTumblrRegisterUnityExtension>();
            Container.AddNewExtension<EntityCounterFunctionInstagramRegisterUnityExtension>();
            Container.AddNewExtension<EntityCounterFunctionLinkedinRegisterUnityExtension>();
            //Container.AddNewExtension<EntityCounterFunctionGplusRegisterUnityExtension>();
            Container.AddNewExtension<EntityCounterFunctionPinterestRegisterUnityExtension>();
            Container.AddNewExtension<EntityCounterFunctionYoutubeRegisterUnityExtension>();
            Container.AddNewExtension<EntityCounterFunctionTikTokRegisterUnityExtension>();
        }
    }
}