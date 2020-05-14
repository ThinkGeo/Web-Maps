<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DrawThematicFeatures.aspx.cs"
    Inherits="HowDoI.DrawThematicFeatures" %>

<%@ Register Assembly="ThinkGeo.MapSuite.WebForms" Namespace="ThinkGeo.MapSuite.WebForms" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <link href="~/theme/default/samplepic/style.css" rel="stylesheet" type="text/css" />
    <title>Draw Thematic Features</title>
    <style>
        .legend
        {
            height: 10px;
            width: 10px;
            border: #000000 1px solid;
            font-size: 10px;
        }
        td
        {
            font-size: 10px;
            font-family: Verdana;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="Map1_Script" runat="server">
    </asp:ScriptManager>
    <cc1:Map ID="Map1" runat="server" Width="100%" Height="100%">
    </cc1:Map>
    <Description:DescriptionPanel ID="Description" runat="server">
        We have broken down the population of each country into ranges and then used different
        colors to represent the ranges.
        <br />
        <br />
        <b>Population of contries</b>
        <br />
        <table cellpadding="4px" cellspacing="0px" border="0px">
            <tr>
                <td>
                    <input type="button" class="legend" style="background-color: #d8dd8c;" />
                </td>
                <td>
                    Less than 1,000,000
                </td>
            </tr>
            <tr>
                <td>
                    <input type="button" class="legend" style="background-color: #90EE90;" />
                </td>
                <td>
                    1,000,000 ~ 10,000,000
                </td>
            </tr>
            <tr>
                <td>
                    <input type="button" class="legend" style="background-color: #9ACD32;" />
                </td>
                <td>
                    10,000,000 ~ 50,000,000
                </td>
            </tr>
            <tr>
                <td>
                    <input type="button" class="legend" style="background-color: LightGreen;" />
                </td>
                <td>
                    50,000,000 ~ 100,000,000
                </td>
            </tr>
            <tr>
                <td>
                    <input type="button" class="legend" style="background-color: DarkGreen;" />
                </td>
                <td>
                    More than 100,000,000
                </td>
            </tr>
        </table>
    </Description:DescriptionPanel>
    </form>
</body>
</html>
