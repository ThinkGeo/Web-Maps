using System;
using System.IO;

namespace HowDoI.SyntaxHighlighter
{
    public partial class csCode : System.Web.UI.Page
    {
        private const string PreStart = "<body oncontextmenu='return false;'><pre name='code' class='c-sharp:nocontrols'>";
        private const string PreEnd = "</pre><link type='text/css' rel='stylesheet' href='../../SyntaxHighlighter/SyntaxHighlighter.css'></link><script language='javascript' src='../../SyntaxHighlighter/shCore.js'></script><script language='javascript' src='../../SyntaxHighlighter/shBrushCSharp.js'></script><script language='javascript' src='../../SyntaxHighlighter/shBrushXml.js'></script><script language='javascript'>dp.SyntaxHighlighter.HighlightAll('code');</script></body>";
        private const string ErrorPageUrl = "~/Source Code Not Available in Beta.html";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                String sampleUrl = Request.QueryString["SampleUrl"];
                if (!String.IsNullOrEmpty(sampleUrl))
                {
                    string codePage = GetHtmlPath(sampleUrl);
                    Response.Redirect(String.Format("../{0}", codePage));
                }
            }
        }

        private string GetHtmlPath(string filename)
        {
            filename = String.Format("{0}.cs", filename);
            string htmlFileName = filename.Replace(".cs", ".htm");
            if (File.Exists(htmlFileName))
            {
                return htmlFileName;
            }

            string text;
            try
            {
                using (StreamReader sr = new StreamReader(Server.MapPath(String.Format("../{0}", filename))))
                {
                    text = sr.ReadToEnd();
                }
                text = PreStart + text + PreEnd;

                using (StreamWriter sw = new StreamWriter(Server.MapPath(String.Format("../{0}", htmlFileName))))
                {
                    sw.Write(text);
                }
            }
            catch
            {
                htmlFileName = ErrorPageUrl;
            }
            return htmlFileName;
        }
    }
}
