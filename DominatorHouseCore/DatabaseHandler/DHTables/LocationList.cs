#region

using SQLite;

#endregion

namespace DominatorHouseCore.DatabaseHandler.DHTables
{
    public class LocationList
    {
        [PrimaryKey]
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 1)]
        [Indexed]
        [AutoIncrement]
        public int Id { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 2)]
        public string CountryName { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 3)]
        public string CityName { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 4)]
        public bool IsSelected { get; set; }
    }
}