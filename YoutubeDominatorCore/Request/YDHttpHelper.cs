using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Request;
using System.Net;

namespace YoutubeDominatorCore.Request
{
    public interface IYdHttpHelper : IHttpHelper
    {
    }

    public sealed class YdHttpHelper : HttpHelper, IYdHttpHelper
    {
        public YdHttpHelper() : base(new YdRequestParameters())
        {
        }

        public HttpHelper HttpHelper { get; set; }

        private byte[] PostData { get; set; }


        public override IResponseParameter PostRequest(string url, byte[] postData)
        {
            PostData = postData;
            return base.PostRequest(url, postData);
        }

        /// <summary>
        ///     Returns <see cref="IRequestParameters" /> which contains header details of
        ///     <see cref="HttpHelper" />
        /// </summary>
        /// <returns>A <see cref="IRequestParameters" /></returns>
        public HttpWebResponse GetResponseParameter()
        {
            return Response;
        }


        public IResponseParameter PostRequestIteration(string url, byte[] postData)
        {
            return base.PostRequest(url, postData);
        }
    }
}