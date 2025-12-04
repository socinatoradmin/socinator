using DominatorHouseCore.Interfaces;

namespace LinkedDominatorCore.Response
{
    public class EventInviteResponseHandler: LdResponseHandler
    {
        public string ErrorMessage {  get; set; }
        public bool IsCancelled {  get; set; }
        public EventInviteResponseHandler(IResponseParameter response,bool IsBrowser=false):base(response)
        {
            try
            {
                if(!IsBrowser)
                {
                    Success = !string.IsNullOrEmpty(response.Response)
                        && (response.Response.Contains("\"status\":200")|| response.Response.Contains("\"status\":201"));
                    if (!Success)
                    {
                        if(!string.IsNullOrEmpty(response?.Response) && response.Response.Contains("\"elements\":[]"))
                        {
                            ErrorMessage = "Can't Invite Connection On This Event Due To Event Is Cancelled,Deleted,End,User Are Attendies,Timeout To Invite\nOr Not Have Permission To Invite\nOr Invited Outside By Outside Software.";
                            IsCancelled = true;
                        }
                    }
                }
                else
                {
                    if(!string.IsNullOrEmpty(response?.Response) && response.Response.Contains("Can't Invite"))
                    {
                        Success = false;
                        ErrorMessage = "Can't Invite Connection On This Event Due To Event Is Cancelled,Deleted,End,User Are Attendies,Timeout To Invite\nOr Not Have Permission To Invite\nOr Invited Outside By Outside Software.";
                        IsCancelled = true;
                    }else if(!string.IsNullOrEmpty(response?.Response) && response.Response.Contains("Already Invited"))
                    {
                        Success = false;
                        IsCancelled = false;
                        ErrorMessage = "Already Invited";
                    }
                    else if (!string.IsNullOrEmpty(response?.Response) && response.Response.Contains("Invited"))
                    {
                        Success = true;
                    }
                }
            }
            catch { }
        }
    }
}
