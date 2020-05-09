using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace Utilities
{
    public class PageToString
    {
        private string html="";
        public PageToString(string address)
        {
            WebRequest request = WebRequest.Create(address);
            request.Headers.Add("User-Agent", "PostmanRuntime/7.24.0");
            WebResponse response = request.GetResponse();
            Stream data = response.GetResponseStream();
            //string html = String.Empty;
            using (StreamReader sr = new StreamReader(data))
            {
                html = sr.ReadToEnd();
            }
        }
        public string GetHtml()
        {
            return html;
        }
    }
}
