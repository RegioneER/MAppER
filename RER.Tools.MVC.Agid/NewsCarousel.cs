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
using System.Xml;

namespace RER.Tools.MVC.Agid
{
    public static partial class ExtensionMethods
    {
        public static MvcHtmlString AgidNewsCarousel<TModel>(this HtmlHelper<TModel> helper, IEnumerable<NewsCruscotto> news, int maxLunghezzaTesto = 150)
        {
            StringWriter stringWriter = new StringWriter();
            using (HtmlTextWriter writer = new HtmlTextWriter(stringWriter))
            {
                #region scripts
                /*
                <script>
                   function mostraTesto(itemId) {
                       var spanPuntiniID = "#spanPuntini" + itemId;
                       var spanTestoID = "#spanTesto" + itemId;
                       var spanLinkMostraTestoID = "#spanLinkMostraTesto" + itemId;
                       $(spanPuntiniID).toggle();
                       $(spanTestoID).toggle();
                       if ($(spanTestoID).is(":hidden")) {
                           $(spanLinkMostraTestoID).html("Mostra articolo completo");
                       }
                       else {
                           $(spanLinkMostraTestoID).html("Nascondi testo articolo");
                       }
                       return false;
                   }
               </script>
                */

                writer.RenderBeginTag(HtmlTextWriterTag.Script);
                writer.WriteLine("function mostraTesto(itemId) {");
                writer.WriteLine("    var spanPuntiniID = '#spanPuntini' + itemId;");
                writer.WriteLine("    var spanTestoID = '#spanTesto' + itemId;");
                writer.WriteLine("    var spanLinkMostraTestoID = '#spanLinkMostraTesto' + itemId;");
                writer.WriteLine("    $(spanPuntiniID).toggle();");
                writer.WriteLine("    $(spanTestoID).toggle();");
                writer.WriteLine("    if ($(spanTestoID).is(':hidden')) {");
                writer.WriteLine("       $(spanLinkMostraTestoID).html('Mostra articolo completo');");
                writer.WriteLine("    }");
                writer.WriteLine("    else {");
                writer.WriteLine("       $(spanLinkMostraTestoID).html('Nascondi testo articolo');");
                writer.WriteLine("    }");
                writer.WriteLine("    return false;");
                writer.Write("}");
                writer.RenderEndTag(); // end Script
                #endregion

                writer.AddAttribute(HtmlTextWriterAttribute.Class, $"it-carousel-wrapper it-carousel-landscape-abstract-three-cols");
                writer.RenderBeginTag(HtmlTextWriterTag.Div);
                writer.AddAttribute(HtmlTextWriterAttribute.Class, $"it-carousel-all owl-carousel it-card-bg");
                writer.RenderBeginTag(HtmlTextWriterTag.Div);

                foreach (var item in news)
                {
                    bool oltreLunghezzaLimite = item.Descrizione.Length > maxLunghezzaTesto;
                    writer.AddAttribute(HtmlTextWriterAttribute.Class, $"it-single-slide-wrapper");
                    writer.RenderBeginTag(HtmlTextWriterTag.Div);
                    writer.AddAttribute(HtmlTextWriterAttribute.Class, $"card-wrapper");
                    writer.RenderBeginTag(HtmlTextWriterTag.Div);
                    writer.AddAttribute(HtmlTextWriterAttribute.Class, $"card card-bg");
                    writer.RenderBeginTag(HtmlTextWriterTag.Div);

                    // flag per le news nuove
                    if (item.IsNew.HasValue && item.IsNew.Value)
                    {
                        writer.AddAttribute(HtmlTextWriterAttribute.Class, $"flag-icon");
                        writer.RenderBeginTag(HtmlTextWriterTag.Div);
                        writer.RenderEndTag(); // end Div
                    }

                    writer.AddAttribute(HtmlTextWriterAttribute.Class, $"card-body");
                    writer.RenderBeginTag(HtmlTextWriterTag.Div);

                    // category
                    writer.AddAttribute(HtmlTextWriterAttribute.Class, $"category-top");
                    writer.RenderBeginTag(HtmlTextWriterTag.Div);
                    writer.AddAttribute(HtmlTextWriterAttribute.Class, $"category");
                    writer.RenderBeginTag(HtmlTextWriterTag.Span);
                    writer.Write(item.Data?.ToString("dd/MM/yyyy"));
                    writer.RenderEndTag(); // end Span class = 'cetegory'
                    writer.RenderEndTag(); // end Div class = 'category-top'

                    writer.AddAttribute(HtmlTextWriterAttribute.Class, $"card-title big-heading");
                    writer.RenderBeginTag(HtmlTextWriterTag.H5);
                    writer.Write(item.Titolo);
                    writer.RenderEndTag(); // end H5

                    writer.AddAttribute(HtmlTextWriterAttribute.Class, $"card-text");
                    writer.RenderBeginTag(HtmlTextWriterTag.Div);
                    writer.Write(HttpUtility.HtmlDecode(item.Descrizione));
                    writer.RenderEndTag(); // end Div

                    if (item.Allegati.Any())
                    {
                        writer.WriteBreak();
                        writer.AddAttribute(HtmlTextWriterAttribute.Class, $"card-text");
                        writer.RenderBeginTag(HtmlTextWriterTag.P);
                        writer.RenderBeginTag(HtmlTextWriterTag.I);
                        writer.Write("Allegati: ");
                        writer.RenderEndTag(); // end I
                        writer.RenderEndTag(); // end P

                        writer.RenderBeginTag(HtmlTextWriterTag.Ul);
                        foreach (var allegato in item.Allegati)
                        {
                            UrlHelper uHelper = new UrlHelper(HttpContext.Current.Request.RequestContext);

                            writer.RenderBeginTag(HtmlTextWriterTag.Li);
                            writer.AddAttribute(HtmlTextWriterAttribute.Class, $"card-text");
                            writer.AddAttribute(HtmlTextWriterAttribute.Href, uHelper.Action("ScaricaFileNewsManager", "Home", new { IDDocumento = allegato.ID }));
                            writer.RenderBeginTag(HtmlTextWriterTag.A);
                            writer.Write($"{allegato.Descrizione}({allegato.NomeFile})");
                            writer.RenderEndTag(); // end A
                            writer.RenderEndTag(); // end Li

                        }
                        writer.RenderEndTag(); // end Ul
                    }

                    writer.RenderEndTag(); // end Div class = 'card-body'

                    writer.RenderEndTag(); // end Div class = 'card card-bg'
                    writer.RenderEndTag(); // end Div class = 'card-wrapper'
                    writer.RenderEndTag(); // end Div class = 'it-single-slide-wrapper'
                }

                writer.RenderEndTag(); // end Div class = 'it-carousel-all owl-carousel it-card-bg'
                writer.RenderEndTag(); // end Div class = 'it-carousel-wrapper it-carousel-landscape-abstract-three-cols'


                writer.WriteLineNoTabs("");
                writer.WriteLineNoTabs("");
            }

            return new MvcHtmlString(stringWriter.ToString());

        }
    }
}
