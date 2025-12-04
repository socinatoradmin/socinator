using DominatorHouseCore.Utility;
using System;

namespace FaceDominatorCore.FDLibrary.FdClassLibrary
{
    public class FdLoginParameters
    {

        public string Lsd { get; set; } = string.Empty;

        public string Timezone { get; set; } = "-330";

        public string Lgndim { get; set; } = "eyJ3IjoxMzY2LCJoIjo3NjgsImF3IjoxMzY2LCJhaCI6NzI4LCJjIjoyNH0%3D";

        public string Lgnrnd { get; set; } = string.Empty;

        public string Lgnjs { get; set; } = DateTime.Now.GetCurrentEpochTime().ToString();

        public string AbTestData { get; set; } = "A%2FAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA%2FAAAAA%2FAAAAD";

        public string Locale { get; set; } = string.Empty;

        public string Revision { get; set; } = string.Empty;

        public string DeskToken { get; set; } = string.Empty;

        public string ImpressionId { get; set; } = string.Empty;

        public string RegCookieValue { get; set; } = string.Empty;

        /*
                public string CurrentTimeStamp { get; set; } = DateTimeUtilities.GetEpochTime().ToString();
        */

        public string LoginSource { get; set; } = "login_bluebar";

        public string PrefillSource { get; set; } = "browser_dropdown";

        public string PrefillType = "password";

        public string Skstamp { get; set; } = "eyJyb3VuZHMiOjUsInNlZWQiOiI5NzdhOTNhYTM5ZWZmMWZkZWQyNTg4ZGFjOWJhMmJmOSIsInNlZWQyIjoiYmJhM2JlNTFmNjI2YjcxZmQzZjNjNGM0MTc5MGUxMTgiLCJoYXNoIjoiODBhNzU4OGU5ZTQwYzcyYjEyN2Y0N2FlMzcxN2E0YmIiLCJoYXNoMiI6IjQzOTE0ZWI4YWViMDYxY2IzMWJkZDgyZjAyODQ4NDAxIiwidGltZV90YWtlbiI6NTM5MDAsInN1cmZhY2UiOiJsb2dpbiJ9";

    }
}
