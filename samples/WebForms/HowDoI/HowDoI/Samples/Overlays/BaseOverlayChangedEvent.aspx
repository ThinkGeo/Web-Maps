<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="BaseOverlayChangedEvent.aspx.cs"
    Inherits="HowDoI.Samples.Layers.BaseOverlayChangedEvent" %>

<%@ Register Assembly="ThinkGeo.MapSuite.WebForms" Namespace="ThinkGeo.MapSuite.WebForms" TagPrefix="cc1" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link href="~/theme/default/samplepic/style.css" rel="stylesheet" type="text/css" />
    <title>Add BaseOverlayChanged event to the map</title>
</head>
<body dir="ltr">
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server">
        </asp:ScriptManager>
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                <cc1:Map ID="Map1" runat="server" Height="100%" Width="100%" OnBaseOverlayChanged="Map1_BaseOverlayChanged">
                </cc1:Map>
            </ContentTemplate>
        </asp:UpdatePanel>
        <Description:DescriptionPanel ID="DescPanel" runat="server">
            Switch the base overlays in the overlay switcher panel to raise the BaseOverlayChanged event.
        <asp:UpdatePanel ID="UpdatePanel2" runat="server">
            <ContentTemplate>
                <asp:Panel ID="plControls" runat="server">
                    <asp:Button ID="btnRoad" CssClass="btn" runat="server" Text="Standard" OnClientClick="javascript: Map1.SetCurrentBackgroundMapType('base'); return false;" /><br />
                    <asp:Button ID="btnCycle" CssClass="btn" runat="server" Text="Cycle Map" OnClientClick="javascript: Map1.SetCurrentBackgroundMapType('cycle'); return false;" /><br />
                    <asp:Button ID="btnTransport" CssClass="btn" runat="server" Text="Transport Map" OnClientClick="Map1.SetCurrentBackgroundMapType('trans'); return false;" /><br />
                </asp:Panel>
            </ContentTemplate>
        </asp:UpdatePanel>
        </Description:DescriptionPanel>
    </form>
</body>
</html>
