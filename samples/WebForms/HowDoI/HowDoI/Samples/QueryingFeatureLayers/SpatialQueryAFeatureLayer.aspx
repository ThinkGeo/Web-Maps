<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SpatialQueryAFeatureLayer.aspx.cs"
    Inherits="HowDoI.Samples.Querying_Vector_Layers.SpatialQueryAFeatureLayer" %>

<%@ Register Assembly="ThinkGeo.MapSuite.WebForms" Namespace="ThinkGeo.MapSuite.WebForms" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <link href="~/theme/default/samplepic/style.css" rel="stylesheet" type="text/css" />
    <title>Spatial query a feature layer</title>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <cc1:Map ID="Map1" runat="server" Width="100%" Height="100%">
            </cc1:Map>
        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="ddlSpatialQuery" />
        </Triggers>
    </asp:UpdatePanel>
    <Description:DescriptionPanel ID="DescPanel" runat="server">
        Select a spatial query type from the list to see the features that are found.
        <br />
        <br />
        Choose SpatialQuery Type&nbsp;&nbsp;<asp:DropDownList ID="ddlSpatialQuery" runat="server"
            AutoPostBack="True" OnSelectedIndexChanged="ddlSpatialQuery_SelectedIndexChanged">
            <asp:ListItem Text="Within"></asp:ListItem>
            <asp:ListItem Text="">Containing</asp:ListItem>
            <asp:ListItem Text="">Disjointed</asp:ListItem>
            <asp:ListItem Text="">Intersecting</asp:ListItem>
            <asp:ListItem Text="">Overlapping</asp:ListItem>
            <asp:ListItem Text="">TopologicalEqual</asp:ListItem>
            <asp:ListItem Text="">Touching</asp:ListItem>
        </asp:DropDownList>
    </Description:DescriptionPanel>
    </form>
</body>
</html>
