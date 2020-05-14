<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ChangeTheWidthAndColorOfALine.aspx.cs"
    Inherits="HowDoI.ChangeTheWidthAndColorOfALine" %>

<%@ Register Assembly="ThinkGeo.MapSuite.WebForms" Namespace="ThinkGeo.MapSuite.WebForms" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <link href="~/theme/default/samplepic/style.css" rel="stylesheet" type="text/css" />
    <title>Change the Width & Color of a Line</title>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="Map1_Script" runat="server">
    </asp:ScriptManager>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <cc1:Map ID="Map1" runat="server" Width="100%" Height="100%">
            </cc1:Map>
            <Description:DescriptionPanel ID="Description" runat="server">
                Click the buttons below to change the width or color of the roads.
                <br />
                <br />
                <table>
                    <tr>
                        <td>
                            <asp:Button ID="btnWider" runat="server" Text="Widen the road" Width="150" OnClick="btnWider_Click" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Button ID="btnNarrow" runat="server" Text="Thin the road" Width="150" OnClick="btnNarrow_Click" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Button ID="btnLineColorYellow" runat="server" Text="Draw with light yellow"
                                Width="150" OnClick="btnLineColorYellow_Click" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Button ID="btnLineColorPink" runat="server" Text="Draw with light pink" Width="150"
                                OnClick="btnLineColorPink_Click" />
                        </td>
                    </tr>
                </table>
            </Description:DescriptionPanel>
        </ContentTemplate>
    </asp:UpdatePanel>
    </form>
</body>
</html>
