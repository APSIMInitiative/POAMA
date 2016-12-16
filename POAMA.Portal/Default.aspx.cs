using APSIM.Shared.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace POAMA.Portal
{
    public partial class Main : System.Web.UI.Page
    {
        /// <summary>Page has been loaded.</summary>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (date.Text == string.Empty)
                date.Text = DateTime.Now.ToString("yyyy/MM/dd");
        }

        /// <summary>The generate button has been clicked.</summary>
        protected void OnGenerateClick(object sender, EventArgs e)
        {
            if (stationNumberValidator.IsValid &&
                dateValidator.IsValid &&
                passwordValidator.IsValid)
            {
                string url = string.Format("http://150.229.130.160/Forecast/Get?stationNumber={0}&date={1}&rainOnly={2}",
                                            stationNumber.Text, date.Text, !allData.Checked);

                string contents = CallRESTService(url) as string;
                ShowString(contents);
            }
        }

        
        /// <summary>Shows the specified string to the user</summary>
        private void ShowString(string st)
        {
            Response.Clear();
            Response.ContentType = "text/plain";
            Response.ContentEncoding = System.Text.Encoding.UTF8;
            Response.Write(st);
            Response.End();
        }

        /// <summary>Call REST web service.</summary>
        /// <param name="url">The URL of the REST service.</param>
        public static string CallRESTService(string url)
        {
            WebRequest wrGETURL;
            wrGETURL = WebRequest.Create(url);
            wrGETURL.Method = "GET";
            wrGETURL.ContentType = @"application/xml; charset=utf-8";
            wrGETURL.ContentLength = 0;
            using (HttpWebResponse webresponse = wrGETURL.GetResponse() as HttpWebResponse)
            {
                Encoding enc = System.Text.Encoding.GetEncoding("utf-8");
                // read response stream from response object
                using (StreamReader loResponseStream = new StreamReader(webresponse.GetResponseStream(), enc))
                {
                    return  loResponseStream.ReadToEnd();
                }
            }
        }

        /// <summary>Validate the date</summary>
        protected void OnDateValidation(object source, ServerValidateEventArgs args)
        {
            DateTime d;
            args.IsValid = DateTime.TryParseExact(date.Text, "yyyy/MM/dd", null, System.Globalization.DateTimeStyles.AssumeLocal, out d);
        }

        /// <summary>Validate the password</summary>
        protected void OnPasswordValidation(object source, ServerValidateEventArgs args)
        {
            args.IsValid = password.Text.Equals("ResearchOnly", StringComparison.CurrentCultureIgnoreCase);
        }

        /// <summary>The generate button has been clicked.</summary>
        protected void OnDownloadReportClick(object sender, EventArgs e)
        {
            Response.Redirect("http://www.marine.csiro.au/~mcintosh/POAMA_biascal.pdf");
        }
    }
}