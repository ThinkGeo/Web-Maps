using System;
using System.Globalization;
using System.IO;
using System.Web.Mvc;
using System.Text.RegularExpressions;

namespace CSharp_HowDoISamples.Controllers
{
    public class GetSourceCodeController : Controller
    {
        private const string preTemplate = "<b><h2 class='title'>Source Code</h2></b><hr class='hr'/><div  style='margin-top:-15px; height:100%;'/><pre name='code' class='{0}:nocontrols'>";
        private const string endTemplate = "</pre></div><link type='text/css' rel='stylesheet' href='{1}://{0}Scripts/SyntaxHighlighter/SyntaxHighlighter.css'></link><script language='javascript' src='{1}://{0}Scripts/SyntaxHighlighter/shCore.js'></script><script language='javascript' src='{1}://{0}Scripts/SyntaxHighlighter/shBrushCSharp.js'></script><script language='javascript' src='{1}://{0}Scripts/SyntaxHighlighter/shBrushXml.js'></script><script language='javascript'>";
        private const string suffix = "try{dp.SyntaxHighlighter.HighlightAll('code');}catch(error){}</script>";

        private const string sourceCodeFolder = @"~/Resources/SourceCode/";

        public string Razor()
        {
            ClearTempPages();

            string protocol = (Url.RequestContext.HttpContext).Request.QueryString["protocol"].ToString();
            string subhost = (Url.RequestContext.HttpContext).Request.QueryString["subhost"].ToString();
            string category = (Url.RequestContext.HttpContext).Request.QueryString["category"].ToString();
            string pageName = (Url.RequestContext.HttpContext).Request.QueryString["pageName"].ToString();

            string baseFileName = string.Format(CultureInfo.InvariantCulture, "~/Views/{0}/{1}.cshtml", category, pageName);
            string filePath = Server.MapPath(baseFileName);

            string resultString = String.Empty;
            try
            {
                using (Stream stream = System.IO.File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    StreamReader reader = new StreamReader(stream);
                    resultString = reader.ReadToEnd();

                    reader.Close();
                    reader.Dispose();
                    reader = null;
                }
            }
            catch
            {
                resultString = String.Empty;
            }

            string pre = string.Format(CultureInfo.InvariantCulture, preTemplate, "xml");
            resultString = ScriptFilter(resultString);
            string end = string.Format(CultureInfo.InvariantCulture, endTemplate, (Url.RequestContext.HttpContext).Request.Url.Authority + subhost + "/", protocol);
            resultString = string.Format(CultureInfo.InvariantCulture, "{0}{1}{2}{3}", pre, resultString, end, suffix);

            string tempPageId = Guid.NewGuid().ToString();
            try
            {
                string tempPath = Server.MapPath(string.Format(CultureInfo.InvariantCulture, "{0}/{1}.htm", sourceCodeFolder, tempPageId));
                using (FileStream stream = System.IO.File.Create(tempPath))
                {
                    using (StreamWriter writer = new StreamWriter(stream))
                    {
                        writer.Write(resultString);
                    }
                }
            }
            catch
            {}

            return tempPageId;
        }

        public string Controller()
        {
            ClearTempPages();

            string protocol = (Url.RequestContext.HttpContext).Request.QueryString["protocol"].ToString();
            string subhost = (Url.RequestContext.HttpContext).Request.QueryString["subhost"].ToString();
            string category = (Url.RequestContext.HttpContext).Request.QueryString["category"].ToString();
            string pageName = (Url.RequestContext.HttpContext).Request.QueryString["pageName"].ToString();

            string baseFileName = string.Format(CultureInfo.InvariantCulture, "~/Controllers/{0}/{1}Controller.cs", category, pageName);
            string filePath = Server.MapPath(baseFileName);

            string resultString = String.Empty;
            try
            {
                using (Stream stream = System.IO.File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    StreamReader reader = new StreamReader(stream);
                    resultString = reader.ReadToEnd();

                    reader.Close();
                    reader.Dispose();
                    reader = null;
                }
            }
            catch
            {
                resultString = String.Empty;
            }

            string pre = string.Format(CultureInfo.InvariantCulture, preTemplate, "c-sharp");
            string end = string.Format(CultureInfo.InvariantCulture, endTemplate, (Url.RequestContext.HttpContext).Request.Url.Authority + subhost + "/", protocol);
            resultString = string.Format(CultureInfo.InvariantCulture, "{0}{1}{2}{3}", pre, resultString, end, suffix);

            string tempPageId = Guid.NewGuid().ToString();
            try
            {
                string tempPath = Server.MapPath(string.Format(CultureInfo.InvariantCulture, "{0}/{1}.htm", sourceCodeFolder, tempPageId));
                using (FileStream stream = System.IO.File.Create(tempPath))
                {
                    using (StreamWriter writer = new StreamWriter(stream))
                    {
                        resultString = resultString.Replace("<object>", "&lt;object&gt;");
                        writer.Write(resultString);
                    }
                }
            }
            catch
            { }

            return tempPageId;
        }

        private void ClearTempPages()
        {
            string folderPath = Server.MapPath(sourceCodeFolder);

            DirectoryInfo dir = new DirectoryInfo(folderPath);
            foreach (FileInfo file in dir.GetFiles())
            {
                double mits = DateTime.Now.Subtract(file.LastAccessTime).TotalSeconds;
                if (mits > 30)
                {
                    try
                    {
                        file.Delete();
                    }
                    catch { }
                }
            }
        }

        public string ScriptFilter(string input)
        {
            return Regex.Replace(input, @"<script[^>]*?>.*?</script>", "", RegexOptions.IgnoreCase);
        }
    }
}