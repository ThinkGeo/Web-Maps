﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DisplayASatelliteImage.aspx.cs"
    Inherits="HowDoI.DisplayASatelliteImage" %>

<%@ Register Assembly="ThinkGeo.MapSuite.WebForms" Namespace="ThinkGeo.MapSuite.WebForms" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link href="~/theme/default/samplepic/style.css" rel="stylesheet" type="text/css" />
    <title>Display a Satellite Image</title>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="Map1_Script" runat="server">
    </asp:ScriptManager>
    <cc1:Map ID="Map1" runat="server" Width="100%" Height="100%">
    </cc1:Map>
    <Description:DescriptionPanel ID="Description" runat="server">
    This sample simply adds an ECW layer to show a satellite image on the map.
    </Description:DescriptionPanel>
    </form>
</body>
</html>