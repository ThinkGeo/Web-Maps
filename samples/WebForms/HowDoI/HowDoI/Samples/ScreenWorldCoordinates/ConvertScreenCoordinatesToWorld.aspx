<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ConvertScreenCoordinatesToWorld.aspx.cs"
    Inherits="HowDoI.ConvertScreenCoordinatesToWorld" %>

<%@ Register Assembly="ThinkGeo.MapSuite.WebForms" Namespace="ThinkGeo.MapSuite.WebForms" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <link href="~/theme/default/samplepic/style.css" rel="stylesheet" type="text/css" />
    <title>Convert Screen Coordinates to World</title>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="Map1_Script" runat="server">
    </asp:ScriptManager>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
        <ContentTemplate>
            <cc1:Map ID="Map1" runat="server" Width="100%" Height="100%">
            </cc1:Map>
            <Description:DescriptionPanel ID="Description" runat="server">
                Click the button to add a marker where the screen coordinates in the textbox represents.
                <br />
                <br />
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="screenXLabel" runat="server" Text="ScreenX:" Font-Size="10"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="ScreenXTextBox" runat="server" ReadOnly="true" Text="350"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="screenYLabel" runat="server" Text="ScreenY:" Font-Size="10"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="ScreenYTextBox" runat="server" ReadOnly="true" Text="350"></asp:TextBox>
                        </td>
                    </tr>
                </table>
                <asp:Button ID="btnConvert" runat="server" Width="215" Text="Convert To World Coordinate"
                    OnClick="btnConvert_Click" />
                <br />
                <asp:Label ID="Label1" ForeColor="#0065CE" runat="server"></asp:Label>
                <br />
            </Description:DescriptionPanel>
        </ContentTemplate>
    </asp:UpdatePanel>
    </form>
</body>
</html>
