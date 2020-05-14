<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RoutingDirections.aspx.cs"
    Inherits="ThinkGeo.MapSuite.RoutingSamples.RoutingDirections" %>

<%@ Register Assembly="ThinkGeo.MapSuite.WebForms" Namespace="ThinkGeo.MapSuite.WebForms" TagPrefix="cc1" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <link href="../theme/default/samplepic/style.css" rel="stylesheet" type="text/css" />
    <title>Route Direction</title>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <cc1:Map ID="Map1" runat="server" Height="100%" Width="100%">
            </cc1:Map>
            <Description:DescriptionPanel ID="DescPanel" runat="server">
                Click the button below to find the path along with turn by turn instructions.
                <br />
                <br />
                Start FeatureId:
                <input type="text" id="txtStartId" readonly="readonly" runat="server" value="6849"
                    style="margin: 10px 5px 3px 5px;" />
                <br />
                &nbsp;End FeatureId:
                <input type="text" id="txtEndId" readonly="readonly" runat="server" value="6782"
                    style="margin: 3px 5px 6px 7px;" />
                <br />
                <div style="width: 100%; text-align: right;">
                    <asp:Button ID="btnRoute" runat="server" Text="Get Route" OnClick="btnRoute_Click" />&nbsp;&nbsp;&nbsp;
                </div>
                <br />
                <asp:GridView ID="gvDirections" runat="server" CellPadding="4" ForeColor="#333333"
                    GridLines="None" FooterStyle-Font-Size="8px" AllowPaging="true" ShowFooter="false"
                    BorderStyle="Solid" BorderColor="Gray" BorderWidth="1px" Width="260px" OnPageIndexChanging="gvDirections_PageIndexChanging">
                    <RowStyle BackColor="#EFF3FB" Font-Size="10px" />
                    <PagerStyle BackColor="#EFF3FB" Height="21px" ForeColor="#333333" HorizontalAlign="Right"
                        Font-Bold="false" Font-Size="8px" />
                    <HeaderStyle BackColor="#a0cf87" Font-Bold="True" Height="25px" ForeColor="#666666"
                        Font-Size="11px" />
                    <AlternatingRowStyle BackColor="White" Font-Size="10px" />
                    <EmptyDataTemplate>
                        <table width="260px" cellpadding="0px" cellspacing="0px" border="0px" style="margin: -5px;">
                            <tr style="background-color: #a0cf87; text-align: center; font-weight: bold; font-size: 10px;
                                color: #666666; height: 25px;">
                                <td>
                                    RoadName
                                </td>
                                <td>
                                    Direction
                                </td>
                                <td>
                                    Length(Meter)
                                </td>
                            </tr>
                            <tr>
                                <td colspan="3" style="text-align: center; height: 30px; width: 260px; background-color: #EFF3FB;
                                    font-size: 10px; color: #666666;">
                                    There is no data to render now
                                </td>
                            </tr>
                        </table>
                    </EmptyDataTemplate>
                </asp:GridView>
            </Description:DescriptionPanel>
        </ContentTemplate>
    </asp:UpdatePanel>
    </form>
</body>
</html>
