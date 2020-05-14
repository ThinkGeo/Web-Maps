using System;
using System.Web.UI;

namespace HowDoI
{
    public partial class Banner : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (Request["SampleId"] != null)
                {
                    string sampleId = Request.QueryString["SampleId"];
                    string Name = Request.QueryString["Name"];
                    pageTitle.InnerText = Name;
                    Sample.HRef = String.Format("Content.aspx?SampleId={0}&Tab={1}", sampleId, Sample.ID);
                    CSharp.HRef = String.Format("Content.aspx?SampleId={0}&Tab={1}", sampleId, CSharp.ID);
                    ViewOnGitHub.HRef = @"https://github.com/ThinkGeo/HowDoISample-ForWebForms";
                }
            }
        }
    }
}
