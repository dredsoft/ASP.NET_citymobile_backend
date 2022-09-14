using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace CityApp.Web.Infrastructure.TagHelpers
{
    /// <summary>
    /// If <see cref="HideIf"/> is true, then add display:none to the element's style. Otherwise, do nothing.
    /// </summary>
    [HtmlTargetElement("div", Attributes = "hide-if")]
    public class ElementHideIfTagHelper : TagHelper
    {
        public bool? HideIf { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            // If it's false or null, don't do anything.
            if (HideIf == null || HideIf == false) { return; }

            // Add the disabled attribute to the input tag.
            var builder = new TagBuilder("ignored");
            builder.Attributes["style"] = "display:none;";

            output.MergeAttributes(builder);
        }
    }

    /// <summary>
    /// If <see cref="HideIfNot"/> is true, then add display:none to the element's style. Otherwise, do nothing.
    /// </summary>
    [HtmlTargetElement("div", Attributes = "hide-if-not")]
    public class ElementHideIfNotTagHelper : TagHelper
    {
        public bool? HideIfNot { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            // If it's true or null, don't do anything.
            if (HideIfNot == null || HideIfNot == true) { return; }

            // Add the disabled attribute to the input tag.
            var builder = new TagBuilder("ignored");
            builder.Attributes["style"] = "display:none;";

            output.MergeAttributes(builder);
        }
    }
}
