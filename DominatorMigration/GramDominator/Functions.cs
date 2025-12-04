using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DominatorMigration.GramDominator.Database;
using DominatorMigration.GramDominator.Models;

namespace DominatorMigration.GramDominator
{
    public class Functions
    {
        public List<Account> GetListOfAccounts()
        {
            Read read = new Read();
            return read.Accounts();
        }

        public void GetCampaigns()
        {

        }

        public void GetCampaignReport(string campaignId)
        {

        }

        public void GetBlacklistedUsers()
        {

        }

        public void GetWhitelistedUsers()
        {

        }
    }
}
