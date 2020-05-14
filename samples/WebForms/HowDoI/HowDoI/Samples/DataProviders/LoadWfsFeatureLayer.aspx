<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LoadWfsFeatureLayer.aspx.cs"
    Inherits="HowDoI.Samples.DataProviders.LoadWfsFeatureLayer" %>

<%@ Register Assembly="ThinkGeo.MapSuite.WebForms" Namespace="ThinkGeo.MapSuite.WebForms" TagPrefix="cc1" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <link href="~/theme/default/samplepic/style.css" rel="stylesheet" type="text/css" />
    <title>Load wfs Features</title>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <cc1:Map ID="Map1" runat="server" Height="100%" Width="100%">
    </cc1:Map>
    <Description:DescriptionPanel ID="DescPanel" runat="server">
        This sample shows how to load wfs data from  and render them.
        <br />
    </Description:DescriptionPanel>
    </form>
</body>
</html>
