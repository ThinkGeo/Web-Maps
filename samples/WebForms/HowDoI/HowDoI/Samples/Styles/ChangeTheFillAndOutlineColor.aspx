<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ChangeTheFillAndOutlineColor.aspx.cs"
    Inherits="HowDoI.ChangeTheFillAndOutlineColor" %>

<%@ Register Assembly="ThinkGeo.MapSuite.WebForms" Namespace="ThinkGeo.MapSuite.WebForms" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <link href="~/theme/default/samplepic/style.css" rel="stylesheet" type="text/css" />
    <title>Change the Fill & Outline Color</title>
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
            <asp:AsyncPostBackTrigger ControlID="btnFillColorYellow" />
            <asp:AsyncPostBackTrigger ControlID="btnOutLineColorGray" />
            <asp:AsyncPostBackTrigger ControlID="btnFillColorGreen" />
            <asp:AsyncPostBackTrigger ControlID="btnOutlineColorBlack" />
        </Triggers>
    </asp:UpdatePanel>
    <Description:DescriptionPanel ID="Description" runat="server">
        Click the buttons below to change the fill or outline color.
        <br />
        <br />
        <table cellpadding="0" cellspacing="2">
            <tr>
                <td>
                    <asp:Button ID="btnFillColorYellow" runat="server" Width="120" Text="Fill yellow color"
                        OnClick="btnFillColorYellow_Click" />
                </td>
                <td>
                    <asp:Button ID="btnOutLineColorGray" runat="server" Width="120" Text="Outline gray color"
                        OnClick="btnOutLineColorGray_Click" />
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Button ID="btnFillColorGreen" runat="server" Width="120" Text="Fill green color"
                        OnClick="btnFillColorGreen_Click" />
                </td>
                <td>
                    <asp:Button ID="btnOutlineColorBlack" runat="server" Width="120" Text="Outline black color"
                        OnClick="btnOutlineColorBlack_Click" />
                </td>
            </tr>
        </table>
    </Description:DescriptionPanel>
    </form>
</body>
</html>
