using System;
using System.ComponentModel;
using System.IO;
using System.Web.UI.WebControls;

namespace ThinkGeo.MapSuite.RoutingSamples
{
    // This class represents the "instructions" floating panel.
    [ToolboxItem(false)]
    public class DescriptionPanel : Panel
    {
        protected override void OnPreRender(System.EventArgs e)
        {
            StreamReader reader = File.OpenText(Page.MapPath("~/helper/DescriptionPanel.js"));
            string script = reader.ReadToEnd();
            reader.Close();
            Page.ClientScript.RegisterClientScriptBlock(GetType(), "PanelScript", script, true);
            base.OnPreRender(e);
        }

        protected override void Render(System.Web.UI.HtmlTextWriter writer)
        {
            StreamReader reader = File.OpenText(Page.MapPath("~/helper/DescriptionPanel.htm"));
            string[] script = reader.ReadToEnd().Split(new string[]{"<!--Split-->"}, StringSplitOptions.RemoveEmptyEntries);
            reader.Close();
            writer.WriteLine(script[0].Trim());
            base.Render(writer);
            writer.WriteLine(script[1].Trim());
        }
    }
}
