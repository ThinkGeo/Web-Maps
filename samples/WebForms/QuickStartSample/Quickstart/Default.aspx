<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Quickstart.WebForm1" %>

<%@ Register Assembly="ThinkGeo.MapSuite.WebForms" Namespace="ThinkGeo.MapSuite.WebForms" TagPrefix="cc1" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml" style="width: 100%; height: 100%">
<head runat="server">
    <title>Hello World</title>
</head>
<body style="width: 100%; height: 100%">
    <form id="form1" runat="server" style="width: 100%; height: 100%">
        <div style="width: 100%; height: 100%">
            <asp:ScriptManager ID="ScriptManager1" runat="server">
            </asp:ScriptManager>
            <cc1:Map ID="Map1" runat="server" Height="100%" Width="100%">
            </cc1:Map>
        </div>
    </form>
</body>
</html>
