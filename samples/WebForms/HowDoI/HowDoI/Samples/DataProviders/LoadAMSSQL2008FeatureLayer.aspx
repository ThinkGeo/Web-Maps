<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LoadAMSSQL2008FeatureLayer.aspx.cs"
    Inherits="HowDoI.Samples.DataProviders.LoadAMSSQL2008FeatureLayer" %>

<%@ Register Assembly="ThinkGeo.MapSuite.WebForms" Namespace="ThinkGeo.MapSuite.WebForms" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <link href="~/theme/default/samplepic/style.css" rel="stylesheet" type="text/css" />
    <title>Load a MSSQL2008 FeatureLayer</title>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <cc1:Map ID="Map1" runat="server" Width="100%" Height="100%" Visible="false">
    </cc1:Map>
    <div class="comment">
        Please modify the visible property of the map in .aspx file if you load the map from MsSQL2008
        using following code.
    </div>
    <div id="MapView" style="height: 100%; z-index: 9999" runat="server">
        <img alt="map" width="99%" height="98%" src="../../theme/default/samplepic/MapView.jpg" />
    </div>
    <Description:DescriptionPanel ID="DescPanel" runat="server">
        This sample specifies how to work with MSSQL2008FeatureLayer.
    </Description:DescriptionPanel>
    </form>
</body>
</html>
