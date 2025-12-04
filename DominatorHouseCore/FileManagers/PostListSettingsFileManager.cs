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
    public static class PostListSettingsFileManager
    {
        private static readonly IBinFileHelper BinFileHelper;

        static PostListSettingsFileManager()
        {
            BinFileHelper = InstanceProvider.GetInstance<IBinFileHelper>();
        }

        internal static void SaveAll(List<PublisherPostlistSettingsModel> lstPublisherSettings)
        {
            // Warning: make sure lstPublisherSettings contains all settings            
            BinFileHelper.UpdateAllPostListSettings(lstPublisherSettings);
        }

        public static bool Add(PublisherPostlistSettingsModel settings)
        {
            var lst = GetAll() ?? new List<PublisherPostlistSettingsModel>();
            lst.Add(settings);
            BinFileHelper.UpdateAllPostListSettings(lst);
            return true;
        }

        public static void AddOrUpdateDestinations(PublisherPostlistSettingsModel settings)
        {
            var all = BinFileHelper.GetPublisherPostListSettingsModels() ?? new List<PublisherPostlistSettingsModel>();

            var ix = all.FindIndex(a => settings.CampaignId == a.CampaignId);
            if (ix == -1)
                all.Add(settings);
            else
                all[ix] = settings;

            BinFileHelper.UpdateAllPostListSettings(all);
        }


        public static List<PublisherPostlistSettingsModel> GetAll()
        {
            return BinFileHelper.GetPublisherPostListSettingsModels();
        }


        public static void DeleteSelected(List<PublisherPostlistSettingsModel> settings)
        {
            var all = GetAll().Where(a => settings.FirstOrDefault(p => p.CampaignId == a.CampaignId) == null).ToList();
            SaveAll(all);
        }

        public static void Delete(Predicate<PublisherPostlistSettingsModel> match)
        {
            var accs = GetAll();
            accs.RemoveAll(match);
            BinFileHelper.UpdateAllAccounts(accs);
        }
    }
}