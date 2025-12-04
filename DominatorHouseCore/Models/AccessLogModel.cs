using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DominatorHouseCore.Models
{
    public class AccessLogModel
    {
        public string licKey { get; set; }
        public ActivityDetail[] activity_details { get; set; }
        public AccDetail[] acc_details { get; set; }
        public RunningTime running_time { get; set; }
    }
    public class LogModel
    {
        public string name { get; set; }
        public string username { get; set; }
        public string license_key { get; set; }
        public string plan { get; set; }
        public string expire_date { get; set; }
        public string email { get; set; }
        public UserLogs user_logs { get; set; }
    }

    public class UserLogs
    {
        public string license_key { get; set; }
        public long log_time { get; set; }
    }

    public class RunningTime
    {
        public long start { get; set; }
        public long end { get; set; }
    }

    public class ActivityDetail
    {
        public string activity { get; set; }
        public string sub_activity { get; set; }
        public string network { get; set; }
        public string status { get; set; }
        public int creation_date { get; set; }
    }

    public class AccDetail
    {
        public string network { get; set; }
        public int acc_count { get; set; }
        public int success_count { get; set; }
    }



}
