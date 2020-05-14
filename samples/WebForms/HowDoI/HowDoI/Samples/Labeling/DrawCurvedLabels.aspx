<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DrawCurvedLabels.aspx.cs"
    Inherits="HowDoI.Samples.Labeling.DrawCurvedLabels" %>

<%@ Register Assembly="ThinkGeo.MapSuite.WebForms" Namespace="ThinkGeo.MapSuite.WebForms" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <link href="~/theme/default/samplepic/style.css" rel="stylesheet" type="text/css" />
    <title>Curved rendering</title>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <cc1:Map ID="Map1" runat="server" Height="100%" Width="100%">
    </cc1:Map>
    <Description:DescriptionPanel ID="DescPanel" runat="server">
        The sample shows how to render labels along the street.
    </Description:DescriptionPanel>
    </form>
</body>
</html>
