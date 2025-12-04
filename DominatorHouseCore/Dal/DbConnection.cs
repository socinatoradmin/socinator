#region

using SQLite;

#endregion

namespace DominatorHouseCore.Dal
{
    public abstract class DbConnection
    {
        protected virtual SQLiteConnection GetConnection(string path)
        {
            var dbConnection = new SQLiteConnection(path);
            return dbConnection;
        }
    }
}