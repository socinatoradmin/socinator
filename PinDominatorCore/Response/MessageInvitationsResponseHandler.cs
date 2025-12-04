using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using PinDominatorCore.PDModel;
using PinDominatorCore.PDUtility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PinDominatorCore.Response
{
    public class MessageInvitationsResponseHandler : PdResponseHandler
    {
        public MessageInvitationsResponseHandler(IResponseParameter response) : base(response)
        {
            var jsonHand = new DominatorHouseCore.Utility.JsonHandler(response.Response);

            var dataToken = jsonHand.GetJToken("resource_response", "data");

            string sender = "sender";
            string recipient = "recipient";

            foreach (var Token in dataToken)
                try
                {
                    var user = new PinterestUser();
                    var conversation = jsonHand.GetJTokenOfJToken(Token, "conversation");
                    if (conversation.HasValues)
                    {
                        user.PinterestUserSender = new PinterestUser();
                        user.Read = jsonHand.GetJTokenValue(Token, "read")?.ToLower().Equals("true");
                        user.Type = jsonHand.GetJTokenValue(Token, "type");
                        user.ContactRequestId = jsonHand.GetJTokenValue(Token, "id");

                        user.PinterestUserSender.Gender =
                            jsonHand.GetJTokenValue(Token, sender, "gender") == "female"
                                ? PDEnums.PinterestGender.Female
                                : jsonHand.GetJTokenValue(Token, sender, "gender") == "male"
                                    ? PDEnums.PinterestGender.Male
                                    : PDEnums.PinterestGender.Unknown;
                        user.PinterestUserSender.Username = jsonHand.GetJTokenValue(Token, sender, "username");
                        user.PinterestUserSender.FirstName = jsonHand.GetJTokenValue(Token, sender, "first_name");
                        user.PinterestUserSender.LastName = jsonHand.GetJTokenValue(Token, sender, "last_name");
                        user.PinterestUserSender.FullName = jsonHand.GetJTokenValue(Token, sender, "full_name");

                        user.PinterestUserSender.ImageSmallUrl =
                            jsonHand.GetJTokenValue(Token, sender, "image_small_url");
                        user.PinterestUserSender.ImageMediumUrl =
                            jsonHand.GetJTokenValue(Token, sender, "image_medium_url");
                        user.PinterestUserSender.ImageLargeUrl =
                            jsonHand.GetJTokenValue(Token, sender, "image_large_url");
                        user.PinterestUserSender.ImageXlargeUrl =
                            jsonHand.GetJTokenValue(Token, sender, "image_xlarge_url");
                        user.PinterestUserSender.UserId = jsonHand.GetJTokenValue(Token, sender, "id");

                        user.PinterestUserRecipient = new PinterestUser();
                        user.PinterestUserRecipient.Gender =
                            jsonHand.GetJTokenValue(Token, recipient, "gender") == "female" ? PDEnums.PinterestGender.Female :
                            jsonHand.GetJTokenValue(Token, recipient, "gender") == "male" ? PDEnums.PinterestGender.Male :
                            PDEnums.PinterestGender.Unknown;
                        user.PinterestUserRecipient.Username =
                            jsonHand.GetJTokenValue(Token,recipient, "username");
                        user.PinterestUserRecipient.FirstName =
                            jsonHand.GetJTokenValue(Token, recipient,"first_name");
                        user.PinterestUserRecipient.LastName =
                            jsonHand.GetJTokenValue(Token,recipient,"last_name");
                        user.PinterestUserRecipient.FullName =
                            jsonHand.GetJTokenValue(Token,recipient, "full_name");
                        user.PinterestUserRecipient.ImageSmallUrl =
                            jsonHand.GetJTokenValue(Token,recipient, "image_small_url");
                        user.PinterestUserRecipient.ImageMediumUrl =
                            jsonHand.GetJTokenValue(Token,recipient, "image_medium_url");
                        user.PinterestUserRecipient.ImageLargeUrl =
                            jsonHand.GetJTokenValue(Token,recipient, "image_large_url");
                        user.PinterestUserRecipient.ImageXlargeUrl =
                            jsonHand.GetJTokenValue(Token,recipient, "image_xlarge_url");
                        user.PinterestUserRecipient.UserId = jsonHand.GetJTokenValue(Token,recipient, "id");

                        user.CreatedAt = jsonHand.GetJTokenValue(Token, "created_at");

                        user.Conversation = jsonHand.GetJTokenValue(Token, "last_message").Replace("\r\n", "").Replace(" ", "");

                        UsersList.Add(user);
                    }
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
        }

        public bool HasMoreResults { get; set; }

        public string BookMark { get; set; }

        public List<PinterestUser> UsersList { get; } = new List<PinterestUser>();
    }
}
