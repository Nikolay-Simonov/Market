using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Market.Infrastructure
{
    [HtmlTargetElement("datalist", Attributes = ForAttributeName)]
    [HtmlTargetElement("datalist", Attributes = ItemsAttributeName)]
    public class DataListTagHelper : TagHelper
    {
        private const string ForAttributeName = "asp-for";
        private const string ItemsAttributeName = "asp-items";

        /// <summary>
        /// Creates a new <see cref="DataListTagHelper"/>.
        /// </summary>
        /// <param name="generator">The <see cref="IHtmlGenerator"/>.</param>
        public DataListTagHelper(IHtmlGenerator generator)
        {
            Generator = generator;
        }

        public override int Order => -1000;

        protected IHtmlGenerator Generator { get; }

        /// <summary>
        /// An expression to be evaluated against the current model.
        /// </summary>
        [HtmlAttributeName(ForAttributeName)]
        public ModelExpression For { get; set; }

        [HtmlAttributeName(ItemsAttributeName)]
        public IEnumerable<SelectListItem> Items { get; set; }

        public string Id { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (output == null)
            {
                throw new ArgumentNullException(nameof(output));
            }

            string id = string.IsNullOrWhiteSpace(Id) ? For?.Name : Id;
                
            if (Items == null || !Items.Any() || string.IsNullOrWhiteSpace(id))
            {
                return;
            }

            foreach (var item in Items)
            {
                TagBuilder option = new TagBuilder("option");
                option.Attributes.Add("value", item.Text);
                option.Attributes.Add("data-value", item.Value);
                output.Content.AppendHtml(option);
            }
        
            if (!string.IsNullOrWhiteSpace(id))
            {
                output.Attributes.Add("id", id);
            }
        }
    }
}