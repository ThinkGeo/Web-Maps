<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="ThinkGeo.MapSuite.CustomWmsServer.Admin" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Admin</title>
    <style type="text/css">
        body, div {
            font-size: 10px;
            font-family: Arial;
            margin: 0px;
        }

        img.preview {
            width: 128px;
            height: 128px;
            border: solid 1px #cccccc;
            margin-top: 14px;
        }

        .name, .name1 {
            height: 20px;
            margin-left: 10px;
            font-size: 16px;
            color: #666666;
            font-weight: bolder;
            margin-top: 10px;
        }

        .name1 {
            font-size: 10px;
            color: Gray;
            margin: 5px 0 5px 0;
            display: none;
        }

        .content {
            font-size: 12px;
            line-height: 20px;
            color: gray;
            margin-left: 10px;
            margin-bottom: 20px;
        }

        .button, #FileUpload1 {
            font-size: 10px;
        }

        .button {
            width: 70px;
            height: 20px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div style="width: 766px; margin: auto; text-align: left;">
            <table cellpadding="0" cellspacing="0" border="0" style="margin-top: 0px; vertical-align: top; width: 100%; height: 100%">
                <tr style="height: 10px; background-color: #2c4056">
                    <td colspan="2"></td>
                </tr>
                <tr style="height: 50px; width: 100%; background-image: url(Images/header_bkg.jpg); background-repeat: no-repeat; background-color: #5179ac">
                    <td colspan="2" style="font-size: 24px; color: White;">
                        <b style="margin-left: 15px">ThinkGeo Wms Server Admin</b>
                    </td>
                </tr>
                <tr>
                    <td style="background-color: Transparent" valign="top" colspan="2">
                        <div style="border: solid 1px #e4e4e4; padding: 20px">
                            <div>
                                <a href="WmsServer.axd?SERVICE=WMS&VERSION=1.1.1&REQUEST=GetCapabilities"
                                    target="_blank">View Capabilities</a>
                                <asp:Label runat="server" ID="ExceptionMessage" ForeColor="Red"></asp:Label><br />
                                <asp:CheckBox ID="RebuildCheckbox" runat="server" Text="Rebuild Tiles" AutoPostBack="true" /><br />
                                <asp:CheckBox ID="AlternateServerCheckBox" runat="server" Text="Use Alternate Server" AutoPostBack="true" />
                            </div>
                            <asp:Repeater runat="server" ID="PluginsRepeater" OnItemDataBound="PluginsRepeater_ItemDataBound">
                                <ItemTemplate>
                                    <table border="0">
                                        <tr>
                                            <td valign="top">
                                                <div>
                                                    <div id="NameDiv" runat="server" class="name">
                                                        <%# DataBinder.Eval(Container.DataItem, "Name") %>
                                                    </div>
                                                    <div id="FullNameDiv" runat="server" class="name1" visible="false">
                                                        <%# DataBinder.Eval(Container.DataItem, "FullName") %>
                                                    </div>
                                                    <asp:GridView ID="PluginGridView" runat="server" AutoGenerateColumns="False" CellPadding="4"
                                                        Font-Names="Verdana" ForeColor="#333333" GridLines="None" OnRowDataBound="PluginGridView_RowDataBound">
                                                        <RowStyle BackColor="#EFF3FB" />
                                                        <Columns>
                                                            <asp:BoundField HeaderText="Name" DataField="Name" ItemStyle-Width="250px" ItemStyle-HorizontalAlign="Center">
                                                                <HeaderStyle HorizontalAlign="Center" />
                                                            </asp:BoundField>
                                                            <asp:BoundField HeaderText="Supported Style" ItemStyle-Width="150px" ItemStyle-HorizontalAlign="Center" DataField="SupportedStyle" />
                                                            <asp:BoundField HeaderText="Supported Srs" ItemStyle-Width="150px" ItemStyle-HorizontalAlign="Center" DataField="SupportedSrs" />
                                                            <asp:BoundField HeaderText="Supported Srs" ItemStyle-Width="150px" ItemStyle-HorizontalAlign="Center" DataField="BoundingBox" />
                                                            <asp:BoundField DataField="FullName" ItemStyle-HorizontalAlign="Center" />
                                                        </Columns>
                                                        <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                                                        <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
                                                        <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
                                                        <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                                                        <EditRowStyle BackColor="#2461BF" />
                                                        <AlternatingRowStyle BackColor="White" />
                                                    </asp:GridView>
                                                </div>
                                            </td>
                                        </tr>
                                    </table>
                                    <hr />
                                </ItemTemplate>
                            </asp:Repeater>
                        </div>
                    </td>
                </tr>
            </table>
        </div>
    </form>
    <script type="text/javascript">
        function OpenViewLayer(obj, href) {
            window.open(
                href,
                'WmsSample',
                'width=640,height=480,directories=no,menubar=no,toolbar=no,scrollbars=no,resizable=yes'
            );
        }

        function HoverToPreview(id, url) {
            var image = document.getElementById(id);
            image.setAttribute('src', url);
        }
    </script>
</body>
</html>
