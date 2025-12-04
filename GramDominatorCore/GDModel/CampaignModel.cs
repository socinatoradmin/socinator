using DominatorHouseCore.Utility;
using ProtoBuf;

namespace GramDominatorCore.GDModel
{
    [ProtoContract]
    public class CampaignModel : BindableBase
    {
        private string _campaignName;
       // private string _campaignType;
        private ObservableCollectionBase<string> _lstSelectedAccounts = new ObservableCollectionBase<string>();
        private bool? _status;
        //private ObservableCollectionBase<string> _lstCampaignType = new ObservableCollectionBase<string>();
        private ObservableCollectionBase<CampaignModel> _campaignDetails = new ObservableCollectionBase<CampaignModel>();


        private string _campaignId;
        [ProtoMember(1)]
        public string CampaignID
        {
            get
            {
                return _campaignId;
            }
            set
            {
                if (value == _campaignId)
                    return;
                SetProperty(ref _campaignId, value);
            }
        }
        [ProtoMember(2)]
        public string CampaignName
        {
            get
            {
                return _campaignName;
            }
            set
            {
                if (value == _campaignName)
                    return;
                SetProperty(ref _campaignName, value);
            }
        }
        //[ProtoMember(3)]
        //public string CampaignType
        //{
        //    get
        //    {
        //        return _campaignType;
        //    }
        //    set
        //    {
        //        if (value == _campaignType)
        //            return;
        //        SetProperty(ref _campaignType, value);
        //    }
        //}
        [ProtoMember(4)]
        public ObservableCollectionBase<string> LstSelectedAccounts
        {
            get
            {
                return _lstSelectedAccounts;
            }
            set
            {
                if (value == _lstSelectedAccounts)
                    return;
                SetProperty(ref _lstSelectedAccounts, value);
            }
        }
        [ProtoMember(5)]
        public bool? Status
        {
            get
            {
                return _status;
            }
            set
            {
                if (value == _status)
                    return;
                SetProperty(ref _status, value);
            }
        }

        //[ProtoMember(6)]
        //public ObservableCollectionBase<string> lstCampaignType
        //{
        //    get
        //    {
        //        return _lstCampaignType;
        //    }
        //    set
        //    {
        //        if (value == _lstCampaignType)
        //            return;
        //        SetProperty(ref _lstCampaignType, value);
        //    }
        //}

        public ObservableCollectionBase<CampaignModel> CampaignDetails
        {
            get
            {
                return _campaignDetails;
            }
            set
            {

                if (value == _campaignDetails)
                    return;
                SetProperty(ref _campaignDetails, value);
            }
        }
    }
}
