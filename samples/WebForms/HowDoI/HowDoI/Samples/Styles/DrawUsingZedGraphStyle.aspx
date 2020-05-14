<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DrawUsingZedGraphStyle.aspx.cs"
    Inherits="HowDoI.Samples.Styles.DrawUsingZedGraphStyle" %>

<%@ Register Assembly="ThinkGeo.MapSuite.WebForms" Namespace="ThinkGeo.MapSuite.WebForms" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link href="../../theme/default/samplepic/style.css" rel="stylesheet" type="text/css" />
    <title>Draw Using ZedGraphStyle</title>
    <style type="text/css">
        .center
        {
            text-align: center;
        }
        .comment
        {
            width: 400px;
            height: 60px;
            left: 5px;
            top: 5px;
            padding: 5px;
            font-size: 10px;
            line-height: 20px;
            z-index: 999;
            position: absolute;
            background-color: #feffc7;
            border: solid 2px #cccccc;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="Map1_Script" runat="server">
    </asp:ScriptManager>
    <cc1:Map ID="Map1" runat="server" Height="100%" Width="100%">
    </cc1:Map>
    <Description:DescriptionPanel ID="DescPanel" runat="server">
        We used zedgraph pie chart to represent the ratio of white to asian of the major
        US cities.
        <br />
        <br />
        <table border="0">
            <tr>
                <td>
                    <asp:Panel ID="Panel1" runat="server" Width="60" Height="18" BorderStyle="Dotted"
                        BorderWidth="1px" BorderColor="Gray" BackColor="LightBlue" CssClass="center">
                        White
                    </asp:Panel>
                </td>
                <td>
                    <asp:Panel ID="Panel2" runat="server" Width="60" Height="18" BorderStyle="Dotted"
                        BorderWidth="1px" BorderColor="Gray" BackColor="LawnGreen" CssClass="center">
                        Asian
                    </asp:Panel>
                </td>
            </tr>
        </table>
    </Description:DescriptionPanel>
    </form>
</body>
</html>
