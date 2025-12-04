#region

using DominatorHouseCore.Utility;
using OpenPop.Pop3;
using ProtoBuf;

#endregion

namespace DominatorHouseCore.EmailService
{
    [ProtoContract]
    public class MailCredentials : BindableBase
    {
        private string _hostname;
        private int? _port;
        private string _username;
        private string _password;

        [ProtoMember(1)]
        public string Hostname
        {
            get => _hostname;
            set
            {
                if (_hostname == value) return;
                SetProperty(ref _hostname, value);
            }
        }

        [ProtoMember(2)]
        public int? Port
        {
            get => _port;
            set
            {
                if (_port == value) return;
                SetProperty(ref _port, value);
            }
        }

        [ProtoMember(3)]
        public string Username
        {
            get => _username;
            set
            {
                if (_username == value) return;
                SetProperty(ref _username, value);
            }
        }

        [ProtoMember(4)]
        public string Password
        {
            get => _password;
            set
            {
                if (_password == value) return;
                SetProperty(ref _password, value);
            }
        }
    }

    public class IncomingData
    {
        public string From { get; set; }
        public string Subject { get; set; }
        public string Date { get; set; }
        public string Message { get; set; }
    }

    public class EmailClient
    {
        public static IncomingData FetchLastMailFromSender(MailCredentials mailCredentials, bool sslRequired,
            string senderEmail)
        {
            if (mailCredentials.Username.ToLower().Contains("gmail.com"))
                mailCredentials.Username = $"recent:{mailCredentials.Username}";
            using (var client = new Pop3Client())
            {
                var messageCount = ConnectAndGetMessageCount(mailCredentials, sslRequired, client);
                for (var i = messageCount; i > 0; i--)
                {
                    var a = client.GetMessage(i);
                    var mailData = new IncomingData();
                    if (!a.Headers.From.Raw.Contains(senderEmail)) continue;
                    mailData.From = senderEmail;
                    mailData.Date = a.Headers.Date;
                    mailData.Subject = a.Headers.Subject;
                    mailData.Message = a.MessagePart.Body == null
                        ? a.MessagePart.MessageParts[0].GetBodyAsText()
                        : a.MessagePart.GetBodyAsText();

                    return mailData;
                }
            }

            return null;
        }

        private static int ConnectAndGetMessageCount(MailCredentials mailCredentials, bool sslRequired,
            Pop3Client client)
        {
            ConnectToMailServer(mailCredentials, sslRequired, client);

            // Get the number of messages in the inbox
            var messageCount = client.GetMessageCount();
            return messageCount;
        }

        private static void ConnectToMailServer(MailCredentials mailCredentials, bool sslRequired, Pop3Client client)
        {
            // Connect to the server
            if (mailCredentials.Port != null)
                client.Connect(mailCredentials.Hostname, mailCredentials.Port.Value, sslRequired);

            // Authenticate ourselves towards the server
            client.Authenticate(mailCredentials.Username, mailCredentials.Password,
                AuthenticationMethod.UsernameAndPassword);
        }
    }
}