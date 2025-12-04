#region

using System;
using System.Net;

#endregion

namespace DominatorHouseCore.Requests
{
    public static class WebHelper
    {
        public static WebExceptionIssue GetErrorMsgWebrequest(this WebException ex)
        {
            switch (ex.Status)
            {
                case WebExceptionStatus.Success:
                    return new WebExceptionIssue
                    {
                        MessageShort = "Success",
                        MessageLong = "No error was encountered.",
                        MessageSolution = "Everything is fine"
                    };
                case WebExceptionStatus.NameResolutionFailure:
                    return new WebExceptionIssue
                    {
                        MessageShort = "No connection",
                        MessageLong = "Unable to resolve host name.",
                        MessageSolution =
                            "Make sure there is an internet connection. Make sure your DNS server is working."
                    };
                case WebExceptionStatus.ConnectFailure:
                    return new WebExceptionIssue
                    {
                        MessageShort = "No connection",
                        MessageLong = "Unable to make a request to the host. Unable to create a connection.",
                        MessageSolution = "Make sure there is an internet connection."
                    };
                case WebExceptionStatus.ReceiveFailure:
                    return new WebExceptionIssue
                    {
                        MessageShort = "Server Error",
                        MessageLong = "A complete response was not received from the remote server.",
                        MessageSolution =
                            "Make sure there is an internet connection and that your connection is stable."
                    };
                case WebExceptionStatus.SendFailure:
                    return new WebExceptionIssue
                    {
                        MessageShort = "DNS Error",
                        MessageLong = "A complete request could not be sent to the remote server.",
                        MessageSolution =
                            "Make sure there is an internet connection and that your connection is stable."
                    };
                case WebExceptionStatus.PipelineFailure:
                    return new WebExceptionIssue
                    {
                        MessageShort = "Connection error",
                        MessageLong =
                            "The request was a pipelined request and the connection was closed before the response was received. ",
                        MessageSolution =
                            "Make sure there is an internet connection and that your connection is stable."
                    };
                case WebExceptionStatus.RequestCanceled:
                    return new WebExceptionIssue
                    {
                        MessageShort = "Request canceled",
                        MessageLong = "The request was canceled/aborted.",
                        MessageSolution = "Contact support"
                    };
                case WebExceptionStatus.ProtocolError:
                    switch (((HttpWebResponse) ex.Response).StatusCode)
                    {
                        case HttpStatusCode.RequestTimeout:
                            return new WebExceptionIssue
                            {
                                MessageShort = "Bad Request",
                                MessageLong = "The data you sent was not expected by the server.",
                                MessageSolution =
                                    "Make sure the data you send to the server is correct. If the issue persists, contact support."
                            };
                        case HttpStatusCode.InternalServerError:
                        case HttpStatusCode.BadGateway:
                            return new WebExceptionIssue
                            {
                                MessageShort = "Connection error",
                                MessageLong = "An incomplete request was made.",
                                MessageSolution =
                                    "Make sure there is an internet connection. If the issue persists, contact support."
                            };
                        case HttpStatusCode.ServiceUnavailable:
                            return new WebExceptionIssue
                            {
                                MessageShort = "Connection error",
                                MessageLong = "Server has closed.",
                                MessageSolution =
                                    "Make sure there is an internet connection. If the issue persists, contact support."
                            };
                        default:
                            return new WebExceptionIssue
                            {
                                MessageShort = "Protocol error",
                                MessageLong = "Unknown protocol error occurred.",
                                MessageSolution =
                                    "Make sure there is an internet connection. If the issue persists, contact support."
                            };
                    }

                case WebExceptionStatus.ConnectionClosed:
                    return new WebExceptionIssue
                    {
                        MessageShort = "Connection error",
                        MessageLong = "The Connection was prematurely closed.",
                        MessageSolution =
                            "Make sure there is an internet connection. If the issue persists, contact support."
                    };
                case WebExceptionStatus.TrustFailure:
                    return new WebExceptionIssue
                    {
                        MessageShort = "Validation error",
                        MessageLong = "A server certificate could not validated.",
                        MessageSolution =
                            "Make sure there is an internet connection. If the issue persists, contact support."
                    };
                case WebExceptionStatus.SecureChannelFailure:
                    return new WebExceptionIssue
                    {
                        MessageShort = "Connection error",
                        MessageLong = "An error occurred while establishing a connection using SSL.",
                        MessageSolution =
                            "Make sure there is an internet connection. If the issue persists, contact support."
                    };
                case WebExceptionStatus.ServerProtocolViolation:
                    return new WebExceptionIssue
                    {
                        MessageShort = "Server error",
                        MessageLong = "The server response was not a valid HTTP Response.",
                        MessageSolution =
                            "Make sure there is an internet connection. If the issue persists, contact support."
                    };
                case WebExceptionStatus.KeepAliveFailure:
                    return new WebExceptionIssue
                    {
                        MessageShort = "Connection error",
                        MessageLong =
                            "The connection for a request that specifies the Keep-alive header was closed unexpectedly.",
                        MessageSolution =
                            "Make sure there is an internet connection. If the issue persists, contact support."
                    };
                case WebExceptionStatus.Pending:
                    return new WebExceptionIssue
                    {
                        MessageShort = "Connection error",
                        MessageLong = "An internal asynchronous request is pending.",
                        MessageSolution = "Contact support"
                    };
                case WebExceptionStatus.Timeout:
                    return new WebExceptionIssue
                    {
                        MessageShort = "Request Timed Out",
                        MessageLong = "When requesting an url, the request timed out.",
                        MessageSolution =
                            "Make sure there is an internet connection, or try to increase the request timeout."
                    };
                case WebExceptionStatus.ProxyNameResolutionFailure:
                    return new WebExceptionIssue
                    {
                        MessageShort = "No connection",
                        MessageLong = "The name resolver service could not resolve the proxy host name.",
                        MessageSolution = "Make sure there is an internet connection."
                    };
                case WebExceptionStatus.UnknownError:
                    return new WebExceptionIssue
                    {
                        MessageShort = "Unknown Error",
                        MessageLong = "The exception of unknown type has occurred",
                        MessageSolution = "Make sure there is an internet connection."
                    };
                case WebExceptionStatus.MessageLengthLimitExceeded:
                    return new WebExceptionIssue
                    {
                        MessageShort = "Request/Response Error",
                        MessageLong =
                            "A message was received that exceeded the specified limit when sending a request or receiving response from the server.",
                        MessageSolution = "Make sure there is an internet connection."
                    };
                case WebExceptionStatus.CacheEntryNotFound:
                    return new WebExceptionIssue
                    {
                        MessageShort = "Server Error",
                        MessageLong = "The specified cache entry was not found",
                        MessageSolution = "Make sure there is an internet connection."
                    };
                case WebExceptionStatus.RequestProhibitedByCachePolicy:
                    return new WebExceptionIssue
                    {
                        MessageShort = "Request Error",
                        MessageLong = "The request was not permitted by the cache policy.",
                        MessageSolution = "Make sure there is an internet connection."
                    };
                case WebExceptionStatus.RequestProhibitedByProxy:
                    return new WebExceptionIssue
                    {
                        MessageShort = "Request Error",
                        MessageLong = "The request was not permitted by the proxy.",
                        MessageSolution = "Make sure there is an internet connection."
                    };
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public class WebExceptionIssue
        {
            public string MessageLong { get; set; }

            public string MessageShort { get; set; }

            public string MessageSolution { get; set; }
        }
    }
}