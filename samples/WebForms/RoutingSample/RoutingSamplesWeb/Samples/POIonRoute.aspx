<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="POIonRoute.aspx.cs" Inherits="ThinkGeo.MapSuite.RoutingSamples.POIonRoute" %>

<%@ Register Assembly="ThinkGeo.MapSuite.WebForms" Namespace="ThinkGeo.MapSuite.WebForms" TagPrefix="cc1" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link href="../theme/default/samplepic/style.css" rel="stylesheet" type="text/css" />
    <title>POI on Route</title>
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
                Click the button to get the shortest path between the start and end roads whose
                feature ids are set as below. And do a spatial query to get the nearest gas stations
                along the route.
                <br />
                <br />
                Start FeatureId:
                <input type="text" id="txtStartFeatureId" readonly="readonly" value="4636" runat="server"
                    style="margin: 5px;" />
                <br />
                &nbsp;&nbsp;End FeatureId:
                <input type="text" id="txtEndFeatureId" readonly="readonly" value="7516" runat="server"
                    style="margin: 5px 5px 5px 3px;" />
                <br />
                <asp:Button ID="btnRoute" runat="server" 
                    Text="Find gas stations along the route" Width="212px" 
                    onclick="btnRoute_Click"/>
                <br />
            </Description:DescriptionPanel>
        </ContentTemplate>
    </asp:UpdatePanel>
    </form>
</body>
</html>
