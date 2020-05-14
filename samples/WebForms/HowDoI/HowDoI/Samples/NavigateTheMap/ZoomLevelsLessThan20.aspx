<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ZoomLevelsLessThan20.aspx.cs" Inherits="HowDoI.Samples.NavigateTheMap.ZoomLevelsLessThan20" %>

<%@ Register Assembly="ThinkGeo.MapSuite.WebForms" Namespace="ThinkGeo.MapSuite.WebForms" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link href="~/theme/default/samplepic/style.css" rel="stylesheet" type="text/css" />
    <title>Zoom Levels less than 20</title>
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
    <cc1:Map ID="Map1" runat="server" Width="100%" Height="100%">
    </cc1:Map>
    <Description:DescriptionPanel ID="DescPanel" runat="server">
        We keep only 5 zoomlevels in the default 20 zoomlevels.
    </Description:DescriptionPanel>
    </form>
</body>
</html>
