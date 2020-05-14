<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MeasureMapTool.aspx.cs"
    Inherits="HowDoI.MeasureMapTool" %>

<%@ Register Assembly="ThinkGeo.MapSuite.WebForms" Namespace="ThinkGeo.MapSuite.WebForms" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <link href="~/theme/default/samplepic/style.css" rel="stylesheet" type="text/css" />
    <title>Measure Map Tool</title>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <cc1:Map ID="Map1" runat="server" Width="100%" Height="100%">
    </cc1:Map>
    <Description:DescriptionPanel ID="DescPanel" runat="server">
        This sample demonstrates how to use MeasureMapTool to measure path and polygon on
        the map.
        <br />
        <br />
        Try to click buttons below to measure path or area on the client.
        <br />
        <asp:ImageButton ID="buttonNormal" runat="server" ToolTip="Normal Mode" ImageUrl="~/theme/default/samplepic/Cursor.png"
            OnClientClick="Map1.SetMeasureMode('Normal');return false;" />
        <asp:ImageButton ID="buttonDrawLine" runat="server" ToolTip="Draw Line" ImageUrl="~/theme/default/samplepic/line28.png"
            OnClientClick="Map1.SetMeasureMode('PathMeasure');return false;" />
        <asp:ImageButton ID="buttonDrawPolygon" runat="server" ToolTip="Draw Polygon" ImageUrl="~/theme/default/samplepic/polygon28.png"
            OnClientClick="Map1.SetMeasureMode('PolygonMeasure');return false;" />
    </Description:DescriptionPanel>
    </form>
</body>
</html>
