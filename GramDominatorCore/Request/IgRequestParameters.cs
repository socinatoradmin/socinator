using DominatorHouseCore.Request;
using DominatorHouseCore.Utility;
using GramDominatorCore.GDUtility;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace GramDominatorCore.Request
{

    public class IgRequestParameters : RequestParameters
    {
        public IgRequestParameters(string url=null) : base(url)
        {
            Url = url;
            if (!url.Contains("Mozilla"))
                SetupDefaultHeaderFor169Version(url);
            else
                SetupWebHeaderFor169Version(url);
        }

        public IgRequestParameters()
        {
        }
       
        public JsonElements Body { private get; set; }

        private bool Sign { get; set; } = true;


        public void DontSign()
        {
            Sign = false;
        }

        public void CreateSign()
        {
            Sign = true;
        }

        public byte[] GenerateBody(bool isDecode = false)
        {
            // this.SetupDefaultHeaders();
            string str = Body == null ? null : JsonConvert.SerializeObject(Body, new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore
            });
            if (IsMultiPart)
                return CreateMultipartBody(str);
            if (IsMultiPartForBroadCast)
                return CreateMultipartBodyForBroadCastMessage(str);
            if (str == null)
                return null;
            if (!Sign)
                return GeneratePostData(str,isDecode);
      
            return CreateSignature(str);
        }


        private byte[] CreateSignature(string message)
        {
            string PostData =
                $"signed_body=SIGNATURE.{WebUtility.UrlEncode(message)}";

            string requestData = PostDataParameters.Aggregate(PostData,
                ((current, keyPair) =>
                    current + $"&{keyPair.Key}={WebUtility.UrlEncode(keyPair.Value)}"));
            return Encoding.UTF8.GetBytes(requestData);
        }

        private void SetupDefaultHeaderFor169Version(string userAgent)
        {
            Headers.Clear();
            string Rawclienttime = GdUtilities.GetRowClientTime();
            UserAgent = userAgent;
            AddHeader("X-IG-App-Locale", "en_US");
            AddHeader("X-IG-Device-Locale", "en_US");
            AddHeader("X-IG-Mapped-Locale", "en_US");
            AddHeader("X-Pigeon-Session-Id", "UFS-" + Utilities.GetGuid()+"-0");
            AddHeader("X-Pigeon-Rawclienttime", Rawclienttime);
            AddHeader("X-IG-Connection-Speed", "-1kbps");
            AddHeader("X-IG-Bandwidth-Speed-KBPS", "-1.000");//{(object)RandomUtilties.GetRandomNumber(3700, -1)}kbps");
            AddHeader("X-IG-Bandwidth-TotalBytes-B", "0");
            AddHeader("X-IG-Bandwidth-TotalTime-MS", "0");
            AddHeader("X-Bloks-Version-Id", Constants.BlockVersionningId);
            //"fe808146fcbce04d3a692219680092ef89873fda1e6ef41c09a5b6a9852bed94"//a4133d45315981587e894655571231d55dfc135b78884619213116e1e19379d1//"0a3ae4c88248863609c67e278f34af44673cff300bc76add965a9fb036bd3ca3");//a4b4b8345a67599efe117ad96b8a9cb357bb51ac3ee00c3a48be37ce10f2bb4c;
            AddHeader("X-Bloks-Is-Layout-RTL", "false");
            AddHeader("X-IG-Device-ID", "");
            AddHeader("X-IG-Android-ID", "");
            AddHeader("X-IG-Connection-Type", Constants.X_IG_Connection_Type);// "MOBILE(HSPA+)");//
            AddHeader("X-IG-Capabilities", Constants.X_IG_Capabilities);
            AddHeader("X-IG-App-ID", "567067343352427");
            AddHeader("Accept-Language", "en-US");
            AddHeader("X-IG-WWW-Claim", "0");
            KeepAlive = true;
            //AddHeader("Host", "i.instagram.com");
            ContentType = Constants.ContentTypeDefault;
        }

        private void SetupWebHeaderFor169Version(string userAgent)
        {
            Headers.Clear();
            UserAgent = userAgent;
            AddHeader("Connection", "keep-alive");
            AddHeader("sec-ch-ua", "\"Chromium\";v=\"116\", \"Not)A;Brand\";v=\"24\", \"Google Chrome\";v=\"116\"");
            AddHeader("sec-ch-ua-mobile", "?0");
            AddHeader("sec-ch-ua-platform", "\"windows\"");
            AddHeader("Origin", "https://www.instagram.com");
            AddHeader("Referer", "https://www.instagram.com/");
            AddHeader("Sec-Fetch-Site", "same-site");
            AddHeader("Sec-Fetch-Mode", "cors");
            AddHeader("Sec-Fetch-Dest", "empty");
            AddHeader("X-Requested-With", "com.instagram.android");
            AddHeader("Accept", "*/*");
            AddHeader("Accept-Language", "en-US,en;q=0.9");
            KeepAlive = true;
            ContentType = Constants.ContentTypeDefault;
        }
        public string IgGetResponse(string url)
        {
            HttpWebRequest requestForLocation = (HttpWebRequest)WebRequest.Create(url);
            requestForLocation.Method = "GET";
            String test;
            using (HttpWebResponse responseForLocation = (HttpWebResponse)requestForLocation.GetResponse())
            {
                Stream dataStream = responseForLocation.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                test = reader.ReadToEnd();
                reader.Close();
                dataStream.Close();
            }
            return test;
        }

    }
}
