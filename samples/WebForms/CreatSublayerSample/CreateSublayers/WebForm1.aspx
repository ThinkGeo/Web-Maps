<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WebForm1.aspx.cs" Inherits="CreateSublayers.WebForm1" %>

<%@ Register Assembly="ThinkGeo.MapSuite.WebForms" Namespace="ThinkGeo.MapSuite.WebForms" TagPrefix="cc1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="~/Styles/Site.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript" src="Scripts/jquery-1.11.1.min.js"></script>
    <script type="text/javascript" src="Scripts/thinkgeo.js"></script>
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
        <asp:UpdatePanel ID="updatePanel" runat="server">
            <ContentTemplate>
                <div id="mapContent">
                    <cc1:Map ID="Map1" runat="server" Height="100%" Width="100%"></cc1:Map>

                </div>
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="population1" EventName="CheckedChanged" />
                <asp:AsyncPostBackTrigger ControlID="population2" EventName="CheckedChanged" />
                <asp:AsyncPostBackTrigger ControlID="population3" EventName="CheckedChanged" />
            </Triggers>
        </asp:UpdatePanel>
        <div id="layerSwitcher">
            <div id="switcherController" class="expand"></div>
            <div id="switcherContent">
                US census sublayers:
                            <br />
                <asp:CheckBox ID="population1" runat="server" OnCheckedChanged="SelectedLayers" Text="Population < 1000000" Checked="true" AutoPostBack="true" />
                <br />
                <asp:CheckBox ID="population2" runat="server" OnCheckedChanged="SelectedLayers" Text="1000000<=Population<10000000" Checked="true" AutoPostBack="true" />
                <br />
                <asp:CheckBox ID="population3" runat="server" OnCheckedChanged="SelectedLayers" Text="10000000<=Population" Checked="true" AutoPostBack="true" />
                <br />
            </div>
        </div>
    </form>
</body>
</html>
