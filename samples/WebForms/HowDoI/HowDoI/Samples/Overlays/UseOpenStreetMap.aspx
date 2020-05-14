<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UseOpenStreetMap.aspx.cs"
    Inherits="HowDoI.Samples.NewFeatures.UseOpenStreetMap" %>

<%@ Register Assembly="ThinkGeo.MapSuite.WebForms" Namespace="ThinkGeo.MapSuite.WebForms" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <link href="~/theme/default/samplepic/style.css" rel="stylesheet" type="text/css" />
    <title>Display a Simple Map</title>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <cc1:Map ID="Map1" runat="server" Width="100%" Height="100%">
    </cc1:Map>
    <Description:DescriptionPanel ID="DescPanel" runat="server">
        This sample displays OpenStreet Maps. Click the buttons below to display different
        tilesets.
        <br />
        <asp:Button ID="btnRoad" CssClass="btn" runat="server" Text="Standard" OnClientClick="javascript: Map1.SetCurrentBackgroundMapType('base'); return false;" /><br />
        <asp:Button ID="btnCycle" CssClass="btn" runat="server" Text="Cycle Map" OnClientClick="javascript: Map1.SetCurrentBackgroundMapType('cycle'); return false;" /><br />
        <asp:Button ID="btnTransport" CssClass="btn" runat="server" Text="Transport Map"
            OnClientClick="Map1.SetCurrentBackgroundMapType('trans'); return false;" /><br />
    </Description:DescriptionPanel>
    </form>
</body>
</html>
