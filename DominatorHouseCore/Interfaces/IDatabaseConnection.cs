#region

using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.DHEnum;
using SQLite;

#endregion

namespace DominatorHouseCore.Interfaces
{
    public interface IDatabaseConnection
    {
        SQLiteConnection GetSqlConnection(string accountId);
    }

    public interface IAccountDatabaseConnection : IDatabaseConnection
    {
    }

    public interface ICampaignDatabaseConnection : IDatabaseConnection
    {
    }

    public interface IGlobalDatabaseConnection
    {
        SQLiteConnection GetSqlConnection();
        SQLiteConnection GetSqlConnection(SocialNetworks networks, UserType userType);
    }
}