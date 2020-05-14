<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DrawFeaturesBasedOnRegularExpression.aspx.cs"
    Inherits="HowDoI.DrawFeaturesBasedOnRegularExpression" %>

<%@ Register Assembly="ThinkGeo.MapSuite.WebForms" Namespace="ThinkGeo.MapSuite.WebForms" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">

    <script type="text/javascript" src="../Script/JScript.js"></script>

    <link href="~/theme/default/samplepic/style.css" rel="stylesheet" type="text/css" />
    <title>Draw features based on regular expression</title>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="Map1_Script" runat="server">
    </asp:ScriptManager>
    <cc1:Map ID="Map1" runat="server" Width="100%" Height="100%">
    </cc1:Map>
    <Description:DescriptionPanel ID="Description" runat="server">
        We used a regular expression to draw the countries differently that have the string
        “land” in them such as Greenland.
        <br />
        <br />
        <asp:Label ID="Label1" runat="server" Text="Regular Expression:" Font-Size="10" />
        <asp:TextBox ID="RegularExpressionTextBox" runat="server" Text=".*land" ReadOnly="true"
            Width="120"></asp:TextBox>
    </Description:DescriptionPanel>
    </form>
</body>
</html>
