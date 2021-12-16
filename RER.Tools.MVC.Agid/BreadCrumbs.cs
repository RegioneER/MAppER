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
    public class BreadCrumbItem
    { 
        public string LinkText { get; set; }
        public string ActionName { get; set; }
        public string ControllerName { get; set; }
    }

    public static partial class ExtensionMethods
    {
        public static string BuildAgidBreadcrumbNavigation(this HtmlHelper<dynamic> helper, string actionHome, List<BreadCrumbItem> bcItems)
        {
            try
            {
                StringWriter stringWriter = new StringWriter();
                using (HtmlTextWriter writer = new HtmlTextWriter(stringWriter))
                {
                    foreach (var item in bcItems)
                    {
                        writer.RenderBeginTag(HtmlTextWriterTag.Li);
                        writer.Write($"&nbsp;/&nbsp;");
                        writer.Write(helper.ActionLink(item.LinkText, item.ActionName, item.ControllerName).ToHtmlString());
                        writer.RenderEndTag(); // end I                        
                    }
                }

                return stringWriter.ToString();
            }
            catch (Exception)
            {
                return "";
            }
        }

    }
}
