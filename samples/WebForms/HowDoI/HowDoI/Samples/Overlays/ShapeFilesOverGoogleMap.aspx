<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ShapeFilesOverGoogleMap.aspx.cs" Inherits="HowDoI.Samples.Overlays.ShapeFilesOverGoogleMap" %>

<%@ Register Assembly="ThinkGeo.MapSuite.WebForms" Namespace="ThinkGeo.MapSuite.WebForms" TagPrefix="cc1" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <link href="~/theme/default/samplepic/style.css" rel="stylesheet" type="text/css" />
    <title>shape files over google map</title>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <cc1:Map ID="Map1" runat="server" Height="100%" Width="100%">
    </cc1:Map>
    <Description:DescriptionPanel ID="DescPanel" runat="server">
        In this sample, a layer overlay that contains a shapefile layer overlaps upon a Google overlay.
    </Description:DescriptionPanel>
    </form>
</body>
</html>
