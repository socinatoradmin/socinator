using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Utility;
using ProtoBuf;
using System.Collections.Generic;

namespace FaceDominatorCore.FDModel.AccountSelectorModel
{
    [ProtoContract]
    public class SelectOptionModel : BindableBase
    {
        private SelectAccountDetailsModel _selectAccountDetailsModel = new SelectAccountDetailsModel();

        [ProtoMember(1)]
        public SelectAccountDetailsModel SelectAccountDetailsModel
        {
            get
            {
                return _selectAccountDetailsModel;

            }
            set
            {
                SetProperty(ref _selectAccountDetailsModel, value);
            }
        }



        private List<string> _listCustomDetailsUrl = new List<string>();

        [ProtoMember(2)]
        public List<string> ListCustomDetailsUrl
        {
            get
            {
                return _listCustomDetailsUrl;
            }
            set
            {
                SetProperty(ref _listCustomDetailsUrl, value);
            }
        }

        private string _customDetailsText = string.Empty;

        [ProtoMember(3)]
        public string CustomDetailsText
        {
            get
            {
                return _customDetailsText;
            }
            set
            {
                SetProperty(ref _customDetailsText, value);
            }
        }

        private List<KeyValuePair<string, string>> _accountFriendsPair = new List<KeyValuePair<string, string>>();


        [ProtoMember(4)]
        public List<KeyValuePair<string, string>> AccountFriendsPair
        {
            get
            {
                return _accountFriendsPair;
            }
            set
            {
                SetProperty(ref _accountFriendsPair, value);
            }
        }

        private List<KeyValuePair<string, string>> _accountPagesPair = new List<KeyValuePair<string, string>>();


        [ProtoMember(5)]
        public List<KeyValuePair<string, string>> AccountPagesPair
        {
            get
            {
                return _accountPagesPair;
            }
            set
            {
                SetProperty(ref _accountPagesPair, value);
            }
        }

    }
}
