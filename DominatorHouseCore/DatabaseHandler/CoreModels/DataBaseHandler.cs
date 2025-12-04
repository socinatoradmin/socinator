#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DominatorHouseCore.DatabaseHandler.FdTables.Accounts;
using DominatorHouseCore.DatabaseHandler.LdTables.Account;
using DominatorHouseCore.DatabaseHandler.TumblrTables.Account;
using DominatorHouseCore.DatabaseHandler.Utility;
using DominatorHouseCore.DatabaseHandler.YdTables.Campaign;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;
using Friendships = DominatorHouseCore.DatabaseHandler.TdTables.Accounts.Friendships;
using InteractedUsers = DominatorHouseCore.DatabaseHandler.RdTables.Accounts.InteractedUsers;

#endregion

namespace DominatorHouseCore.DatabaseHandler.CoreModels
{
    public interface IDataBaseHandler
    {
        IReadOnlyDictionary<SocialNetworks, Action<DbOperations>> DbInitialCounters { get; }
        IReadOnlyDictionary<SocialNetworks, Action<DbOperations>> DbCampaignInitialCounters { get; }
        void DeleteDatabase(IEnumerable<string> DBNames, DatabaseType? databaseType = DatabaseType.AccountType);
    }


    public class DataBaseHandler : IDataBaseHandler
    {
        #region database Helper Methodtext,

        public IReadOnlyDictionary<SocialNetworks, Action<DbOperations>> DbInitialCounters { get; } =
            new Dictionary<SocialNetworks, Action<DbOperations>>
            {
                //{SocialNetworks.Gplus,(operation) => {operation.Count<GplusTables.Accounts.Friendships>();}},
                {SocialNetworks.Twitter, operation => { operation.Count<Friendships>(); }},
                {SocialNetworks.Facebook, operation => { operation.Count<Friends>(); }},
                {SocialNetworks.Instagram, operation => { operation.Count<GdTables.Accounts.Friendships>(); }},
                {SocialNetworks.Pinterest, operation => { operation.Count<PdTables.Accounts.Friendships>(); }},
                {SocialNetworks.Quora, operation => { operation.Count<QdTables.Accounts.Friendships>(); }},
                {SocialNetworks.LinkedIn, operation => { operation.Count<Connections>(); }},
                {SocialNetworks.YouTube, operation => { operation.Count<YdTables.Accounts.Friendships>(); }},
                {SocialNetworks.Reddit, operation => { operation.Count<InteractedUsers>(); }},
                {SocialNetworks.Tumblr, operation => { operation.Count<InteractedUser>(); }},
                {SocialNetworks.TikTok, operation => { operation.Count<TtdTables.Accounts.InteractedUsers>(); }}
            };

        public IReadOnlyDictionary<SocialNetworks, Action<DbOperations>> DbCampaignInitialCounters { get; } =
            new Dictionary<SocialNetworks, Action<DbOperations>>
            {
                //{SocialNetworks.Gplus,operation=>{ operation.Count<GplusTables.Campaigns.InteractedUsersReport>();}},
                {SocialNetworks.Twitter, operation => { operation.Count<TdTables.Campaign.InteractedUsers>(); }},
                {SocialNetworks.Facebook, operation => { operation.Count<FdTables.Campaigns.InteractedUsers>(); }},
                {SocialNetworks.Instagram, operation => { operation.Count<GdTables.Campaigns.InteractedUsers>(); }},
                {SocialNetworks.Pinterest, operation => { operation.Count<PdTables.Campaigns.InteractedUsers>(); }},
                {SocialNetworks.Quora, operation => { operation.Count<QdTables.Campaigns.InteractedUsers>(); }},
                {SocialNetworks.LinkedIn, operation => { operation.Count<LdTables.Campaign.InteractedUsers>(); }},
                {SocialNetworks.YouTube, operation => { operation.Count<InteractedChannels>(); }},
                {SocialNetworks.Reddit, operation => { operation.Count<RdTables.Campaigns.InteractedUsers>(); }},
                {SocialNetworks.Tumblr, operation => { operation.Count<TumblrTables.Campaign.InteractedUser>(); }},
                {SocialNetworks.TikTok, operation => { operation.Count<TtdTables.Campaigns.InteractedUsers>(); }}
            };


        private static string GetDbPath(string DBName, string directoryName)
        {
            return directoryName + $"\\{DBName}.db";
        }

        private static string GetDirectory(DatabaseType? databaseType)
        {
            string directoryName;

            switch (databaseType)
            {
                case DatabaseType.CampaignType:
                    directoryName = ConstantVariable.GetIndexCampaignDir() + "\\DB";
                    break;
                case DatabaseType.AccountType:
                    directoryName = ConstantVariable.GetIndexAccountDir() + "\\DB";
                    break;
                default:
                    directoryName = ConstantVariable.GetPlatformBaseDirectory() + @"\Index\Global\DB";
                    break;
            }

            return directoryName;
        }

        public void DeleteDatabase(IEnumerable<string> DBNames, DatabaseType? databaseType = DatabaseType.AccountType)
        {
            var directory = GetDirectory(databaseType);
            if (Directory.Exists(directory))
                DBNames
                    .Select(name => GetDbPath(name, directory))
                    .Where(File.Exists)
                    .ForEach(File.Delete);
            // if directories are now empty, remove them
            try
            {
                DirectoryInfo parent;
                for (var dir = new DirectoryInfo(directory);
                    dir.EnumerateDirectories().FirstOrDefault() == null &&
                    dir.EnumerateFiles().FirstOrDefault() == null;
                    dir = parent)
                {
                    parent = dir.Parent;
                    dir.Delete();
                }
            }
            catch (IOException)
            {
            }
        }

        #endregion
    }
}