using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace CityApp.Web.Infrastructure.TagHelpers
{
    /// <summary>
    /// If <see cref="DisabledIf"/> is true, adds a &quot;disabled&quot; attribute to the input element.
    /// </summary>
    [HtmlTargetElement("input", Attributes = "disabled-if")]
    [HtmlTargetElement("select", Attributes = "disabled-if")]
    [HtmlTargetElement("button", Attributes = "disabled-if")]
    public class InputDisabledIfTagHelper : TagHelper
    {
        public bool? DisabledIf { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            // If it's false or null, don't do anything.
            if (DisabledIf == null || DisabledIf == false) { return; }

            // Add the disabled attribute to the input tag.
            var builder = new TagBuilder("ignored");
            builder.Attributes["disabled"] = "";

            output.MergeAttributes(builder);
        }
    }

    /// <summary>
    /// If <see cref="DisabledIfNot" /> is false, adds a &quot;disabled&quot; attribute to the input element.
    /// </summary>
    [HtmlTargetElement("input", Attributes = "disabled-if-not")]
    [HtmlTargetElement("select", Attributes = "disabled-if-not")]
    [HtmlTargetElement("button", Attributes = "disabled-if-not")]
    public class InputDisabledIfNotTagHelper : TagHelper
    {
        public bool? DisabledIfNot { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            // If it's true or null, don't do anything.
            if (DisabledIfNot == null || DisabledIfNot == true) { return; }

            // Add the disabled attribute to the input tag.
            var builder = new TagBuilder("ignored");
            builder.Attributes["disabled"] = "";

            output.MergeAttributes(builder);
        }
    }

    /// <summary>
    /// If <see cref="DisabledIfNull" /> is null, adds a &quot;disabled&quot; attribute to the input element.
    /// </summary>
    [HtmlTargetElement("input", Attributes = "disabled-if-null")]
    [HtmlTargetElement("select", Attributes = "disabled-if-null")]
    [HtmlTargetElement("button", Attributes = "disabled-if-null")]
    public class InputDisabledIfNullTagHelper : TagHelper
    {
        public bool? DisabledIfNull { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            // If it's not null, don't do anything.
            if (DisabledIfNull != null) { return; }

            // Add the disabled attribute to the input tag.
            var builder = new TagBuilder("ignored");
            builder.Attributes["disabled"] = "";

            output.MergeAttributes(builder);
        }
    }
}
