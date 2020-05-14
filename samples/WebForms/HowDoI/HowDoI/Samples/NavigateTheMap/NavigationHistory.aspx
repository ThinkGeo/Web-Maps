<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="NavigationHistory.aspx.cs" Inherits="HowDoI.Samples.NavigationHistory" %>

<%@ Register Assembly="ThinkGeo.MapSuite.WebForms" Namespace="ThinkGeo.MapSuite.WebForms" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <link href="~/theme/default/samplepic/style.css" rel="stylesheet" type="text/css" />
    <title>Navigate the history</title>
    <style type="text/css">
        .btn
        {
        	margin:3px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="Map1_Script" runat="server">
    </asp:ScriptManager>
    <cc1:Map ID="Map1" runat="server" Width="100%" Height="100%">
    </cc1:Map>
    <Description:DescriptionPanel ID="DescPanel" runat="server">
        Change the extent of the map and click the buttons below to revert extent or toggle extent.
        <br />
        <br />
        <asp:Button ID="btnToPreviouse" CssClass="btn"  runat="server"  Width="120" Text="To Previous Extent"
            OnClientClick="Map1.ZoomToPreviousExtent();return false;" />
        <asp:Button ID="btnToNext" CssClass="btn"  runat="server"  Width="120" Text="To Next Extent"
            OnClientClick="Map1.ZoomToNextExtent();return false;" />
        <asp:Button ID="btnToggle" CssClass="btn" runat="server"  Width="120" Text="Toggle Extent"
            OnClientClick="Map1.ToggleExtent();return false;" />
    </Description:DescriptionPanel>
    </form>
</body>
</html>

