using System;
using System.Collections.Generic;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;
using ProtoBuf;

namespace PinDominatorCore.PDModel
{
    [ProtoContract]
    public class CampaignDetailsPd : BindableBase
    {
        private ObservableCollectionBase<CampaignDetailsPd> _campaignDetails =
            new ObservableCollectionBase<CampaignDetailsPd>();

        private List<string> _selectedAccountList = new List<string>();


        public CampaignDetailsPd()
        {
            CampaignId = Utilities.GetGuid();
        }

        [ProtoMember(10)] public string CampaignId { get; set; }


        [ProtoMember(1)] public string CampaignName { get; set; }


        [ProtoMember(2)] public string MainModule { get; set; }


        [ProtoMember(3)] public string SubModule { get; set; }


        [ProtoMember(4)] public SocialNetworks SocialNetworks { get; set; }

        [ProtoMember(5)]
        public List<string> SelectedAccountList
        {
            get => _selectedAccountList;
            set
            {
                if (_campaignDetails != null && _selectedAccountList == value)
                    return;
                SetProperty(ref _selectedAccountList, value);
            }
        }


        [ProtoMember(6)] public string TemplateId { get; set; }


        [ProtoMember(7)] public int CreationDate { get; set; }


        [ProtoMember(8)] public string Status { get; set; }


        [ProtoMember(9)] public int LastEditedDate { get; set; }

        public ObservableCollectionBase<CampaignDetailsPd> ObjCampaignDetails
        {
            get => _campaignDetails;
            set
            {
                if (_campaignDetails != null && _campaignDetails == value)
                    return;
                SetProperty(ref _campaignDetails, value);
            }
        }
    }
}