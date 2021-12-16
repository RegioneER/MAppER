using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        public enum TextBoxType
        {
            Text,
            Email,
            Number,
            Phone,
            Time,
            Date,
            Url
        }

        public static MvcHtmlString AgidUrlTextBoxFor<TModel, TValue>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TValue>> expression,
            string label = null, string placeholder = null, string testoInformativo = null, bool isDisabled = false, bool isReadonly = false, string iconClass = null, string id = null, bool required = false)
        {
            StringWriter stringWriter = new StringWriter();
            using (HtmlTextWriter writer = new HtmlTextWriter(stringWriter))
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Class, $"row");
                writer.RenderBeginTag(HtmlTextWriterTag.Div);
                writer.AddAttribute(HtmlTextWriterAttribute.Class, $"col-md-10");
                writer.RenderBeginTag(HtmlTextWriterTag.Div);

                string name = helper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(ExpressionHelper.GetExpressionText(expression));
                string inputClass = "form-control";
                string linkControlId = $"{(id ?? name).Replace(".", "_")}_link";
                string textBoxId = (id ?? helper.AgidIDFor(expression).ToString());
                string textBoxName = (name ?? id).Replace(".", "_");
                //Func<TModel, TValue> method = expression.Compile();
                ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);
                IDictionary<string, object> unobtrusiveValitaionAttributes = helper.GetUnobtrusiveValidationAttributes(name, metadata);

                ModelState modelState;
                if (helper.ViewData.ModelState.TryGetValue(name, out modelState) && modelState.Errors.Count > 0)
                {
                    inputClass = $"{inputClass} {HtmlHelper.ValidationInputCssClassName}";
                }

                string value;
                if (modelState != null && modelState.Value != null)
                {
                    value = modelState.Value.AttemptedValue;
                }
                else if (metadata.Model != null)
                {
                    value = metadata.Model.ToString();
                }
                else
                {
                    value = String.Empty;
                }

                // external Div
                writer.AddAttribute(HtmlTextWriterAttribute.Class, $"form-group");
                writer.RenderBeginTag(HtmlTextWriterTag.Div);

                if (!string.IsNullOrWhiteSpace(iconClass))
                {
                    writer.AddAttribute(HtmlTextWriterAttribute.Class, $"input-group");
                    writer.RenderBeginTag(HtmlTextWriterTag.Div);

                    writer.AddAttribute(HtmlTextWriterAttribute.Class, $"input-group-prepend");
                    writer.RenderBeginTag(HtmlTextWriterTag.Div);

                    writer.AddAttribute(HtmlTextWriterAttribute.Class, $"input-group-text");
                    writer.RenderBeginTag(HtmlTextWriterTag.Div);

                    writer.AddAttribute(HtmlTextWriterAttribute.Class, $"icon icon-sm{" " + iconClass}");
                    writer.RenderBeginTag(HtmlTextWriterTag.I);
                    writer.RenderEndTag(); // end I

                    writer.RenderEndTag(); // end Div

                    writer.RenderEndTag(); // end Div
                }

                // label
                if (!isReadonly && !isDisabled)
                {
                    if (unobtrusiveValitaionAttributes.ContainsKey("data-val-required") || required)
                        writer.AddAttribute(HtmlTextWriterAttribute.Class, "required");
                }

                writer.AddAttribute(HtmlTextWriterAttribute.Style, "width: auto;");
                writer.AddAttribute(HtmlTextWriterAttribute.For, textBoxId);
                writer.RenderBeginTag(HtmlTextWriterTag.Label);
                if (label != null)
                    writer.Write(label);
                else
                    writer.Write(helper.DisplayNameFor(expression).ToHtmlString());
                writer.RenderEndTag(); // end Label

                // Input
                // TODO - capire il tipo dal modello e togliere il tipo dai parametri
                var dataTypeName = metadata.DataTypeName;


                writer.AddAttribute(HtmlTextWriterAttribute.Type, "text");

                writer.AddAttribute(HtmlTextWriterAttribute.Name, name);
                writer.AddAttribute(HtmlTextWriterAttribute.Id, textBoxId);
                writer.AddAttribute(HtmlTextWriterAttribute.Class, inputClass);
                if (!string.IsNullOrWhiteSpace(testoInformativo))
                    writer.AddAttribute("placeholder", placeholder);
                if (!string.IsNullOrWhiteSpace(placeholder))
                    writer.AddAttribute("aria-describedby", $"{textBoxId}Desc");
                if (isDisabled)
                    writer.AddAttribute(HtmlTextWriterAttribute.Disabled, "disabled");
                if (isReadonly)
                    writer.AddAttribute(HtmlTextWriterAttribute.ReadOnly, "readonly");
                foreach (var attr in unobtrusiveValitaionAttributes)
                {
                    writer.AddAttribute(attr.Key, attr.Value.ToString());
                }

                if (!isReadonly && !isDisabled)
                {
                    if (!unobtrusiveValitaionAttributes.ContainsKey("data-val-required") && required)
                    {
                        if (!unobtrusiveValitaionAttributes.Any())
                            writer.AddAttribute("data-val", $"true");

                        writer.AddAttribute("data-val-required", $"Il campo {label} è obbligatorio");
                    }
                }

                writer.AddAttribute(HtmlTextWriterAttribute.Value, HttpUtility.HtmlEncode(value));
                writer.AddAttribute("onkeyup", $"validaUrl_{textBoxId.Replace(".", "_")}();");
                writer.RenderBeginTag(HtmlTextWriterTag.Input);
                writer.RenderEndTag(); // end Input

                if (!string.IsNullOrWhiteSpace(testoInformativo))
                {
                    writer.RenderBeginTag(HtmlTextWriterTag.Div);
                    writer.AddAttribute(HtmlTextWriterAttribute.Id, $"{textBoxId}_info");
                    writer.RenderBeginTag(HtmlTextWriterTag.Small);
                    writer.Write(testoInformativo);
                    writer.RenderEndTag(); // end Small
                    writer.RenderEndTag(); // end div
                }

                writer.Write(helper.ValidationMessageFor(expression, "", new { @class = "text-danger", @id = $"{textBoxId}_validator" }).ToHtmlString());

                if (!string.IsNullOrWhiteSpace(iconClass))
                {
                    writer.RenderEndTag(); // end Div
                }

                writer.RenderEndTag(); // end Div
                writer.RenderEndTag(); // end Div

                writer.AddAttribute(HtmlTextWriterAttribute.Class, $"col-md-2");
                writer.RenderBeginTag(HtmlTextWriterTag.Div);
                writer.AddAttribute(HtmlTextWriterAttribute.Class, $"btn btn-default nospinner{((!value.StartsWith("http://") && !value.StartsWith("https://")) ? " disabled" : "")}");
                if (value.StartsWith("http://") || value.StartsWith("https://"))
                {
                    writer.AddAttribute(HtmlTextWriterAttribute.Href, value);
                }
                else
                {
                    writer.AddAttribute(HtmlTextWriterAttribute.Href, "javascript:void(0)");
                }
                writer.AddAttribute(HtmlTextWriterAttribute.Target, "_blank");
                writer.AddAttribute(HtmlTextWriterAttribute.Id, $"{(id ?? name).Replace(".", "_")}_link");
                writer.RenderBeginTag(HtmlTextWriterTag.A);
                if (value.StartsWith("http://") || value.StartsWith("https://"))
                {
                    writer.Write("Verifica URL");
                }
                else
                {
                    writer.Write("URL non valido o incompleto");
                }

                writer.RenderEndTag(); // end A
                writer.RenderEndTag(); // end Div       
                writer.RenderEndTag(); // end Div

                //writer.WriteLine("");
                //writer.WriteLine("");
                writer.RenderBeginTag(HtmlTextWriterTag.Script);
                writer.WriteLine($"function validaUrl_{textBoxId}(e) " + "{");

                writer.WriteLine($"  var txtValue = $('#{textBoxId.Replace(".", "_")}').val();");
                writer.WriteLine("  if (txtValue.startsWith('http://') || txtValue.startsWith('https://'))");
                writer.WriteLine("  {");
                writer.WriteLine($"    $('#{linkControlId}').attr('href', $('#{(id ?? name).Replace(".", "_")}').val());");
                writer.WriteLine($"    $('#{linkControlId}').removeClass('disabled');");
                writer.WriteLine($"    $('#{linkControlId}').text('Verifica URL');");
                writer.WriteLine("  } else {");
                writer.WriteLine($"    $('#{linkControlId}').attr('href', 'javascript:void(0)');");
                writer.WriteLine($"    $('#{linkControlId}').addClass('disabled');");
                writer.WriteLine($"    $('#{linkControlId}').text('URL non valido o incompleto');");
                writer.WriteLine("  }");
                writer.WriteLine("}");
                writer.RenderEndTag(); // end script

            }

            return new MvcHtmlString(stringWriter.ToString());
        }

        public static MvcHtmlString AgidTextBox<TModel>(this HtmlHelper<TModel> helper, string name, string label, TextBoxType type = TextBoxType.Text,
            string value = null, string placeholder = null, string testoInformativo = null, bool isDisabled = false, bool isReadonly = false, string iconClass = null, bool isRequired = false, string id = null)
        {
            StringWriter stringWriter = new StringWriter();
            using (HtmlTextWriter writer = new HtmlTextWriter(stringWriter))
            {
                string inputClass = "form-control";

                ModelState modelState;
                if (helper.ViewData.ModelState.TryGetValue(name, out modelState) && modelState.Errors.Count > 0)
                {
                    inputClass = $"{inputClass} {HtmlHelper.ValidationInputCssClassName}";
                }

                if (type == TextBoxType.Date)
                {
                    writer.AddAttribute(HtmlTextWriterAttribute.Class, "it-datepicker-wrapper");
                    writer.RenderBeginTag(HtmlTextWriterTag.Div);
                }

                // external Div
                writer.AddAttribute(HtmlTextWriterAttribute.Class, $"form-group");
                writer.RenderBeginTag(HtmlTextWriterTag.Div);

                if (!string.IsNullOrWhiteSpace(iconClass))
                {
                    writer.AddAttribute(HtmlTextWriterAttribute.Class, $"input-group");
                    writer.RenderBeginTag(HtmlTextWriterTag.Div);

                    writer.AddAttribute(HtmlTextWriterAttribute.Class, $"input-group-prepend");
                    writer.RenderBeginTag(HtmlTextWriterTag.Div);

                    writer.AddAttribute(HtmlTextWriterAttribute.Class, $"input-group-text");
                    writer.RenderBeginTag(HtmlTextWriterTag.Div);

                    writer.AddAttribute(HtmlTextWriterAttribute.Class, $"icon icon-sm{" " + iconClass}");
                    writer.RenderBeginTag(HtmlTextWriterTag.I);
                    writer.RenderEndTag(); // end I

                    writer.RenderEndTag(); // end Div

                    writer.RenderEndTag(); // end Div
                }

                // label
                if (isRequired)
                {
                    writer.AddAttribute("data-val", $"true");
                    writer.AddAttribute("data-val-required", $"Il campo '{label}' è obbligatorio");
                    writer.AddAttribute(HtmlTextWriterAttribute.Class, "required");
                }
                writer.AddAttribute(HtmlTextWriterAttribute.Id, $"{id ?? name}Desc");
                writer.AddAttribute(HtmlTextWriterAttribute.For, id ?? name);
                writer.RenderBeginTag(HtmlTextWriterTag.Label);
                if (label != null)
                    writer.Write(label);
                else
                    writer.Write(label);
                writer.RenderEndTag(); // end Label

                switch (type)
                {
                    case TextBoxType.Email:
                        writer.AddAttribute(HtmlTextWriterAttribute.Type, "email");
                        break;
                    case TextBoxType.Number:
                        writer.AddAttribute(HtmlTextWriterAttribute.Type, "number");
                        break;
                    case TextBoxType.Phone:
                        writer.AddAttribute(HtmlTextWriterAttribute.Type, "tel");
                        break;
                    case TextBoxType.Time:
                        writer.AddAttribute(HtmlTextWriterAttribute.Type, "time");
                        break;
                    case TextBoxType.Date:
                        writer.AddAttribute(HtmlTextWriterAttribute.Type, "text");
                        inputClass += " it-date-datepicker";
                        break;
                    case TextBoxType.Text:
                    default:
                        writer.AddAttribute(HtmlTextWriterAttribute.Type, "text");
                        break;
                }
                writer.AddAttribute(HtmlTextWriterAttribute.Name, name);
                writer.AddAttribute(HtmlTextWriterAttribute.Id, id ?? name);
                writer.AddAttribute(HtmlTextWriterAttribute.Class, inputClass);
                if (!string.IsNullOrWhiteSpace(testoInformativo))
                    writer.AddAttribute("placeholder", placeholder);
                if (!string.IsNullOrWhiteSpace(placeholder))
                    writer.AddAttribute("aria-describedby", $"{id ?? name}Desc");
                if (isDisabled)
                    writer.AddAttribute(HtmlTextWriterAttribute.Disabled, "disabled");
                if (isReadonly)
                    writer.AddAttribute(HtmlTextWriterAttribute.ReadOnly, "readonly");

                if (!isReadonly && !isDisabled)
                {
                    if (isRequired)
                    {
                        writer.AddAttribute("data-val", $"true");
                        writer.AddAttribute("data-val-required", $"Il campo {label} è obbligatorio");
                    }
                }

                writer.AddAttribute(HtmlTextWriterAttribute.Value, HttpUtility.HtmlEncode(value));
                writer.RenderBeginTag(HtmlTextWriterTag.Input);
                writer.RenderEndTag(); // end Input

                if (!string.IsNullOrWhiteSpace(testoInformativo))
                {
                    writer.AddAttribute(HtmlTextWriterAttribute.Id, $"{id ?? name}Desc");
                    writer.RenderBeginTag(HtmlTextWriterTag.Small);
                    writer.Write(testoInformativo);
                    writer.RenderEndTag(); // end Small
                }

                writer.Write(helper.ValidationMessage(name, "", new { @class = "text-danger" }));

                if (!string.IsNullOrWhiteSpace(iconClass))
                {
                    writer.RenderEndTag(); // end Div
                }

                writer.RenderEndTag(); // end Div

                if (type == TextBoxType.Date)
                {
                    writer.RenderEndTag(); // end Div it-datepicker-wrapper
                }
            }

            return new MvcHtmlString(stringWriter.ToString());
        }


        public static MvcHtmlString AgidTextBoxFor<TModel, TValue>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TValue>> expression, TextBoxType type = TextBoxType.Text,
            string label = null, string placeholder = null, string testoInformativo = null, bool isDisabled = false, bool isReadonly = false, string iconClass = null, string id = null, bool required = false,
            string textBoxValue = null)
        {
            StringWriter stringWriter = new StringWriter();
            using (HtmlTextWriter writer = new HtmlTextWriter(stringWriter))
            {
                string name = helper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(ExpressionHelper.GetExpressionText(expression));
                string controlId = id ?? helper.AgidIDFor(expression).ToString();
                string inputClass = "form-control";
                //Func<TModel, TValue> method = expression.Compile();
                ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);
                IDictionary<string, object> unobtrusiveValitaionAttributes = helper.GetUnobtrusiveValidationAttributes(name, metadata);

                ModelState modelState;
                if (helper.ViewData.ModelState.TryGetValue(name, out modelState) && modelState.Errors.Count > 0)
                {
                    inputClass = $"{inputClass} {HtmlHelper.ValidationInputCssClassName}";
                }

                string value;
                if (!string.IsNullOrWhiteSpace(textBoxValue))
                {
                    value = textBoxValue;
                }
                else if (modelState != null && modelState.Value != null)
                {
                    value = modelState.Value.AttemptedValue;
                }
                else if (metadata.Model != null)
                {
                    value = metadata.Model.ToString();
                }
                else
                {
                    value = String.Empty;
                }

                if (type == TextBoxType.Date)
                {
                    writer.AddAttribute(HtmlTextWriterAttribute.Class, "it-datepicker-wrapper");
                    writer.RenderBeginTag(HtmlTextWriterTag.Div);
                    if (!String.IsNullOrEmpty(value))
                    {
                        DateTime tmp = new DateTime();
                        if (DateTime.TryParse(value, out tmp))
                        {
                            value = tmp.ToString("dd/MM/yyyy");
                        }
                        else
                        {
                            value = String.Empty;
                        }
                    }
                }

                // external Div
                writer.AddAttribute(HtmlTextWriterAttribute.Class, $"form-group");
                writer.RenderBeginTag(HtmlTextWriterTag.Div);

                if (!string.IsNullOrWhiteSpace(iconClass))
                {
                    writer.AddAttribute(HtmlTextWriterAttribute.Class, $"input-group");
                    writer.RenderBeginTag(HtmlTextWriterTag.Div);

                    writer.AddAttribute(HtmlTextWriterAttribute.Class, $"input-group-prepend");
                    writer.RenderBeginTag(HtmlTextWriterTag.Div);

                    writer.AddAttribute(HtmlTextWriterAttribute.Class, $"input-group-text");
                    writer.RenderBeginTag(HtmlTextWriterTag.Div);

                    writer.AddAttribute(HtmlTextWriterAttribute.Class, $"icon icon-sm{" " + iconClass}");
                    writer.RenderBeginTag(HtmlTextWriterTag.I);
                    writer.RenderEndTag(); // end I

                    writer.RenderEndTag(); // end Div

                    writer.RenderEndTag(); // end Div
                }

                // label
                if (!isReadonly && !isDisabled)
                {
                    if (unobtrusiveValitaionAttributes.ContainsKey("data-val-required") || required)
                        writer.AddAttribute(HtmlTextWriterAttribute.Class, "required");
                }

                writer.AddAttribute(HtmlTextWriterAttribute.Style, "width: auto;");
                writer.AddAttribute(HtmlTextWriterAttribute.Id, $"{id ?? controlId}Desc");

                writer.AddAttribute(HtmlTextWriterAttribute.For, id ?? controlId);
                writer.RenderBeginTag(HtmlTextWriterTag.Label);
                if (label != null)
                    writer.Write(label);
                else
                    writer.Write(helper.DisplayNameFor(expression).ToHtmlString());
                writer.RenderEndTag(); // end Label


                /*setto l'attributo maxlength nel caso sia stato aggiunto come attributo nella partial_class*/
                var member = expression.Body as MemberExpression;
                MetadataTypeAttribute metadataTypeAttr = member.Member.ReflectedType.GetCustomAttributes(typeof(MetadataTypeAttribute), false).FirstOrDefault() as MetadataTypeAttribute;
                if (metadataTypeAttr != null)
                {
                    var prop = metadataTypeAttr.MetadataClassType
                      .GetProperty(member.Member.Name);
                    if (prop != null)
                    {
                        var stringLength = prop.GetCustomAttributes(typeof(MaxLengthAttribute), false)
                                             .FirstOrDefault() as MaxLengthAttribute;

                        if (stringLength != null)
                        {
                            writer.AddAttribute(HtmlTextWriterAttribute.Maxlength, stringLength.Length.ToString());
                        }
                    }
                }

                // Input
                // TODO - capire il tipo dal modello e togliere il tipo dai parametri
                var dataTypeName = metadata.DataTypeName;

                switch (type)
                {
                    case TextBoxType.Email:
                        writer.AddAttribute(HtmlTextWriterAttribute.Type, "email");
                        break;
                    case TextBoxType.Number:
                        writer.AddAttribute(HtmlTextWriterAttribute.Type, "number");
                        break;
                    case TextBoxType.Phone:
                        writer.AddAttribute(HtmlTextWriterAttribute.Type, "tel");
                        break;
                    case TextBoxType.Time:
                        writer.AddAttribute(HtmlTextWriterAttribute.Type, "time");
                        break;
                    case TextBoxType.Date:
                        writer.AddAttribute(HtmlTextWriterAttribute.Type, "text");
                        inputClass += " it-date-datepicker";
                        unobtrusiveValitaionAttributes.Remove("data-val-date");
                        if (!unobtrusiveValitaionAttributes.ContainsKey("data-val-regex"))
                            unobtrusiveValitaionAttributes.Add("data-val-regex", $"Il campo {label} deve essere una data valida");
                        if (!unobtrusiveValitaionAttributes.ContainsKey("data-val-regex-pattern"))
                            unobtrusiveValitaionAttributes.Add("data-val-regex-pattern", @"^\d\d?\/\d\d?\/\d\d\d?\d?$");
                        //data-val-regex="Formato data non valido"
                        //data-val-regex-pattern="/^\d\d?\.\d\d?\.\d\d\d?\d?$/"
                        break;
                    case TextBoxType.Text:
                    default:
                        writer.AddAttribute(HtmlTextWriterAttribute.Type, "text");
                        break;
                }
                writer.AddAttribute(HtmlTextWriterAttribute.Name, name);
                writer.AddAttribute(HtmlTextWriterAttribute.Id, id ?? controlId);
                writer.AddAttribute(HtmlTextWriterAttribute.Class, inputClass);
                if (!string.IsNullOrWhiteSpace(testoInformativo))
                    writer.AddAttribute("placeholder", placeholder);
                if (!string.IsNullOrWhiteSpace(placeholder))
                    writer.AddAttribute("aria-describedby", $"{id ?? controlId}Desc");
                if (isDisabled)
                    writer.AddAttribute(HtmlTextWriterAttribute.Disabled, "disabled");
                if (isReadonly)
                    writer.AddAttribute(HtmlTextWriterAttribute.ReadOnly, "readonly");
                foreach (var attr in unobtrusiveValitaionAttributes)
                {
                    writer.AddAttribute(attr.Key, attr.Value.ToString());
                }
                if (!isReadonly && !isDisabled)
                {
                    if (!unobtrusiveValitaionAttributes.ContainsKey("data-val-required") && required)
                    {
                        if (!unobtrusiveValitaionAttributes.Any())
                            writer.AddAttribute("data-val", $"true");

                        writer.AddAttribute("data-val-required", $"Il campo {label} è obbligatorio");
                    }
                }
                writer.AddAttribute(HtmlTextWriterAttribute.Value, HttpUtility.HtmlDecode(value));
                writer.RenderBeginTag(HtmlTextWriterTag.Input);
                //writer.Write(HttpUtility.HtmlEncode(value));
                writer.RenderEndTag(); // end Input

                if (!string.IsNullOrWhiteSpace(testoInformativo))
                {
                    writer.AddAttribute(HtmlTextWriterAttribute.Id, $"{id ?? controlId}Desc");
                    writer.RenderBeginTag(HtmlTextWriterTag.Small);
                    writer.Write(testoInformativo);
                    writer.RenderEndTag(); // end Small
                }

                //writer.Write(helper.DropDownListFor(expression, items, htmlAttributes != null ? htmlAttributes : new { htmlAttributes = new { @multiple = "true", @data_multiple_separator = "" } }).ToHtmlString());
                writer.Write(helper.ValidationMessageFor(expression, "", new { @class = "text-danger" }).ToHtmlString());

                if (!string.IsNullOrWhiteSpace(iconClass))
                {
                    writer.RenderEndTag(); // end Div
                }

                writer.RenderEndTag(); // end Div

                if (type == TextBoxType.Date)
                {
                    writer.RenderEndTag(); // end Div it-datepicker-wrapper
                }
            }

            return new MvcHtmlString(stringWriter.ToString());
        }

        public static MvcHtmlString AgidPasswordFor<TModel, TValue>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TValue>> expression, string label = null, object htmlAttributes = null)
        {
            /*
            <div>
              <div class="form-group">
                <input type="password" class="form-control input-password" id="exampleInputPassword" aria-labelledby="infoPassword">
                <span class="password-icon" aria-hidden="true">
                  <svg class="password-icon-visible icon icon-sm"><use xlink:href="/bootstrap-italia/dist/svg/sprite.svg#it-password-visible"></use></svg>
                  <svg class="password-icon-invisible icon icon-sm d-none"><use xlink:href="/bootstrap-italia/dist/svg/sprite.svg#it-password-invisible"></use></svg>
                </span>
                <label for="exampleInputPassword">Password con label, placeholder e testo di aiuto</label>
                <small id="infoPassword" class="form-text text-muted">Inserisci almeno 8 caratteri e una lettera maiuscola</small>
              </div>
              <div class="form-group">
                <input type="password" class="form-control input-password input-password-strength-meter" data-enter-pass="Puoi usare un testo di aiuto personalizzato" id="exampleInputPassword3">
                <span class="password-icon" aria-hidden="true">
                  <svg class="password-icon-visible icon icon-sm"><use xlink:href="/bootstrap-italia/dist/svg/sprite.svg#it-password-visible"></use></svg>
                  <svg class="password-icon-invisible icon icon-sm d-none"><use xlink:href="/bootstrap-italia/dist/svg/sprite.svg#it-password-invisible"></use></svg>
                </span>
                <label for="exampleInputPassword3">Password con strength meter</label>
              </div>
            </div>
             */
            throw new NotImplementedException(MethodBase.GetCurrentMethod().ReflectedType.Name);
        }

        public static MvcHtmlString AgidCurrencyFor<TModel, TValue>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TValue>> expression, string label = null, object htmlAttributes = null)
        {
            throw new NotImplementedException(MethodBase.GetCurrentMethod().ReflectedType.Name);
        }

        public static MvcHtmlString AgidPercentageFor<TModel, TValue>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TValue>> expression, string label = null, object htmlAttributes = null)
        {
            throw new NotImplementedException(MethodBase.GetCurrentMethod().ReflectedType.Name);
        }

        public static MvcHtmlString AgidNumericFor<TModel, TValue>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TValue>> expression, string label = null, object htmlAttributes = null)
        {
            throw new NotImplementedException(MethodBase.GetCurrentMethod().ReflectedType.Name);
        }

        public static MvcHtmlString AgidTimePickerForFor<TModel, TValue>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TValue>> expression, string label = null, object htmlAttributes = null)
        {
            throw new NotImplementedException(MethodBase.GetCurrentMethod().ReflectedType.Name);
        }

        public static MvcHtmlString AgidDatePickerFor<TModel, TValue>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TValue>> expression, string label = null, object htmlAttributes = null)
        {
            return new MvcHtmlString(helper.TextBoxFor(expression, new { @class = "form-control it-date-datepicker", @placeholder = "gg/mm/aaaa" }).ToString() +
                                        helper.LabelFor(expression, htmlAttributes: new { @class = "control-label" }).ToString() +
                                        helper.ValidationMessageFor(expression, "", new { @class = "text-danger" }).ToString());
            throw new NotImplementedException(MethodBase.GetCurrentMethod().ReflectedType.Name);
        }

        public static MvcHtmlString AgidEditorFor<TModel, TValue>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TValue>> expression, string label = null, object htmlAttributes = null)
        {
            StringWriter stringWriter = new StringWriter();
            using (HtmlTextWriter writer = new HtmlTextWriter(stringWriter))
            {
                string name = ExpressionHelper.GetExpressionText(expression);
                ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);
                IDictionary<string, object> unobtrusiveValitaionAttributes = helper.GetUnobtrusiveValidationAttributes(name, metadata);

                writer.AddAttribute(HtmlTextWriterAttribute.Class, "form-group");
                writer.RenderBeginTag(HtmlTextWriterTag.Div);

                if (label == null)
                    writer.Write(helper.LabelFor(expression));
                else
                    writer.Write(helper.LabelFor(expression, label));

                if (htmlAttributes == null)
                    writer.Write(helper.EditorFor(expression, new { htmlAttributes = new { @class = "form-control" } }));
                else
                    writer.Write(helper.EditorFor(expression, new { htmlAttributes = htmlAttributes }));

                writer.Write(helper.ValidationMessageFor(expression, "", new { @class = "text-danger" }));

                writer.RenderEndTag(); // end Div
            }

            return new MvcHtmlString(stringWriter.ToString());
        }

        public static MvcHtmlString AgidTextAreaFor<TModel, TValue>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TValue>> expression, string label = null,
            string rows = null, bool required = false, string textBoxValue = null, string id = null)
        {
            StringWriter stringWriter = new StringWriter();
            using (HtmlTextWriter writer = new HtmlTextWriter(stringWriter))
            {
                string name = ExpressionHelper.GetExpressionText(expression);
                string controlId = id ?? helper.AgidIDFor(expression).ToString();
                ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);
                IDictionary<string, object> unobtrusiveValitaionAttributes = helper.GetUnobtrusiveValidationAttributes(name, metadata);

                string taClass = string.Empty;

                ModelState modelState;
                if (helper.ViewData.ModelState.TryGetValue(name, out modelState) && modelState.Errors.Count > 0)
                {
                    taClass = $"{taClass} {HtmlHelper.ValidationInputCssClassName}";
                }

                string value;
                if (!string.IsNullOrWhiteSpace(textBoxValue))
                {
                    value = textBoxValue;
                }
                else if (modelState != null && modelState.Value != null)
                {
                    value = modelState.Value.AttemptedValue;
                }
                else if (metadata.Model != null)
                {
                    value = metadata.Model.ToString();
                }
                else
                {
                    value = String.Empty;
                }

                writer.AddAttribute(HtmlTextWriterAttribute.Class, "form-group");
                writer.RenderBeginTag(HtmlTextWriterTag.Div);

                // label
                if (label == null)
                    writer.Write(helper.LabelFor(expression, new { @style = "width:100%", @class = string.Format("control-label{0}", required ? " required" : ""), @for = (controlId) }));
                else
                    writer.Write(helper.LabelFor(expression, label, new { @style = "width:100%", @class = string.Format("control-label{0}", required ? " required" : ""), @for = (controlId) }));

                //textarea
                writer.AddAttribute(HtmlTextWriterAttribute.Name, name);
                writer.AddAttribute(HtmlTextWriterAttribute.Id, controlId);

                /*setto l'attributo maxlength nel caso sia stato aggiunto come attributo nella partial_class*/
                var member = expression.Body as MemberExpression;
                MetadataTypeAttribute metadataTypeAttr = member.Member.ReflectedType.GetCustomAttributes(typeof(MetadataTypeAttribute), false).FirstOrDefault() as MetadataTypeAttribute;
                if (metadataTypeAttr != null)
                {
                    var prop = metadataTypeAttr.MetadataClassType
                     .GetProperty(member.Member.Name);
                    if (prop != null)
                    {
                        var stringLength = prop.GetCustomAttributes(typeof(MaxLengthAttribute), false)
                                             .FirstOrDefault() as MaxLengthAttribute;

                        if (stringLength != null)
                        {
                            writer.AddAttribute(HtmlTextWriterAttribute.Maxlength, stringLength.Length.ToString());
                        }
                    }
                }

                foreach (var attr in unobtrusiveValitaionAttributes)
                {
                    writer.AddAttribute(attr.Key, attr.Value.ToString());
                }

                if (required && !unobtrusiveValitaionAttributes.Any(x => x.Key == "data-val-required"))
                {
                    if (!unobtrusiveValitaionAttributes.Any())
                        writer.AddAttribute("data-val", $"true");

                    writer.AddAttribute("data-val-required", $"Il campo '{helper.DisplayNameFor(expression).ToHtmlString()}' è obbligatorio");
                }

                writer.AddAttribute(HtmlTextWriterAttribute.Cols, "20");
                writer.AddAttribute(HtmlTextWriterAttribute.Rows, string.IsNullOrEmpty(rows) ? "2" : rows);
                writer.AddAttribute(HtmlTextWriterAttribute.Class, taClass);
                writer.RenderBeginTag(HtmlTextWriterTag.Textarea);
                writer.Write(value);
                writer.RenderEndTag(); // Textarea


                // validation
                writer.Write(helper.ValidationMessageFor(expression, "", new { @class = "text-danger" }));

                writer.RenderEndTag(); // Div
            }

            return new MvcHtmlString(stringWriter.ToString());
        }

        public static MvcHtmlString AgidTextArea<TModel>(this HtmlHelper<TModel> helper, string name, string label, string value, object htmlAttributes = null, bool required = false, string id = null)
        {
            StringWriter stringWriter = new StringWriter();
            using (HtmlTextWriter writer = new HtmlTextWriter(stringWriter))
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Class, "form-group");
                writer.RenderBeginTag(HtmlTextWriterTag.Div);

                // label
                writer.Write(helper.Label(name, label, new { @style = "width:100%", @class = string.Format("control-label{0}", required ? " required" : ""), @for = (id ?? name) }));

                //textarea
                writer.AddAttribute(HtmlTextWriterAttribute.Name, name);
                writer.AddAttribute(HtmlTextWriterAttribute.Id, id ?? name);
                if (htmlAttributes == null)
                    writer.Write(helper.TextArea(name, value));
                else
                    writer.Write(helper.TextArea(name, value, htmlAttributes));

                // validation
                writer.Write(helper.ValidationMessage(name, "", new { @class = "text-danger" }));

                writer.RenderEndTag(); // Div
            }

            return new MvcHtmlString(stringWriter.ToString());
        }
    }
}
