<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="OptimizeRoadType.aspx.cs"
    Inherits="ThinkGeo.MapSuite.RoutingSamples.OptimizeRoadType" %>

<%@ Register Assembly="ThinkGeo.MapSuite.WebForms" Namespace="ThinkGeo.MapSuite.WebForms" TagPrefix="cc1" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <link href="../theme/default/samplepic/style.css" rel="stylesheet" type="text/css" />
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <cc1:Map ID="Map1" runat="server" Height="100%" Width="100%">
            </cc1:Map>
            <Description:DescriptionPanel ID="DescPanel" runat="server">
                You have two chioces,"By Car" is normal way, the result of "On Foot" is optimized
                for walking. For example, the route will avoid highway roads, use pedestrian walkways.
                <br />
                <br />
                Start FeatureId:
                <input type="text" id="txtStartFeatureId" value="13208" readonly="readonly" runat="server" style="margin: 5px;" />
                <br />
                &nbsp;&nbsp;End FeatureId:
                <input type="text" id="txtEndFeatureId" value="12910" readonly="readonly" runat="server" style="margin: 5px 5px 9px 3px;" />
                <br />
                &nbsp;&nbsp;&nbsp;&nbsp;Route Type:&nbsp;&nbsp;
                <asp:DropDownList ID="ddlRouteType" AutoPostBack="true" runat="server" 
                    onselectedindexchanged="ddlRouteType_SelectedIndexChanged">
                    <asp:ListItem Selected="True">On Foot</asp:ListItem>
                    <asp:ListItem>By Car</asp:ListItem>
                </asp:DropDownList>
                <br />
            </Description:DescriptionPanel>
        </ContentTemplate>
    </asp:UpdatePanel>
    </form>
</body>
</html>
