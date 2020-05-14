<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RouteAvoidingCertainArea.aspx.cs"
    Inherits="ThinkGeo.MapSuite.RoutingSamples.RouteAvoidingCertainArea" %>

<%@ Register Assembly="ThinkGeo.MapSuite.WebForms" Namespace="ThinkGeo.MapSuite.WebForms" TagPrefix="cc1" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <link href="../theme/default/samplepic/style.css" rel="stylesheet" type="text/css" />
    <title>Route Avoiding Certain Area</title>
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
                The sample demonstrates how to avoid certain area when finding the shortest path
                by FindingPath event.
                <br />
                <br />
                The WKT Of Area:
                <asp:TextBox ID="txtAvoidWKT" Text="POLYGON((-97.7559 30.2889,-97.7605 30.2794,-97.7563 30.2712,-97.7421 30.2704,-97.7392 30.2831,-97.744 30.2946,-97.7559 30.2889))"
                    runat="server" Width="250px" ReadOnly="true" TextMode="MultiLine" Height="70px"></asp:TextBox>
                <br />
                Start FeatureId:
                <input type="text" id="txtStartId" runat="server" readonly="readonly" value="4226" style="margin: 10px 5px 3px 5px;" />
                <br />
                &nbsp;End FeatureId:
                <input type="text" id="txtEndId" runat="server" value="8094" readonly="readonly" style="margin: 3px 5px 6px 7px;" /><br />
                <div style="width: 100%; text-align: right; line-height: 25px; vertical-align: middle;">
                    <asp:CheckBox ID="idAvoidArea" Text="Avoid The Area" runat="server" AutoPostBack="True"
                        OnCheckedChanged="idAvoidArea_CheckedChanged" />&nbsp;&nbsp;&nbsp;&nbsp;
                </div>
            </Description:DescriptionPanel>
        </ContentTemplate>
    </asp:UpdatePanel>
    </form>
</body>
</html>
