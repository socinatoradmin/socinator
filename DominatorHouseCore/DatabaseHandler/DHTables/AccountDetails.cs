#region

using System;
using SQLite;

#endregion

namespace DominatorHouseCore.DatabaseHandler.DHTables
{
    public class AccountDetails
    {
        [PrimaryKey]
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 1)]
        [Indexed]
        [AutoIncrement]
        public int Id { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 2)]
        public string AccountNetwork { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 3)]
        public string AccountId { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 4)]
        public string AccountGroup { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 5)]
        public string UserName { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 6)]
        public string Password { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 7)]
        public string UserFullName { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 8)]
        public string Status { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 9)]
        public string ProxyIP { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 10)]
        public string ProxyPort { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 11)]
        public string ProxyUserName { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 12)]
        public string ProxyPassword { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 13)]
        public string ProfilePictureUrl { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 14)]
        public string Cookies { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 15)]
        public string UserAgent { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 16)]
        public DateTime AddedDate { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 17)]
        public string ActivityManager { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 18)]
        public int? DisplayColumnValue1 { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 19)]
        public int? DisplayColumnValue2 { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 20)]
        public int? DisplayColumnValue3 { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 21)]
        public int? DisplayColumnValue4 { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 22)]
        public string AccountName { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 23)]
        public string DisplayColumnValue11 { get; set; }
    }
}