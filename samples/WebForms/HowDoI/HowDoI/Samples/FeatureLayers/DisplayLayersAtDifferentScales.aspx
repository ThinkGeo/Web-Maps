<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DisplayLayersAtDifferentScales.aspx.cs"
    Inherits="HowDoI.DisplayLayersAtDifferentScales" %>

<%@ Register Assembly="ThinkGeo.MapSuite.WebForms" Namespace="ThinkGeo.MapSuite.WebForms" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <link href="~/theme/default/samplepic/style.css" rel="stylesheet" type="text/css" />
    <title>Display Layers at Different Scales</title>
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
            <asp:AsyncPostBackTrigger ControlID="btnLow" />
            <asp:AsyncPostBackTrigger ControlID="btnNoraml" />
            <asp:AsyncPostBackTrigger ControlID="btnHigh" />
        </Triggers>
    </asp:UpdatePanel>
    <Description:DescriptionPanel ID="DescPanel" runat="server">
        Click the buttons to go to different scales and see the new layers.
        <br />
        <br />
        <table>
            <tr>
                <td>
                    <asp:Button ID="btnLow" runat="server" Text="1:10,000" Width="150" OnClick="btnLow_Click" /><br />
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Button ID="btnNoraml" runat="server" Text="1:10,000,000" Width="150" OnClick="btnNoraml_Click" /><br />
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Button ID="btnHigh" runat="server" Text="1:100,000,000" Width="150" OnClick="btnHigh_Click" />
                </td>
            </tr>
        </table>
    </Description:DescriptionPanel>
    </form>
</body>
</html>
