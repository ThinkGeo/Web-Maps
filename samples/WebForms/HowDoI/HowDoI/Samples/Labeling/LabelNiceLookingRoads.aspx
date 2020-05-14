<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LabelNiceLookingRoads.aspx.cs"
    Inherits="HowDoI.Samples.Labeling.LabelNiceLookingRoads" %>

<%@ Register Assembly="ThinkGeo.MapSuite.WebForms" Namespace="ThinkGeo.MapSuite.WebForms" TagPrefix="cc1" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link href="~/theme/default/samplepic/style.css" rel="stylesheet" type="text/css" />
    <title>Label Nice Looking Roads</title>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="Map1_Script" runat="server">
    </asp:ScriptManager>
    <cc1:Map ID="Map1" runat="server" Width="100%" Height="100%">
    </cc1:Map>
    <Description:DescriptionPanel ID="DescPanel" runat="server">
        We have drawn and labeled these roads using a pleasing style.
    </Description:DescriptionPanel>
    </form>
</body>
</html>
