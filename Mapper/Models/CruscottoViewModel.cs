using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using RER.Tools.MVC.Agid;

namespace Mapper.Models
{
    public class CruscottoViewModel
    {
        public List<ElementoCruscottoModel> ElementiCruscotto { get; internal set; }
        public List<NewsCruscotto> NewsCruscotto { get; internal set; }

        public void CaricaNewsDaXml(string xml)
        {
            XmlDocument xDoc = new XmlDocument();
            xDoc.LoadXml(xml);

            var notizie = xDoc.SelectNodes("/ArrayOfNotizia/Notizia");

            NewsCruscotto = new List<NewsCruscotto>();
            foreach (XmlNode n in notizie)
            {
                DateTime tmpData, tmpVisibileDal, tmpVisibileAl;
                bool tmpIsInEvidenza, tmpIsVisibile, tmpIsNew;

                NewsCruscotto tmpNews = new NewsCruscotto();

                tmpNews.Data = DateTime.TryParse(n.SelectSingleNode("Data").InnerText, out tmpData) ? tmpData : (DateTime?)null;
                tmpNews.DataLibera = n.SelectSingleNode("DataLibera").InnerText;
                tmpNews.Descrizione = n.SelectSingleNode("Descrizione").InnerText;
                tmpNews.IsInEvidenza = bool.TryParse(n.SelectSingleNode("IsInEvidenza").InnerText, out tmpIsInEvidenza) ? tmpIsInEvidenza : (bool?)null;
                tmpNews.IsNew = bool.TryParse(n.SelectSingleNode("IsNew").InnerText, out tmpIsNew) ? tmpIsNew : (bool?)null;
                tmpNews.IsVisibile = bool.TryParse(n.SelectSingleNode("IsVisibile").InnerText, out tmpIsVisibile) ? tmpIsVisibile : (bool?)null;
                tmpNews.VisibileDal = DateTime.TryParse(n.SelectSingleNode("VisibileDal").InnerText, out tmpVisibileDal) ? tmpVisibileDal : (DateTime?)null;
                tmpNews.VisibileAl = DateTime.TryParse(n.SelectSingleNode("VisibileAl").InnerText, out tmpVisibileAl) ? tmpVisibileAl : (DateTime?)null;
                tmpNews.ID = int.Parse(n.SelectSingleNode("ID").InnerText);
                tmpNews.Titolo = n.SelectSingleNode("Titolo").InnerText;

                tmpNews.Allegati = new List<AllegatoNews>();
                XmlNodeList allegati = n.SelectNodes("Allegati/Allegato");
                foreach (XmlNode a in allegati)
                {
                    int tmpDimensione, tmpIDNotizia;
                    DateTime tmpDataOra;
                    Guid tmpGuid;

                    AllegatoNews tmpAllegato = new AllegatoNews
                    {
                        GUID = Guid.TryParse(a.SelectSingleNode("GUID")?.InnerText, out tmpGuid) ? tmpGuid : (Guid?)null,
                        DataOra = DateTime.TryParse(a.SelectSingleNode("DataOra")?.InnerText, out tmpDataOra) ? tmpDataOra : (DateTime?)null,
                        Descrizione = a.SelectSingleNode("Descrizione").InnerText,
                        Dimensione = int.TryParse(a.SelectSingleNode("Dimensione")?.InnerText, out tmpDimensione) ? tmpDimensione : (int?)null,
                        Formato = a.SelectSingleNode("Formato")?.InnerText,
                        ID = int.Parse(a.SelectSingleNode("ID").InnerText),
                        IDNotizia = int.TryParse(a.SelectSingleNode("IDNotizia")?.InnerText, out tmpIDNotizia) ? tmpIDNotizia : (int?)null,
                        NomeFile = a.SelectSingleNode("NomeFile")?.InnerText
                    };
                    tmpNews.Allegati.Add(tmpAllegato);
                }
                NewsCruscotto.Add(tmpNews);
            }
        }
    }
}