using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Market.Infrastructure
{
    [HtmlTargetElement("div", Attributes = RadioGroupEnumAttributeName)]
    public class EnumRadioGroupTagHelper : TagHelper
    {
        private const string RadioGroupEnumAttributeName = "radio-group-enum";

        /// <summary>
        /// Creates a new <see cref="EnumRadioGroupTagHelper"/>.
        /// </summary>
        /// <param name="generator">The <see cref="IHtmlGenerator"/>.</param>
        public EnumRadioGroupTagHelper(IHtmlGenerator generator)
        {
            Generator = generator;
        }

        public override int Order => -1000;

        protected IHtmlGenerator Generator { get; }

        /// <summary>
        /// An expression to be evaluated against the current model.
        /// </summary>
        [HtmlAttributeName(RadioGroupEnumAttributeName)]
        public ModelExpression RadioGroupEnum { get; set; }

        public string Id { get; set; }

        public string Name { get; set; }

        /// <summary>
        /// Класс радиокнопки.
        /// </summary>
        public string InputClass { get; set; }

        /// <summary>
        /// Класс текущей радиокнопки.
        /// </summary>
        public string ActiveClass { get; set; }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (output == null)
            {
                throw new ArgumentNullException(nameof(output));
            }

            string id = string.IsNullOrWhiteSpace(Id)
                ? RadioGroupEnum?.Name?.Replace(".", "_")
                : Id;

            string name = string.IsNullOrWhiteSpace(Name)
                ? RadioGroupEnum?.Name
                : Name;

            if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(name))
            {
                return;
            }

            if (RadioGroupEnum?.Model == null || !RadioGroupEnum.Metadata.IsEnum)
            {
                throw new ArgumentException("Model expression is null or not enumeration.");
            }

            List<string> enumNames = RadioGroupEnum.Metadata
                .EnumNamesAndValues.Select(kvp => kvp.Key).ToList();

            output.Content.AppendHtml(await output.GetChildContentAsync());

            foreach (string enumName in enumNames)
            {
                var label = new TagBuilder("label");
                var radio = new TagBuilder("input");

                label.AddCssClass(InputClass);
                radio.Attributes["type"] = "radio";
                radio.Attributes["name"] = name;

                if (enumName == RadioGroupEnum.Model.ToString())
                {
                    radio.Attributes.Add("checked", "");
                    label.AddCssClass(ActiveClass);
                }

                radio.Attributes["id"] = id + "_" + enumName;
                radio.Attributes["value"] = enumName;
                label.InnerHtml.AppendHtml(radio);
                label.InnerHtml.Append(enumName);

                output.Content.AppendHtml(label);
            }
        }
    }
}