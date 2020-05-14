<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RouteOnlyInASpecificArea.aspx.cs"
    Inherits="ThinkGeo.MapSuite.RoutingSamples.RouteOnlyInASpecificArea" %>

<%@ Register Assembly="ThinkGeo.MapSuite.WebForms" Namespace="ThinkGeo.MapSuite.WebForms" TagPrefix="cc1" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <link href="../theme/default/samplepic/style.css" rel="stylesheet" type="text/css" />
    <title>Route only in a specific area</title>
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
                The sample demonstrates how to route only in a specific area when finding the shortest
                path by FindingPath event.
                <br />
                <br />
                The WKT Of Area:
                <asp:TextBox ID="txtAvoidWKT" Text="POLYGON((-97.7734 30.2968,-97.7733 30.2784,-97.7707 30.2649,-97.7621 30.2549,-97.7262 30.2567, -97.7289 30.2629, -97.7556 30.2671,-97.7650 30.2808, -97.7646 30.2984,-97.7734 30.2968))"
                    runat="server" Width="260px" TextMode="MultiLine" Height="80px"></asp:TextBox>
                <br />
                Start FeatureId:
                <input type="text" id="txtStartId" runat="server" readonly="readonly" value="3913" style="margin: 10px 5px 3px 5px;" />
                <br />
                &nbsp;End FeatureId:
                <input type="text" id="txtEndId" runat="server" readonly="readonly" value="9021" style="margin: 3px 5px 6px 7px;" /><br />
                <div style="width: 100%; text-align: right; line-height: 25px; vertical-align: middle;">
                    <asp:CheckBox ID="chbSpecifyArea" Text="Specify The Area" runat="server" AutoPostBack="True"
                        OnCheckedChanged="chbSpecifyArea_CheckedChanged" />&nbsp;&nbsp;&nbsp;&nbsp;
                </div>
            </Description:DescriptionPanel>
        </ContentTemplate>
    </asp:UpdatePanel>
    </form>
</body>
</html>
