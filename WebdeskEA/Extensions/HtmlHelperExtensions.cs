using System;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;

public static class HtmlHelperExtensions
{
    private const string _partialViewScriptItemPrefix = "scripts_";

    /// <summary>
    /// Captures a script block in a partial view for later rendering in the layout.
    /// </summary>
    public static IHtmlContent PartialSectionScripts(this IHtmlHelper htmlHelper, Func<object, HelperResult> template)
    {
        // Store the template in HttpContext.Items, keyed by a unique ID.
        htmlHelper.ViewContext.HttpContext.Items[_partialViewScriptItemPrefix + Guid.NewGuid()] = template;
        // Return an empty builder (so nothing is rendered in place).
        return new HtmlContentBuilder();
    }

    /// <summary>
    /// Renders all captured partial scripts in the layout by iterating over HttpContext.Items.
    /// </summary>
    public static IHtmlContent RenderPartialSectionScripts(this IHtmlHelper htmlHelper)
    {
        var partialSectionScripts = htmlHelper.ViewContext.HttpContext.Items.Keys
            .Where(k => Regex.IsMatch(
                k.ToString(),
                "^" + _partialViewScriptItemPrefix + "([0-9A-Fa-f]{8}[-][0-9A-Fa-f]{4}[-][0-9A-Fa-f]{4}[-][0-9A-Fa-f]{4}[-][0-9A-Fa-f]{12})$"));

        var contentBuilder = new HtmlContentBuilder();
        foreach (var key in partialSectionScripts)
        {
            var template = htmlHelper.ViewContext.HttpContext.Items[key] as Func<object, HelperResult>;
            if (template != null)
            {
                var writer = new System.IO.StringWriter();
                template(null).WriteTo(writer, HtmlEncoder.Default);
                contentBuilder.AppendHtml(writer.ToString());
            }
        }
        return contentBuilder;
    }
}
