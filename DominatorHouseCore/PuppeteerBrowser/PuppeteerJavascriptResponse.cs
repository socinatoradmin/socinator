using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace DominatorHouseCore.PuppeteerBrowser
{
    [DataContract]
    [DebuggerDisplay("Success = {Success}, Message = {Message}, Result = {Result}")]
    [KnownType(typeof(object[]))]
    [KnownType(typeof(Dictionary<string, object>))]
    public class PuppeteerJavascriptResponse
    {

        public PuppeteerJavascriptResponse()
        {

        }
        [DataMember]
        public string Message { get; set; }
        //
        // Summary:
        //     Was the javascript executed successfully
        [DataMember]
        public bool Success { get; set; }
        //
        // Summary:
        //     Javascript response
        [DataMember]
        public object Result { get; set; }
    }
}
