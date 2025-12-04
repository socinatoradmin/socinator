using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using PinDominatorCore.PDModel;
using System.Collections.Generic;

namespace PinDominatorCore.Response
{
    public enum SignUpActivityType
    {
        Register=1,
        CheckExist=2,
        SignUpHandShake=3,
        Login =4,
        LoginHandShake=5,
        None=6,
        InterestUpdate=7,
        CheckEmailValid=8
    }
    public class CreateAccountRespHandler : PdResponseHandler
    {
        public string Message { get; set; }
        public string Token {  get; set; }
        public CreateAccountRespHandler(IResponseParameter response,SignUpActivityType signUpActivity=SignUpActivityType.Register) : base(response)
        {
            if (string.IsNullOrEmpty(response.Response))
            {
                Success = false;
                return;
            }
            var jObject = handler.ParseJsonToJObject(response.Response);
            if(signUpActivity == SignUpActivityType.CheckExist)
            {
                bool.TryParse(handler.GetJTokenValue(jObject, "resource_response","data"), out bool isExist);
                Success = !isExist;
                if(!Success)
                {
                    Issue = new PinterestIssue
                    {
                        Message = "Taken Email Already Exist"
                    };
                }
                return;
            }
            if(signUpActivity == SignUpActivityType.CheckEmailValid)
            {
                bool.TryParse(handler.GetJTokenValue(jObject, "resource_response", "data", "is_valid"), out bool isValidEmail);
                Success = isValidEmail;
                if(!Success)
                {
                    Issue = new PinterestIssue
                    {
                        Message = "Given Email Is Not Valid"
                    };
                }
                return;
            }
            var isSuccess = handler.GetJTokenValue(jObject, "status") == "success";
            if (handler.GetJTokenValue(jObject, "resource_response", "status") != "success" && !isSuccess)
            {
                Success = false;
                var message = handler.GetJTokenValue(jObject, "resource_response", "error", "message");
                message = string.IsNullOrEmpty(message) ? handler.GetJTokenValue(jObject,"message"):message;
                Issue = new PinterestIssue
                {
                    Message = message
                };
            }
            else
            {
                Success = true;
                Token = handler.GetJTokenValue(jObject, "client_context", "unauth_id");
                Token = string.IsNullOrEmpty(Token) ? handler.GetJTokenValue(jObject, "data"):Token;
            }
        }
    }
    public class InterestPickerResponse: PdResponseHandler
    {
        public List<InterestData> InterestCollection { get; set; }=new List<InterestData>();
        public string PaginationToken {  get; set; }
        public InterestPickerResponse(IResponseParameter response):base(response)
        {
            try
            {
                var jObject = handler.ParseJsonToJObject(response.Response);
                var interestCollection = handler.GetJArrayElement(handler.GetJTokenValue(jObject, "resource_response","data"));
                PaginationToken = handler.GetJTokenValue(jObject, "resource_response", "bookmark");
                PaginationToken = string.IsNullOrEmpty(PaginationToken) ? handler.GetJTokenValue(jObject, "resource", "options", "bookmarks",0) : PaginationToken;
                if(interestCollection!=null && interestCollection.HasValues)
                {
                    interestCollection.ForEach(item =>
                    {
                        InterestCollection.Add(new InterestData
                        {
                            InterestID = handler.GetJTokenValue(item,"id"),
                            LogData = handler.GetJTokenValue(item, "log_data"),
                            Name = handler.GetJTokenValue(item,"name")
                        });
                    });
                }
            }
            catch { }
        }
    }
    public class InterestData
    {
        public string InterestID { get; set; }
        public string LogData {  get; set; }
        public string Name {  get; set; }
    }
}
