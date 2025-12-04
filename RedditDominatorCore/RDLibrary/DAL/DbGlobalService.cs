using DominatorHouseCore.DatabaseHandler.DHTables;
using DominatorHouseCore.DatabaseHandler.Utility;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;

namespace RedditDominatorCore.RDLibrary.DAL
{
    public interface IDbGlobalService
    {
        bool CheckForBlackListedUser(SocialNetworks network, string userName);
    }

    public class DbGlobalService : IDbGlobalService
    {
        private readonly DbOperations _dbOperations;

        public DbGlobalService()
        {
            _dbOperations = new DbOperations(SocinatorInitialize.GetGlobalDatabase().GetSqlConnection());
        }

        public bool CheckForBlackListedUser(SocialNetworks network, string userName)
        {
            return _dbOperations.Any<BlackWhiteListUser>(x =>
                x.Network == network.ToString() &&
                x.UserName == userName);
        }
    }
}