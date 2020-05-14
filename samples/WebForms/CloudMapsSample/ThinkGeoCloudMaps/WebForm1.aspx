<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WebForm1.aspx.cs" Inherits="ThinkGeoCloudMapsSample.WebForm1" %>

<%@ Register Assembly="ThinkGeo.MapSuite.WebForms" Namespace="ThinkGeo.MapSuite.WebForms" TagPrefix="cc1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml" style="height: 100%">
<head runat="server">
    <title>ThinkGeo Cloud Maps</title>
</head>
<body style="height: 100%; margin: 0px; padding: 0px">
    <form id="form1" runat="server" style="height: 100%">
        <div style="height: 100%">
            <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
            <cc1:Map ID="Map1" runat="server" Height="100%" Width="100%"></cc1:Map>
        </div>
    </form>
</body>
</html>
