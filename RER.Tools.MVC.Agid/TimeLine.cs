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
    public class TimeLineItem
    {
        public string ItemIconClass { get; set; }

        public string PinText { get; set; }
        public DateTime PinDate { get; set; }

        public bool HasCategory { get; set; }
        public string CategoryText { get; set; }
        public string CategoryHref { get; set; }

        public string CardTitleText { get; set; }
        public string CardText { get; set; }
        public string CardSignature { get; set; }
        public string CardReadMoreText { get; set; }
        public string CardReadMoreHref { get; set; }
        public string CardReadMoreIconClass { get; set; }
    }

    public static partial class ExtensionMethods
    {
        public static MvcHtmlString AgidTimeLine<TModel>(this HtmlHelper<TModel> helper, IEnumerable<TimeLineItem> items)
        {
            StringWriter stringWriter = new StringWriter();
            using (HtmlTextWriter writer = new HtmlTextWriter(stringWriter))
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Class, "it-timeline-wrapper");
                writer.RenderBeginTag(HtmlTextWriterTag.Div);
                writer.AddAttribute(HtmlTextWriterAttribute.Class, "row");
                writer.RenderBeginTag(HtmlTextWriterTag.Div);
                foreach (var item in items)
                {
                    writer.AddAttribute(HtmlTextWriterAttribute.Class, "col-12");
                    writer.RenderBeginTag(HtmlTextWriterTag.Div);
                    writer.AddAttribute(HtmlTextWriterAttribute.Class, "timeline-element");
                    writer.RenderBeginTag(HtmlTextWriterTag.Div);

                    #region pin
                    writer.AddAttribute(HtmlTextWriterAttribute.Class, "it-pin-wrapper it-evidence");
                    writer.RenderBeginTag(HtmlTextWriterTag.Div);

                    writer.AddAttribute(HtmlTextWriterAttribute.Class, "pin-icon");
                    writer.RenderBeginTag(HtmlTextWriterTag.Div);
                    //writer.AddAttribute(HtmlTextWriterAttribute.Class, "icon");
                    //writer.RenderBeginTag(HtmlTextWriterTag.Div);

                    //writer.AddAttribute(HtmlTextWriterAttribute.Class, "fa-stack fa-2x");
                    //writer.RenderBeginTag(HtmlTextWriterTag.Span);
                    //writer.AddAttribute(HtmlTextWriterAttribute.Class, "fas fa-circle fa-stack-2x");
                    //writer.RenderBeginTag(HtmlTextWriterTag.I);
                    //writer.RenderEndTag(); // end I
                    // writer.AddAttribute(HtmlTextWriterAttribute.Class, $"{item.ItemIconClass} fa-stack-1x fa-inverse" ?? "fas fa-code fa-stack-1x fa-inverse");
                    writer.AddAttribute(HtmlTextWriterAttribute.Class, $"icon {item.ItemIconClass ?? "fas fa-code"}");
                    writer.RenderBeginTag(HtmlTextWriterTag.I);
                    writer.RenderEndTag(); // end I
                                           //writer.RenderEndTag(); // end Span

                    //writer.RenderEndTag(); // end Div icon
                    writer.RenderEndTag(); // end Div pin-icon

                    writer.AddAttribute(HtmlTextWriterAttribute.Class, "pin-text");
                    writer.RenderBeginTag(HtmlTextWriterTag.Div);
                    writer.RenderBeginTag(HtmlTextWriterTag.Span);
                    writer.Write(item.PinText);
                    writer.RenderEndTag(); // end Div Span
                    writer.RenderEndTag(); // end Div pin-text

                    writer.RenderEndTag(); // end Div it-pin-wrapper it-evidence
                    #endregion pin

                    #region card

                    writer.AddAttribute(HtmlTextWriterAttribute.Class, "card-wrapper");
                    writer.RenderBeginTag(HtmlTextWriterTag.Div);
                    writer.AddAttribute(HtmlTextWriterAttribute.Class, "card");
                    writer.RenderBeginTag(HtmlTextWriterTag.Div);
                    writer.AddAttribute(HtmlTextWriterAttribute.Class, "card-body");
                    writer.RenderBeginTag(HtmlTextWriterTag.Div);

                    #region category

                    if (item.HasCategory)
                    {
                        writer.AddAttribute(HtmlTextWriterAttribute.Class, "category-top");
                        writer.RenderBeginTag(HtmlTextWriterTag.Div);

                        writer.AddAttribute(HtmlTextWriterAttribute.Class, "category");
                        writer.AddAttribute(HtmlTextWriterAttribute.Href, item.CategoryHref ?? "#");
                        writer.RenderBeginTag(HtmlTextWriterTag.A);
                        writer.Write(item.CategoryText);
                        writer.RenderEndTag(); // end A

                        writer.AddAttribute(HtmlTextWriterAttribute.Class, "data");
                        writer.RenderBeginTag(HtmlTextWriterTag.Span);
                        writer.Write(item.PinDate.ToString("dd/MM/yyyy"));
                        writer.RenderEndTag(); // end Span

                        writer.RenderEndTag(); // end Div category-top
                    }

                    #endregion category

                    writer.AddAttribute(HtmlTextWriterAttribute.Class, "card-title big-heading");
                    writer.RenderBeginTag(HtmlTextWriterTag.H5);
                    writer.Write(item.CardTitleText);
                    writer.RenderEndTag(); // end H5

                    writer.AddAttribute(HtmlTextWriterAttribute.Class, "card-text");
                    writer.RenderBeginTag(HtmlTextWriterTag.P);
                    writer.Write(item.CardText);
                    writer.RenderEndTag(); // end P

                    writer.AddAttribute(HtmlTextWriterAttribute.Class, "card-signature");
                    writer.RenderBeginTag(HtmlTextWriterTag.Span);
                    writer.Write(item.CardSignature);
                    writer.RenderEndTag(); // end Span

                    if (!string.IsNullOrEmpty(item.CardReadMoreHref))
                        writer.AddAttribute(HtmlTextWriterAttribute.Href, item.CardReadMoreHref);
                    writer.AddAttribute(HtmlTextWriterAttribute.Class, "read-more");
                    writer.RenderBeginTag(HtmlTextWriterTag.A);
                    writer.AddAttribute(HtmlTextWriterAttribute.Class, "text");
                    writer.RenderBeginTag(HtmlTextWriterTag.Span);
                    if (!string.IsNullOrEmpty(item.CardReadMoreHref))
                        writer.Write(item.CardReadMoreText ?? "Leggi di più");
                    writer.RenderEndTag(); // end Span
                                           // TODO manca l'icona CardReadMoreIconClass
                    writer.RenderEndTag(); // end A


                    writer.RenderEndTag(); // end Div card-body
                    writer.RenderEndTag(); // end Div card
                    writer.RenderEndTag(); // end Div card-wrapper

                    #endregion card

                    writer.RenderEndTag(); // end Div timeline-element
                    writer.RenderEndTag(); // end Div col-12
                }
                writer.RenderEndTag(); // end Div row
                writer.RenderEndTag(); // end Div it-timeline-wrapper
            }
            return new MvcHtmlString(stringWriter.ToString());
        }
    }
}
