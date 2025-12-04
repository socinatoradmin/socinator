#region

using SQLite;

#endregion

namespace DominatorHouseCore.DatabaseHandler.Common
{
    public abstract class Entity : IPrimaryKey
    {
        [PrimaryKey]
        [AutoIncrement]
        [Indexed]
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 1)]
        public int Id { get; set; }
    }
}