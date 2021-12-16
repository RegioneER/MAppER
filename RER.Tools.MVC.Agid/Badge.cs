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
        public static MvcHtmlString AgidBadgeButton<TModel>(this HtmlHelper<TModel> helper, string text, string value, string href = null, AgidColor buttonColor = AgidColor.Default, AgidColor badgeColor = AgidColor.Default, string srOnlyText = null)
        {
            string buttonColorClass, badgeColorClass;
            switch (buttonColor)
            {
                case AgidColor.Danger:
                    buttonColorClass = "btn btn-danger";
                    break;
                case AgidColor.Secondary:
                    buttonColorClass = "btn btn-secondary";
                    break;
                case AgidColor.Success:
                    buttonColorClass = "btn btn-success";
                    break;
                case AgidColor.Warning:
                    buttonColorClass = "btn btn-warning";
                    break;
                case AgidColor.Primary:
                default:
                    buttonColorClass = "btn btn-primary";
                    break;
            }

            switch (badgeColor)
            {
                case AgidColor.Danger:
                    badgeColorClass = "badge badge-danger";
                    break;
                case AgidColor.Secondary:
                    badgeColorClass = "badge badge-secondary";
                    break;
                case AgidColor.Success:
                    badgeColorClass = "badge badge-success";
                    break;
                case AgidColor.Warning:
                    badgeColorClass = "badge badge-warning";
                    break;
                case AgidColor.Primary:
                    badgeColorClass = "badge badge-primary";
                    break;
                default:
                    badgeColorClass = "badge badge-light";
                    break;
            }


            StringWriter stringWriter = new StringWriter();
            using (HtmlTextWriter writer = new HtmlTextWriter(stringWriter))
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Class, buttonColorClass);
                if (!string.IsNullOrWhiteSpace(href))
                {
                    writer.AddAttribute(HtmlTextWriterAttribute.Href, href);
                }
                writer.RenderBeginTag(HtmlTextWriterTag.A);
                writer.Write($"{text}&nbsp;&nbsp;");
                writer.AddAttribute(HtmlTextWriterAttribute.Class, badgeColorClass);
                writer.RenderBeginTag(HtmlTextWriterTag.Span);
                writer.Write($"{value}");
                writer.RenderEndTag(); // end Span
                if (!string.IsNullOrWhiteSpace(srOnlyText))
                {
                    writer.AddAttribute(HtmlTextWriterAttribute.Class, "sr-only");
                    writer.RenderBeginTag(HtmlTextWriterTag.Span);
                    writer.Write(srOnlyText);
                    writer.RenderEndTag(); // end Span
                }
                writer.RenderEndTag(); // end A       
            }

            return new MvcHtmlString(stringWriter.ToString());
        }
    }
}
