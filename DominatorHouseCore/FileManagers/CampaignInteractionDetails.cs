#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorHouseCore.ViewModel;

#endregion

namespace DominatorHouseCore.FileManagers
{
    /// <summary>
    ///     Thread-safe layer to manage network-related interacted data
    /// </summary>
    public class CampaignInteractionDetails : ICampaignInteractionDetails
    {
        private readonly IBinFileHelper _binFileHelper;

        private readonly IReadOnlyDictionary<SocialNetworks, Lazy<Dictionary<string, CampaignInteractionDataModel>>>
            _campaignInteractedCollections;

        private readonly IReadOnlyDictionary<SocialNetworks, object> _networkLocks;

        public CampaignInteractionDetails(IBinFileHelper binFileHelper)
        {
            _binFileHelper = binFileHelper;
            _campaignInteractedCollections = Enum.GetValues(typeof(SocialNetworks)).Cast<SocialNetworks>().ToDictionary(
                a => a, a => new Lazy<Dictionary<string, CampaignInteractionDataModel>>(
                    () =>
                    {
                        var saveData = _binFileHelper.GetCampaignInteractedDetails(a);
                        if (saveData?.Count > 0)
                            return saveData[0].CampaignInteractedCollections;

                        return new Dictionary<string, CampaignInteractionDataModel>();
                    }, LazyThreadSafetyMode.ExecutionAndPublication));
            _networkLocks = Enum.GetValues(typeof(SocialNetworks)).Cast<SocialNetworks>()
                .ToDictionary(a => a, a => new object());
        }

        public CampaignInteractionDataModel this[SocialNetworks networks, string campaignId]
        {
            get
            {
                lock (_networkLocks[networks])
                {
                    var interactedData = _campaignInteractedCollections[networks].Value;
                    if (interactedData.ContainsKey(campaignId)) return interactedData[campaignId];

                    return null;
                }
            }
        }

        public void AddInteractedData(SocialNetworks networks, string campaignId, string interactedData)
        {
            lock (_networkLocks[networks])
            {
                var hashsetValue = new SortedList<string, DateTime> {{interactedData, DateTime.Now}};
                var collections = _campaignInteractedCollections[networks].Value;

                if (collections.ContainsKey(campaignId))
                    collections[campaignId].InteractedData.Add(interactedData, DateTime.Now);
                else
                    collections.Add(campaignId,
                        new CampaignInteractionDataModel
                        {
                            CampaignId = campaignId,
                            InteractedData = hashsetValue
                        });

                UpdateInteractedData(networks);
            }
        }

        public void RemoveIfExist(SocialNetworks networks, string campaignId, string interactedData)
        {
            var model = this[networks, campaignId];
            if (model?.InteractedData?.ContainsKey(interactedData) ?? false)
            {
                model.InteractedData.Remove(interactedData);
                UpdateInteractedData(networks);
            }
        }

        private void UpdateInteractedData(SocialNetworks networks)
        {
            var campaignInteractionDataModel = new CampaignInteractionViewModel
                {CampaignInteractedCollections = _campaignInteractedCollections[networks].Value};
            var campaignInteractedData = new List<CampaignInteractionViewModel> {campaignInteractionDataModel};

            _binFileHelper.UpdateCampaignInteractedDetails(campaignInteractedData, networks);
        }

        public List<string> GetCampaignInteractedData(SocialNetworks networks, string campaignId)
        {
            var lstData = new List<string>();
            lock (_networkLocks[networks])
            {
                var collections = _campaignInteractedCollections[networks].Value;

                if (collections.ContainsKey(campaignId))
                    collections[campaignId].InteractedData.ForEach(x => lstData.Add(x.Key));
            }

            return lstData;
        }
    }
}