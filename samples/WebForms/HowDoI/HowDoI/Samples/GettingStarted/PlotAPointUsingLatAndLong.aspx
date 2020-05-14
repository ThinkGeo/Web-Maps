<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PlotAPointUsingLatAndLong.aspx.cs"
    Inherits="HowDoI.Samples.PlotAPointUsingLatAndLong" %>

<%@ Register Assembly="ThinkGeo.MapSuite.WebForms" Namespace="ThinkGeo.MapSuite.WebForms" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <link href="~/theme/default/samplepic/style.css" rel="stylesheet" type="text/css" />
    <title>Plot a Point Using Lat & Long</title>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="Map1_Script" runat="server">
    </asp:ScriptManager>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <cc1:Map ID="Map1" runat="server" Width="100%" Height="100%">
            </cc1:Map>
        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="btnPlotPoint" EventName="Click" />
        </Triggers>
    </asp:UpdatePanel>
    <Description:DescriptionPanel ID="DescPanel" runat="server">
        Click the button to plot a point at the coordinates in the text boxes.
        <br />
        <br />
        <table>
            <tr>
                <td>
                    <asp:Label ID="Label1" runat="server" Text="Longitude:" Width="80px" Font-Bold="True"
                        Font-Size="10pt"></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="LongitudeTextBox" runat="server" ReadOnly="true" Text="-95.28109"
                        Font-Size="10px" Font-Names="verdana"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="Label2" runat="server" Text="Latitude:" Width="80px" Font-Bold="True"
                        Font-Size="10pt"></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="LatitudeTextBox" runat="server" ReadOnly="true" Text="38.95363"
                        Font-Size="10px" Font-Names="verdana"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>
                    <br />
                    <asp:Button ID="btnPlotPoint" runat="server" Text="Plot a Point" OnClick="btnAddPoint_Click" />
                </td>
            </tr>
        </table>
    </Description:DescriptionPanel>
    </form>
</body>
</html>
