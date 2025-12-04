using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PinDominatorCore.PDUtility
{
    public class PDClassesConstant
    {
        public static class Board
        {
            #region Create Board.
            #endregion
            #region Send Board Invitation.
            #endregion
            #region Accept board invitation.
            #endregion
        }
        public static class PinMessenger
        {
            #region Broadcast message.
            public static string MessageInputTextAreaClass { get; set; } = "Gnj Hsu tBJ dyH iFc sAJ L4E Bvi iyn H_e pBj qJc TKt LJB";
            #endregion
            #region Auto Reply to new message.
            #endregion
            #region Message to new followers.
            #endregion
        }
        public static class SocioPublisherPost
        {
            public static string SelectMediaClass1 { get; set; } = "KO4 oqv xcv L4E zI7 iyn Hsu";
            public static string SelectMediaClass2 { get; set; } = "XiG Zr3 hUC s2n sLG xcv L4E zI7 iyn Hsu";
            public static string PinDescriptionTextAreaClass { get; set; } = "public-DraftStyleDefault-block public-DraftStyleDefault-ltr";
        }
        public static class ScriptConstant
        {
            public static string ScriptWithQuerySelectorToGetXandY { get; set; } = "document.querySelector(\'{0}[{1}=\"{2}\"]\').getBoundingClientRect().{3}";
            public static string ScriptWithQuerySelectorAllToGetXandY { get; set; } = "document.querySelectorAll(\'{0}[{1}=\"{2}\"]\')[{3}].getBoundingClientRect().{4}";
            public static string ScriptWithQuerySelectorAllToFilter { get; set; } = "[...document.querySelectorAll(\'{0}[{1}=\"{2}\"]\')].filter(x=>x.textContent.trim()===\"{3}\")[{4}].{5}";
            public static string ScriptWithQuerySelectorAllToFilterNormal { get; set; } = "[...document.querySelectorAll(\'{0}\')].filter(x=>x.textContent.trim()===\"{1}\")[{2}].{3}";
            public static string ScriptWithQuerySelectorToClick { get; set; } = "document.querySelector(\'{0}[{1}=\"{2}\"]\').click();";
            public static string ScriptWithQuerySelectorWithAttributeToClick { get; set; } = "document.querySelector(\'{0}[{1}=\"{2}\"]\').[3].click();";
            public static string ScriptWithQuerySelectorAllToClick { get; set; } = "document.querySelectorAll(\'{0}[{1}=\"{2}\"]\')[{3}].click();";
            public static string ScriptWithQuerySelector { get; set; } = "document.querySelector(\'{0}[{1}]\')";
            public static string ScriptWithQuerySelectorAll { get; set; } = "document.querySelectorAll(\'{0}[{1}]\')[{2}]";
            public static string ScriptGetElementByIdToClick { get; set; } = "document.getElementById(\'{0}\').click();";
            public static string ScriptGetElementsByClassNameToClick { get; set; } = "document.getElementsByClassName(\'{0}\')[{1}].click();";
            public static string ScriptGetElementsByClassNameToGetXY { get; set; } = "document.getElementsByClassName(\'{0}\')[{1}].getBoundingClientRect().{2}";
            public static string ScriptGetElementByIdToGetXY { get; set; } = "document.getElementById(\'{0}\').getBoundingClientRect().{1}";
            public static string ScrollWindowByXXPixel { get; set; } = "window.scrollBy({0},{1})";
            public static string ScrollWindowToXXPixel { get; set; } = "window.scrollTo({0},{1})";
            public static string ScrollWindow { get; set; } = "window.scroll({0},{1})";
        }
    }
}
