#region

using System;
using System.Collections.Generic;
using System.Linq;
using CommonServiceLocator;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Utility;

#endregion

namespace DominatorHouseCore.FileManagers
{
    public static class ManageDestinationFileManager
    {
        //  private static List<PublisherManageDestinationModel> _allDestinationsCache = new List<PublisherManageDestinationModel>();
        private static readonly IBinFileHelper BinFileHelper;

        static ManageDestinationFileManager()
        {
            BinFileHelper = InstanceProvider.GetInstance<IBinFileHelper>();
        }

        // Saves all destinations. Have to work Only in Social library. Otherwise use UpdateDestinations() method to update PublisherDestinations.bin
        // NOTE: make sure lstPublisherDetails contains all destinations
        internal static void SaveAll(List<PublisherManageDestinationModel> lstPublisherDetails)
        {
            // Warning: make sure lstPublisherDetails contains all publisher            
            BinFileHelper.UpdateAllManageDestination(lstPublisherDetails);
            GlobusLogHelper.log.Debug($"{lstPublisherDetails.Count} Destination successfully saved");
        }

        // Update publisher entries and save to PublisherDestinations.bin        
        public static void UpdateDestinations(IList<PublisherManageDestinationModel> libraryDestinations)
        {
            var all = BinFileHelper.GetPublisherManageDestinationModels();

            // Update all entries that exists in libraryDestinations, and add that does not exists
            for (var i = 0; i < libraryDestinations.Count; i++)
            {
                var acc = libraryDestinations[i];
                var ix = all.FindIndex(a => acc.DestinationId == a.DestinationId);
                if (ix == -1)
                    all.Add(acc);
                else
                    all[ix] = acc;
            }

            BinFileHelper.UpdateAllManageDestination(all);
        }

        public static List<PublisherManageDestinationModel> GetAll()
        {
            return BinFileHelper.GetPublisherManageDestinationModels();
        }

        public static PublisherManageDestinationModel GetByDestinationId(string destinationId)
        {
            var lst = GetAll();
            return lst.FirstOrDefault(x => x.DestinationId == destinationId);
        }

        public static bool Add(PublisherManageDestinationModel account)
        {
            var lst = GetAll() ?? new List<PublisherManageDestinationModel>();
            lst.Add(account);
            BinFileHelper.UpdateAllManageDestination(lst);
            return true;
        }

        public static bool AddRange(List<PublisherManageDestinationModel> destinationList)
        {
            var lst = GetAll() ?? new List<PublisherManageDestinationModel>();
            lst.AddRange(destinationList);
            BinFileHelper.UpdateAllManageDestination(lst);
            return true;
        }

        public static void DeleteSelected(List<PublisherManageDestinationModel> accs)
        {
            var all = GetAll().Where(a => accs.FirstOrDefault(p => p.DestinationId == a.DestinationId) == null)
                .ToList();
            SaveAll(all);
        }

        public static void Delete(Predicate<PublisherManageDestinationModel> match)
        {
            var accs = GetAll();
            accs.RemoveAll(match);
            BinFileHelper.UpdateAllAccounts(accs);
        }
    }
}