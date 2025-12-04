#region

using System.Collections.Generic;
using System.Linq;
using ProtoBuf;

#endregion

namespace DominatorHouseCore.Utility
{
    public class TiktokDeviceGenerator
    {
        public TiktokDeviceGenerator()
        {
            GenerateDetails();
        }

        [ProtoMember(1)] public string DeviceType { get; private set; }

        [ProtoMember(2)] public string SystemRegion { get; private set; }

        [ProtoMember(3)] public string DeviceId { get; set; }

        [ProtoMember(4)] public string Region { get; private set; }

        [ProtoMember(5)] public string Brand { get; set; }

        [ProtoMember(6)] public string Dpi { get; set; }

        [ProtoMember(7)] public string Resolution { get; set; }
        [ProtoMember(8)] public string Useragent { get; set; }
        [ProtoMember(9)] public string AndroidVersion { get; set; }
        [ProtoMember(10)] public string Model { get; set; }
        [ProtoMember(11)] public string Device { get; set; }
        [ProtoMember(12)] public string IId { get; set; }

        public void GenerateDetails()
        {
            var splitDeviceDetails = GetRandomDevice().Split(':');

            var splitAndroidDetails = splitDeviceDetails[0].Split(';');
            AndroidVersion = splitAndroidDetails[2].Split(' ')[2];
            SystemRegion = splitAndroidDetails[3].Split('_')[1];
            Model = splitAndroidDetails[4];
            Device = splitAndroidDetails[5].Split('/')[1];
            Useragent = splitDeviceDetails[0];
            DeviceType = splitDeviceDetails[1];
            Resolution = splitDeviceDetails[2];
            Dpi = splitDeviceDetails[3];
            Brand = splitDeviceDetails[4];
            DeviceId = GetRandomDeviceId(); //splitDeviceDetails[1];
            IId = GetIId(DeviceId);
        }
        //public void GenerateDetails()
        //{
        //    var splitDeviceDetails = GetRandomDevice().Split(':');

        //    var splitAndroidDetails = splitDeviceDetails[0].Split(';');
        //    AndroidVersion = splitAndroidDetails[2].Split(' ')[2];
        //    SystemRegion =splitAndroidDetails[3].Split('_')[1];
        //    Model = splitAndroidDetails[4];
        //    Device = splitAndroidDetails[5].Split('/')[1];
        //    Useragent = splitDeviceDetails[0];
        //    DeviceId = splitDeviceDetails[1];
        //    DeviceType = splitDeviceDetails[2];
        //    Resolution = splitDeviceDetails[3];
        //    Dpi = splitDeviceDetails[4];
        //    Brand = splitDeviceDetails[5];
        //    IId = splitDeviceDetails[6];
        //}
        /// <summary>
        ///     GetRandomDevice method is used to get the random device details from the array of list
        /// </summary>
        /// <returns>Return the any one from list of devices</returns>
        //private static string GetRandomDevice()
        //{
        //    // retrun any one from device items
        //    return new[]
        //    {
        //       "com.zhiliaoapp.musically / 2018110931(Linux; U; Android 8.1.0; en_US; Moto G(5) Plus; Build/OPS28.85 - 17 - 6 - 2; Cronet / 58.0.2991.0):Moto+G+%285%29+Plus:1080*1776:480:motorola",
        //       "com.zhiliaoapp.musically / 2018110931(Linux; U; Android 8.0.0; en_US; ASUS_Z012DB; Build/OPR1.170623.026; Cronet / 58.0.2991.0):ASUS_Z012DB:1080*1920:480:asus",
        //       "com.zhiliaoapp.musically / 2018110931(Linux; U; Android 5.1.1; en_US; vivo V3Max; Build/LMY47V; Cronet / 58.0.2991.0):vivo+V3Max:1080*1920:480:vivo",
        //       //"com.zhiliaoapp.musically / 2018110931(Linux; U; Android 4.1.2; en_GB; SM - N910G; Build / JZO54K; Cronet / 58.0.2991.0):6760618510928741893:SM-N910G:480*800:240:samsung",
        //       "com.zhiliaoapp.musically / 2018110931(Linux; U; Android 5.1; en_US; A1601; Build/LMY47I; Cronet / 58.0.2991.0):A1601:720*1280:320:OPPO",
        //       "com.zhiliaoapp.musically / 2018110931(Linux; U; Android 6.0; en_GB; CPH1609; Build/MRA58K; Cronet / 58.0.2991.0):CPH1609:1080*1920:480:OPPO",
        //       "com.zhiliaoapp.musically / 2018110931(Linux; U; Android 6.0; en_IN; Lenovo A7020a48; Build/MRA58K; Cronet / 58.0.2991.0):Lenovo+A7020a48:1080*1920:480:Lenovo",
        //       "com.zhiliaoapp.musically / 2018110931(Linux; U; Android 6.0; en_US; vivo 1601; Build/MRA58K; Cronet / 58.0.2991.0):vivo+1601:720*1280:320:vivo",
        //       "com.zhiliaoapp.musically / 2018110931(Linux; U; Android 6.0.1; en_US; Redmi 3S; Build/MMB29M; Cronet / 58.0.2991.0):Redmi+3S:720*1280:320:Xiaomi",
        //       "com.zhiliaoapp.musically/2018110931 (Linux; U; Android 6.0; en_IN; Lenovo A7020a48; Build/MRA58K; Cronet/58.0.2991.0):Lenovo+A7020a48:1080*1920:480:Lenovo",
        //       "com.zhiliaoapp.musically/2018110931 (Linux; U; Android 8.0.0; en_IN; G3416; Build/48.1.A.2.122; Cronet/58.0.2991.0):G3416:1080*1776:480:Sony",
        //       "com.zhiliaoapp.musically/2018110931 (Linux; U; Android 8.0.0; en_IN; ASUS_Z012DB; Build/OPR1.170623.026; Cronet/58.0.2991.0):ASUS_Z012DB:1080*1920:480:asus"
        //    }.GetRandomItem();
        //}
        private static string GetRandomDevice()
        {
            // retrun any one from device items
            return new[]
            {
                //Got this useragent using mobile without registered
                "com.zhiliaoapp.musically / 2018110931(linux; u; android 8.1.0; en_us; moto g(5) plus; build/ops28.85 - 17 - 6 - 2; cronet / 58.0.2991.0):moto+g+%285%29+plus:1080*1776:480:motorola",
                "com.zhiliaoapp.musically / 2018110931(linux; u; android 8.0.0; en_us; asus_z012db; build/opr1.170623.026; cronet / 58.0.2991.0):asus_z012db:1080*1920:480:asus",
                "com.zhiliaoapp.musically / 2018110931(linux; u; android 5.1.1; en_us; vivo v3max; build/lmy47v; cronet / 58.0.2991.0):vivo+v3max:1080*1920:480:vivo",
                "com.zhiliaoapp.musically / 2018110931(linux; u; android 4.1.2; en_gb; sm - n910g; build / jzo54k; cronet / 58.0.2991.0):sm-n910g:480*800:240:samsung",
                "com.zhiliaoapp.musically / 2018110931(linux; u; android 5.1; en_us; a1601; build/lmy47i; cronet / 58.0.2991.0):a1601:720*1280:320:oppo",
                "com.zhiliaoapp.musically / 2018110931(linux; u; android 6.0; en_gb; cph1609; build/mra58k; cronet / 58.0.2991.0):cph1609:1080*1920:480:oppo",
                "com.zhiliaoapp.musically / 2018110931(linux; u; android 6.0; en_in; lenovo a7020a48; build/mra58k; cronet / 58.0.2991.0):lenovo+a7020a48:1080*1920:480:lenovo",
                "com.zhiliaoapp.musically / 2018110931(linux; u; android 6.0; en_us; vivo 1601; build/mra58k; cronet / 58.0.2991.0):vivo+1601:720*1280:320:vivo:6777607007597709062",
                "com.zhiliaoapp.musically / 2018110931(linux; u; android 6.0.1; en_us; redmi 3s; build/mmb29m; cronet / 58.0.2991.0):redmi+3s:720*1280:320:xiaomi:6777609850875070214",
                "com.zhiliaoapp.musically / 2018110931(linux; u; android 6.0; en_in; lenovo a7020a48; build/mra58k; cronet/58.0.2991.0):lenovo+a7020a48:1080*1920:480:lenovo",
                "com.zhiliaoapp.musically / 2018110931(linux; u; android 8.0.0; en_in; g3416; build/48.1.a.2.122; cronet/58.0.2991.0):g3416:1080*1776:480:sony",
                "com.zhiliaoapp.musically / 2018110931(linux; u; android 8.0.0; en_in; asus_z012db; build/opr1.170623.026; cronet/58.0.2991.0):asus_z012db:1080*1920:480:asus",

                //===============Registerd Device id along with UserAgent using Nox
                "com.zhiliaoapp.musically/2018110931 (Linux; U; Android 8.1.0; en_IN; Redmi 5A; Build/OPM1.171019.026; Cronet/58.0.2991.0):Redmi 5A:720*1280:320:xiaomi",
                "com.zhiliaoapp.musically/2018110931 (Linux; U; Android 4.4.2; en_IN; SM-N950N; Build/NMF26X; Cronet/58.0.2991.0):SM-N950N:720*1280:240:samsung",
                "com.zhiliaoapp.musically/2018110931 (Linux; U; Android 4.4.2; en_IN; SM-G930K; Build/NRD90M; Cronet/58.0.2991.0):SM-G930:720*1280:240:samsung",
                "com.zhiliaoapp.musically/2018110931 (Linux; U; Android 4.4.2; en_IN; SM-G955N; Build/NRD90M; Cronet/58.0.2991.0):SM-G955N:720*1280:240:samsung",
                "com.zhiliaoapp.musically/2018110931 (Linux; U; Android 4.4.2; en_IN; SM-G965N; Build/NRD90M; Cronet/58.0.2991.0):SM-G965N:720*1280:240:samsung",
                "com.zhiliaoapp.musically/2018110931 (Linux; U; Android 4.4.2; en_IN; SM-G930L; Build/NRD90M; Cronet/58.0.2991.0):SM-G930L:720*1280:240:samsung",
                "com.zhiliaoapp.musically/2018110931 (Linux; U; Android 4.4.2; en_IN; LGM-V300K; Build/N2G47H; Cronet/58.0.2991.0):LGM-V300K:720*1280:240:lge",
                "com.zhiliaoapp.musically/2018110931 (Linux; U; Android 4.4.2; en_IN; google Pixel 2; Build/LMY47I; Cronet/58.0.2991.0):google+Pixel+2:720*1280:240:google",
                "com.zhiliaoapp.musically/2018110931 (Linux; U; Android 4.4.2; en_IN; SM-G925F; Build/JLS36C; Cronet/58.0.2991.0):M-G925F:720*1280:240:samsung",
                "com.zhiliaoapp.musically/2018110931 (Linux; U; Android 4.4.2; en_IN; SM-N950F; Build/NMF26X; Cronet/58.0.2991.0):SM-N950F:720*1280:240:samsung",
                "com.zhiliaoapp.musically/2018110931 (Linux; U; Android 4.4.2; en_IN; SM-N9005; Build/NJH47F; Cronet/58.0.2991.0):SM-N9005:720*1280:240:samsung",
                "com.zhiliaoapp.musically/2018110931 (Linux; U; Android 4.4.2; en_IN; SM-G9508; Build/NRD90M; Cronet/58.0.2991.0):SM-G9508:720*1280:240:samsung",
                "com.zhiliaoapp.musically/2018110931 (Linux; U; Android 4.4.2; en_IN; SM-N935F; Build/JLS36C; Cronet/58.0.2991.0):SM-N935F:720*1280:240:samsung",
                "com.zhiliaoapp.musically/2018110931 (Linux; U; Android 4.4.2; en_IN; HUAWEI MLA-AL10; Build/HUAWEIMLA-AL10; Cronet/58.0.2991.0):HUAWEI+MLA-AL10:720*1280:240:HUAWEI",
                "com.zhiliaoapp.musically/2018110931 (Linux; U; Android 4.4.2; en_IN; SM-N950W; Build/NMF26X; Cronet/58.0.2991.0):SM-N950:720*1280:240:samsung",
                "com.zhiliaoapp.musically/2018110931 (Linux; U; Android 4.4.2; en_IN; HUAWEI MLA-L12; Build/HUAWEIMLA-L12; Cronet/58.0.2991.0):HUAWEI+MLA-L12:720*1280:240:HUAWEI",
                "com.zhiliaoapp.musically/2018110931 (Linux; U; Android 4.4.2; en_IN; SM-G9350; Build/JLS36C; Cronet/58.0.2991.0):SM-G9350:720*1280:240:samsung",
                "com.zhiliaoapp.musically/2018110931 (Linux; U; Android 4.4.2; en_IN; SM-G955F; Build/JLS36C; Cronet/58.0.2991.0):SM-G955F:720*1280:240:samsung",
                "com.zhiliaoapp.musically/2018110931 (Linux; U; Android 9; en_IN; Nokia 5.1 Plus; Build/PPR1.180610.011; Cronet/58.0.2991.0):Nokia+5.1+Plus:720*1362:320:Nokia"
            }.GetRandomItem();
        }

        private static readonly Dictionary<string, string> DeviceIds = new Dictionary<string, string>
        {
            {"6777958453002929669", "6777959026073274117"},
            {"6585385698721908230", "6771316542938662661"},
            {"6630751755615569414", "6779840480567789317"}
        };

        public static string GetRandomDeviceId()
        {
            return DeviceIds.Keys.ToArray()[RandomUtilties.ObjRandom.Next(0, DeviceIds.Count)];
        }

        public static string GetIId(string deviceId)
        {
            return DeviceIds[deviceId];
        }

        //{"6585385698721908230", "6757918322598004486"},
        //{"6720473090419623429", "6720473796165863174"},
        //{"6630751755615569414", "6761387620859512582"},
        //{"6777588069886608901", "6777588267183310598"},
        //{"6746793713215129093", "6777592082356111109"},
        //{"6760704845549831686", "6777595420233058053"},
        //{"6760586893559285254", "6777601720819418886"},
        //{"6760592492963972614", "6777604894855333638"},
        //{"6729395505317283334", "6777606434089699078"},
        //{"6746787552278595077", "6777607007597709062"},
        //{"6774702208153601541", "6777609850875070214"},
        //{"6760594866655315461", "6777630106111969029"},
        //{"6760579683085846022", "6777630829042239237"},
        //{"6724207280285419014", "6777631288230299398"},
        //{ "6585385698721908230","6771316542938662661"}


        //"com.zhiliaoapp.musically / 2018110931(Linux; U; Android 8.1.0; en_US; Moto G(5) Plus; Build/OPS28.85 - 17 - 6 - 2; Cronet / 58.0.2991.0):6777588069886608901:Moto+G+%285%29+Plus:1080*1776:480:motorola:6777588267183310598",
        //"com.zhiliaoapp.musically / 2018110931(Linux; U; Android 8.0.0; en_US; ASUS_Z012DB; Build/OPR1.170623.026; Cronet / 58.0.2991.0):6746793713215129093:ASUS_Z012DB:1080*1920:480:asus:6777592082356111109",
        //"com.zhiliaoapp.musically / 2018110931(Linux; U; Android 5.1.1; en_US; vivo V3Max; Build/LMY47V; Cronet / 58.0.2991.0):6760704845549831686:vivo+V3Max:1080*1920:480:vivo:6777595420233058053",
        //"com.zhiliaoapp.musically / 2018110931(Linux; U; Android 4.1.2; en_GB; SM - N910G; Build / JZO54K; Cronet / 58.0.2991.0):6760618510928741893:SM-N910G:480*800:240:samsung",
        //"com.zhiliaoapp.musically / 2018110931(Linux; U; Android 5.1; en_US; A1601; Build/LMY47I; Cronet / 58.0.2991.0):6760586893559285254:A1601:720*1280:320:OPPO:6777601720819418886",
        //"com.zhiliaoapp.musically / 2018110931(Linux; U; Android 6.0; en_GB; CPH1609; Build/MRA58K; Cronet / 58.0.2991.0):6760592492963972614:CPH1609:1080*1920:480:OPPO:6777604894855333638",
        //"com.zhiliaoapp.musically / 2018110931(Linux; U; Android 6.0; en_IN; Lenovo A7020a48; Build/MRA58K; Cronet / 58.0.2991.0):6729395505317283334:Lenovo+A7020a48:1080*1920:480:Lenovo:6777606434089699078",
        //"com.zhiliaoapp.musically / 2018110931(Linux; U; Android 6.0; en_US; vivo 1601; Build/MRA58K; Cronet / 58.0.2991.0):6746787552278595077:vivo+1601:720*1280:320:vivo:6777607007597709062",
        //"com.zhiliaoapp.musically / 2018110931(Linux; U; Android 6.0.1; en_US; Redmi 3S; Build/MMB29M; Cronet / 58.0.2991.0):6774702208153601541:Redmi+3S:720*1280:320:Xiaomi:6777609850875070214",
        //"com.zhiliaoapp.musically/2018110931 (Linux; U; Android 6.0; en_IN; Lenovo A7020a48; Build/MRA58K; Cronet/58.0.2991.0):6760594866655315461:Lenovo+A7020a48:1080*1920:480:Lenovo:6777630106111969029",
        // "com.zhiliaoapp.musically/2018110931 (Linux; U; Android 8.0.0; en_IN; G3416; Build/48.1.A.2.122; Cronet/58.0.2991.0):6760579683085846022:G3416:1080*1776:480:Sony:6777630829042239237",
        //"com.zhiliaoapp.musically/2018110931 (Linux; U; Android 8.0.0; en_IN; ASUS_Z012DB; Build/OPR1.170623.026; Cronet/58.0.2991.0):6745513012340557573:ASUS_Z012DB:1080*1920:480:asus:6745512624513435241",//6724207280285419014,6777631288230299398

        //"com.zhiliaoapp.musically/2018110931 (Linux; U; Android 8.1.0; en_IN; Redmi 5A; Build/OPM1.171019.026; Cronet/58.0.2991.0):6585385698721908230:Redmi 5A:720*1280:320:xiaomi:6771316542938662661",
        //"com.zhiliaoapp.musically/2018110931 (Linux; U; Android 4.4.2; en_IN; SM-N950N; Build/NMF26X; Cronet/58.0.2991.0):6777958453002929669:SM-N950N:720*1280:240:samsung:6777959026073274117",
        //"com.zhiliaoapp.musically/2018110931 (Linux; U; Android 4.4.2; en_IN; SM-G930K; Build/NRD90M; Cronet/58.0.2991.0):6777958453002929669:SM-G930:720*1280:240:samsung:6777959026073274117",
        //"com.zhiliaoapp.musically/2018110931 (Linux; U; Android 4.4.2; en_IN; SM-G955N; Build/NRD90M; Cronet/58.0.2991.0):6777958453002929669:SM-G955N:720*1280:240:samsung:6777959026073274117",
        //"com.zhiliaoapp.musically/2018110931 (Linux; U; Android 4.4.2; en_IN; SM-G965N; Build/NRD90M; Cronet/58.0.2991.0):6777958453002929669:SM-G965N:720*1280:240:samsung:6777959026073274117",
        //"com.zhiliaoapp.musically/2018110931 (Linux; U; Android 4.4.2; en_IN; SM-G930L; Build/NRD90M; Cronet/58.0.2991.0):6777958453002929669:SM-G930L:720*1280:240:samsung:6777959026073274117",
        //"com.zhiliaoapp.musically/2018110931 (Linux; U; Android 4.4.2; en_IN; LGM-V300K; Build/N2G47H; Cronet/58.0.2991.0):6777958453002929669:LGM-V300K:720*1280:240:lge:6777959026073274117",
        //"com.zhiliaoapp.musically/2018110931 (Linux; U; Android 4.4.2; en_IN; google Pixel 2; Build/LMY47I; Cronet/58.0.2991.0):6777958453002929669:google+Pixel+2:720*1280:240:google:6777959026073274117",
        //"com.zhiliaoapp.musically/2018110931 (Linux; U; Android 4.4.2; en_IN; SM-G925F; Build/JLS36C; Cronet/58.0.2991.0):6777958453002929669:M-G925F:720*1280:240:samsung:6777959026073274117",
        //"com.zhiliaoapp.musically/2018110931 (Linux; U; Android 4.4.2; en_IN; SM-N950F; Build/NMF26X; Cronet/58.0.2991.0):6777958453002929669:SM-N950F:720*1280:240:samsung:6777959026073274117",
        //"com.zhiliaoapp.musically/2018110931 (Linux; U; Android 4.4.2; en_IN; SM-N9005; Build/NJH47F; Cronet/58.0.2991.0):6777958453002929669:SM-N9005:720*1280:240:samsung:6777959026073274117",
        //"com.zhiliaoapp.musically/2018110931 (Linux; U; Android 4.4.2; en_IN; SM-G9508; Build/NRD90M; Cronet/58.0.2991.0):6777958453002929669:SM-G9508:720*1280:240:samsung:6777959026073274117",
        //"com.zhiliaoapp.musically/2018110931 (Linux; U; Android 4.4.2; en_IN; SM-N935F; Build/JLS36C; Cronet/58.0.2991.0):6777958453002929669:SM-N935F:720*1280:240:samsung:6777959026073274117",
        //"com.zhiliaoapp.musically/2018110931 (Linux; U; Android 4.4.2; en_IN; HUAWEI MLA-AL10; Build/HUAWEIMLA-AL10; Cronet/58.0.2991.0):6777958453002929669:HUAWEI+MLA-AL10:720*1280:240:HUAWEI:6777959026073274117",
        //"com.zhiliaoapp.musically/2018110931 (Linux; U; Android 4.4.2; en_IN; SM-N950W; Build/NMF26X; Cronet/58.0.2991.0):6777958453002929669:SM-N950:720*1280:240:samsung:6777959026073274117",
        //"com.zhiliaoapp.musically/2018110931 (Linux; U; Android 4.4.2; en_IN; HUAWEI MLA-L12; Build/HUAWEIMLA-L12; Cronet/58.0.2991.0):6777958453002929669:HUAWEI+MLA-L12:720*1280:240:HUAWEI:6777959026073274117",
        //"com.zhiliaoapp.musically/2018110931 (Linux; U; Android 4.4.2; en_IN; SM-G9350; Build/JLS36C; Cronet/58.0.2991.0):6777958453002929669:SM-G9350:720*1280:240:samsung:6777959026073274117",
        //"com.zhiliaoapp.musically/2018110931 (Linux; U; Android 4.4.2; en_IN; SM-G955F; Build/JLS36C; Cronet/58.0.2991.0):6777958453002929669:SM-G955F:720*1280:240:samsung:6777959026073274117",
    }
}