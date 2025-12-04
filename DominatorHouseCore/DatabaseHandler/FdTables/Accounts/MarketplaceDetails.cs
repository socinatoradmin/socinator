#region

using SQLite;

#endregion

namespace DominatorHouseCore.DatabaseHandler.FdTables.Accounts
{
    public class MarketplaceDetails
    {
        [PrimaryKey]
        [AutoIncrement]
        [Indexed]
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 1)]
        public int Id { get; set; }


        /// <summary>
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 2)]
        //[Unique]
        public string LocationName { get; set; }


        /// <summary>
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 3)]
        public string FbLocationName { get; set; }

        /// <summary>
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 4)]
        public string FbLocationCurrency { get; set; }

        /// <summary>
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 5)]
        public long FbLocationId { get; set; }

        /// <summary>
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 6)]
        public bool IsMarketPlaceAvailable { get; set; }
    }
}