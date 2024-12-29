using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Text;
using System.Text.RegularExpressions;

namespace CourseProject.Utilities
{
    public class HtmlGenerator
    {
        public static async Task<string> HtmlPage(string viewName, object model, HttpContext httpContext, RouteData routeData, ControllerContext controllerContext)
        {
            var htmlContent = await RenderViewToStringAsync(viewName, model, httpContext, routeData, controllerContext);
            return await InlineStylesAsync(htmlContent);
        }
        public static async Task<string> InlineStylesAsync(string htmlContent)
        {
            var httpClient = new HttpClient();

            var linkRegex = new Regex("<link[^>]*rel=\"stylesheet\"[^>]*href=\"([^\"]+)\"[^>]*>", RegexOptions.IgnoreCase);
            var matches = linkRegex.Matches(htmlContent);

            var inlinedStyles = new StringBuilder();

            foreach (Match match in matches)
            {
                var href = match.Groups[1].Value;

                try
                {
                    string cssContent;
                    if (href.StartsWith("http"))
                    {
                        cssContent = await httpClient.GetStringAsync(href);
                    }
                    else
                    {
                        cssContent = System.IO.File.ReadAllText(Path.Combine(AppContext.BaseDirectory, href));
                    }

                    inlinedStyles.AppendLine(cssContent);

                    htmlContent = htmlContent.Replace(match.Value, string.Empty);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка загрузки стиля: {href}. {ex.Message}");
                }
            }

            var styleTag = $"<style>{inlinedStyles}</style>";
            htmlContent = htmlContent.Replace("</head>", $"{styleTag}</head>");

            return htmlContent;
        }

        public static async Task<string> RenderViewToStringAsync(string viewName, object model, HttpContext httpContext, RouteData routeData, ControllerContext controllerContext)
        {
            var viewEngine = httpContext.RequestServices.GetService(typeof(IRazorViewEngine)) as IRazorViewEngine;
            var tempDataProvider = httpContext.RequestServices.GetService(typeof(ITempDataProvider)) as ITempDataProvider;
            var actionContext = new ActionContext(httpContext, routeData, controllerContext.ActionDescriptor);

            using var sw = new StringWriter();
            var viewResult = viewEngine.FindView(actionContext, viewName, false);

            var viewDictionary = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary())
            {
                Model = model
            };

            var tempData = new TempDataDictionary(actionContext.HttpContext, tempDataProvider);
            var viewContext = new ViewContext(actionContext, viewResult.View, viewDictionary, tempData, sw, new HtmlHelperOptions());
            await viewResult.View.RenderAsync(viewContext);

            return sw.ToString();
        }
    }
}
