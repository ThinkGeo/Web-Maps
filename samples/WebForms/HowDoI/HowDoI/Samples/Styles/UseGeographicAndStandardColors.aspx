<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UseGeographicAndStandardColors.aspx.cs"
    Inherits="HowDoI.UseGeographicAndStandardColors" %>

<%@ Register Assembly="ThinkGeo.MapSuite.WebForms" Namespace="ThinkGeo.MapSuite.WebForms" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <link href="~/theme/default/samplepic/style.css" rel="stylesheet" type="text/css" />
    <title>Using Geographic, Simple & Standard Colors</title>
    <style>
        .colorbtn
        {
            width: 20px;
            height: 20px;
        }
    </style>
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
            <asp:AsyncPostBackTrigger ControlID="ddlGeofraphicColor" />
            <asp:AsyncPostBackTrigger ControlID="ddlStandardColor" />
        </Triggers>
    </asp:UpdatePanel>
    <Description:DescriptionPanel ID="Description" runat="server">
        This sample shows some of the predefined colors. Select a color from the drop down list
        to change the countries.
        <br />
        <br />
        <asp:Label ID="Label1" runat="server" Text="Border color:" Width="90" Font-Size="10px"></asp:Label><br />
        <asp:DropDownList ID="ddlStandardColor" Width="230" runat="server" AutoPostBack="True"
            Font-Size="10px" Font-Names="verdana" OnSelectedIndexChanged="ddlStandardColor_SelectedIndexChanged">
            <asp:ListItem Selected="true">GeoColor.StandardColors.LightGray</asp:ListItem>
            <asp:ListItem>GeoColor.StandardColors.Pink</asp:ListItem>
            <asp:ListItem>GeoColor.StandardColors.Teal</asp:ListItem>
            <asp:ListItem>GeoColor.StandardColors.Plum</asp:ListItem>
            <asp:ListItem>GeoColor.SimpleColors.DarkGreen</asp:ListItem>
        </asp:DropDownList>
        <br />
        <br />
        <asp:Label ID="Label2" runat="server" Text="Fill color:" Width="90" Font-Size="10px"></asp:Label><br />
        <asp:DropDownList ID="ddlGeofraphicColor" Width="230" runat="server" AutoPostBack="True"
            Font-Size="10px" Font-Names="verdana" OnSelectedIndexChanged="ddlGeofraphicColor_SelectedIndexChanged">
            <asp:ListItem Selected="true">GeoColor.GeographicColors.Sand</asp:ListItem>
            <asp:ListItem>GeoColor.GeographicColors.Dirt</asp:ListItem>
            <asp:ListItem>GeoColor.GeographicColors.Swamp</asp:ListItem>
            <asp:ListItem>GeoColor.StandardColors.LightGreen</asp:ListItem>
            <asp:ListItem>GeoColor.StandardColors.LightPink</asp:ListItem>
            <asp:ListItem>GeoColor.SimpleColors.DarkBlue</asp:ListItem>
        </asp:DropDownList>
    </Description:DescriptionPanel>
    </form>
</body>
</html>
