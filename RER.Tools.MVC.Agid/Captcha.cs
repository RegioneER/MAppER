using RestSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.UI;

namespace RER.Tools.MVC.Agid
{
    public static class Captcha
    {
        public static bool VerificaTokenReCaptcha(HttpRequestBase httpRequest, string secret)
        {
            string g_recaptcha_response = httpRequest.Params["g-recaptcha-response"]?.ToString();

            if (string.IsNullOrWhiteSpace(g_recaptcha_response))
                return false;

            RestClient restClient = new RestClient("https://www.google.com/recaptcha/api/");
            var restRequest = new RestRequest("siteverify", Method.POST, DataFormat.Json);
            restRequest.AddParameter("secret", secret);
            restRequest.AddParameter("response", g_recaptcha_response);
            restRequest.AddParameter("remoteip", httpRequest.UserHostAddress);

#if (DEBUG)
            //TODO: se si utilizza un proxy...
            System.Net.WebProxy proxy = new System.Net.WebProxy("MettiQuiHost", 1234);
            proxy.Credentials = new NetworkCredential("MettiQuiUser", "MettiQuiPassword", "MettiQuiDominio");
            restClient.Proxy = proxy;
#endif

            var response = restClient.Execute(restRequest);

            dynamic data = Json.Decode((response?.Content) ?? "");
            return response.IsSuccessful && data.success == true;
        }
    }


    public static partial class ExtensionMethods
    {
        public static MvcHtmlString reCaptcha<TModel>(this HtmlHelper<TModel> helper, string siteKey)
        {
            StringWriter stringWriter = new StringWriter();
            using (HtmlTextWriter writer = new HtmlTextWriter(stringWriter))
            {
                writer.AddAttribute("data-sitekey", siteKey);
                writer.AddAttribute(HtmlTextWriterAttribute.Class, "g-recaptcha");
                writer.RenderBeginTag(HtmlTextWriterTag.Div);
                writer.RenderEndTag(); // end Input
            }
            return new MvcHtmlString(stringWriter.ToString());
        }

        public static MvcHtmlString reCaptchaScript<TModel>(this HtmlHelper<TModel> helper)
        {
            StringWriter stringWriter = new StringWriter();
            using (HtmlTextWriter writer = new HtmlTextWriter(stringWriter))
            {
                writer.AddAttribute("defer", "");
                writer.AddAttribute(HtmlTextWriterAttribute.Src, "https://www.google.com/recaptcha/api.js");
                writer.RenderBeginTag(HtmlTextWriterTag.Script);
                writer.RenderEndTag(); // end Input
            }
            return new MvcHtmlString(stringWriter.ToString());
        }
    }
}
