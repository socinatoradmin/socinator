using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DominatorMigration
{
    public class AccountBase
    {

        public string AccountNetwork { get; set; }

        public string AccountGroup { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public string UserId { get; set; }

        public string UserFullName { get; set; }

        public string ProfilePictureUrl { get; set; }

        public string AccountProxy { get; set; }

        public string AccountId { get; set; }

        public string Status { get; set; }

        public string ProfileId { get; set; }


    }

    public class Account

        {

            public AccountBase AccountBaseModel { get; set; }

            public bool IsUserLoggedIn { get; set; }

            public string UserAgentWeb { get; set; } 

            public string UserAgentMobile { get; set; } = string.Empty;

            public bool UseMobileRequestOnly { get; set; } = false;

            public bool IsloggedinWithPhone { get; set; }

            public string SessionId { get; set; } = string.Empty;

            public DeviceGenerator DeviceDetails { get; set; }

            public int LastLogin { get; set; }

            public int LastUpdateTime { get; set; }

            public int? DisplayColumnValue1 { get; set; }

            public int? DisplayColumnValue2 { get; set; }

            public int? DisplayColumnValue3 { get; set; }

            public int? DisplayColumnValue4  { get; set; }

            public int? DisplayColumnValue5  { get; set; }

            public int? DisplayColumnValue6  { get; set; }

            public int? DisplayColumnValue7  { get; set; }

            public int? DisplayColumnValue8  { get; set; }

            public int? DisplayColumnValue9  { get; set; }

            public int? DisplayColumnValue10 { get; set; }

            public string AccountId { get; set; }

            public string UserName => AccountBaseModel?.UserName;

            public string Cookies { get; set; }

            public Proxy Proxy { get; set; }
        }
    public class DeviceGenerator
    {
        //public DeviceGenerator()
        //{
        //    GenerateDetails();
        //}

        public string AndroidRelease { get;  set; }
        public string AndroidVersion { get;  set; }

        public string Device { get;  set; }

        public string DeviceId { get;  set; }
        public string Manufacturer { get;  set; }

        public string Model { get;  set; }
        public string PhoneId { get;  set; }

        public string Useragent { get; set; }
        public string Brand { get; set; }

        public string Cpu { get; set; }

        public string Dpi { get; set; }

        public string ManufacturerBrand { get; set; }

        public string Resolution { get; set; }

        public string AdId { get; set; }

        public string Guid { get; set; }

        /// <summary>
        /// Generate a unique id for each device
        /// </summary>
        /// <returns></returns>
        public static string GenerateGuid()
        {
            var rand = new System.Random();
            return string.Format("{0}{1}-{2}-{3}-{4}-{5}{6}{7}",
                rand.Next(0, 65535).ToString("x4"),
                rand.Next(0, 65535).ToString("x4"),
                rand.Next(0, 65535).ToString("x4"),
                rand.Next(16384, 20479).ToString("x4"),
                rand.Next(32768, 49151).ToString("x4"),
                rand.Next(0, 65535).ToString("x4"),
                rand.Next(0, 65535).ToString("x4"),
                rand.Next(0, 65535).ToString("x4"));
        }

    }

    public class Proxy
    {
        public string IP { get; set; }
        public string Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }

}
