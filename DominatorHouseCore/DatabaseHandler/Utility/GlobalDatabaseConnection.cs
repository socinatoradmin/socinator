#region

using DominatorHouseCore.Dal;
using DominatorHouseCore.Dal.DbMigrations;
using DominatorHouseCore.DatabaseHandler.DHTables;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.DHEnum;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using SQLite;

#endregion

namespace DominatorHouseCore.DatabaseHandler.Utility
{
    public class GlobalDatabaseConnection : VersionedDbConnection, IGlobalDatabaseConnection
    {
        private readonly IGlobalDatabaseBlackListMigrations _globalDatabaseBlackListMigrations;
        private readonly IGlobalDatabaseWhiteListMigrations _globalDatabaseWhiteListMigrations;

        public GlobalDatabaseConnection(IGlobalDatabaseMigrations dbMigration,
            IGlobalDatabaseBlackListMigrations globalDatabaseBlackListMigrations,
            IGlobalDatabaseWhiteListMigrations globalDatabaseWhiteListMigrations) : base(dbMigration)
        {
            _globalDatabaseBlackListMigrations = globalDatabaseBlackListMigrations;
            _globalDatabaseWhiteListMigrations = globalDatabaseWhiteListMigrations;
        }


        public SQLiteConnection GetSqlConnection()
        {
            var directoryName = ConstantVariable.GetPlatformBaseDirectory() + @"\Index\Global\DB";
            DirectoryUtilities.CreateDirectory(directoryName);
            var connectionString = directoryName + "\\Global.db";
            return GetSqlConnectionAndRunMigration(connectionString);
        }

        public SQLiteConnection GetSqlConnection(SocialNetworks networks, UserType userType)
        {
            var directoryName = ConstantVariable.GetPlatformBaseDirectory() + $"\\Index\\Global\\DB\\{userType}";
            var connectionString = directoryName + $"\\{networks}.db";
            DirectoryUtilities.CreateDirectory(directoryName);
            var dbConnection = GetConnection(connectionString);
            if (userType == UserType.BlackListedUser)
            {
                dbConnection.CreateTable<BlackListUser>();
                _globalDatabaseBlackListMigrations.RunMigration(dbConnection);
            }
            else
            {
                dbConnection.CreateTable<WhiteListUser>();
                _globalDatabaseWhiteListMigrations.RunMigration(dbConnection);
            }

            return dbConnection;
        }
    }
}