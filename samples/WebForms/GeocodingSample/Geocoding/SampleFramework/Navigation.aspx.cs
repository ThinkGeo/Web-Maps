using System;
using System.Web.UI.WebControls;
using System.Xml;
using System.Globalization;
using System.Web.UI;

namespace ThinkGeo.MapSuite.HowDoI
{
    public partial class Navigation : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                LoadTreeview(string.Empty);
            }
        }

        private void LoadTreeview(string filter)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(Server.MapPath("SampleList.xml"));

            XmlNodeList nodeList = doc.SelectNodes("//SampleList/Category");

            for (int index = 0; index < nodeList.Count; index++)
            {
                TreeNode categoryNode = new TreeNode();
                categoryNode.Text = nodeList[index].Attributes["Name"].Value;
                categoryNode.Expanded = false;
                categoryNode.SelectAction = TreeNodeSelectAction.Expand;
                for (int subCategoryIndex = 0; subCategoryIndex < nodeList[index].SelectNodes("Category").Count; subCategoryIndex++)
                {
                    XmlNode subSample = nodeList[index].ChildNodes[subCategoryIndex].ChildNodes[subCategoryIndex];
                    if (subSample.Attributes["Name"].Value.ToLower(CultureInfo.InvariantCulture).Contains(filter))
                    {
                        TreeNode subSampleNode = new TreeNode();
                        subSampleNode.Text = subSample.Attributes["Name"].Value;
                        subSampleNode.NavigateUrl = String.Format("Content.aspx?SampleId={0}&Name={1}", subSample.Attributes["Id"].Value, subSample.Attributes["Name"].Value);
                        subSampleNode.Target = "content";
                        subSampleNode.ToolTip = subSample.ChildNodes[0].Value;
                        categoryNode.ChildNodes.Add(subSampleNode);
                    }
                    if (categoryNode.ChildNodes.Count > 0)
                    {
                        categoryNode.ChildNodes.Add(categoryNode);
                    }
                }
                for (int sampleIndex = 0; sampleIndex < nodeList[index].SelectNodes("Sample").Count; sampleIndex++)
                {
                    XmlNode subSample = nodeList[index].ChildNodes[sampleIndex];
                    if (subSample.Attributes["Name"].Value.ToLower(CultureInfo.InvariantCulture).Contains(filter))
                    {
                        TreeNode subSampleNode = new TreeNode();
                        subSampleNode.Text = subSample.Attributes["Name"].Value;
                        subSampleNode.NavigateUrl = String.Format("Content.aspx?SampleId={0}&Name={1}", subSample.Attributes["Id"].Value, subSample.Attributes["Name"].Value);
                        subSampleNode.Target = "content";
                        subSampleNode.ToolTip = subSample.ChildNodes[0].Value;
                        categoryNode.ChildNodes.Add(subSampleNode);
                    }
                }
                this.SampleListTreeView.Nodes.Add(categoryNode);
            }

            // Set the first node as default selected node
            if (this.SampleListTreeView.Nodes.Count > 0)
            {
                if (this.SampleListTreeView.Nodes[0].ChildNodes.Count > 0)
                {
                    this.SampleListTreeView.Nodes[0].Expand();

                    TreeNode firstNode = this.SampleListTreeView.Nodes[0].ChildNodes[0];
                    //firstNode.Select();
                    String script = String.Format("parent.content.location = '{0}';", firstNode.NavigateUrl);

                    Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "FrameLoadScript", script, true);
                }
            }
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            this.SampleListTreeView.Nodes.Clear();
            LoadTreeview(filter.Text.Trim().ToLower(CultureInfo.InvariantCulture));
            TreeViewContainer.Style[HtmlTextWriterStyle.Display] = "";
        }
    }
}
