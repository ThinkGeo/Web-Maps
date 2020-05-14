<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ZoomToAClickedFeature.aspx.cs"
    Inherits="HowDoI.Samples.NavigateTheMap.ZoomToAClickedFeature" %>

<%@ Register Assembly="ThinkGeo.MapSuite.WebForms" Namespace="ThinkGeo.MapSuite.WebForms" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link href="~/theme/default/samplepic/style.css" rel="stylesheet" type="text/css" />
    <title>Zoom to a feature you clicked</title>
    <style type="text/css">
        html
        {
            height: 100%;
        }
    </style>
</head>
<body style="height: 100%;">
    <form id="form1" runat="server" style="height: 100%;">
    <asp:ScriptManager ID="Map1_Script" runat="server">
    </asp:ScriptManager>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <cc1:Map ID="Map1" runat="server" Width="100%" Height="100%" OnClick="Map1_Click">
            </cc1:Map>
        </ContentTemplate>
    </asp:UpdatePanel>
    <Description:DescriptionPanel ID="DescPanel" runat="server">
        Click on a country to zoom to it.
    </Description:DescriptionPanel>
    </form>
</body>
</html>
