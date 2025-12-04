using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DominatorMigration.GramDominator.Models;
using Newtonsoft.Json.Linq;

namespace DominatorMigration.GramDominator.Database
{
    public class Read
    {
        private static string databasePath = "";
        public List<Account> Accounts()
        {
            List<Account> accounts = new List<Account>();
            SQLiteConnection conread = new SQLiteConnection("Data Source=" + databasePath);
            conread.Open();

            #region Without proxy

            try
            {
                string selectSQL = "SELECT * from TblInstagram_Account WHERE ProxyId IS NULL";
                SQLiteCommand selectCommand = new SQLiteCommand(selectSQL, conread);
                SQLiteDataReader dataReader = selectCommand.ExecuteReader();
                DataSet ds = new DataSet();
                DataTable dt = new DataTable("TblInstagram_Account");
                dt.Load(dataReader);
                ds.Tables.Add(dt);
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    var accessTokens = JObject.Parse(ds.Tables[0].Rows[i][16].ToString());
                    var splitDeviceDetails = ds.Tables[0].Rows[i][14].ToString().Split(';');
                    var splitAndroidDetails = splitDeviceDetails.ToString().Split(';');
                    var splitManufacture = splitDeviceDetails[3].Split('/');
                    var manufacturer = splitManufacture[0];
                    var brand = string.Empty;
                    if (splitManufacture.Length == 2)
                    {
                        brand = splitManufacture[1];
                    }


                    var uuid = accessTokens["uuid"].ToString();
                    var csrfToken = accessTokens["csrftoken"].ToString();

                    Account data = new Account
                    {
                        AccountBaseModel = new AccountBase
                        {
                            UserName = ds.Tables[0].Rows[i][1].ToString(),
                            AccountNetwork = "Instagram",
                            AccountGroup = ds.Tables[0].Rows[i][20].ToString(),
                            Password = ds.Tables[0].Rows[i][2].ToString(),
                            UserId = ds.Tables[0].Rows[i][10].ToString().Replace("auth_user_id%2522%253A", "*")
                                .Split('*')[1].Split('%')[0],
                            UserFullName = "",
                            ProfilePictureUrl = "",
                            AccountId = "",
                            Status = ds.Tables[0].Rows[i][19].ToString(),
                            ProfileId = null
                        },
                        DisplayColumnValue1 = (int) ds.Tables[0].Rows[i][8],
                        DisplayColumnValue2 = (int) ds.Tables[0].Rows[i][7],
                        DisplayColumnValue4 = (int) ds.Tables[0].Rows[i][6],


                        DeviceDetails = new DeviceGenerator
                        {
                            AndroidRelease = accessTokens["AndroidRelease"].ToString(),
                            AndroidVersion = accessTokens["AndroidVersion"].ToString(),
                            Device = splitDeviceDetails[5],
                            DeviceId = accessTokens["deviceId"].ToString(),
                            Manufacturer = accessTokens["Manufacturer"].ToString(),
                            Model = accessTokens["Model"].ToString(),
                            PhoneId = accessTokens["phoneId"].ToString(),
                            Useragent = ds.Tables[0].Rows[i][14].ToString(),
                            Brand = brand,
                            Cpu = splitDeviceDetails[6],
                            Dpi = splitDeviceDetails[1],
                            ManufacturerBrand = splitManufacture[0],
                            Resolution = splitDeviceDetails[2],
                            AdId = accessTokens["adid"].ToString(),
                            Guid = accessTokens["guid"].ToString(),

                        },
                        Cookies = ds.Tables[0].Rows[i][10].ToString()
                    };
                    accounts.Add(data);
                }
            }
            catch (Exception ex)
            {
                
            }


            try
            {
                string selectSQL = "SELECT * from TblInstagram_Account INNER JOIN TblProxy ON TblProxy.Id != TblInstagram_Account.ProxyId";
                SQLiteCommand selectCommand = new SQLiteCommand(selectSQL, conread);
                SQLiteDataReader dataReader = selectCommand.ExecuteReader();
                DataSet ds = new DataSet();
                DataTable dt = new DataTable("TblInstagram_Account");
                dt.Load(dataReader);
                ds.Tables.Add(dt);
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    var accessTokens = JObject.Parse(ds.Tables[0].Rows[i][16].ToString());
                    var splitDeviceDetails = ds.Tables[0].Rows[i][14].ToString().Split(';');
                    var splitAndroidDetails = splitDeviceDetails.ToString().Split(';');
                    var splitManufacture = splitDeviceDetails[3].Split('/');
                    var manufacturer = splitManufacture[0];
                    var brand = string.Empty;
                    if (splitManufacture.Length == 2)
                    {
                        brand = splitManufacture[1];
                    }


                    var uuid = accessTokens["uuid"].ToString();
                    var csrfToken = accessTokens["csrftoken"].ToString();

                    Account data = new Account
                    {
                        AccountBaseModel = new AccountBase
                        {
                            UserName = ds.Tables[0].Rows[i][1].ToString(),
                            AccountNetwork = "Instagram",
                            AccountGroup = ds.Tables[0].Rows[i][20].ToString(),
                            Password = ds.Tables[0].Rows[i][2].ToString(),
                            UserId = ds.Tables[0].Rows[i][10].ToString().Replace("auth_user_id%2522%253A", "*")
                                .Split('*')[1].Split('%')[0],
                            UserFullName = "",
                            ProfilePictureUrl = "",
                            AccountId = "",
                            Status = ds.Tables[0].Rows[i][19].ToString(),
                            ProfileId = null
                        },
                        DisplayColumnValue1 = (int)ds.Tables[0].Rows[i][8],
                        DisplayColumnValue2 = (int)ds.Tables[0].Rows[i][7],
                        DisplayColumnValue4 = (int)ds.Tables[0].Rows[i][6],


                        DeviceDetails = new DeviceGenerator
                        {
                            AndroidRelease = accessTokens["AndroidRelease"].ToString(),
                            AndroidVersion = accessTokens["AndroidVersion"].ToString(),
                            Device = splitDeviceDetails[5],
                            DeviceId = accessTokens["deviceId"].ToString(),
                            Manufacturer = accessTokens["Manufacturer"].ToString(),
                            Model = accessTokens["Model"].ToString(),
                            PhoneId = accessTokens["phoneId"].ToString(),
                            Useragent = ds.Tables[0].Rows[i][14].ToString(),
                            Brand = brand,
                            Cpu = splitDeviceDetails[6],
                            Dpi = splitDeviceDetails[1],
                            ManufacturerBrand = splitManufacture[0],
                            Resolution = splitDeviceDetails[2],
                            AdId = accessTokens["adid"].ToString(),
                            Guid = accessTokens["guid"].ToString(),

                        },
                        Proxy = new Proxy
                        {
                            IP = ds.Tables[0].Rows[i][24].ToString(),
                            Port = ds.Tables[0].Rows[i][25].ToString(),
                            Username = ds.Tables[0].Rows[i][26].ToString(),
                            Password = ds.Tables[0].Rows[i][27].ToString()
                        },
                        Cookies = ds.Tables[0].Rows[i][10].ToString()
                    };
                    accounts.Add(data);
                }
            }
            catch (Exception ex)
            {

            }
            #endregion




            return accounts;
        }

    public void Campaigns()
    {
        //string fullPath = "C:\\Users\\GLB-104\\Downloads\\Telegram Desktop\\GramData.db";
        //SQLiteConnection conread = new SQLiteConnection("Data Source=" + fullPath);
        //conread.Open();

        //string selectSQL = "SELECT * from TblCampaignDetails INNER JOIN TblCampaigns ON TblCampaigns.Id = TblCampaignDetails.CampaignsId group By RunningForCampaignId";
        //SQLiteCommand selectCommand = new SQLiteCommand(selectSQL, conread);
        //SQLiteDataReader dataReader = selectCommand.ExecuteReader();
        //DataSet ds = new DataSet();
        //DataTable dt = new DataTable("TblCampaignDetails");
        //dt.Load(dataReader);
        //ds.Tables.Add(dt);
        ////  List<GramDominatorAccount> GdAccounts = new List<GramDominatorAccount>();
        //List<TblCampaignDetail> campaigns = new List<TblCampaignDetail>();
        //for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
        //{
        //    TblCampaignDetail campaign = new TblCampaignDetail
        //    {
        //        AccountId = (long)ds.Tables[0].Rows[i][2],
        //        CampaignJSON = JsonConvert.DeserializeObject<CampaignJSon>(ds.Tables[0].Rows[i][3].ToString()),
        //        Status = (long)ds.Tables[0].Rows[i][4],
        //        Name = ds.Tables[0].Rows[i][8].ToString(),
        //        ModuleType = ds.Tables[0].Rows[i][9].ToString()
        //    };
        //    campaigns.Add(campaign);

        //}
    }
}
}
