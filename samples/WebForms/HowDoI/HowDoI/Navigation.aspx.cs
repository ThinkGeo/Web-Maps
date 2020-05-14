using System;
using System.Web.UI.WebControls;
using System.Xml;
using System.Globalization;
using System.Web.UI;

namespace HowDoI
{
    public partial class Navigation : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                LoadTreeview(string.Empty);

                # region Implement By Linq to XML
                //XDocument xmlFile = XDocument.Load(Server.MapPath("SampleList.xml"));
                //var categories = from c in xmlFile.Root.Elements("Category")
                //                 select c;
                //foreach (XElement category in categories)
                //{
                //    TreeNode categoryNode = new TreeNode();
                //    categoryNode.Text = category.Attribute("Name").Value;
                //    categoryNode.Expanded = false;
                //    categoryNode.SelectAction = TreeNodeSelectAction.Expand;

                //    foreach (XElement subCategory in category.Elements("Category"))
                //    {
                //        TreeNode subCategoryNode = new TreeNode();
                //        subCategoryNode.Text = subCategory.Attribute("Name").Value;
                //        subCategoryNode.Expanded = false;
                //        subCategoryNode.SelectAction = TreeNodeSelectAction.Expand;
                //        foreach (XElement subSample in subCategory.Elements("Sample"))
                //        {
                //            TreeNode subSampleNode = new TreeNode();
                //            subSampleNode.Text = subSample.Attribute("Name").Value;
                //            subSampleNode.NavigateUrl = String.Format("Content.aspx?SampleId={0}", subSample.Attribute("Id").Value);
                //            subSampleNode.Target = "content";
                //            subSampleNode.ToolTip = subSample.Element("Description").Value;
                //            subCategoryNode.ChildNodes.Add(subSampleNode);
                //        }
                //        categoryNode.ChildNodes.Add(subCategoryNode);
                //    }
                //    foreach (XElement sampleElement in category.Elements("Sample"))
                //    {
                //        TreeNode sample = new TreeNode();
                //        sample.Text = sampleElement.Attribute("Name").Value;
                //        sample.NavigateUrl = String.Format("Content.aspx?SampleId={0}", sampleElement.Attribute("Id").Value);
                //        sample.Target = "content";
                //        sample.ToolTip = sampleElement.Element("Description").Value;
                //        categoryNode.ChildNodes.Add(sample);
                //    }
                //    this.SampleListTreeView.Nodes.Add(categoryNode);
                //}
                # endregion
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
                    TreeNode subCategoryNode = new TreeNode();
                    subCategoryNode.Text = nodeList[index].ChildNodes[subCategoryIndex].Attributes["Name"].Value;
                    subCategoryNode.Expanded = false;
                    subCategoryNode.SelectAction = TreeNodeSelectAction.Expand;
                    for (int sampleIndex = 0; sampleIndex < nodeList[index].ChildNodes[subCategoryIndex].SelectNodes("Sample").Count; sampleIndex++)
                    {
                        XmlNode subSample = nodeList[index].ChildNodes[subCategoryIndex].ChildNodes[sampleIndex];
                        if (subSample.Attributes["Name"].Value.ToLower(CultureInfo.InvariantCulture).Contains(filter))
                        {
                            TreeNode subSampleNode = new TreeNode();
                            subSampleNode.Text = subSample.Attributes["Name"].Value;
                            subSampleNode.NavigateUrl = String.Format("Content.aspx?SampleId={0}&Name={1}", subSample.Attributes["Id"].Value, subSample.Attributes["Name"].Value);
                            subSampleNode.Target = "content";
                            subSampleNode.ToolTip = subSample.ChildNodes[0].Value;
                            subCategoryNode.ChildNodes.Add(subSampleNode);
                        }
                    }
                    if (subCategoryNode.ChildNodes.Count > 0)
                    {
                        categoryNode.ChildNodes.Add(subCategoryNode);
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

                    TreeNode firstNode = this.SampleListTreeView.Nodes[0].ChildNodes[0].ChildNodes[0];
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
