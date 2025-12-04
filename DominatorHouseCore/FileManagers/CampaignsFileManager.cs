#region

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;

#endregion

namespace DominatorHouseCore.FileManagers
{
    public interface ICampaignsFileManager : IEnumerable<CampaignDetails>
    {
        void DeleteSelectedAccount(string templateId, string accountName);
        CampaignDetails GetCampaignById(string id);
        void UpdateCampaigns(IList<CampaignDetails> libraryCampaign);
        void Add(CampaignDetails campaign);
        void Delete(CampaignDetails campaign);
        void Edit(CampaignDetails campaign);
        bool SaveTemp(DeletedCampaignTempModel campaign);
        bool RemoveTemp();
        List<DeletedCampaignTempModel> GetTemp();
    }

    public class CampaignsFileManager : ICampaignsFileManager
    {
        private readonly Lazy<List<CampaignDetails>> _campaignDetailses;
        private readonly IBinFileHelper _binFileHelper;

        public CampaignsFileManager(IBinFileHelper binFileHelper)
        {
            _binFileHelper = binFileHelper;
            _campaignDetailses = new Lazy<List<CampaignDetails>>(() =>
            {
                var result = _binFileHelper.GetCampaignDetail();
                return result;
            });
        }

        public void DeleteSelectedAccount(string templateId, string accountName)
        {
            this.ForEach(campaign =>
            {
                if (campaign.TemplateId == templateId)
                    campaign.SelectedAccountList.Remove(accountName);
            });

            Save(_campaignDetailses.Value);
        }

        public CampaignDetails GetCampaignById(string id)
        {
            return _campaignDetailses.Value.FirstOrDefault(x => x.CampaignId == id);
        }

        public void UpdateCampaigns(IList<CampaignDetails> libraryCampaign)
        {
            var all = _campaignDetailses.Value;

            // Update all entries that exists in libraryAccount, and add that does not exists
            foreach (var campaign in libraryCampaign)
            {
                var ix = all.FindIndex(a => campaign.CampaignId == a.CampaignId);
                if (ix == -1)
                    all.Add(campaign);
                else
                    all[ix] = campaign;
            }

            _binFileHelper.UpdateCampaigns(all);
        }

        public void Add(CampaignDetails campaign)
        {
            _binFileHelper.Append(campaign);
            _campaignDetailses.Value.Add(campaign);
        }

        // finds by id and delete
        public void Delete(CampaignDetails campaign)
        {
            var toDelete = _campaignDetailses.Value.FirstOrDefault(c => c.CampaignId == campaign.CampaignId);
            if (toDelete != null)
            {
                _campaignDetailses.Value.Remove(toDelete);
                Save(_campaignDetailses.Value);
            }
        }

        public void Edit(CampaignDetails campaign)
        {
            var index = _campaignDetailses.Value.FindIndex(c => c.CampaignId == campaign.CampaignId);
            if (index != -1)
            {
                _campaignDetailses.Value[index] = campaign;
                Save(_campaignDetailses.Value);
            }
        }

        public IEnumerator<CampaignDetails> GetEnumerator()
        {
            return _campaignDetailses.Value.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private void Save(List<CampaignDetails> campaigns)
        {
            _binFileHelper.UpdateCampaigns(campaigns);
        }

        public bool SaveTemp(DeletedCampaignTempModel campaign)
        {
            return _binFileHelper.SaveDeletedCampaign(campaign);
        }

        public bool RemoveTemp()
        {
            return _binFileHelper.RemoveDeletedCampaign();
        }

        public List<DeletedCampaignTempModel> GetTemp()
        {
            return _binFileHelper.GetDeletedCampaign();
        }
    }
}