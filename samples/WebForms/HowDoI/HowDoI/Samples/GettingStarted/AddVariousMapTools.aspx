<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AddVariousMapTools.aspx.cs"
    Inherits="HowDoI.Samples.AddVariousMappingControls" %>

<%@ Register Assembly="ThinkGeo.MapSuite.WebForms" Namespace="ThinkGeo.MapSuite.WebForms" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link href="~/theme/default/samplepic/style.css" rel="stylesheet" type="text/css" />
    <title>Add Various Map Tools</title>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="Map1_Script" runat="server">
    </asp:ScriptManager>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <cc1:Map ID="Map1" runat="server" Width="100%" Height="100%">
            </cc1:Map>
        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="lsbControls" />
            <asp:AsyncPostBackTrigger ControlID="btnSelectAll" />
            <asp:AsyncPostBackTrigger ControlID="btnNoSelect" />
        </Triggers>
    </asp:UpdatePanel>
    <Description:DescriptionPanel ID="Description" runat="server">
        Select a number of items from the list below to show them on the map.
        <br />
        <br />
        <asp:ListBox ID="lsbControls" CssClass="lsb_normal" AutoPostBack="true" runat="server"
            SelectionMode="Multiple" OnSelectedIndexChanged="lsbControls_SelectedIndexChanged"
            Width="220" Font-Names="verdana" Font-Size="10px" Height="110">
            <asp:ListItem Text="PanZoom" Value="PanZoom"></asp:ListItem>
            <asp:ListItem Text="PanZoomBar" Value="PanZoomBar"></asp:ListItem>
            <asp:ListItem Text="MouseCoordinate(LonLat)" Value="MouseCoordinateLonLat"></asp:ListItem>
            <asp:ListItem Text="MouseCoordinate(LatLon)" Value="MouseCoordinateLatLon"></asp:ListItem>
            <asp:ListItem Text="MouseCoordinate(Dms)" Value="MouseCoordinateDms"></asp:ListItem>
            <asp:ListItem Text="MiniMap" Value="MiniMap"></asp:ListItem>
            <asp:ListItem Text="ScaleLine" Value="ScaleLine"></asp:ListItem>
            <asp:ListItem Text="OverlaySwitcher" Value="OverlaySwitcher"></asp:ListItem>
        </asp:ListBox>
        <div>
            <asp:Button ID="btnSelectAll" runat="server" Width="100"  Text="Select All"
                OnClick="btnSelectAll_Click" />
            <asp:Button ID="btnNoSelect" runat="server" Width="100"  Text="Unselect All"
                OnClick="btnNoSelect_Click" />
        </div>
    </Description:DescriptionPanel>
    </form>
</body>
</html>
