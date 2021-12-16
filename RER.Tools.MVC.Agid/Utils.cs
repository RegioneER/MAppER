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
        public static MvcHtmlString AgidIDFor<TModel, TValue>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TValue>> expression)
        {
            string name = helper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(ExpressionHelper.GetExpressionText(expression));
            ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);
            string modelClassName = metadata.ContainerType.Name;

            return new MvcHtmlString($"{modelClassName}_{name}".Replace(".", "_"));
        }

        public static MvcHtmlString AgidIDFor<TModel, TValue>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TValue>> expression, string modelClassName)
        {
            string name = helper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(ExpressionHelper.GetExpressionText(expression));
            return new MvcHtmlString($"{modelClassName}_{name}".Replace(".", "_"));
        }

        public static bool IsDebug(this HtmlHelper htmlHelper)
        {
#if DEBUG
            return true;
#else
      return false;
#endif
        }

        public static bool IsRequiredFor<TModel, TValue>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TValue>> expression)
        {
            string name = helper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(ExpressionHelper.GetExpressionText(expression));
            ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);
            IDictionary<string, object> unobtrusiveValitaionAttributes = helper.GetUnobtrusiveValidationAttributes(name, metadata);

            return unobtrusiveValitaionAttributes.ContainsKey("data-val-required");
        }

        public static bool IsNullOrWhiteSpaceFor<TModel, TValue>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TValue>> expression)
        {
            string name = helper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(ExpressionHelper.GetExpressionText(expression));
            ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);
            IDictionary<string, object> unobtrusiveValitaionAttributes = helper.GetUnobtrusiveValidationAttributes(name, metadata);

            ModelState modelState;
            if (helper.ViewData.ModelState.TryGetValue(name, out modelState) && modelState.Errors.Count > 0)
            {
                string value = string.Empty;
                if (modelState != null && modelState.Value != null)
                {
                    value = modelState.Value.AttemptedValue;
                }
                else if (metadata.Model != null)
                {
                    value = metadata.Model.ToString();
                }

                return string.IsNullOrWhiteSpace(value);
            }
            else
            {
                return true;
            }
        }
    }

    public enum AgidColor
    {
        Primary,
        Secondary,
        Success,
        Danger,
        Warning,
        Default
    }
}