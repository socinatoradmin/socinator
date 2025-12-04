using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DominatorHouseCore.Enums
{
    public enum MimeType
    {
        [Description("application/javascript")]
        JavaSript = 1,

        [Description("text/html")]
        Html = 2,

        [Description("application/x-bzip")]
        BZipArchive = 3,

        [Description("application/json")]
        Json = 4,

        [Description("text/javascript")]
        JavaSriptText = 5,

        [Description("application/xhtml+xml")]
        XHTML = 6,

        [Description("application/ld+json")]
        JsonLd = 7,

        [Description("application/x-javascript")]
        JavaScriptApp = 8
    }
}
