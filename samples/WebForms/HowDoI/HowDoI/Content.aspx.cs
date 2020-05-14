using System;
using System.Web.UI;
using System.Xml;

namespace HowDoI
{
    public partial class Content : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (Request["SampleId"] != null)
                {
                    string samplePath = "", csharpCodePath = "";
                    string sampleId = Request.QueryString["SampleId"];
                    string pageName = Request.QueryString["Name"];


                    XmlDocument doc = new XmlDocument();
                    doc.Load(Server.MapPath("SampleList.xml"));
                    XmlNode nodeList = doc.SelectSingleNode("//Sample[@Id='" + sampleId + "']");

                    if (nodeList != null)
                    {
                        String basePath = nodeList.ParentNode.Attributes[2].Value;
                        samplePath = basePath + "/" + (nodeList.ChildNodes[1]).FirstChild.Value;
                        csharpCodePath = String.Format("{0}?SampleUrl={1}", "~/SyntaxHighlighter/csCode.aspx", samplePath);
                    }

                    if (Request["Tab"] == null)
                    {
                        String script = String.Format("parent.banner.location = 'Banner.aspx?SampleId={0}&Name={1}';", sampleId, pageName);

                        script += String.Format("window.location='{0}'", samplePath.Replace("\\", "/"));

                        Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "FrameLoadScript", script, true);
                    }
                    else
                    {
                        String tabId = Request.QueryString["Tab"];
                        switch (tabId)
                        {
                            case "Sample":
                                String script = String.Format("window.location='{0}'", samplePath.Replace("\\", "/"));
                                Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "FrameLoadScript", script, true);
                                break;
                            case "CSharp":
                                Response.Redirect(csharpCodePath, false);
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
        }
    }
}
