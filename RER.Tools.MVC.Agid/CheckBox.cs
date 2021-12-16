using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.UI;

namespace RER.Tools.MVC.Agid
{
    public static partial class ExtensionMethods
    {
        public static MvcHtmlString AgidCheckBoxFor<TModel>(this HtmlHelper<TModel> helper, Expression<Func<TModel, bool>> expression, bool isDisabled = false, string id = null)
        {
            /*
             <div>
                <div class="form-check">
                    <input id="checkbox1" type="checkbox">
                    <label for="checkbox1">Checkbox di esempio</label>
                </div>
             </div>
             */

            string controlId = id ?? helper.AgidIDFor(expression).ToString();

            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }

            ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);
            bool? isChecked = null;
            if (metadata.Model != null)
            {
                bool modelChecked;
                if (Boolean.TryParse(metadata.Model.ToString(), out modelChecked))
                {
                    isChecked = modelChecked;
                }
            }

            StringWriter stringWriter = new StringWriter();
            using (HtmlTextWriter writer = new HtmlTextWriter(stringWriter))
            {
                string name = ExpressionHelper.GetExpressionText(expression);
                IDictionary<string, object> unobtrusiveValitaionAttributes = helper.GetUnobtrusiveValidationAttributes(name, metadata);

                writer.AddAttribute(HtmlTextWriterAttribute.Class, $"form-check");
                writer.RenderBeginTag(HtmlTextWriterTag.Div);

                if (isDisabled)
                    writer.AddAttribute(HtmlTextWriterAttribute.Disabled, "disabled");
                if (isChecked.HasValue && isChecked.Value)
                    writer.AddAttribute(HtmlTextWriterAttribute.Checked, "checked");
                writer.AddAttribute(HtmlTextWriterAttribute.Value, "true");
                writer.AddAttribute(HtmlTextWriterAttribute.Name, name);
                writer.AddAttribute(HtmlTextWriterAttribute.Id, controlId);
                writer.AddAttribute(HtmlTextWriterAttribute.Type, "checkbox");
                writer.RenderBeginTag(HtmlTextWriterTag.Input);
                writer.RenderEndTag(); // end Input

                if (unobtrusiveValitaionAttributes.ContainsKey("data-val-required"))
                    writer.AddAttribute(HtmlTextWriterAttribute.Class, "required");
                writer.AddAttribute(HtmlTextWriterAttribute.For, controlId);
                writer.RenderBeginTag(HtmlTextWriterTag.Label);
                writer.Write(helper.DisplayNameFor(expression).ToHtmlString());
                writer.RenderEndTag(); // end Label

                writer.Write(helper.ValidationMessageFor(expression, "", new { @class = "text-danger" }).ToHtmlString());

                writer.RenderEndTag(); // end Div
            }

            return new MvcHtmlString(stringWriter.ToString());
        }

        public static MvcHtmlString AgidToggleFor<TModel>(this HtmlHelper<TModel> helper, Expression<Func<TModel, bool?>> expression, bool isDisabled = false)
        {
            /*
            <div class="form-check">
                <div class="toggles">
                    <label for="toggleEsempio1a">Label dell'interruttore 1
                        <input type="checkbox" id="toggleEsempio1a">
                        <span class="lever"></span>
                    </label>
                </div>
            </div>
             */

            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }

            ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);
            bool? isChecked = null;
            if (metadata.Model != null)
            {
                bool modelChecked;
                if (Boolean.TryParse(metadata.Model.ToString(), out modelChecked))
                {
                    isChecked = modelChecked;
                }
            }

            StringWriter stringWriter = new StringWriter();
            using (HtmlTextWriter writer = new HtmlTextWriter(stringWriter))
            {
                string name = ExpressionHelper.GetExpressionText(expression);
                IDictionary<string, object> unobtrusiveValitaionAttributes = helper.GetUnobtrusiveValidationAttributes(name, metadata);

                writer.AddAttribute(HtmlTextWriterAttribute.Class, "form-check");
                writer.RenderBeginTag(HtmlTextWriterTag.Div);

                writer.AddAttribute(HtmlTextWriterAttribute.Class, "toggles");
                writer.RenderBeginTag(HtmlTextWriterTag.Div);

                if (unobtrusiveValitaionAttributes.ContainsKey("data-val-required"))
                    writer.AddAttribute(HtmlTextWriterAttribute.Class, "required");
                writer.RenderBeginTag(HtmlTextWriterTag.Label);
                writer.Write(helper.DisplayNameFor(expression).ToHtmlString());

                if (isDisabled)
                    writer.AddAttribute(HtmlTextWriterAttribute.Disabled, "disabled");
                if (isChecked.HasValue && isChecked.Value)
                    writer.AddAttribute(HtmlTextWriterAttribute.Checked, "checked");
                writer.AddAttribute(HtmlTextWriterAttribute.Value, "true");
                writer.AddAttribute(HtmlTextWriterAttribute.Name, name);
                writer.AddAttribute(HtmlTextWriterAttribute.Id, helper.AgidIDFor(expression).ToString());
                writer.AddAttribute(HtmlTextWriterAttribute.Type, "checkbox");
                writer.RenderBeginTag(HtmlTextWriterTag.Input);
                writer.RenderEndTag(); // end Input

                writer.AddAttribute(HtmlTextWriterAttribute.Class, "lever");
                writer.RenderBeginTag(HtmlTextWriterTag.Span);
                writer.RenderEndTag(); // end Span

                writer.RenderEndTag(); // end Label

                writer.Write(helper.ValidationMessageFor(expression, "", new { @class = "text-danger" }).ToHtmlString());

                writer.RenderEndTag(); // end Div
                writer.RenderEndTag(); // end Div
            }

            return new MvcHtmlString(stringWriter.ToString());
        }

        public static MvcHtmlString AgidCheckBoxListFor<TModel, TValue>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TValue>> expression, MultiSelectList items, int maxItemsPerRow = 1, bool isRequired = false)
        {
            /*
             <div>
                <div class="row">
                    <div class="form-check form-check-inline">
                        <input id="checkbox2" type="checkbox" name="checkboxList">
                        <label for="checkbox2">Checkbox non selezionato</label>
                    </div>
                    <div class="form-check form-check-inline">
                        <input id="checkbox3" type="checkbox" name="checkboxList" checked="checked">
                        <label for="checkbox3">Checkbox selezionato</label>
                    </div>
                </div>
             </div>
             */
            ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);
            string name = ExpressionHelper.GetExpressionText(expression);
            IDictionary<string, object> unobtrusiveValitaionAttributes = helper.GetUnobtrusiveValidationAttributes(name, metadata);

            StringWriter stringWriter = new StringWriter();
            using (HtmlTextWriter writer = new HtmlTextWriter(stringWriter))
            {
                int index = 0;
                bool closed = true;
                foreach (var item in items)
                {
                    // apro il Div row ogni volta che serve
                    if (maxItemsPerRow > 1 && index % maxItemsPerRow == 0)
                    {
                        writer.AddAttribute(HtmlTextWriterAttribute.Class, $"row");
                        writer.RenderBeginTag(HtmlTextWriterTag.Div);
                        closed = false;
                    }

                    writer.AddAttribute(HtmlTextWriterAttribute.Class, $"form-check {(maxItemsPerRow > 1 ? "form-check-inline" : "")}");
                    writer.RenderBeginTag(HtmlTextWriterTag.Div);

                    writer.AddAttribute(HtmlTextWriterAttribute.Type, "checkbox");
                    writer.AddAttribute(HtmlTextWriterAttribute.Name, name);
                    writer.AddAttribute(HtmlTextWriterAttribute.Id, $"{name}_{index + 1}");
                    if (item.Selected)
                        writer.AddAttribute(HtmlTextWriterAttribute.Checked, "checked");
                    writer.AddAttribute(HtmlTextWriterAttribute.Value, item.Value);
                    writer.RenderBeginTag(HtmlTextWriterTag.Input);
                    writer.RenderEndTag(); // end Input

                    if (isRequired || unobtrusiveValitaionAttributes.ContainsKey("data-val-required"))
                        writer.AddAttribute(HtmlTextWriterAttribute.Class, "required");
                    writer.AddAttribute(HtmlTextWriterAttribute.For, $"{name}_{index + 1}");
                    writer.RenderBeginTag(HtmlTextWriterTag.Label);
                    writer.Write(item.Text);
                    writer.RenderEndTag(); // end Label

                    writer.RenderEndTag(); // end Div form-check form-check-inline

                    // chiudo il Div row un giro prima di aprirne un'altro
                    if (maxItemsPerRow > 1 && (index + 1) % maxItemsPerRow == 0)
                    {
                        writer.RenderEndTag(); // end Div row
                        closed = true;
                    }

                    index++;
                }

                // Se il Div row non viene chiuso nel loop, lo chiudo alla fine
                if (!closed)
                {
                    writer.RenderEndTag(); // end Div row
                }

                //writer.Write(helper.DropDownListFor(expression, items, htmlAttributes != null ? htmlAttributes : new { htmlAttributes = new { @multiple = "true", @data_multiple_separator = "" } }).ToHtmlString());
                writer.Write(helper.ValidationMessageFor(expression, "", new { @class = "text-danger" }).ToHtmlString());
                

                if (isRequired || unobtrusiveValitaionAttributes.ContainsKey("data-val-required"))
                {
                    writer.WriteLine("");
                    writer.WriteLine("");
                    writer.RenderBeginTag(HtmlTextWriterTag.Script);
                    writer.WriteLine("$( document ).ready(function(e) {");
                    writer.Write($"    $('#{name}_1').parents('form:first').submit(function(e)");
                    writer.WriteLine("    {");
                    writer.WriteLine($"        var visible_checkboxes = $('input[name={name}]:visible');");
                    writer.WriteLine($"        var checked_checkboxes = $('input[name={name}]:checked');");
                    writer.WriteLine($"        var container = $(\"[data-valmsg-for='{name}']\"), replaceAttrValue = container.attr(\"data-valmsg-replace\"), replace = replaceAttrValue ? $.parseJSON(replaceAttrValue) !== false : null;");
                    writer.WriteLine("        if (visible_checkboxes.length > 0 && checked_checkboxes.length == 0)");
                    writer.WriteLine("        {");
                    writer.WriteLine($"            var error = $(\"<span></span>\").text(\"Il campo '{WebUtility.HtmlDecode(helper.DisplayNameFor(expression).ToHtmlString())}' è obbligatorio\");   ");
                    writer.WriteLine("            container.removeClass(\"field-validation-valid\").addClass(\"field-validation-error\");");
                    writer.WriteLine("            error.data(\"unobtrusiveContainer\", container);");
                    writer.WriteLine("            if (replace) {");
                    writer.WriteLine("                container.empty();");
                    writer.WriteLine("                error.removeClass(\"input-validation-error\").appendTo(container);");
                    writer.WriteLine("            }");
                    writer.WriteLine("            else {");
                    writer.WriteLine("                error.hide();");
                    writer.WriteLine("            }");
                    writer.WriteLine("            event.preventDefault();");
                    writer.WriteLine("            try{");
                    writer.WriteLine("                SetOverlay(false);");
                    writer.WriteLine("            } catch (error) {}");
                    writer.WriteLine("        } else {");
                    writer.WriteLine("            container.empty();");
                    writer.WriteLine("            try{");
                    writer.WriteLine("                SetOverlay(false);");
                    writer.WriteLine("            } catch (error) {}");
                    writer.WriteLine("        }");
                    writer.WriteLine("    });");
                    writer.WriteLine("});");
                    writer.RenderEndTag(); // end script
                }
            }

            return new MvcHtmlString(stringWriter.ToString());
        }
    }
}
