using System;
using Utilities;
using System.Collections.Generic;
using System.Text;

namespace BezposrednieIntegration
{
    class BezposrednieGetOffersFromMainPage
    {
        public List<string> OffersList = null;
        public BezposrednieGetOffersFromMainPage(string url)
        {
            string mainPage = new Utilities.PageToString(url).GetHtml();
            OffersList = ListOffers(mainPage);
        }

        private List<string> ListOffers(string html)
        {
            List<string> OffersList = new List<string>();
            string cut = html.Substring(html.IndexOf("<tbody"), html.IndexOf("/tbody") - html.IndexOf("<tbody") + 7);
            while(cut.Contains("single-property"))
            {
                string href = cut.Substring(cut.IndexOf("<a"), cut.IndexOf("/a>")- cut.IndexOf("<a")+3);
                href = href.Substring(href.IndexOf("href")+6, href.IndexOf(">")- href.IndexOf("href")-7);
                if (href.Contains("mieszkanie-") || href.Contains("dom-") )
                {
                    if (!href.Contains("dzialka") && !href.Contains("garaz") && !href.Contains("pokoj"))
                    {
                        OffersList.Add(href);
                    }                   
                }
                cut = cut.Substring(cut.IndexOf("</tr>") + 5);
            }
            return OffersList;
        }
    }
}
