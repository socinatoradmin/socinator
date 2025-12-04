#region

using System;
using System.Collections.Generic;
using System.Linq;
using CommonServiceLocator;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Utility;

#endregion

namespace DominatorHouseCore.FileManagers
{
    public static class PostlistFileManager
    {
        private static readonly IBinFileHelper BinFileHelper;

        static PostlistFileManager()
        {
            BinFileHelper = InstanceProvider.GetInstance<IBinFileHelper>();
        }

        // Saves all destinations. Have to work Only in Social library. Otherwise use UpdateDestinations() method to update PublisherDestinations.bin
        // NOTE: make sure lstPublisherDetails contains all destinations
        internal static void SaveAll(string campaignId, List<PublisherPostlistModel> lstPublisherDetails)
        {
            // Warning: make sure lstPublisherDetails contains all publisher            
            BinFileHelper.UpdateAllPostlists(campaignId, lstPublisherDetails);
        }


        // Update publisher entries and save to PublisherDestinations.bin        
        public static void UpdatePostlists(string campaignId, IList<PublisherPostlistModel> libraryposts)
        {
            var all = BinFileHelper.GetPublisherPostListModels(campaignId);

            // Update all entries that exists in libraryDestinations, and add that does not exists
            for (var i = 0; i < libraryposts.Count; i++)
            {
                var acc = libraryposts[i];
                var ix = all.FindIndex(a => acc.PostId == a.PostId);
                if (ix == -1)
                    all.Add(acc);
                else
                    all[ix] = acc;
            }

            BinFileHelper.UpdateAllPostlists(campaignId, all);
        }


        public static void UpdatePost(string campaignId, PublisherPostlistModel posts)
        {
            var all = BinFileHelper.GetPublisherPostListModels(campaignId);

            var requiredPost = all.FindIndex(a => posts.PostId == a.PostId);

            all[requiredPost] = posts;

            BinFileHelper.UpdateAllPostlists(campaignId, all);
        }


        public static List<PublisherPostlistModel> GetAll(string campaignId)
        {
            return BinFileHelper.GetPublisherPostListModels(campaignId);
        }

        public static PublisherPostlistModel GetByPostId(string campaignId, string postId)
        {
            var lst = GetAll(campaignId);
            return lst.FirstOrDefault(x => x.PostId == postId);
        }

        public static bool Add(string campaignId, PublisherPostlistModel postlist)
        {
            var lst = GetAll(campaignId) ?? new List<PublisherPostlistModel>();
            lst.Add(postlist);
            BinFileHelper.UpdateAllPostlists(campaignId, lst);
            return true;
        }

        public static bool AddRange(string campaignId, List<PublisherPostlistModel> postlist)
        {
            var lst = GetAll(campaignId) ?? new List<PublisherPostlistModel>();
            lst.AddRange(postlist);
            BinFileHelper.UpdateAllPostlists(campaignId, lst);
            return true;
        }

        public static void DeleteSelected(string campaignId, List<PublisherPostlistModel> postlistModels)
        {
            var all = GetAll(campaignId).Where(a => postlistModels.FirstOrDefault(p => p.PostId == a.PostId) == null)
                .ToList();
            SaveAll(campaignId, all);
        }

        public static void Delete(string campaignId, Predicate<PublisherPostlistModel> match)
        {
            var postList = GetAll(campaignId);
            postList.RemoveAll(match);
            BinFileHelper.UpdateAllPostlists(campaignId, postList);
        }
    }
}