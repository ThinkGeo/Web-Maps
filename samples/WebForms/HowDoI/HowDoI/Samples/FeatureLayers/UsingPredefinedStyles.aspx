<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UsingPredefinedStyles.aspx.cs"
    Inherits="HowDoI.FeatureLayers.UsingPredefinedStyles" %>

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
            <asp:AsyncPostBackTrigger ControlID="btnState1" />
            <asp:AsyncPostBackTrigger ControlID="btnState2" />
        </Triggers>
    </asp:UpdatePanel>
    <Description:DescriptionPanel ID="DescPanel" runat="server">
        Click on one of the buttons to change the map style between the two styles.
        <br />
        <br />
        <table>
            <tr>
                <td>
                    <asp:Button ID="btnState1" runat="server" Text="style1(Country1)" OnClick="btnState1_Click" />
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Button ID="btnState2" runat="server" Text="style2(Country2)" OnClick="btnState2_Click" />
                </td>
            </tr>
        </table>
    </Description:DescriptionPanel>
    </form>
</body>
</html>
