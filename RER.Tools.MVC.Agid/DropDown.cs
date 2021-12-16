using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.UI;

namespace RER.Tools.MVC.Agid
{
    public static partial class ExtensionMethods
    {
        public static MvcHtmlString AgidDropDownListFor<TModel, TValue>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TValue>> expression, SelectList items, string externalDivAdditionalClasses = null, string optionLabel = null, bool liveSearch = false, bool? required = null, string id = null)
        {
            StringWriter stringWriter = new StringWriter();
            using (HtmlTextWriter writer = new HtmlTextWriter(stringWriter))
            {


                //Func<TModel, TValue> method = expression.Compile();
                ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);

                string name = ExpressionHelper.GetExpressionText(expression);
                string displayName = helper.DisplayNameFor(expression).ToString();
                IDictionary<string, object> unobtrusiveValitaionAttributes = helper.GetUnobtrusiveValidationAttributes(name, metadata);

                writer.AddAttribute(HtmlTextWriterAttribute.Class, $"form-group");
                writer.RenderBeginTag(HtmlTextWriterTag.Div);

                writer.AddAttribute(HtmlTextWriterAttribute.Class, $"bootstrap-select-wrapper{(!string.IsNullOrWhiteSpace(externalDivAdditionalClasses) ? " " + externalDivAdditionalClasses : "")}");
                writer.RenderBeginTag(HtmlTextWriterTag.Div);

                writer.AddAttribute(HtmlTextWriterAttribute.For, id ?? helper.AgidIDFor(expression).ToString());
                writer.AddAttribute(HtmlTextWriterAttribute.Id, $"{(id ?? helper.AgidIDFor(expression).ToString())}-label");
                writer.AddAttribute(HtmlTextWriterAttribute.Class, string.Format("control-label{0}", required.HasValue && required.Value ? " required" : ""));
                writer.RenderBeginTag(HtmlTextWriterTag.Label);
                writer.Write(displayName);// new MvcHtmlString(HttpUtility.HtmlEncode(displayName)));
                writer.RenderEndTag(); // end Label

                if (liveSearch)
                {
                    writer.AddAttribute("data-live-search", "true");
                    writer.AddAttribute("data-live-search-placeholder", "Cerca");
                }
                if (required.HasValue && required.Value && !unobtrusiveValitaionAttributes.ContainsKey("data-val-required"))
                {
                    if (!unobtrusiveValitaionAttributes.ContainsKey("data-val"))
                        writer.AddAttribute("data-val", "true");
                    writer.AddAttribute("data-val-required", string.Format("Il campo {0} è obbligatorio", displayName));
                }
                writer.AddAttribute(HtmlTextWriterAttribute.Name, name);
                writer.AddAttribute(HtmlTextWriterAttribute.Id, id ?? helper.AgidIDFor(expression).ToString());
                writer.AddAttribute("aria-labelledby", $"{(id ?? helper.AgidIDFor(expression).ToString())}-label");

                foreach (var attr in unobtrusiveValitaionAttributes)
                {
                    writer.AddAttribute(attr.Key, attr.Value.ToString());
                }
                writer.RenderBeginTag(HtmlTextWriterTag.Select);

                if (optionLabel != null)
                {
                    writer.AddAttribute(HtmlTextWriterAttribute.Value, "");
                    writer.RenderBeginTag(HtmlTextWriterTag.Option);
                    writer.Write(optionLabel);
                    writer.RenderEndTag(); // end Option
                }

                foreach (var item in items)
                {
                    if (item.Selected)
                        writer.AddAttribute(HtmlTextWriterAttribute.Selected, "selected");
                    writer.AddAttribute(HtmlTextWriterAttribute.Value, item.Value);
                    writer.RenderBeginTag(HtmlTextWriterTag.Option);
                    writer.Write(item.Text);
                    writer.RenderEndTag(); // end Option
                }

                writer.RenderEndTag(); // end Select
                writer.Write(helper.ValidationMessageFor(expression, "", new { @class = "text-danger" }).ToHtmlString());

                writer.RenderEndTag(); // end Div
                writer.RenderEndTag(); // end Div
            }

            return new MvcHtmlString(stringWriter.ToString());
        }

        public static MvcHtmlString AgidMultipleDropDownListFor<TModel, TValue>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TValue>> expression, MultiSelectList items, string optionLabel = null, bool? required = null, string id = null)
        {
            StringWriter stringWriter = new StringWriter();
            using (HtmlTextWriter writer = new HtmlTextWriter(stringWriter))
            {

                string name = ExpressionHelper.GetExpressionText(expression);
                string label_name = helper.DisplayNameFor(expression).ToString();
                Func<TModel, TValue> method = expression.Compile();
                ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);
                IDictionary<string, object> unobtrusiveValitaionAttributes = helper.GetUnobtrusiveValidationAttributes(name, metadata);

                writer.AddAttribute(HtmlTextWriterAttribute.Class, "form-group");
                writer.RenderBeginTag(HtmlTextWriterTag.Div);

                writer.AddAttribute(HtmlTextWriterAttribute.Class, "bootstrap-select-wrapper");
                writer.RenderBeginTag(HtmlTextWriterTag.Div);

                if (unobtrusiveValitaionAttributes.ContainsKey("data-val-required") || (required.HasValue && required.Value))
                    writer.AddAttribute(HtmlTextWriterAttribute.Class, "required");

                writer.AddAttribute(HtmlTextWriterAttribute.For, id ?? helper.AgidIDFor(expression).ToString());
                writer.AddAttribute(HtmlTextWriterAttribute.Id, $"{(id ?? helper.AgidIDFor(expression).ToString())}-label");
                writer.RenderBeginTag(HtmlTextWriterTag.Label);
                writer.Write(label_name);
                writer.RenderEndTag(); // end Label

                if (required.HasValue && required.Value && !unobtrusiveValitaionAttributes.ContainsKey("data-val-required"))
                {
                    writer.AddAttribute("data-val", "true");
                    writer.AddAttribute("data-val-required", string.Format("Il campo {0} è obbligatorio", label_name));
                }

                writer.AddAttribute(HtmlTextWriterAttribute.Name, name);
                writer.AddAttribute(HtmlTextWriterAttribute.Id, id ?? helper.AgidIDFor(expression).ToString());
                writer.AddAttribute("aria-labelledby", $"{(id ?? helper.AgidIDFor(expression).ToString())}-label");

                if (optionLabel != null)
                    writer.AddAttribute(HtmlTextWriterAttribute.Title, optionLabel);

                writer.AddAttribute(HtmlTextWriterAttribute.Multiple, "multiple");
                writer.AddAttribute("data-multiple-separator", "");
                foreach (var attr in unobtrusiveValitaionAttributes)
                {
                    writer.AddAttribute(attr.Key, attr.Value.ToString());
                }
                writer.RenderBeginTag(HtmlTextWriterTag.Select);

                foreach (var item in items)
                {
                    if (item.Selected)
                        writer.AddAttribute(HtmlTextWriterAttribute.Selected, "selected");
                    writer.AddAttribute(HtmlTextWriterAttribute.Value, item.Value);
                    writer.AddAttribute("data-content", $"<span class='select-pill'><span class='select-pill-text'>{item.Text}</span></span>");
                    writer.RenderBeginTag(HtmlTextWriterTag.Option);
                    writer.Write(item.Text);
                    writer.RenderEndTag(); // end Option
                }

                writer.RenderEndTag(); // end Select

                //writer.Write(helper.DropDownListFor(expression, items, htmlAttributes != null ? htmlAttributes : new { htmlAttributes = new { @multiple = "true", @data_multiple_separator = "" } }).ToHtmlString());
                writer.Write(helper.ValidationMessageFor(expression, "", new { @class = "text-danger" }).ToHtmlString());

                writer.RenderEndTag(); // end Div
                writer.RenderEndTag(); // end Div
            }

            return new MvcHtmlString(stringWriter.ToString());
        }

        public static MvcHtmlString AgidDropDownList<TModel>(this HtmlHelper<TModel> helper, string name, string label, SelectList items, object htmlAttributes, string optionLabel = null, bool liveSearch = false, string titleLabel = null, bool required = false, string id = null)
        {
            /*
            <div class="bootstrap-select-wrapper">
                @Html.LabelFor(model => model.Username, "Username", htmlAttributes: new { @class = "control-label" })
                @Html.DropDownList("Username", null, htmlAttributes: new { @class = "form-control" })
                @Html.ValidationMessageFor(model => model.Username, "", new { @class = "text-danger" })
            </div>

            <div class="form-group">
                <div class="bootstrap-select-wrapper">
                    <label class="control-label" for="Titolare_Responsabile">Titolare/Responsabile</label><select htmlAttributes="{ }" id="Titolare_Responsabile" name="Titolare_Responsabile"><option value="R">Sono responsabile</option>
            <option value="T">Sono titolare</option>
            </select><span class="field-validation-valid text-danger" data-valmsg-for="Titolare_Responsabile" data-valmsg-replace="true"></span>
                </div>
            </div>
             */
            StringWriter stringWriter = new StringWriter();
            using (HtmlTextWriter writer = new HtmlTextWriter(stringWriter))
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Class, "form-group");
                writer.RenderBeginTag(HtmlTextWriterTag.Div);

                writer.AddAttribute(HtmlTextWriterAttribute.Class, "bootstrap-select-wrapper");
                writer.RenderBeginTag(HtmlTextWriterTag.Div);

                if (!string.IsNullOrEmpty(label))
                    writer.Write(helper.Label(name, label, htmlAttributes: new { @class = string.Format("control-label{0}", required ? " required" : ""), @title = titleLabel, @for = id ?? name }).ToHtmlString());

                if (liveSearch)
                {
                    writer.AddAttribute("data-live-search", "true");
                    writer.AddAttribute("data-live-search-placeholder", "Cerca");
                }

                writer.AddAttribute(HtmlTextWriterAttribute.Name, name);
                writer.AddAttribute(HtmlTextWriterAttribute.Id, id ?? name);
                writer.RenderBeginTag(HtmlTextWriterTag.Select);

                if (optionLabel != null)
                {
                    writer.AddAttribute(HtmlTextWriterAttribute.Value, "");
                    writer.RenderBeginTag(HtmlTextWriterTag.Option);
                    writer.Write(optionLabel);
                    writer.RenderEndTag(); // end Option
                }

                foreach (var item in items)
                {
                    if (item.Selected)
                        writer.AddAttribute(HtmlTextWriterAttribute.Selected, "selected");
                    writer.AddAttribute(HtmlTextWriterAttribute.Value, item.Value);
                    writer.RenderBeginTag(HtmlTextWriterTag.Option);
                    writer.Write(item.Text);
                    writer.RenderEndTag(); // end Option
                }

                writer.RenderEndTag(); // end Select



                writer.Write(helper.ValidationMessage(name, "", new { @class = "text-danger" }).ToHtmlString());

                writer.RenderEndTag(); // Div

                writer.RenderEndTag(); // Div
            }

            return new MvcHtmlString(stringWriter.ToString());
        }
    }
}
