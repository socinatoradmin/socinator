using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using GramDominatorCore.GDModel;
using System;
using System.Linq;
using System.Net;
using System.Text;

namespace GramDominatorCore.GDUtility
{
    public class IGMobileUtilities
    {
        public static IGMobileUtilities Instance(DominatorAccountModel dominatorAccount)
        {
            return new IGMobileUtilities(dominatorAccount);
        }
        public DominatorAccountModel AccountModel { get; set; }
        public AccountModel accountModel { get; set; }
        public IGMobileUtilities(DominatorAccountModel dominatorAccount)
        {
            AccountModel = dominatorAccount;
            accountModel = new AccountModel(AccountModel);
        }
        public string GetRawClientTime(int length = 3)
        {
            var Rawclienttime = string.Empty;
            try
            {
                Rawclienttime = ((long)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds).ToString();
                var initialtime = Rawclienttime.Substring(0, Rawclienttime.Length - length);
                var endtime = Rawclienttime.Substring(Rawclienttime.Length - length);
                Rawclienttime = initialtime + "." + endtime;

            }
            catch { }
            return Rawclienttime;
        }
        public string GetGuid(bool isDashesNeed = true)
        {
            // Generate the GUID 
            var getGuid = Guid.NewGuid().ToString();
            // return the GUID without dashes if isDashesNeed is true 
            return !isDashesNeed ? getGuid.Replace('-', char.MinValue) : getGuid;
        }
        public string GetTimeStamp()
        {
            DateTimeOffset unixEpoch = new DateTimeOffset(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc));

            // Get current UTC time
            DateTimeOffset now = DateTimeOffset.UtcNow;

            // Calculate Unix timestamp
            long unixTimestamp = (long)(now - unixEpoch).TotalSeconds;
            return unixTimestamp.ToString();
        }
        public string GetTimeZoneOffSet()
        {
            TimeZoneInfo localTimeZone = TimeZoneInfo.Local;

            // Get the current local time
            DateTime localTime = DateTime.Now;

            // Get the UTC offset for the local time
            TimeSpan offset = localTimeZone.GetUtcOffset(localTime);

            // Convert the TimeSpan offset to seconds
            int offsetInSeconds = (int)offset.TotalSeconds;
            return offsetInSeconds.ToString();
        }
        public string GetMID()
        {
            return AccountModel.Cookies.Cast<Cookie>().FirstOrDefault(x => x.Name == "mid")?.Value;
        }
        public string GetSHBID(string UserID)
        {
            var shbid = AccountModel.Cookies.Cast<Cookie>().FirstOrDefault(x => x.Name == "shbid")?.Value;
            var final = shbid != null ? shbid?.Replace("\"", "")?.Replace("\\05", ",") : AccountModel?.DeviceDetails?.IGUSHBID;
            if (!string.IsNullOrEmpty(final))
            {
                var data = final.Split(',');
                final = data[0] + "," + UserID + "," + data[2];
            }
            return final;
        }
        public string GetSHBTS(string UserID)
        {
            var shbts = AccountModel.Cookies.Cast<Cookie>().FirstOrDefault(x => x.Name == "shbts")?.Value;
            var final = shbts != null ? shbts?.Replace("\"", "")?.Replace("\\05", ",") : AccountModel?.DeviceDetails?.IGUSHBTS;
            if (!string.IsNullOrEmpty(final))
            {
                var data = final.Split(',');
                final = data[0] + "," + UserID + "," + data[2];
            }
            return final;
        }
        public string GetRUR(string UserID)
        {
            var RUR = AccountModel.Cookies.Cast<Cookie>().FirstOrDefault(x => x.Name == "rur")?.Value;
            var final = RUR?.Replace("\\05", ",")?.Replace("\"", "");
            if (!string.IsNullOrEmpty(final))
            {
                var data = final.Split(',');
                final = data[0] + "," + UserID + "," + data[2];
            }
            return final;
        }
        public string GetBearer()
        {
            var bearer = string.Empty;
            try
            {
                var dsuserID = AccountModel.Cookies.Cast<Cookie>().FirstOrDefault(x => x.Name == "ds_user_id")?.Value;
                var sessionID = AccountModel.Cookies.Cast<Cookie>().FirstOrDefault(x => x.Name == "sessionid")?.Value;
                var data = $"{{\"ds_user_id\":\"{dsuserID}\",\"sessionid\":\"{sessionID}\"}}";
                bearer = Convert.ToBase64String(Encoding.UTF8.GetBytes(data));
            }
            catch { }
            return bearer;
        }
        public string DeviceID()
        {
            return accountModel.Device_Id ?? Utilities.GetMobileDeviceId();
        }
        public string GetID()
        {
            return accountModel.Id ?? Utilities.GetGuid();
        }
        public string FamilyID()
        {
            return accountModel.FamilyId ?? Utilities.GetGuid();
        }
        public string GetPigeonSessionID()
        {
            var psessionID = accountModel.PigeonSessionId ?? Utilities.GetGuid();
            return psessionID.StartsWith("UFS") ? psessionID : "UFS-" + psessionID + "-0";
        }

        public string GetUserID()
        {
            return AccountModel.Cookies.Cast<Cookie>().FirstOrDefault(x => x.Name == "ds_user_id")?.Value;
        }
        public string GetClientContext()
        {
            long timestampMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            long randomPart = new Random().Next(1000, 9999); // Or better, use secure random if needed
            // Combine the timestamp and randomness to get a 64-bit-like ID
            return timestampMs.ToString() + randomPart.ToString();
        }
    }
}
