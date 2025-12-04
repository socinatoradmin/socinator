#region

using System;
using System.Collections.Generic;
using CommonServiceLocator;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;

#endregion

namespace DominatorHouseCore.FileManagers
{
    public static class PostFileManager
    {
        private static readonly IBinFileHelper BinFileHelper;

        static PostFileManager()
        {
            BinFileHelper = InstanceProvider.GetInstance<IBinFileHelper>();
        }

        public static bool SavePost<T>(T post) where T : class
        {
            try
            {
                BinFileHelper.SavePosts(post);
                GlobusLogHelper.log.Info("LangKeyPostSuccessfullySaved".FromResourceDictionary());
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static List<AddPostModel> GetAllPost()
        {
            return BinFileHelper.GetPostDetails();
        }


        public static void EditPost(AddPostModel post)
        {
            BinFileHelper.UpdatePost(post);
        }

        public static void Delete(Predicate<AddPostModel> match)
        {
            var posts = BinFileHelper.GetPostDetails();
            posts.RemoveAll(match);
            BinFileHelper.UpdateAllPosts(posts);
        }
    }
}