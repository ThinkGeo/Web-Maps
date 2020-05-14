<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ChangeTheCursorStyle.aspx.cs"
    Inherits="HowDoI.Samples.ChangeTheCursorStyle" %>

<%@ Register Assembly="ThinkGeo.MapSuite.WebForms" Namespace="ThinkGeo.MapSuite.WebForms" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <link href="~/theme/default/samplepic/style.css" rel="stylesheet" type="text/css" />
    <title>Change the Cursor Style</title>
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
            <asp:AsyncPostBackTrigger ControlID="lsbCursorStyles" />
        </Triggers>
    </asp:UpdatePanel>
    <Description:DescriptionPanel ID="DescPanel" runat="server">
        Select one of the styles from the list below and then move your mouse over the map
        to see the new cursor style.
        <br />
        <br />
        <b>Try the cursor styles below.</b>
        <asp:ListBox ID="lsbCursorStyles" Height="300px" Width="160px" CssClass="lsb_normal"
            runat="server" SelectionMode="Single" AutoPostBack="true" OnSelectedIndexChanged="lsbCursorStyles_SelectedIndexChanged">
            <asp:ListItem Text="default" Value="0" Selected="True"></asp:ListItem>
            <asp:ListItem Text="Crosshair" Value="1"></asp:ListItem>
            <asp:ListItem Text="Help" Value="2"></asp:ListItem>
            <asp:ListItem Text="Move" Value="3"></asp:ListItem>
            <asp:ListItem Text="e-resize" Value="4"></asp:ListItem>
            <asp:ListItem Text="n-resize" Value="5"></asp:ListItem>
            <asp:ListItem Text="ne-resize" Value="6"></asp:ListItem>
            <asp:ListItem Text="nw-resize" Value="7"></asp:ListItem>
            <asp:ListItem Text="Pointer" Value="8"></asp:ListItem>
            <asp:ListItem Text="Progress" Value="9"></asp:ListItem>
            <asp:ListItem Text="Text" Value="10"></asp:ListItem>
            <asp:ListItem Text="s-resize" Value="11"></asp:ListItem>
            <asp:ListItem Text="se-resize" Value="12"></asp:ListItem>
            <asp:ListItem Text="sw-resize" Value="13"></asp:ListItem>
            <asp:ListItem Text="w-resize" Value="14"></asp:ListItem>
            <asp:ListItem Text="Wait" Value="15"></asp:ListItem>
            <asp:ListItem Text="Custom" Value="16"></asp:ListItem>
        </asp:ListBox>
    </Description:DescriptionPanel>
    </form>
</body>
</html>
