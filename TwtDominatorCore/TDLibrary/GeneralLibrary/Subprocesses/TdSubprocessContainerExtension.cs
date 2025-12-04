using System;
using TwtDominatorCore.TDModels;
using Unity;
using Unity.Extension;
using Unity.Injection;
using Unity.Lifetime;
using Unity.Resolution;

namespace TwtDominatorCore.TDLibrary.GeneralLibrary.Subprocesses
{
    public class TdSubprocessContainerExtension : UnityContainerExtension
    {
        protected override void Initialize()
        {
            // LikeCommentsSubprocess
            RegSubprocess<LikeCommentsSubprocess<LikeModel>, LikeModel>(a =>
                    a.IsChkEnableLikeComments ? a.LikeCommentRange.GetRandom() : 0,
                a => a.DelayBetweenLikeCommentRange.GetRandom());
            RegSubprocess<LikeCommentsSubprocess<RetweetModel>, RetweetModel>(a =>
                    a.IsChkEnableLikeComments ? a.LikeCommentRange.GetRandom() : 0,
                a => a.DelayBetweenLikeTweetRange.GetRandom());
            RegSubprocess<LikeCommentsSubprocess<CommentModel>, CommentModel>(a =>
                    a.IsChkEnableLikeComment ? a.LikeMaxRange.GetRandom() : 0,
                a => a.DelayBetweenLikeTweetRange.GetRandom());

            // RetweetCommentsSubprocess
            RegSubprocess<RetweetCommentsSubprocess<LikeModel>, LikeModel>(a =>
                    a.IsChkEnableRetweetComments ? a.RetweetCommentRange.GetRandom() : 0,
                a => a.DelayBetweenRetweetRange.GetRandom());
            RegSubprocess<RetweetCommentsSubprocess<RetweetModel>, RetweetModel>(a =>
                    a.IsChkEnableRetweetComments ? a.RetweetCommentRange.GetRandom() : 0,
                a => a.DelayBetweenRetweetRange.GetRandom());
            RegSubprocess<RetweetCommentsSubprocess<CommentModel>, CommentModel>(a =>
                    a.IsChkEnableRetweetComment ? a.RetweetMaxRange.GetRandom() : 0,
                a => a.DelayBetweenRetweetRange.GetRandom());

            // CommentOntweetSubprocess
            RegSubprocess<CommentOntweetSubprocessPerLike, LikeModel>(a =>
                    a.IsChkUploadComments ? a.CommentMaxRange.GetRandom() : 0,
                a => a.DelayBetweenCommentRange.GetRandom());
            RegSubprocess<CommentOntweetSubprocessPerRetweet, RetweetModel>(a =>
                    a.IsChkUploadComments ? a.CommentMaxRange.GetRandom() : 0,
                a => a.DelayBetweenCommentRange.GetRandom());
            RegSubprocess<CommentOnUserNotificationTweetsSubprocessPerComment, CommentModel>(a =>
                    a.IsChkCommentOnTweetsOnUserNotifications ? a.CommentOnTweetsFromUserNotificationsMaxRange.GetRandom() : 0,
                a => a.DelayBetweenCommentOnTweetsFromUserNotifications.GetRandom());

            // LikeOthersCommentsSubprocess
            RegSubprocess<LikeOthersCommentsSubprocess<CommentModel>, CommentModel>(a =>
                    a.IsChkEnableLikeOthersComment ? a.LikeOthersCommentRange.GetRandom() : 0,
                a => a.DelayBetweenLikeOthersCommentRange.GetRandom());

            // FollowTweetOwnerSubprocess
            RegSubprocess<FollowTweetOwnerSubprocess<LikeModel>, LikeModel>(a =>
                    a.IsChkFollowTWeetOwner ? 1 : 0,
                a => 0);
            RegSubprocess<FollowTweetOwnerSubprocess<RetweetModel>, RetweetModel>(a =>
                    a.IsChkFollowTWeetOwner ? 1 : 0,
                a => 0);
            RegSubprocess<FollowTweetOwnerSubprocess<CommentModel>, CommentModel>(a =>
                    a.IsChkFollowTWeetOwner ? 1 : 0,
                a => 0);

            //  LikeTweetSubProcess
            RegSubprocess<LikeTweetSubProcess<RetweetModel>, RetweetModel>(a =>
                    a.IsChkLikeTweet ? 1 : 0,
                a => 0);
        }

        private void RegSubprocess<TType, T>(Func<T, int> getRange, Func<T, int> getDelay)
            where TType : ISubprocess<T>
        {
            Container.RegisterType<ISubprocess<T>>($"TwtSubprocess{typeof(TType).Name}{typeof(T).Name}",
                new HierarchicalLifetimeManager(),
                new InjectionFactory(container => container.Resolve<TType>(
                    new ParameterOverride("getAfterActionRange", getRange),
                    new ParameterOverride("getAfterActionDelay", getDelay))));
        }
    }
}