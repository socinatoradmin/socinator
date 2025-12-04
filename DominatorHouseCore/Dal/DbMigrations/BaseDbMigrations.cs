#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DominatorHouseCore.DatabaseHandler.CoreModels;
using SQLite;

#endregion

namespace DominatorHouseCore.Dal.DbMigrations
{
    public interface IDbMigration
    {
        void RunMigration(SQLiteConnection connection);
    }

    public abstract class BaseDbMigrations : IDbMigration
    {
        private static readonly Dictionary<string, DateTime> MigratedDbs;

        static BaseDbMigrations()
        {
            MigratedDbs = new Dictionary<string, DateTime>();
        }

        private readonly IDictionary<int, Func<SQLiteConnection, string>> _migrations;

        protected BaseDbMigrations()
        {
            _migrations = new Dictionary<int, Func<SQLiteConnection, string>>();
        }

        protected void AddMigrations(int version, Func<SQLiteConnection, string> migration)
        {
            _migrations.Add(version, migration);
        }

        public void RunMigration(SQLiteConnection connection)
        {
            if (!MigratedDbs.ContainsKey(connection.DatabasePath))
            {
                var version = GetDbVersion(connection);
                if (version == -1)
                {
                    connection.CreateTable<DbVersions>();
                    connection.Insert(new DbVersions
                        {Description = "Version table", Id = 0, MIgrationDate = DateTime.UtcNow});
                }

                foreach (var migration in _migrations.Where(a => a.Key > version))
                {
                    var descr = migration.Value(connection);
                    connection.Insert(new DbVersions
                        {Description = descr, Id = migration.Key, MIgrationDate = DateTime.UtcNow});
                }

                //Added condition(to prevent exception) for not adding same key in MigratedDBs
                if (!MigratedDbs.ContainsKey(connection.DatabasePath))
                    MigratedDbs.Add(connection.DatabasePath, DateTime.UtcNow);
            }
        }

        private int GetDbVersion(SQLiteConnection connection)
        {
            if (!File.Exists(connection.DatabasePath)) return -1;

            if (connection.DeferredQuery<int>("SELECT 1 FROM sqlite_master WHERE name=?", "DbVersions").Any())
                return connection.Table<DbVersions>().OrderByDescending(a => a.Id).FirstOrDefault()?.Id ?? 0;

            return -1;
        }
    }
}