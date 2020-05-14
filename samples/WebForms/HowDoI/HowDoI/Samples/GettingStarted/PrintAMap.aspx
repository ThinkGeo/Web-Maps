<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PrintAMap.aspx.cs" Inherits="HowDoI.Samples.PrintAMap" %>

<%@ Register Assembly="ThinkGeo.MapSuite.WebForms" Namespace="ThinkGeo.MapSuite.WebForms" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <link href="~/theme/default/samplepic/style.css" rel="stylesheet" type="text/css" />
    <title>Print a Map</title>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="Map1_Script" runat="server">
    </asp:ScriptManager>
    <cc1:Map ID="Map1" runat="server" Width="100%" Height="100%">
    </cc1:Map>
    <Description:DescriptionPanel ID="DescPanel" runat="server">
        Click the icon to print the current map.
        <br />
        <br />
        <img alt="Print map" id="print" onclick="Map1.PrintMap();" src="../../theme/default/samplepic/Print Map.png"
            class="imgToolbar" />
    </Description:DescriptionPanel>
    </form>
</body>
</html>
