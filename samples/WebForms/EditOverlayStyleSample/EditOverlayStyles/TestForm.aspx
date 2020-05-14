<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TestForm.aspx.cs" Inherits="EditOverlayStyles.TestForm" %>

<%@ Register Assembly="ThinkGeo.MapSuite.WebForms" Namespace="ThinkGeo.MapSuite.WebForms" TagPrefix="cc1" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <link href="~/theme/default/samplepic/style.css" rel="stylesheet" type="text/css" />
    <title>EditOverlay Styles</title>
    <script type="text/javascript">
        var OnMapCreating = function(map) {
            OpenLayers.Feature.Vector.style = {
                'default': {
                    fillColor: "blue",
                    fillOpacity: 0.4,
                    hoverFillColor: "white",
                    hoverFillOpacity: 0.8,
                    strokeColor: "#ee9900",
                    strokeOpacity: 1,
                    strokeWidth: 1,
                    strokeLinecap: "round",
                    strokeDashstyle: "solid",
                    hoverStrokeColor: "red",
                    hoverStrokeOpacity: 1,
                    hoverStrokeWidth: 0.2,
                    pointRadius: 6,
                    hoverPointRadius: 1,
                    hoverPointUnit: "%",
                    pointerEvents: "visiblePainted",
                    cursor: "inherit"
                },
                'select': {
                    fillColor: "green",
                    fillOpacity: 0.4,
                    hoverFillColor: "white",
                    hoverFillOpacity: 0.8,
                    strokeColor: "blue",
                    strokeOpacity: 1,
                    strokeWidth: 2,
                    strokeLinecap: "round",
                    strokeDashstyle: "solid",
                    hoverStrokeColor: "red",
                    hoverStrokeOpacity: 1,
                    hoverStrokeWidth: 0.2,
                    pointRadius: 6,
                    hoverPointRadius: 1,
                    hoverPointUnit: "%",
                    pointerEvents: "visiblePainted",
                    cursor: "pointer"
                }
            };
        }
        
    </script>    
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <cc1:Map ID="Map1" runat="server" Height="100%" Width="100%">
    </cc1:Map>
   <%-- <description:descriptionpanel ID="DescPanel" runat="server">
        This sample displays a google map. Click buttons below to switch between different Google maps.
        <br />
        <br />
        <asp:Button CssClass="btn" ID="btnRoad" runat="server" Text="Normal" OnClientClick="Map1.SetCurrentBackgroundMapType(G_NORMAL_MAP); return false;" />
        <asp:Button CssClass="btn" ID="btnAerial" runat="server" Text="Hybrid" OnClientClick="Map1.SetCurrentBackgroundMapType(G_HYBRID_MAP); return false;" /><br />
        <asp:Button CssClass="btn" ID="btnSatellite" runat="server" Text="Satellite" OnClientClick="Map1.SetCurrentBackgroundMapType(G_SATELLITE_MAP); return false;" />
        <asp:Button CssClass="btn" ID="btnPhysical" runat="server" Text="Physical" OnClientClick="Map1.SetCurrentBackgroundMapType(G_PHYSICAL_MAP); return false;" />
    </description:descriptionpanel>--%>
    </form>
</body>
</html>