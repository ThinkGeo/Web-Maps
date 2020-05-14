<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MoveAFeature.aspx.cs" Inherits="HowDoI.Samples.Features.MoveAFeature" %>

<%@ Register Assembly="ThinkGeo.MapSuite.WebForms" Namespace="ThinkGeo.MapSuite.WebForms" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link href="~/theme/default/samplepic/style.css" rel="stylesheet" type="text/css" />
    <title>Move a Feature</title>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <cc1:Map ID="Map1" runat="server" Width="100%" Height="100%">
            </cc1:Map>
            <Description:DescriptionPanel ID="DescriptionPanel1" runat="server">
                Click one of the buttons to move the feature on the screen either up, down, left
                or right.
                <br />
                <br />
                <table cellpadding="0" cellspacing="4">
                    <tr>
                        <td colspan="3" align="center">
                            <asp:ImageButton ID="btnMoveUp" runat="server" OnClick="btnMoveUp_Click" ToolTip="Move Up"
                                ImageUrl="~/theme/default/img/north_mini.gif" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:ImageButton ID="btnMoveLeft" runat="server" OnClick="btnMoveLeft_Click" ToolTip="Move Left"
                                ImageUrl="~/theme/default/img/west_mini.gif" />
                        </td>
                        <td>
                            &nbsp;&nbsp;&nbsp;
                        </td>
                        <td>
                            <asp:ImageButton ID="btnMoveRight" runat="server" OnClick="btnMoveRight_Click" ToolTip="Move Right"
                                ImageUrl="~/theme/default/img/east_mini.gif" />
                        </td>
                    </tr>
                    <tr>
                        <td colspan="3" align="center">
                            <asp:ImageButton ID="btnMoveDown" runat="server" OnClick="btnMoveDown_Click" ToolTip="Move Down"
                                ImageUrl="~/theme/default/img/south_mini.gif" />
                        </td>
                    </tr>
                </table>
            </Description:DescriptionPanel>
        </ContentTemplate>
    </asp:UpdatePanel>
    </form>
</body>
</html>
