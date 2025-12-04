using System;
using System.Collections.Generic;
using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using Newtonsoft.Json;
using TwtDominatorCore.Requests;
using TwtDominatorCore.Response;
using TwtDominatorCore.TDEnums;
using TwtDominatorCore.TDFactories;
using TwtDominatorCore.TDModels;
using Unity;

namespace TwtDominatorCore.TDLibrary
{
    public class AccountVerification
    {
        private readonly IAccountScopeFactory _accountScopeFactory;
        private readonly IAccountsFileManager _accountsFileManager;

        public AccountVerification()
        {
            _accountsFileManager = InstanceProvider.GetInstance<IAccountsFileManager>();
            _accountScopeFactory = InstanceProvider.GetInstance<IAccountScopeFactory>();
        }

        public AccountModel AccountModel { get; set; }
        public LogInResponseHandler LogInPageResponse { get; set; }

        public string VerificationCode { get; set; } = string.Empty;

        public bool VerifyingAccount(DominatorAccountModel dominatorAccountModel,
            LogInResponseHandler logInResponseHandler)
        {
            var isSuccess = false;
            try
            {
                var status = logInResponseHandler.status;

                switch (status)
                {
                    case Enums.LoginStatus.RetypeEmail:
                    {
                        //return if not having email in protobuf or verify code
                        if (!IsHavingRetypeEmailPhoneVerification(status, dominatorAccountModel))
                            return isSuccess;
                        isSuccess = ReTypeEmail(dominatorAccountModel);
                    }
                        break;

                    case Enums.LoginStatus.RetypePhoneNumber:
                    {
                        if (!IsHavingRetypeEmailPhoneVerification(status, dominatorAccountModel))
                            return isSuccess;
                        isSuccess = ReTypePhoneNumber(dominatorAccountModel);
                        if (!isSuccess)
                            dominatorAccountModel.AccountBaseModel.Status = AccountStatus.ReTypePhoneNumber;
                    }
                        break;

                    case Enums.LoginStatus.VerifyEmail:
                        isSuccess = EmailVerification();
                        break;

                    case Enums.LoginStatus.VerifyPhoneNumber:
                        isSuccess = PhoneVerification();
                        break;
                    case Enums.LoginStatus.Captcha:
                        //isSuccess = CaptchaSolving(logInResponseHandler, dominatorAccountModel);
                        isSuccess = ResolveHttpCaptcha(logInResponseHandler, dominatorAccountModel,true);
                        break;
                }

                // after login success saving cookies and updating status 
                if (isSuccess)
                {
                    var loginProcess = _accountScopeFactory[dominatorAccountModel.AccountId]
                        .Resolve<ITwtLogInProcess>();
                    loginProcess.AfterLoginSuccess(dominatorAccountModel);
                    var tdAccountUpdateFactory = _accountScopeFactory[dominatorAccountModel.AccountId]
                        .Resolve<ITDAccountUpdateFactory>();


                    GlobusLogHelper.log.Info(Log.SuccessfulLogin, dominatorAccountModel.AccountBaseModel.AccountNetwork,
                        dominatorAccountModel.UserName);
                    tdAccountUpdateFactory.UpdateAccountFollowCount(dominatorAccountModel, AccountModel).Wait();
                }
                else
                {
                    if (status == Enums.LoginStatus.VerifyPhoneNumber)
                        dominatorAccountModel.AccountBaseModel.Status = AccountStatus.PhoneVerification;
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return isSuccess;
        }

        public bool ResolveHttpCaptcha(LogInResponseHandler logInResponseHandler, DominatorAccountModel dominatorAccountModel,bool IsFunCaptcha=false)
        {
            try
            {
                var twtFunct = _accountScopeFactory[dominatorAccountModel.AccountId]
                    .Resolve<ITwitterFunctions>();
                var ChallengeDetails = GetChallengeDetails(dominatorAccountModel, IsFunCaptcha);
                var response = twtFunct.ResolveCaptcha(logInResponseHandler, dominatorAccountModel, ChallengeDetails,string.Empty,IsFunCaptcha);
                return !string.IsNullOrEmpty(response) && !response.Contains("PageHeader Edge") && !response.Contains("UserHeader-details");
            }
            catch { return false; }
        }

        private bool PhoneVerification()
        {
            try
            {
                return false;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return false;
        }

        private bool EmailVerification()
        {
            try
            {
                return false;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return false;
        }

        private bool ReTypePhoneNumber(DominatorAccountModel dominatorAccountModel)
        {
            var isSuccess = false;
            var twtFunct = _accountScopeFactory[dominatorAccountModel.AccountId]
                .Resolve<ITwitterFunctions>();
            try
            {
                var ChallengeDetails = GetChallengeDetails(dominatorAccountModel);
                isSuccess = twtFunct.ReTypePhoneNumber(dominatorAccountModel, ChallengeDetails);
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return isSuccess;
        }

        private bool ReTypeEmail(DominatorAccountModel dominatorAccountModel)
        {
            var IsSuccess = false;
            try
            {
                var twtFunct = _accountScopeFactory[dominatorAccountModel.AccountId]
                    .Resolve<ITwitterFunctions>();
                var ChallengeDetails = GetChallengeDetails(dominatorAccountModel);
                IsSuccess = twtFunct.ReTypeEmail(dominatorAccountModel, ChallengeDetails);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return IsSuccess;
        }

        private ChallengeDetails GetChallengeDetails(DominatorAccountModel dominatorAccountModel,bool IsFunCaptcha=false)
        {
            var httpHelper = _accountScopeFactory[dominatorAccountModel.AccountId]
                .Resolve<ITdHttpHelper>();
            var ChallengeDetails = new ChallengeDetails();
            try
            {
                if (!IsFunCaptcha)
                {
                    //ChallengeDetails.ChallengeId =
                    //    Utilities.GetBetween(LogInPageResponse.Response, "challenge_id\" value=\"", "\"");
                    // ChallengeDetails.UserId = Utilities.GetBetween(LogInPageResponse.Response, "user_id\" value=\"", "\"");
                    ChallengeDetails.PostAuthenticityToken = Utilities
                        .GetBetween(LogInPageResponse.Response, "authenticity_token\" value=\"", "\"")
                        .Trim(); //authenticity_token"
                    ChallengeDetails.lang = Utilities.GetBetween(LogInPageResponse.Response, "lang\" value=\"", "\"");
                    ChallengeDetails.RefererUrl = httpHelper.Request.Address.AbsoluteUri;
                    ChallengeDetails.RequestParameter = httpHelper.GetRequestParameter();
                    //ChallengeDetails.ChallengeType = Utilities
                    //    .GetBetween(LogInPageResponse.Response, "challenge_type\" value=\"", "\"").Trim();
                    ChallengeDetails.AssignmentToken = Utilities
                        .GetBetween(LogInPageResponse.Response, "name=\"assignment_token\" value=\"", "\"").Trim();
                }
                else
                {

                }
            }

            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return ChallengeDetails;
        }

        /// <summary>
        ///     Checking already stored email or phone in protobuf
        ///     for verify retype email or phone
        /// </summary>
        /// <param name="StatusType"></param>
        /// <returns></returns>
        private bool IsHavingRetypeEmailPhoneVerification(Enums.LoginStatus StatusType,
            DominatorAccountModel dominatorAccountModel)
        {
            var IsHaveRetypeEmailPhone = false;
            try
            {
                var serializeUserDetails = _accountsFileManager.GetAccountById(dominatorAccountModel.AccountId)
                    .ExtraParameters[Enums.ModuleExtraDetails.UserProfileDetails.ToString()];
                var userProfileDetails = JsonConvert.DeserializeObject<UserProfileDetails>(serializeUserDetails);

                if (!string.IsNullOrEmpty(dominatorAccountModel?.VarificationCode))
                {
                    VerificationCode = dominatorAccountModel.VarificationCode;
                    IsHaveRetypeEmailPhone = true;
                }
                else if (StatusType == Enums.LoginStatus.RetypeEmail &&
                         !string.IsNullOrEmpty(userProfileDetails?.Email))
                {
                    VerificationCode = userProfileDetails.Email.Replace("(pending confirmation)", "");
                    IsHaveRetypeEmailPhone = true;
                }
                else if (StatusType == Enums.LoginStatus.RetypePhoneNumber &&
                         !string.IsNullOrEmpty(userProfileDetails?.Email))
                {
                    VerificationCode = userProfileDetails.PhoneNumber;
                    IsHaveRetypeEmailPhone = true;
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return IsHaveRetypeEmailPhone;
        }

        private bool CaptchaSolving(LogInResponseHandler logInResponseHandler,
            DominatorAccountModel dominatorAccountModel)
        {
            var IsSuccess = false;
            try
            {
                var twtFunct = _accountScopeFactory[dominatorAccountModel.AccountId]
                    .Resolve<ITwitterFunctions>();
                var ChallengeDetails = GetChallengeDetails(dominatorAccountModel);
                var response = twtFunct.ResolveCaptcha(logInResponseHandler, dominatorAccountModel, ChallengeDetails);
                IsSuccess = !response.Contains("PageHeader Edge") && !response.Contains("UserHeader-details");
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return IsSuccess;
        }
    }

    public class UserProfileDetails
    {
        public string AccountCreationDate = string.Empty;
        public string Age = string.Empty;
        public string authenticityToken = string.Empty;
        public string Bio = string.Empty;
        public string BirthDate = string.Empty;
        public string Contact = string.Empty;
        public string csrfToken = string.Empty;
        public string Email = string.Empty;
        public string FullName = string.Empty;
        public string Gender = string.Empty;

        public bool IsFirstUpdated = false;
        public string Language = string.Empty;
        public string Location = string.Empty;
        public string password = string.Empty;
        public string PhoneNumber = string.Empty;
        public string ProfilePicUrl = string.Empty;
        public string UserId = string.Empty;
        public string UserName = string.Empty;
        public string WebsiteUrl = string.Empty;
    }

    public class ChallengeDetails
    {
        //used in captcha solving
        public string AssignmentToken = string.Empty;
        public string ChallengeId = string.Empty;
        public string ChallengeType = string.Empty;
        public string CsrfToken = string.Empty;
        public string WebsitePublicKey=string.Empty;
        public string PostAuthenticityToken = string.Empty;
        public string RefererUrl = string.Empty;
        public string ReTypeEmail = string.Empty;
        public string lang = string.Empty;
        public string WebsiteUrl=string.Empty;
        public string UserId = string.Empty;
        public string TaskID=string.Empty;
        public string MyAPIKey=string.Empty;
        public string CaptchaType=string.Empty;
        public List<string> Images = new List<string>();
        public IRequestParameters RequestParameter { get; set; }
        public string VerificationString {  get; set; }=string.Empty;
    }
}