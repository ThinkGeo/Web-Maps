<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CreateARestrictionLayer.aspx.cs"
    Inherits="HowDoI.Samples.NavigateTheMap.CreateARestrictionLayer1" %>

<%@ Register Assembly="ThinkGeo.MapSuite.WebForms" Namespace="ThinkGeo.MapSuite.WebForms" TagPrefix="cc1" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <link href="~/theme/default/samplepic/style.css" rel="stylesheet" type="text/css" />
    <title>CreateARestrictionLayer</title>
    <style type="text/css">
        html
        {
            height: 100%;
        }
        .descContent
        {
            font-family::Verdana;
            font-size: 11px;
        }
        input
        {
            margin: 3px;
        }
    </style>
</head>
<body style="height: 100%;">
    <form id="form1" runat="server" style="height: 100%;">
    <asp:ScriptManager ID="Map1_Script" runat="server">
    </asp:ScriptManager>
    <cc1:Map ID="Map1" runat="server" Width="100%" Height="100%">
    </cc1:Map>
    <Description:DescriptionPanel ID="DescPanel" runat="server">
        <table border="0" cellpadding="0" cellspacing="0">
            <tr>
                <td>
                    <span class="descContent">a Restriction Style</span>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:DropDownList ID="ddlRestrictionStyle" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlRestrictionStyle_SelectedIndexChanged">
                        <asp:ListItem Text="HatchPattern" Value="HatchPattern"></asp:ListItem>
                        <asp:ListItem Text="UseCustomStyles" Value="UseCustomStyles"></asp:ListItem>
                        <asp:ListItem Text="CircleWithSlashImage" Value="CircleWithSlashImage"></asp:ListItem>
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:RadioButton CssClass="descContent" GroupName="Vis" ID="radShowZones" Checked="true"
                        Text="Show Zones" runat="server" AutoPostBack="True" OnCheckedChanged="radShowZones_CheckedChanged" />
                    <asp:RadioButton CssClass="descContent" GroupName="Vis" ID="radHideZones" Text="Hide Zones"
                        runat="server" AutoPostBack="True" OnCheckedChanged="radHideZones_CheckedChanged" />
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label CssClass="descContent" ID="lbInfomation" runat="server" Text="You can see only Africa because we have added a RestrictionLayer and its mode is ShowZones."></asp:Label>
                </td>
            </tr>
        </table>
    </Description:DescriptionPanel>
    </form>
</body>
</html>
