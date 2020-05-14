<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ZoomInAndOut.aspx.cs"
    Inherits="HowDoI.Samples.ZoomInAndOut" %>

<%@ Register Assembly="ThinkGeo.MapSuite.WebForms" Namespace="ThinkGeo.MapSuite.WebForms" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link href="~/theme/default/samplepic/style.css" rel="stylesheet" type="text/css" />
    <title>Zoom In and Out</title>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="Map1_Script" runat="server">
    </asp:ScriptManager>
    <cc1:Map ID="Map1" runat="server" Width="100%" Height="100%">
    </cc1:Map>
    <Description:DescriptionPanel ID="DescPanel" runat="server">
        Click the zoom in or zoom out buttons below to show how to zoom on the client side
        in JavaScript.
        <br />
        <br />
        <asp:Button ID="btnZoomIn" runat="server"  Width="100" Text="Zoom In"
            OnClientClick="Map1.ZoomIn();return false;" />
        <asp:Button ID="btnZoomOut" runat="server"  Width="100" Text="Zoom Out"
            OnClientClick="Map1.ZoomOut();return false;" />
    </Description:DescriptionPanel>
    </form>
</body>
</html>
