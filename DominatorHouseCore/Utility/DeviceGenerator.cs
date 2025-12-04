#region

using System.ComponentModel;
using ProtoBuf;

#endregion

namespace DominatorHouseCore.Utility
{
    [Localizable(false)]
    [ProtoContract]
    public class DeviceGenerator
    {
        public DeviceGenerator()
        {
            GenerateDetails();
        }

        [ProtoMember(6)] public string AndroidRelease { get; set; }

        [ProtoMember(7)] public string AndroidVersion { get; set; }

        [ProtoMember(1)] public string Device { get; set; }

        [ProtoMember(2)] public string DeviceId { get; set; }


        [ProtoMember(3)] public string Manufacturer { get; set; }

        [ProtoMember(4)] public string Model { get; set; }

        [ProtoMember(5)] public string PhoneId { get; set; }

        [ProtoMember(18)] public static string UserAgentLocale { get; set; }

        public string Useragent =>
            string.Format(ConstantVariable.UseragentCommonFormat, (object)ConstantVariable.IgVersion,
                (object)AndroidVersion, (object)AndroidRelease, (object)Dpi, (object)Resolution,
                (object)ManufacturerBrand, (object)Model, (object)Device, (object)Cpu,
                (object)ConstantVariable.UseragentLocale + "475221264");//399993134 //217948964 //125398471//155374104//168361630//180322800

        public string TikTokUseragent => string.Format(ConstantVariable.TikTokUserAgentCommonFormat,
            ConstantVariable.UserAgentDomain, ConstantVariable.TikTokManifestVersion, AndroidRelease, UserAgentLocale,
            Model, Device);

        //"Instagram 10.33.0 Android ({1}/{2}; {3}; {4}; {5}; {6}; {7}; {8}; {9})"
        //"Instagram 6.21.2 Android 23/6.0.1; 640dpi; 1440x2560; ZTE; ZTE A2017U; ailsa_ii; qcom;en_US";
        //Instagram 37.0.0.5.97 Android (23/6.0.1; 480dpi; 1080x1920; Samsung;a8hplte;SM-A800IZ; qcom; en_US)

        [ProtoMember(8)] public string Brand { get; set; }

        [ProtoMember(9)] private string Cpu { get; set; }

        [ProtoMember(10)] public string Dpi { get; set; }

        private string ManufacturerBrand => string.IsNullOrWhiteSpace(Brand) ? Manufacturer : $"{Manufacturer}/{Brand}";

        [ProtoMember(11)] private string Resolution { get; set; }
        [ProtoMember(12)] public string AdId { get; set; }

        [ProtoMember(13)] public string Guid { get; set; }
        [ProtoMember(14)] public string Id { get; set; }
        [ProtoMember(15)] public string AttributionId { get; set; }
        [ProtoMember(16)] public string GoogleId { get; set; }
        [ProtoMember(17)] public string FamilyId { get; set; }
        [ProtoMember(19)] public string IGXClaim {  get; set; }
        [ProtoMember(20)] public string IGUSHBID {  get; set; }
        [ProtoMember(21)]public string IGUSHBTS {  get; set; }
        public void GenerateDetails()
        {
            var splitDeviceDetails = GetRandomDevice().Split(';');
            var splitAndroidDetails = splitDeviceDetails[0].Split('/');
            AndroidVersion = splitAndroidDetails[0];
            AndroidRelease = splitAndroidDetails[1];

            Dpi = splitDeviceDetails[1];
            Resolution = splitDeviceDetails[2];

            var splitManufacture = splitDeviceDetails[3].Split('/');
            Manufacturer = splitManufacture[0];
            if (splitManufacture.Length == 2)
                Brand = splitManufacture[1];

            Model = splitDeviceDetails[4];
            Device = splitDeviceDetails[5];
            Cpu = splitDeviceDetails[6];
            PhoneId = Utilities.GetGuid();
            ;
            DeviceId = Utilities.GetMobileDeviceId();
            AdId = Utilities.GetGuid();
            Guid = Utilities.GetGuid();
            Id = Utilities.GetGuid();
            FamilyId = Utilities.GetGuid();
            GoogleId = Utilities.GetGuid();
            AttributionId = Utilities.GetGuid();
        }

        /// <summary>
        ///     GetRandomDevice method is used to get the random device details from the array of list
        /// </summary>
        /// <returns>Return the any one from list of devices</returns>
        private static string GetRandomDevice()
        {
            // retrun any one from device items
            return new[]
            {
                "31/12; 480dpi; 1080x1920; Samsung;a3xelte;SM-A310F; qcom; en_US;",
                "31/12; 420dpi; 1080x2181; samsung; SM-G955F; dream2lte; samsungexynos8895; en_US;",
                "31/12; 480dpi; 1080x2076; samsung; SM-G960U; starqltesq; qcom; en_US;",
                "31/12; 420dpi; 1080x2094; samsung; SM-G955U; dream2qltesq; qcom; en_US;",
                "31/12; 480dpi; 1080x1920; Samsung;a3ltechn;SM-A3000; qcom; en_US;",
                "31/12; 420dpi; 1080x2047; samsung; SM-G975U; beyond2q; qcom; en_US;",
                "31/12; 420dpi; 1080x2094; samsung; SM-A750FN; a7y18lte; samsungexynos7885; en_US;",
                "31/12; 420dpi; 1080x2094; samsung; SM-G955U1; dream2qlteue; qcom; en_US; ",
                "31/12; 320dpi; 1080x2175; samsung; SM-J727T1; j7popeltemtr; samsungexynos7870; en_US;",
                "31/12; 420dpi; 1080x2094; samsung; SM-N960U1; crownqlteue; qcom; en_US;",
                "31/12; 480dpi; 1080x2020; samsung; SM-G970F; beyond0; exynos9820; en_US;",
                "31/12; 240dpi; 540x888; samsung; SM-J260T1; j2corepltemtr; samsungexynos7570; en_US;",
                "31/12; 480dpi; 1080x2175; samsung; SM-G950U; dreamqltesq; qcom; en_US;",
                "31/12; 480dpi; 1080x1920; Samsung;a5xeltecmcc;SM-A5108; qcom; en_US;",
                "31/12; 560dpi; 1440x2733; samsung; SM-G977U; beyondxq; qcom; en_US;",
                "31/12; 640dpi; 1440x2560; samsung; SM-G930F; herolte; samsungexynos8890; en_US;",
                "31/12; 640dpi; 1440x2560; samsung; SM-G935F; hero2lte; samsungexynos8890; en_US;",
                "31/12; 480dpi; 1080x1920; Samsung;a8ltechn;SM-A8000; qcom; en_US; ",
                "31/12; 480dpi; 1080x2076; samsung; SM-A530F; jackpotlte; samsungexynos7885; en_US;",
                "31/12; 320dpi; 1080x2175; samsung; SM-J727T; j7popeltetmo; samsungexynos7870; en_US;",
                "31/12; 480dpi; 1080x1920; Samsung;a5ltezt;SM-A500YZ; qcom; en_US;",
                "31/12; 480dpi; 1080x1920; Samsung;loganrelte;GT-S7275R; qcom; en_US;",
                "31/12; 420dpi; 1080x2034; LGE/lge; LM-Q710(FGN); cv7a; cv7a; en_US;",
                "31/12; 420dpi; 1080x2094; samsung; SM-G965F; star2lte; samsungexynos9810; en_US;",
                "31/12; 420dpi; 1080x1794; Google/google; Pixel; sailfish; sailfish; en_US; ",
                "31/12; 320dpi; 1080x2175; LGE/lge; LM-X410(FG); cv3; cv3; en_US;",
                "31/12; 560dpi; 1440x2845; samsung; SM-G975F; beyond2; exynos9820; en_US;",
                "31/12; 420dpi; 1080x2181; samsung; SM-G965U; star2qltesq; qcom; en_US;",
                "31/12; 480dpi; 1080x1920; Samsung;GT-I5500B;GT-I5500B; qcom; en_US;",
                "31/12; 480dpi; 1080x1920; Samsung;SCH-I509U;SCH-I509U; qcom; en_US;",
                "31/12; 480dpi; 1080x1920; samsung; SM-G610F; on7xelte; samsungexynos7870; en_US;",
                "31/12; 480dpi; 1080x1920; Samsung;a5lte;SM-A500G; qcom; en_US;",
                "31/12; 480dpi; 1080x1920; Samsung;ironcmcc;GT-B9388; qcom; en_US;",
                "31/12; 480dpi; 1080x1920; Samsung;espressorfcmcc;GT-P3108; qcom; en_US;",
                "31/12; 480dpi; 1080x1920; Samsung;a5ltechn;SM-A5000; qcom; en_US;",
                "31/12; 480dpi; 1080x1920; Samsung;SCV32;SCV32; qcom; en_US;",
                "31/12; 480dpi; 1080x1920; Samsung;GT-S5698;GT-S5698; qcom; en_US;",
                "31/12; 420dpi; 1080x2181; samsung; SM-G955F; dream2lte; samsungexynos8895; en_US;",
                "31/12; 640dpi; 1440x2560; samsung; SM-G920F; zeroflte; samsungexynos7420; en_US;",
                "31/12; 480dpi; 1080x1920; Samsung;a5xelte;SM-A510Y; qcom; en_US;",
                "31/12; 480dpi; 1080x1920; Samsung;SHW-M115S;SHW-M115S; qcom; en_US;",
                "31/12; 480dpi; 1080x1920; Samsung;GT-B9062;GT-B9062; qcom; en_US;",
                "31/12; 480dpi; 1080x2175; samsung; SM-G960F; starlte; samsungexynos9810; en_US;",
                "31/12; 440dpi; 1080x2177; Xiaomi/Redmi; M2101K7BI; secret; mt6785; en_US;"
            }.GetRandomItem();
        }
    }
}