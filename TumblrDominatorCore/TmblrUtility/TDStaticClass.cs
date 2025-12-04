using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using System.Collections.ObjectModel;
using System.Linq;

namespace TumblrDominatorCore.TmblrUtility
{
    public static class TdStaticClass
    {
        /// <summary>
        ///     returns Tumblr Module Type - On which Tumblr activity type is going to perform
        /// </summary>
        /// <param name="actType"></param>
        /// <returns>PinterestElements</returns>
        public static TdElements GetTdElementByActivityType(this ActivityType actType)
        {
            if (actType == ActivityType.Like || actType == ActivityType.Comment || actType == ActivityType.Reblog)

                return TdElements.Post;
            if (actType == ActivityType.Follow || actType == ActivityType.BroadcastMessages)
                return TdElements.Users;
            //else if (actType == ActivityType.Unfollow)
            //{
            //    return TdElements.Unfollow;
            //}

            return TdElements.None;
        }

        /// <summary>
        ///     Get a deleting query from the list of QueryContent Just by comparing the another QueryContent with any of the list
        /// </summary>
        /// <param name="queryList">The list of QueryContent</param>
        /// <param name="queryToDelete">the another QueryContent to compare</param>
        /// <returns></returns>
        public static QueryContent GetDeletingQuery(this ObservableCollection<QueryContent> queryList,
            QueryContent queryToDelete)
        {
            return queryList.FirstOrDefault(x =>
                x.Content.QueryType == queryToDelete.Content.QueryType &&
                x.Content.QueryValue == queryToDelete.Content.QueryValue);
        }
    }

    public enum TdElements
    {
        Users,
        Post,
        Unfollow,
        None
    }
}