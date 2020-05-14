using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Data;
using System.Globalization;
using System.Net;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Xml;
using ThinkGeo.MapSuite.WmsServer;

namespace ThinkGeo.MapSuite.CustomWmsServer
{
    /// <summary>
    /// Represent the Default page.
    /// </summary>
    public partial class Admin : System.Web.UI.Page
    {
        [ImportMany(typeof(WmsLayerPlugin), AllowRecomposition = true)]
        private Collection<WmsLayerPlugin> WmsPlugins { get; set; }

        /// <summary>Initialize an instance of the Admin class.</summary>
        /// <overloads>Initialize an instance of the Admin class.</overloads>
        public Admin()
        { }

        /// <summary>
        /// This method is called when the load event of Admin page is triggered. 
        /// </summary>
        protected void Page_Load(object sender, EventArgs e)
        {
            RefreshPlugins();
        }

        private void RefreshPlugins()
        {
            using (DataTable dataSource = new DataTable())
            {
                dataSource.Locale = CultureInfo.InvariantCulture;
                dataSource.Columns.Add("Name");
                dataSource.Columns.Add("FullName");

                foreach (string layerName in GetLayerNames())
                {
                    DataRow row = dataSource.NewRow();
                    row["Name"] = layerName;
                    row["FullName"] = layerName;
                    dataSource.Rows.Add(row);
                }

                PluginsRepeater.DataSource = dataSource;
                PluginsRepeater.DataBind();
            }
        }

        /// <summary>
        /// Fires after an item has been databound. 
        /// </summary>
        /// <param name="sender">Represent the event trigger control.</param>
        /// <param name="e">The event argument for the Repeater control item data bound event.</param>
        protected void PluginsRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            //return;
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                string fullName = ((HtmlGenericControl)e.Item.FindControl("FullNameDiv")).InnerText.Trim();

                DataTable pluginTable = new DataTable
                {
                    Locale = CultureInfo.InvariantCulture
                };

                pluginTable.Columns.Add("Name");
                pluginTable.Columns.Add("SupportedStyle");
                pluginTable.Columns.Add("SupportedSrs");
                pluginTable.Columns.Add("BoundingBox");
                pluginTable.Columns.Add("FullName");

                GridView gridView = (GridView)e.Item.FindControl("PluginGridView");

                GetCapabilities(pluginTable, fullName);

                gridView.DataSource = pluginTable;
                gridView.DataBind();
            }
        }

        /// <summary>
        /// Fires after a row has been databound. 
        /// </summary>
        /// <param name="sender">Represent the event trigger control.</param>
        /// <param name="e">The event argument for the GridView control row data bound event.</param>
        protected void PluginGridView_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            //Hide full name field.
            e.Row.Cells[3].Style.Add("display", "none");

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                string name = e.Row.Cells[0].Text;
                string styleName = e.Row.Cells[1].Text;
                int boundingBoxCount = 0;

                foreach (string projectionName in e.Row.Cells[2].Text.Split("|".ToCharArray()))
                {
                    string boundingBox = e.Row.Cells[3].Text.Split("|".ToCharArray())[boundingBoxCount];
                    string units = string.Empty;
                    switch (projectionName)
                    {
                        case "EPSG:900913":
                        case "EPSG:3857":

                            units = "m";
                            break;
                        case "EPSG:4326":
                        case "CRS:84":
                            units = "degress";
                            break;
                    }

                    string projectionUrl = "<a target='_blank' href='MapView.htm?serverUrl=" + String.Format(CultureInfo.InvariantCulture, "WmsServer.axd&layer={0}&style={1}&projection={2}&units={3}&bbox={4}&rebuild={5}&useraltrenateserver={6}'>{7}</a>", name, styleName, projectionName, units, boundingBox, RebuildCheckbox.Checked, AlternateServerCheckBox.Checked, projectionName);

                    e.Row.Cells[2].Text += projectionUrl + "/";
                    boundingBoxCount++;
                }
                e.Row.Cells[2].Text = e.Row.Cells[2].Text.Substring(e.Row.Cells[2].Text.IndexOf("<"));
                e.Row.Cells[2].Text = e.Row.Cells[2].Text.TrimEnd('/');
            }
        }

        //Returns and array of Layer names 
        private string[] GetLayerNames()
        {
            XmlDocument layersXml = new XmlDocument();
            WebClient client = new WebClient();
            string capabilities = client.DownloadString(@"http://" + Request.Url.Authority + "/WmsServer.axd?SERVICE=WMS&VERSION=1.1.1&REQUEST=GetCapabilities");

            layersXml.XmlResolver = null;
            layersXml.LoadXml(capabilities);

            XmlNodeList layerNodes = layersXml.SelectNodes(@"MS_Capabilities/Capability/Layer/Layer");

            string layer = string.Empty;

            foreach (XmlNode node in layerNodes)
            {
                layer += node["Title"]?.InnerText + ";";
            }

            layer = layer.TrimEnd(";".ToCharArray());
            return layer.Split(";'".ToCharArray());
        }

        private void GetCapabilities(DataTable table, string layerName)
        {
            XmlDocument capabilitiesFileXml = new XmlDocument();
            WebClient client = new WebClient();

            string capabilities = client.DownloadString(@"http://" + Request.Url.Authority + "/WmsServer.axd?SERVICE=WMS&VERSION=1.1.1&REQUEST=GetCapabilities");

            capabilitiesFileXml.XmlResolver = null;
            capabilitiesFileXml.LoadXml(capabilities);

            XmlNodeList list = capabilitiesFileXml.SelectNodes(@"MS_Capabilities/Capability/Layer/Layer");

            foreach (XmlNode node in list)
            {
                foreach (XmlNode item in node.SelectNodes("Name"))
                {
                    if (item.InnerText == layerName)
                    {
                        //Multiple Styles with ...
                        foreach (XmlNode styleItem in node.SelectNodes("Style/Name"))
                        {
                            string projections = string.Empty;
                            string boundingBoxes = string.Empty;

                            //Multiple Projetions with bounding boxes
                            foreach (XmlNode srsItem in node.SelectNodes("SRS"))
                            {
                                XmlNode bbNode = node.SelectNodes("BoundingBox[@SRS='" + srsItem.InnerText + "']")[0];

                                projections += srsItem.InnerText + "|";
                                boundingBoxes += bbNode.Attributes["minx"].Value + "," + bbNode.Attributes["miny"].Value + "," + bbNode.Attributes["maxx"].Value + "," + bbNode.Attributes["maxy"].Value + "|";
                            }

                            projections.TrimEnd("|".ToCharArray());
                            boundingBoxes.TrimEnd("|".ToCharArray());
                            DataRow styleRow = table.NewRow();
                            styleRow["Name"] = layerName;
                            styleRow["SupportedStyle"] = styleItem.InnerText;
                            styleRow["SupportedSrs"] = projections;
                            styleRow["BoundingBox"] = boundingBoxes;
                            table.Rows.Add(styleRow);
                        }
                    }
                }
            }
        }
    }
}