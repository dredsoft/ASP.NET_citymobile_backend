using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace CityApp.Web.Infrastructure.TagHelpers
{
    /// <summary>
    /// If <see cref="CheckedIf"/> is true, adds a &quot;checked&quot; attribute to the checkbox or radio input element.
    /// </summary>
    [HtmlTargetElement(Attributes = "[type=radio],checked-if")]
    [HtmlTargetElement(Attributes = "[type=checkbox],checked-if")]
    public class InputCheckedIfTagHelper : TagHelper
    {
        public bool? CheckedIf { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            // If it's false or null, don't do anything.
            if (CheckedIf == null || CheckedIf == false) { return; }

            // Add the checked attribute to the input tag.
            var builder = new TagBuilder("ignored");
            builder.Attributes["checked"] = "";

            output.MergeAttributes(builder);
        }
    }

    /// <summary>
    /// If <see cref="CheckedIfNot" /> is false, adds a &quot;checked&quot; attribute to the checkbox or radio input element.
    /// </summary>
    [HtmlTargetElement(Attributes = "[type=radio],checked-if-not")]
    [HtmlTargetElement(Attributes = "[type=checkbox],checked-if-not")]
    public class InputCheckedIfNotTagHelper : TagHelper
    {
        public bool? CheckedIfNot { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            // If it's true or null, don't do anything.
            if (CheckedIfNot == null || CheckedIfNot == true) { return; }

            // Add the checked attribute to the input tag.
            var builder = new TagBuilder("ignored");
            builder.Attributes["checked"] = "";

            output.MergeAttributes(builder);
        }
    }
}
