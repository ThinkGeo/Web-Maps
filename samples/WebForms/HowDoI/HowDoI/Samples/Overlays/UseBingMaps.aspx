<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UseBingMaps.aspx.cs" Inherits="HowDoI.Samples.Layers.UseBingMaps" %>

<%@ Register Assembly="ThinkGeo.MapSuite.WebForms" Namespace="ThinkGeo.MapSuite.WebForms" TagPrefix="cc1" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <link href="~/theme/default/samplepic/style.css" rel="stylesheet" type="text/css" />
    <title>Use Bing Maps</title>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <cc1:Map ID="Map1" runat="server" Height="100%" Width="100%">
    </cc1:Map>
    <Description:DescriptionPanel ID="DescPanel" runat="server">
        This sample displays Bing Maps. Click the buttons below to display different types
        of Bing map.
        <br />
        <asp:Button ID="btnRoad" CssClass="btn" runat="server" Text="Road" OnClientClick="Map1.SetCurrentBackgroundMapType('Road'); return false;" /><br />
        <asp:Button ID="btnSatellite" CssClass="btn" runat="server" Text="Aerial" OnClientClick="Map1.SetCurrentBackgroundMapType('Aerial'); return false;" /><br />
        <asp:Button ID="btnPhysical" CssClass="btn" runat="server" Text="Hybrid" OnClientClick="Map1.SetCurrentBackgroundMapType('AerialWithLabels'); return false;" />
    </Description:DescriptionPanel>
    </form>
</body>
</html>
