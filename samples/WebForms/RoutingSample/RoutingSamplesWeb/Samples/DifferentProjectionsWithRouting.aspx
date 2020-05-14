<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DifferentProjectionsWithRouting.aspx.cs"
    Inherits="ThinkGeo.MapSuite.RoutingSamples.DifferentProjectionsWithRouting" %>

<%@ Register Assembly="ThinkGeo.MapSuite.WebForms" Namespace="ThinkGeo.MapSuite.WebForms" TagPrefix="cc1" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link href="../theme/default/samplepic/style.css" rel="stylesheet" type="text/css" />
    <title>Different Projections With Routing</title>
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
                The layers in the map are using Google projection. Select radio button below to
                get the shortest or fastest path between the start and end roads whose feature ids
                are set as below.
                <br />
                <br />
                Start FeatureId:
                <input type="text" id="txtStartFeatureId" readonly="readonly" value="4226" runat="server"
                    style="margin: 5px;" />
                <br />
                &nbsp;&nbsp;End FeatureId:
                <input type="text" id="txtEndFeatureId" readonly="readonly" value="8094" runat="server"
                    style="margin: 5px 5px 5px 3px;" />
                <br />
                <br />
                <fieldset>
                    <legend>Find The Path</legend>
                    <table width="100%" style="font-size: 11px; font-family: Arial;">
                        <tr>
                            <td>
                                <asp:RadioButton Checked="true" GroupName="find" AutoPostBack="true" ID="rbtShortest"
                                    Text="Shortest" runat="server" OnCheckedChanged="rbtShortest_CheckedChanged" />
                            </td>
                            <td>
                                <asp:RadioButton ID="rbtFastest" GroupName="find" Text="Fastest" AutoPostBack="true"
                                    runat="server" OnCheckedChanged="rbtFastest_CheckedChanged" />
                            </td>
                        </tr>
                    </table>
                </fieldset>
            </Description:DescriptionPanel>
        </ContentTemplate>
    </asp:UpdatePanel>
    </form>
</body>
</html>
