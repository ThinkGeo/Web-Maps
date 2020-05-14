<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ConvertAGeoColorToAndFromOleWin32HtmlArgbColors.aspx.cs"
    Inherits="HowDoI.ConvertAGeoColorToAndFromOleWin32HtmlArgbColors" %>

<%@ Register Assembly="ThinkGeo.MapSuite.WebForms" Namespace="ThinkGeo.MapSuite.WebForms" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <link href="~/theme/default/samplepic/style.css" rel="stylesheet" type="text/css" />
    <title>Convert a GeoColor to and from OLE Win32 Argb HTML Colors</title>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="Map1_Script" runat="server">
    </asp:ScriptManager>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server" ChildrenAsTriggers="true">
        <ContentTemplate>
            <cc1:Map ID="Map1" runat="server" Width="100%" Height="100%">
            </cc1:Map>
            <Description:DescriptionPanel ID="Description" runat="server">
                Select a color from the list and see it's different expressions in the
                text boxes.
                <br />
                <br />
                <table style="font-size: 11px;" width="70%">
                    <tr>
                        <td style="width: 10%">
                            GeoColor:
                        </td>
                        <td>
                            <asp:DropDownList ID="ddlGeoColors" Width="170px" runat="server" AutoPostBack="True"
                                OnSelectedIndexChanged="ddlGeoColors_SelectedIndexChanged">
                                <asp:ListItem Selected="True">ShallowOcean</asp:ListItem>
                                <asp:ListItem>Sand</asp:ListItem>
                                <asp:ListItem>Lake</asp:ListItem>
                                <asp:ListItem>Silver</asp:ListItem>
                                <asp:ListItem>Green</asp:ListItem>
                                <asp:ListItem>Transparent</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            OLE:
                        </td>
                        <td>
                            <asp:TextBox ID="txtOLE" CssClass="txt_normal" Width="170px" Text="" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            Win32:
                        </td>
                        <td>
                            <asp:TextBox ID="txtWin32" CssClass="txt_normal" Width="170px" Text="" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            HTML:
                        </td>
                        <td>
                            <asp:TextBox ID="txtHTML" CssClass="txt_normal" Width="170px" Text="" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            Argb:
                        </td>
                        <td>
                            <asp:TextBox ID="txtArgb" CssClass="txt_normal" Width="170px" Text="" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                </table>
            </Description:DescriptionPanel>
        </ContentTemplate>
    </asp:UpdatePanel>
    </form>
</body>
</html>
