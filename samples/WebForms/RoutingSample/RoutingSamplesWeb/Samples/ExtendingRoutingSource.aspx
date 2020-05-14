<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ExtendingRoutingSource.aspx.cs"
    Inherits="ThinkGeo.MapSuite.RoutingSamples.ExtendingRoutingSource" %>

<%@ Register Assembly="ThinkGeo.MapSuite.WebForms" Namespace="ThinkGeo.MapSuite.WebForms" TagPrefix="cc1" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <link href="../theme/default/samplepic/style.css" rel="stylesheet" type="text/css" />
    <title>Extending RoutingSource</title>
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
                This sample demonstrates how to extend the RoutingSource class. We will create a
                routing source that queries a shape file in real-time instead of using an optimized
                routing index. this is going to be much slower but shows the flexibility of the
                struture.
                <br />
                <br />
                Start FeatureId:
                <input type="text" id="txtStartId" readonly="readonly" runat="server" value="4226"
                    style="margin: 10px 5px 3px 5px;" />
                <br />
                &nbsp;End FeatureId:
                <input type="text" id="txtEndId" readonly="readonly" runat="server" value="8094"
                    style="margin: 3px 5px 6px 7px;" />
                <br />
                <div style="width: 100%; text-align: right;">
                    <asp:Button ID="btnRoute" runat="server" Text="Get Shortest Path" OnClick="btnRoute_Click" />&nbsp;&nbsp;&nbsp;
                </div>
            </Description:DescriptionPanel>
        </ContentTemplate>
    </asp:UpdatePanel>
    </form>
</body>
</html>
