<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UseMicrosoftMapsLayer.aspx.cs"
    Inherits="HowDoI.Samples.Extending_MapSuite.UseMicrosoftMapsLayer" %>

<%@ Register Assembly="ThinkGeo.MapSuite.WebForms" Namespace="ThinkGeo.MapSuite.WebForms" TagPrefix="cc1" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link href="~/theme/default/samplepic/style.css" rel="stylesheet" type="text/css" />
    <title>Use MicrosoftMapsLayer</title>
    <style type="text/css">
        .comment
        {
            width: 400px;
            height: 80px;
            left: 80px;
            top: 5px;
            padding: 5px;
            font-size: 10px;
            line-height: 20px;
            z-index: 999;
            position: absolute;
            background-color: #feffc7;
            border: solid 2px #cccccc;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:ScriptManager ID="ScriptManager1" runat="server">
        </asp:ScriptManager>
        <%--<cc1:Map ID="Map1" runat="server" Height="100%" Width="100%">
        </cc1:Map>--%>
        <asp:Image ID="Image1" runat="server" Height="100%" Width="100%" 
            ImageUrl="~/SampleData/World/BingMapsResult.png" />
        <Description:DescriptionPanel ID="DescPanel" runat="server">
            The sample demonstrates how to render Microsoft Layers using BingMapslayer.
            <br />
            <br />
        </Description:DescriptionPanel>
    </div>
    </form>
</body>
</html>
