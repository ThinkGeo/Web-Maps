<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UseGoogleBingWms.aspx.cs"
    Inherits="HowDoI.UseGoogleBingWms" %>

<%@ Register Assembly="ThinkGeo.MapSuite.WebForms" Namespace="ThinkGeo.MapSuite.WebForms" TagPrefix="cc1" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <link href="~/theme/default/samplepic/style.css" rel="stylesheet" type="text/css" />
    <title>Use Google/BingMaps etc...</title>
    <script type="text/javascript">
        function onLayerChanged(newLayerName) {
            var container = document.getElementById("changeSubTypeContainer");
            container.innerHTML = "";

            if (newLayerName == "Google Map") {
                appendButton(container, "Hybrid", google.maps.MapTypeId.HYBRID);
                appendButton(container, "Normal", google.maps.MapTypeId.ROADMAP);
                appendButton(container, "Satellite", google.maps.MapTypeId.SATELLITE);
                appendButton(container, "Physical", google.maps.MapTypeId.TERRAIN);
            }
            else if (newLayerName == "Bing Map") {
                appendButton(container, "Road", VEMapStyle.Road);
                appendButton(container, "Aerial", VEMapStyle.Aerial);
                appendButton(container, "Hybrid", VEMapStyle.Hybrid);
            }
        }

        function appendButton(container, btnName, mapType) {
            container.appendChild(getInputElement(btnName, mapType));
        }

        function getInputElement(name, mapType) {
            var btn = document.createElement("INPUT");
            btn.className = "btn";
            btn.type = "button";
            btn.value = name;
            btn.onclick = function () {
                Map1.SetCurrentBackgroundMapType(mapType);
            };

            return btn;
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="Map1_Script" runat="server">
    </asp:ScriptManager>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <cc1:Map ID="Map1" runat="server" Width="100%" Height="100%">
            </cc1:Map>
        </ContentTemplate>
    </asp:UpdatePanel>
    <Description:DescriptionPanel ID="DescPanel" runat="server">
        Switch between the different base overlays in the overlay switcher.
        <div id="changeSubTypeContainer">
        </div>
    </Description:DescriptionPanel>
    </form>
</body>
</html>
