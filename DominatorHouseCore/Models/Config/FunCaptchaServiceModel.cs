using DominatorHouseCore.Utility;
using ProtoBuf;

namespace DominatorHouseCore.Models.Config
{
    [ProtoContract]
    public class FunCaptchaServiceModel: BindableBase
    {
        private string _apiKey;
        private string _balance="0.0 $";
        private string _CaptchaSettingPath=ConstantVariable.GetCapSolverExtension();
        private FunCaptchaDetails _details=new FunCaptchaDetails();
        [ProtoMember(1)]
        public string APIKey
        {
            get => _apiKey;
            set => SetProperty(ref _apiKey, value);
        }
        [ProtoMember(2)]
        public string Balance
        {
            get => _balance;
            set => SetProperty(ref _balance, value);
        }
        [ProtoMember(3)]
        public FunCaptchaDetails Details
        {
            get => _details;
            set => SetProperty(ref _details, value);
        }
        [ProtoMember(4)]
        public string CaptchaSettingPath
        {
            get => _CaptchaSettingPath;
            set=> SetProperty(ref _CaptchaSettingPath, value);
        }
    }
    [ProtoContract]
    public class FunCaptchaDetails: BindableBase
    {
        private string errorId;
        private string balance;
        private string packageId;
        private string type;
        private string numberOfCalls;
        private string captchaToken;
        private string activeTime;
        private string expireTime;
        private string status;
        [ProtoMember(1)]
        public string ErrorId { get => errorId; set => SetProperty(ref errorId, value); }
        [ProtoMember(2)]
        public string Balance { get => balance; set => SetProperty(ref balance, value); }
        [ProtoMember(3)]
        public string PackageId { get => packageId; set => SetProperty(ref packageId, value); }
        [ProtoMember(4)]
        public string Type { get => type; set => SetProperty(ref type, value); }
        [ProtoMember(5)]
        public string NumberOfCalls { get => numberOfCalls; set => SetProperty(ref numberOfCalls, value); }
        [ProtoMember(6)]
        public string CaptchaToken { get => captchaToken; set => SetProperty(ref captchaToken, value); }
        [ProtoMember(7)]
        public string ActiveTime { get => activeTime; set => SetProperty(ref activeTime, value); }
        [ProtoMember(8)]
        public string ExpireTime { get => expireTime; set => SetProperty(ref expireTime, value); }
        [ProtoMember(9)]
        public string Status { get => status; set => SetProperty(ref status, value); }
    }
}
