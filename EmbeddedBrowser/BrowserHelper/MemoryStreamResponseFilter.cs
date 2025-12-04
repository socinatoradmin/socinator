using System;
using System.IO;
using CefSharp;
using DominatorHouseCore;

namespace EmbeddedBrowser.BrowserHelper
{
    public class MemoryStreamResponseFilter : IResponseFilter
    {
        private MemoryStream _memoryStream;

        public byte[] Data { get; set; }

        bool IResponseFilter.InitFilter()
        {
            //NOTE: We could initialize this earlier, just one possible use of InitFilter
            _memoryStream = new MemoryStream();
            return true;
        }

        FilterStatus IResponseFilter.Filter(Stream dataIn, out long dataInRead, Stream dataOut, out long dataOutWritten)
        {
            try
            {
                if (dataIn == null || !dataIn.CanRead)
                {
                    dataInRead = 0;
                    dataOutWritten = 0;

                    return FilterStatus.Done;
                }

                //Calculate how much data we can read, in some instances dataIn.Length is
                //greater than dataOut.Length
                dataInRead = Math.Min(dataIn.Length, dataOut.Length);
                dataOutWritten = dataInRead;

                var readBytes = new byte[dataInRead];
                dataIn.Read(readBytes, 0, readBytes.Length);
                dataOut.Write(readBytes, 0, readBytes.Length);

                //Write buffer to the memory stream
                _memoryStream.Write(readBytes, 0, readBytes.Length);

                //If we read less than the total amount avaliable then we need
                //return FilterStatus.NeedMoreData so we can then write the rest
                if (dataInRead < dataIn.Length) return FilterStatus.NeedMoreData;

                if (_memoryStream.Length > 0)
                    Data = _memoryStream.ToArray();

                return FilterStatus.Done;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                dataInRead = 0;
                dataOutWritten = 0;
                return FilterStatus.Error;
            }
        }

        public void Dispose()
        {
            try
            { 
               _memoryStream?.Dispose(); 
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}