<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UseADifferentProjectionForAFeatureLayer.aspx.cs"
    Inherits="HowDoI.Samples.Projection.UseADifferentProjectionForAFeatureLayer" %>

<%@ Register Assembly="ThinkGeo.MapSuite.WebForms" Namespace="ThinkGeo.MapSuite.WebForms" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link href="~/theme/default/samplepic/style.css" rel="stylesheet" type="text/css" />
    <title>Use a different projection for a feature layer</title>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="Map1_Script" runat="server">
    </asp:ScriptManager>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <cc1:Map ID="Map1" runat="server" Width="100%" Height="100%">
            </cc1:Map>
        </ContentTemplate>
    </asp:UpdatePanel>
    <Description:DescriptionPanel ID="DescPanel" runat="server">
        This sample converts a layer's projection from EPSG:4326 to EPSG:2163.
        <br />
        <br />
        <asp:Button ID="btnZoomIn" runat="server" Width="100" Text="Zoom In" 
            OnClientClick="Map1.ZoomIn();return false;" />
        <asp:Button ID="btnZoomOut" runat="server" Width="100" Text="Zoom Out" 
            OnClientClick="Map1.ZoomOut();return false;" />
    </Description:DescriptionPanel>
    </form>
</body>
</html>
