<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="StopCertainFeaturesFromDrawing.aspx.cs"
    Inherits="HowDoI.Samples.FeatureLayers.StopCertainFeaturesFromDrawing" %>

<%@ Register Assembly="ThinkGeo.MapSuite.WebForms" Namespace="ThinkGeo.MapSuite.WebForms" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link href="~/theme/default/samplepic/style.css" rel="stylesheet" type="text/css" />
    <title>Stop certain features from drawing</title>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="Map1_Script" runat="server">
    </asp:ScriptManager>
    <cc1:Map ID="Map1" runat="server" Width="100%" Height="100%">
    </cc1:Map>
    <Description:DescriptionPanel ID="Discription" runat="server">
        The example demonstrates how to stop certain features from drawing.
    </Description:DescriptionPanel>
    </form>
</body>
</html>
