using Markdig;
using Markdig.Renderers;

namespace CourseProject
{
    public static class MarkdownHelper
    {
        public static string ExtractPlainText(string markdown)
        {
            if (string.IsNullOrEmpty(markdown))
                return string.Empty;

            return Markdown.ToPlainText(markdown);
        }
        public static string ToHtml(string markdown)
        {
            if (string.IsNullOrEmpty(markdown))
                return string.Empty;

            return Markdown.ToHtml(markdown);
        }
    }
}
