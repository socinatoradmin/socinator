using DominatorHouseCore.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PinDominatorCore.Report
{
    public class CreateAccountReportDetails
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Age { get; set; }
        public string Gender { get; set; }
        public string InteractionDate { get; set; }
        public String ActivityType { get; set; }
        public string Status { get; set; }
    }
}
