using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using GramDominatorCore.Report;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GramDominatorCore.GDModel
{
    public class GetReportFollowCountModel : BindableBase
    {
        private List<string> _lstAccounts = new List<string>();
        public List<string> LstAccounts
        {
            get
            {
                return _lstAccounts;
            }
            set
            {
                if (_lstAccounts == value)
                    return;
                SetProperty(ref _lstAccounts, value);

            }
        }

        private ObservableCollection<ContentSelectGroup> _lstQuery = new ObservableCollection<ContentSelectGroup>();
        public ObservableCollection<ContentSelectGroup> LstQuery
        {
            get
            {
                return _lstQuery;
            }
            set
            {

                if (_lstQuery == value)
                    return;
                SetProperty(ref _lstQuery, value);
            }
        }

        private ObservableCollection<SelectAccountModel> _lstSelectQuery = new ObservableCollection<SelectAccountModel>();
        public ObservableCollection<SelectAccountModel> LstSelectQuery
        {
            get { return _lstSelectQuery; }
            set { SetProperty(ref _lstSelectQuery, value); }
        }
     
        private List<GetReportModel> _lstReportModel=new List<GetReportModel>();

        public List<GetReportModel> lstReportModel
        {
            get
            {
                return _lstReportModel;
            }
            set
            {
                if (_lstReportModel == value)
                    return;
                SetProperty(ref _lstReportModel, value);
            }

        }
    }
    

    public class GetReportModel:BindableBase
    {
        private int _RowNo;
        public int RowNo
        {
            get
            {
                return _RowNo;
            }
            set
            {
                if (_RowNo == value)
                    return;
                SetProperty(ref _RowNo, value);

            }
        }
        private string _NoOfFollowed;
        public string NoOfFollowed
        {
            get
            {
                return _NoOfFollowed;
            }
            set
            {
                if (_NoOfFollowed == value)
                    return;
                SetProperty(ref _NoOfFollowed, value);

            }
        }

        private string _NoOfFollowedBack;
        public string NoOfFollowedBack
        {
            get
            {
                return _NoOfFollowedBack;
            }
            set
            {
                if (_NoOfFollowedBack == value)
                    return;
                SetProperty(ref _NoOfFollowedBack, value);

            }
        }
        private string _userName;
        public string UserName
        {
            get
            {
                return _userName;
            }
            set
            {
                if (_userName == value)
                    return;
                SetProperty(ref _userName, value);

            }
        }

        private string _Query;
        public string Query
        {
            get
            {
                return _Query;
            }
            set
            {
                if (_Query == value)
                    return;
                SetProperty(ref _Query, value);

            }
        }

        private string _QueryType;
        public string QueryType
        {
            get
            {
                return _QueryType;
            }
            set
            {
                if (_QueryType == value)
                    return;
                SetProperty(ref _QueryType, value);

            }
        }
    }
}
