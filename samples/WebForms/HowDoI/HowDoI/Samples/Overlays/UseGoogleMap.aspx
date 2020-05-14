<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UseGoogleMap.aspx.cs" Inherits="HowDoI.Samples.Layers.UseGoogleMap" %>

<%@ Register Assembly="ThinkGeo.MapSuite.WebForms" Namespace="ThinkGeo.MapSuite.WebForms" TagPrefix="cc1" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <link href="~/theme/default/samplepic/style.css" rel="stylesheet" type="text/css" />
    <title>Use Google Map</title>    
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <cc1:Map ID="Map1" runat="server" Height="100%" Width="100%">
    </cc1:Map>
    <Description:DescriptionPanel ID="DescPanel" runat="server">
        This sample displays a google map. Click buttons below to switch between different Google maps.
        <br />
        <br />
        <asp:Button CssClass="btn" ID="btnRoad" runat="server" Text="Normal" OnClientClick="Map1.SetCurrentBackgroundMapType(google.maps.MapTypeId.ROADMAP); return false;" />
        <asp:Button CssClass="btn" ID="btnAerial" runat="server" Text="Hybrid" OnClientClick="Map1.SetCurrentBackgroundMapType(google.maps.MapTypeId.HYBRID); return false;" /><br />
        <asp:Button CssClass="btn" ID="btnSatellite" runat="server" Text="Satellite" OnClientClick="Map1.SetCurrentBackgroundMapType(google.maps.MapTypeId.SATELLITE); return false;" />
        <asp:Button CssClass="btn" ID="btnPhysical" runat="server" Text="Physical" OnClientClick="Map1.SetCurrentBackgroundMapType(google.maps.MapTypeId.TERRAIN); return false;" />
    </Description:DescriptionPanel>
    </form>
</body>
</html>