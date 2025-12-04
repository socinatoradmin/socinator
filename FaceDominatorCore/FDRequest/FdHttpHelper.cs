using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Request;

namespace FaceDominatorCore.FDRequest
{

    public interface IFdHttpHelper : IHttpHelper { }

    public class FdHttpHelper : HttpHelper, IFdHttpHelper
    {
        //public FdHttpHelper(IRequestParameters requestParameters)
        //    : base(requestParameters) { }

        #region Properties

        // To specify the retry count for the request
        /*
                private const int MaxRequestRetry = 1;
        */

        // To compute the how much already requested
        protected int RequestCount { get; set; }

        #endregion


        /// <summary>
        /// Post Request with url and postdata with already saved RequestParameter
        /// </summary>
        /// <param name="url">url of tha page</param>
        /// <param name="postData">post data in byte array which while pass with url</param>
        /// <returns><see cref="IResponseParameter"/></returns>
        public override IResponseParameter PostRequest(string url, byte[] postData)
        {
            RequestCount++;
            return base.PostRequest(url, postData);
        }

        /// <summary>
        /// Post Request with url and postdata as sequences of bytes with new RequestParameter
        /// </summary>
        /// <param name="url">url of tha page</param>
        /// <param name="postData">post data in byte array which while pass with url</param>
        /// <param name="requestParamater"><see cref="IRequestParameters"/></param>
        /// <returns><see cref="IResponseParameter"/></returns>
        public override IResponseParameter PostRequest(string url, byte[] postData, IRequestParameters requestParamater)
        {
            RequestCount = 0;
            return base.PostRequest(url, postData, requestParamater);
        }

        /// <summary>
        /// Post Request with url and postdata with already saved RequestParameter
        /// </summary>
        /// <param name="url">url of tha page</param>
        /// <param name="postData">post data in string which will pass with url</param>
        /// <returns><see cref="T:DominatorHouseCore.Requests.IResponseParameter" /></returns>
        public override IResponseParameter PostRequest(string url, string postData)
        {
            RequestCount = 0;
            return base.PostRequest(url, postData);
        }


        /// <summary>
        /// Post Request with url and postdata with new RequestParameter
        /// </summary>
        /// <param name="url">url of tha page</param>
        /// <param name="postData">post data in byte array which while pass with url</param>
        /// <param name="requestParamater"><see cref="IRequestParameters"/></param>
        /// <returns><see cref="IResponseParameter"/></returns>
        public override IResponseParameter PostRequest(string url, string postData, IRequestParameters requestParamater)
        {
            RequestCount = 0;
            return base.PostRequest(url, postData, requestParamater);
        }



    }
}
