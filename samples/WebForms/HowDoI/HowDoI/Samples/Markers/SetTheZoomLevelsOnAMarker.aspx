﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SetTheZoomLevelsOnAMarker.aspx.cs"
    Inherits="HowDoI.SetTheZoomLevelsOnAMarker" %>

<%@ Register Assembly="ThinkGeo.MapSuite.WebForms" Namespace="ThinkGeo.MapSuite.WebForms" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link href="~/theme/default/samplepic/style.css" rel="stylesheet" type="text/css" />
    <title>Set the Zoom Levels on a Marker</title>
    <style type="text/css">
        .popCss
        {
            font-family: verdana;
            font-size: 10px;
            background-color: Transparent;
            border: none 0px #ffffff;
            color: black;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="Map1_Script" runat="server">
    </asp:ScriptManager>
    <cc1:Map ID="Map1" runat="server" Width="100%" Height="100%">
    </cc1:Map>
    <Description:DescriptionPanel ID="DescPanel" runat="server">
        Zoom in to see the changes of the markers.
    </Description:DescriptionPanel>
    </form>
</body>
</html>