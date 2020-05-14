<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MarkerWithContextMenu.aspx.cs"
    Inherits="HowDoI.Samples.Markers.MarkerWithContextMenu" %>

<%@ Register Assembly="ThinkGeo.MapSuite.WebForms" Namespace="ThinkGeo.MapSuite.WebForms" TagPrefix="cc1" %>
<html>
<head id="Head1" runat="server">
    <link href="~/theme/default/samplepic/style.css" rel="stylesheet" type="text/css" />
    <title>Marker with ContextMenu</title>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <cc1:Map ID="Map1" runat="server" Width="100%" Height="100%">
            </cc1:Map>
            <Description:DescriptionPanel ID="DescPanel" runat="server">
                Right click on the Marker to show the context menu and select an item.
                <br />
                <br />
                <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                    <ContentTemplate>
                        World-X :
                        <input id="Longitude" style="width: 120px;" runat="server" type="text" value="" disabled="disabled"
                            class="txt_normal" /><br />
                        World-Y :
                        <input id="Latitude" style="width: 120px;" runat="server" type="text" value="" disabled="disabled"
                            class="txt_normal" /></ContentTemplate>
                </asp:UpdatePanel>
            </Description:DescriptionPanel>
        </ContentTemplate>
    </asp:UpdatePanel>
    </form>
</body>
</html>
