using System;
using System.Collections.Generic;
using Market.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Market.Infrastructure
{
    [HtmlTargetElement("div", Attributes = PagingInfoAttributeName)]
    public class PageLinkTagHelper : TagHelper
    {
        private const string PagingInfoAttributeName = "paging-info";
        private readonly IUrlHelperFactory _urlHelperFactory;

        public PageLinkTagHelper(IUrlHelperFactory helperFactory)
        {
            _urlHelperFactory = helperFactory;
        }

        [ViewContext, HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        /// <summary>
        /// Модель пагинации.
        /// </summary>
        [HtmlAttributeName(PagingInfoAttributeName)]
        public PagingInfoVM PagingInfo { get; set; }

        /// <summary>
        /// Максимальное количество ссылок на страницы в линию.
        /// </summary>
        public ushort MaxPagesLine { get; set; } = 7;

        /// <summary>
        /// Метод действия текущего контроллера на который
        /// будут создаваться ссылки.
        /// </summary>
        public string PageAction { get; set; }

        /// <summary>
        /// Класс ссылки на страницу.
        /// </summary>
        public string PageClass { get; set; }

        /// <summary>
        /// Класс ссылки не являющейся текущей страницой.
        /// </summary>
        public string PageClassNormal { get; set; }

        /// <summary>
        /// Класс ссылки текущей страницы.
        /// </summary>
        public string PageClassSelected { get; set; }

        /// <summary>
        /// Текст на кнопке перехода в форме.
        /// </summary>
        public string ButtonText { get; set; } = "Navigate";

        /// <summary>
        /// Указывает будут ли отображаться стрелки навагиации: вперед/назад.
        /// </summary>
        public bool EnableArrows { get; set; } = false;

        /// <summary>
        /// Выясняет применяются ли классы стилей для ссылок на страницы.
        /// </summary>
        /// <returns></returns>
        private bool PageClassesEnabled()
        {
            return !string.IsNullOrWhiteSpace(PageClass)
                   && !string.IsNullOrWhiteSpace(PageClassNormal)
                   && !string.IsNullOrWhiteSpace(PageClassSelected);
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (PagingInfo == null || PagingInfo.TotalPages < 2)
            {
                return;
            }

            IUrlHelper urlHelper = _urlHelperFactory.GetUrlHelper(ViewContext);
            var div = new TagBuilder("div");
            div.Attributes["style"] = "display: flex";

            // Получаем данные модели
            var modelState =
                urlHelper.ActionContext?.ModelState ?? new ModelStateDictionary();

            var models = new Dictionary<string, object>();
            var pagePresent = false;

            foreach (var (key, value) in modelState)
            {
                if (string.Compare(key, "page", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    pagePresent = true;
                    continue;
                }

                models.Add(key, value.RawValue);
            }

            if (!pagePresent)
            {
                models.Add("page", string.Empty);
            }

            // Выясняем применяются ли классы стилей
            bool pageClassesEnabled = PageClassesEnabled();

            // Генерируем стрелку <, если она активирована и допустима
            if (EnableArrows && PagingInfo.CurrentPage > 1)
            {
                var a = new TagBuilder("a");
                models["page"] = (PagingInfo.CurrentPage - 1).ToString();
                a.Attributes["href"] = urlHelper.Action(PageAction, models);
                a.Attributes["style"] = "font-weight: bold;";
                a.InnerHtml.Append("<");

                if (pageClassesEnabled)
                {
                    a.AddCssClass(PageClass);
                    a.AddCssClass(PageClassNormal);
                }

                div.InnerHtml.AppendHtml(a);
            }

            // Количество страниц в линию не может превышать общее количество страниц
            if (MaxPagesLine > PagingInfo.TotalPages)
            {
                MaxPagesLine = (ushort)PagingInfo.TotalPages;
            }

            // Вычисляем верхнюю и нижнюю границу
            int upperBound = PagingInfo.CurrentPage + MaxPagesLine - MaxPagesLine / 2 - 1;

            if (upperBound < MaxPagesLine)
            {
                upperBound = MaxPagesLine;
            }
            if (upperBound > PagingInfo.TotalPages)
            {
                upperBound = PagingInfo.TotalPages;
            }

            int lowerBound = upperBound - MaxPagesLine + 1;

            // Генерируем диапазон ссылок на страницы
            for (int i = lowerBound; i <= upperBound; i++)
            {
                var a = new TagBuilder("a");
                models["page"] = i.ToString();
                a.Attributes["href"] = urlHelper.Action(PageAction, models);

                if (pageClassesEnabled)
                {
                    a.AddCssClass(PageClass);

                    string currentClass = i == PagingInfo.CurrentPage
                        ? PageClassSelected
                        : PageClassNormal;

                    a.AddCssClass(currentClass);
                }

                a.InnerHtml.Append(i.ToString());
                div.InnerHtml.AppendHtml(a);
            }

            // Генерируем стрелку >, если она активирована и допустима
            if (EnableArrows && PagingInfo.CurrentPage < PagingInfo.TotalPages)
            {
                var a = new TagBuilder("a");
                models["page"] = (PagingInfo.CurrentPage + 1).ToString();
                a.Attributes["href"] = urlHelper.Action(PageAction, models);
                a.Attributes["style"] = "font-weight: bold;";
                a.InnerHtml.Append(">");

                if (pageClassesEnabled)
                {
                    a.AddCssClass(PageClass);
                    a.AddCssClass(PageClassNormal);
                }

                div.InnerHtml.AppendHtml(a);
            }

            // Если общее количество страниц превышает максимальное в линии,
            // то генерируем форму для навигации
            if (PagingInfo.TotalPages > MaxPagesLine)
            {
                var form = new TagBuilder("form");
                form.Attributes["method"] = "get";
                form.Attributes["action"] = urlHelper.Action(PageAction);

                // Генерируем скрытый ввод для данных модели в форме
                foreach(var (key, value) in models)
                {
                    if (string.Compare(key, "page", StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        continue;
                    }

                    if (value is IEnumerable<string> stringValues)
                    {
                        foreach (string stringValue in stringValues)
                        {
                            var hidden = new TagBuilder("input");
                            hidden.Attributes["type"] = "hidden";
                            hidden.Attributes["id"] = key;
                            hidden.Attributes["name"] = key;
                            hidden.Attributes["value"] = stringValue;
                            form.InnerHtml.AppendHtml(hidden);
                        }
                    }
                    else
                    {
                        var hidden = new TagBuilder("input");
                        hidden.Attributes["type"] = "hidden";
                        hidden.Attributes["id"] = key;
                        hidden.Attributes["name"] = key;
                        hidden.Attributes["value"] = value.ToString();
                        form.InnerHtml.AppendHtml(hidden);
                    }
                }

                var inputGroup = new TagBuilder("div");
                inputGroup.AddCssClass("input-group");

                var inputGroupBtn = new TagBuilder("div");
                inputGroupBtn.AddCssClass("input-group-btn");

                var input = new TagBuilder("input");
                input.Attributes["type"] = "number";
                input.Attributes["name"] = "Page";
                input.Attributes["id"] = "Page";
                input.Attributes["min"] = "1";
                input.Attributes["max"] = PagingInfo.TotalPages.ToString();
                input.Attributes["class"] = "form-control";
                input.Attributes["value"] = PagingInfo.CurrentPage.ToString();

                var submit = new TagBuilder("button");
                submit.Attributes["type"] = "submit";
                submit.InnerHtml.Append(ButtonText);
                submit.AddCssClass(PageClassSelected);
                submit.AddCssClass(PageClass);

                inputGroupBtn.InnerHtml.AppendHtml(submit);
                inputGroup.InnerHtml.AppendHtml(input);
                inputGroup.InnerHtml.AppendHtml(inputGroupBtn);

                form.InnerHtml.AppendHtml(inputGroup);
                div.InnerHtml.AppendHtml(form);
            }

            output.MergeAttributes(div);
            output.Content.AppendHtml(div.InnerHtml);
        }
    }
}