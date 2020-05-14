<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UseMapSimplification.aspx.cs"
    Inherits="HowDoI.Samples.Features.UseMapSimplification" %>

<%@ Register Assembly="ThinkGeo.MapSuite.WebForms" Namespace="ThinkGeo.MapSuite.WebForms" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <link href="~/theme/default/samplepic/style.css" rel="stylesheet" type="text/css" />
    <title>Use map simplification</title>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="Map1_Script" runat="server">
    </asp:ScriptManager>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <cc1:map id="Map1" runat="server" width="100%" height="100%">
            </cc1:map>
        </ContentTemplate>
    </asp:UpdatePanel>
    <description:descriptionpanel id="DescPanel" runat="server">
        This sample shows how to render the map using different simplification types. Please select
        the drawing parameters from the dropdown lists below, then click the button to draw.
        <br />
        <br />
        <table width="100%" border="0px" cellpadding="5px">
            <tr>
                <td>
                    Tolerance:
                </td>
                <td>
                    <asp:DropDownList ID="ddlTolerance" Width="50px" runat="server">
                        <asp:ListItem Text="0.75"></asp:ListItem>
                        <asp:ListItem Text="1.5"></asp:ListItem>
                        <asp:ListItem Text="3"></asp:ListItem>
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td>
                    SimplificationType:
                </td>
                <td>
                    <asp:DropDownList ID="ddlsimplification" runat="server">
                        <asp:ListItem Text="TopologyPreserving"></asp:ListItem>
                        <asp:ListItem Text="DouglasPeucker"></asp:ListItem>
                    </asp:DropDownList>
                </td>
            </tr>
        </table>
        <br />
        <asp:Button ID="btnSimplify" runat="server" Text="Simplify" Width="120px" OnClick="btnSimplify_Click" />
    </description:descriptionpanel>
    </form>
</body>
</html>
