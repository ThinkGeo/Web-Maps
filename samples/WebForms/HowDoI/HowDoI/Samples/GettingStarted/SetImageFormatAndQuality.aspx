<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SetImageFormatAndQuality.aspx.cs"
    Inherits="HowDoI.Samples.SetImageFormatAndQuality" %>

<%@ Register Assembly="ThinkGeo.MapSuite.WebForms" Namespace="ThinkGeo.MapSuite.WebForms" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link href="~/theme/default/samplepic/style.css" rel="stylesheet" type="text/css" />
    <title>Set Image Type And Quality</title>
</head>
<body>
    <form id="form1" runat="server"> 
    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
    <cc1:Map ID="Map1" runat="server" Width="100%" Height="100%">
    </cc1:Map>
    <Description:DescriptionPanel ID="DescPanel" runat="server">
        This sample simply sets the format and quality of the map images.
        <br/>
        <br/>
        
        Select image format:<br/>
        <asp:DropDownList runat="server" ID="ddImageFormat" Width="60px" Font-Size="10px" AutoPostBack="true" Font-Names="verdana" OnSelectedIndexChanged="ddImageFormat_Changed">
            <asp:ListItem Selected="true" Text="Png" Value="Png"></asp:ListItem>
            <asp:ListItem Text="Jpeg" Value="Jpeg"></asp:ListItem>
        </asp:DropDownList>
        <asp:DropDownList Visible="false" runat="server" ID="ddJpegQuality" Width="60px" Font-Size="10px" Font-Names="verdana" AutoPostBack="true" OnSelectedIndexChanged="ddJpegFormat_Changed">
            <asp:ListItem Text="100" Value="100" Selected="True"></asp:ListItem>
            <asp:ListItem Text="90" Value="90"></asp:ListItem>
            <asp:ListItem Text="80" Value="80"></asp:ListItem>
            <asp:ListItem Text="70" Value="70"></asp:ListItem>
            <asp:ListItem Text="60" Value="60"></asp:ListItem>
            <asp:ListItem Text="50" Value="50"></asp:ListItem>
            <asp:ListItem Text="40" Value="40"></asp:ListItem>
            <asp:ListItem Text="30" Value="30"></asp:ListItem>
            <asp:ListItem Text="20" Value="20"></asp:ListItem>
            <asp:ListItem Text="10" Value="10"></asp:ListItem>
        </asp:DropDownList>
    </Description:DescriptionPanel>
    </form>
</body>
</html>
