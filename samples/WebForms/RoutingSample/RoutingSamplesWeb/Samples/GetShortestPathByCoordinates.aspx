<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GetShortestPathByCoordinates.aspx.cs"
    Inherits="ThinkGeo.MapSuite.RoutingSamples.Samples.GetShortestPathByCoordinates" %>

<%@ Register Assembly="ThinkGeo.MapSuite.WebForms" Namespace="ThinkGeo.MapSuite.WebForms" TagPrefix="cc1" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <link href="../theme/default/samplepic/style.css" rel="stylesheet" type="text/css" />
    <title>Get Shortest Path By Coordinates</title>
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
                Please click Button to get the shortest path between the points set as below.
                <br />
                <br />
                Start Coordinate:
                <input type="text" id="txtStart" value="-97.73229734,30.295185" readonly="readonly" runat="server" style="margin: 5px;" />
                <br />
                &nbsp;&nbsp;&nbsp;End Corrdinate:
                <input type="text" id="txtEnd" value="-97.72465841,30.267462" readonly="readonly" runat="server" style="margin: 5px 5px 5px 3px;" />
                <br />
                <br />
                <div style="text-align:right;">
                    <asp:Button ID="btnGetRoute" runat="server" Text="Get Shortest Path" OnClick="btnGetRoute_Click" />&nbsp;&nbsp;
                </div>
            </Description:DescriptionPanel>
        </ContentTemplate>
    </asp:UpdatePanel>
    </form>
</body>
</html>
