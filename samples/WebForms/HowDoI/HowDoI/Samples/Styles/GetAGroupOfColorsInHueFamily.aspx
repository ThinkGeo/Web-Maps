<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GetAGroupOfColorsInHueFamily.aspx.cs"
    Inherits="HowDoI.Samples.Styles.GetAGroupOfColorsInHueFamily" %>

<%@ Register Assembly="ThinkGeo.MapSuite.WebForms" Namespace="ThinkGeo.MapSuite.WebForms" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link href="../../theme/default/samplepic/style.css" rel="stylesheet" type="text/css" />
    <title>Get a group of color in the same quality family</title>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="Map1_Script" runat="server">
    </asp:ScriptManager>
    <cc1:Map ID="Map1" runat="server" Height="100%" Width="100%">
    </cc1:Map>
    <Description:DescriptionPanel ID="Description" runat="server">
        We used a group of colors in red to draw five different countries.
        <br />
        <br />
    </Description:DescriptionPanel>
    </form>
</body>
</html>
