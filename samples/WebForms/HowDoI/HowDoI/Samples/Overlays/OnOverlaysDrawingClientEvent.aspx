<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="OnOverlaysDrawingClientEvent.aspx.cs"
    Inherits="HowDoI.Samples.Overlays.OnOverlaysDrawingClientEvent" %>

<%@ Register Assembly="ThinkGeo.MapSuite.WebForms" Namespace="ThinkGeo.MapSuite.WebForms" TagPrefix="cc1" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <link href="~/theme/default/samplepic/style.css" rel="stylesheet" type="text/css" />
    <title>OnOverlaysDrawingClientEvent</title>

    <script language="javascript" type="text/javascript">
        function OnOverlaysDrawing(layers) {
            for (var index = 0; index < layers.length; index++) {
                var templayer = layers[index];
                if (templayer.id == "Google Map") {
                    templayer.MIN_ZOOM_LEVEL = 10;
                    templayer.MAX_ZOOM_LEVEL = 18;
                }
            }
        }
    </script>

</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <cc1:Map ID="Map1" runat="server" Height="100%" Width="100%">
    </cc1:Map>
    <Description:DescriptionPanel ID="DescPanel" runat="server">
        This sample demonstrates how to reset the GoogleOverlay's PanZoomBar by client event
        'OnOverlaysDrawing'.
    </Description:DescriptionPanel>
    </form>
</body>
</html>
