<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UsingPredefinedStyle.aspx.cs"
    Inherits="HowDoI.UsingPredefinedStyle" %>

<%@ Register Assembly="ThinkGeo.MapSuite.WebForms" Namespace="ThinkGeo.MapSuite.WebForms" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <link href="~/theme/default/samplepic/style.css" rel="stylesheet" type="text/css" />
    <title>Using Predefined Styles</title>
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
            <asp:AsyncPostBackTrigger ControlID="ddlPreDefinedStyles" EventName="SelectedIndexChanged" />
        </Triggers>
    </asp:UpdatePanel>
    <Description:DescriptionPanel ID="Description" runat="server">
        Select a predefined style from the list to use it for the countries.
        <br />
        <br />
        <asp:DropDownList Width="200" ID="ddlPreDefinedStyles" runat="server" AutoPostBack="True"
            OnSelectedIndexChanged="ddlPreDefinedStyles_SelectedIndexChanged">
            <asp:ListItem Selected="true">AreaStyles.Country1</asp:ListItem>
            <asp:ListItem>AreaStyles.Swamp1</asp:ListItem>
            <asp:ListItem>AreaStyles.Grass1</asp:ListItem>
            <asp:ListItem>AreaStyles.Sand1</asp:ListItem>
            <asp:ListItem>AreaStyles.Crop1</asp:ListItem>
        </asp:DropDownList>
    </Description:DescriptionPanel>
    </form>
</body>
</html>
