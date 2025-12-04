using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PinDominatorCore.PDUtility
{
    public class HtmlTags
    {
        public static string Button { get; set; } = "button";
        public static string Anchor { get; set; } = "a";
        public static string Div { get; set; } = "div";
        public static string Img { get; set; } = "img";
        public static string Paragraph { get; set; } = "p";
        public static string Span { get; set; } = "span";
        public static string UnorderedList { get; set; } = "ul";
        public static string List { get; set; } = "li";
        public static string Code { get; set; } = "code";
        public static string Input { get; set; } = "input";
        public static string Title { get; set; } = "title";
        public static string Form { get; set; } = "form";
        public static string TextArea { get; set; } = "textarea";
        public static string Heading(int bold = 1) => (bold < 1 || bold > 6) ? "" : $"h{bold}";
        public static string ListIcon { get; set; } = "li-icon";
        public static string Window { get; set; } = "window";
        public static class HtmlAttribute
        {
            public static string DataTestId { get; set; } = "data-test-id";
            public static string AriaLabel { get; set; } = "aria-label";
            public static string PlaceHolder { get; set; }="placeholder";
            public static string Role { get; set; } = "role";
            public static string DataTutorialId { get; set; } = "data-tutorial-id";
            public static string DataGroupId { get; set; } = "data-group-id";
            public static string Id { get; set; } = "id";
            public static string Class { get; set; } = "class";
            public static string Type { get; set; } = "type";
        }
    }
}
