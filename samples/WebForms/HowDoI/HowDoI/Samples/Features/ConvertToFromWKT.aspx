﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ConvertToFromWKT.aspx.cs"
    Inherits="HowDoI.Samples.Features.ConvertToFromWKT" %>

<%@ Register Assembly="ThinkGeo.MapSuite.WebForms" Namespace="ThinkGeo.MapSuite.WebForms" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">

    <script type="text/javascript" src="../Script/JScript.js"></script>

    <link href="~/theme/default/samplepic/style.css" rel="stylesheet" type="text/css" />
    <title>Convert To/From Well Known Text</title>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="Map1_Script" runat="server">
    </asp:ScriptManager>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <cc1:Map ID="Map1" runat="server" Width="100%" Height="100%">
            </cc1:Map>
            <Description:DescriptionPanel ID="DescPanel" runat="server">
                Click on the button to convert the shape to and from well known Text.
                <br />
                <br />
                <asp:TextBox ID="txtWKT" runat="server" TextMode="MultiLine" Rows="5" Width="96%"
                    ReadOnly="True" Text="POLYGON((-111 34,-111 46,-89 46,-89 34,-111 34))" />
                <br />
                <asp:Button ID="btnConvert" runat="server"  Text="WKT  to  Feature"
                    Width="96%" OnClick="btnConvert_Click" />
            </Description:DescriptionPanel>
        </ContentTemplate>
    </asp:UpdatePanel>
    </form>
</body>
</html>