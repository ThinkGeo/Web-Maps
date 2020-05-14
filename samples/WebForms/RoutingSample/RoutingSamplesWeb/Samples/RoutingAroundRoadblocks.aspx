<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RoutingAroundRoadblocks.aspx.cs"
    Inherits="ThinkGeo.MapSuite.RoutingSamples.RoutingAroundRoadblocks" %>

<%@ Register Assembly="ThinkGeo.MapSuite.WebForms" Namespace="ThinkGeo.MapSuite.WebForms" TagPrefix="cc1" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <link href="../theme/default/samplepic/style.css" rel="stylesheet" type="text/css" />
    <title>Routing Around Roadblocks</title>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <cc1:Map ID="Map1" runat="server" Height="100%" Width="100%" OnClick="Map1_Click">
            </cc1:Map>
            <Description:DescriptionPanel ID="DescPanel" runat="server">
                Click the "Add Roadblock" Button, and then place roadblocks by clicking on the map.
                You can return to normal mode by clicking "Stop Adding" Button. Of course, the "Remove"
                Button is used to delete the previous roadblock you have added and "Clear" button
                is used to clear all roadblocks;
                <br />
                <div style="width: 100%; text-align: center; margin: 10px 5px 10px 5px; padding: 5px;">
                    <asp:Button ID="btnBegin" runat="server" Text="Add Roadblock" Width="110px" OnClick="btnBegin_Click" />&nbsp;&nbsp;
                    <asp:Button ID="btnStop" runat="server" Text="Stop Adding" Width="110px" OnClick="btnStop_Click" />
                    <br />
                    <br />
                    <asp:Button ID="btnRemove" runat="server" Text="Remove" Width="110px" OnClick="btnRemove_Click" />&nbsp;&nbsp;
                    <asp:Button ID="btnClear" runat="server" Text="Clear" Width="110px" OnClick="btnClear_Click" />
                </div>
                Generate a route around any roadblocks by clicking "Route Around Roadblocks".
                <br />
                Start Coordinate:
                <input type="text" id="txtStart" readonly="readonly" value="-97.718463806,30.276611214" runat="server"
                    style="margin: 5px;" />
                <br />
                &nbsp;&nbsp;&nbsp;End Corrdinate:
                <input type="text" id="txtEnd" readonly="readonly" value="-97.7083787008,30.2701095396" runat="server"
                    style="margin: 5px 5px 5px 3px;" />
                <br />
                <br />
                <div style="text-align: right;">
                    <asp:Button ID="btnGetRoute" runat="server" Text="Route Around Roadblocks" OnClick="btnGetRoute_Click" />&nbsp;&nbsp;&nbsp;&nbsp;
                </div>
            </Description:DescriptionPanel>
        </ContentTemplate>
    </asp:UpdatePanel>
    </form>
</body>
</html>
