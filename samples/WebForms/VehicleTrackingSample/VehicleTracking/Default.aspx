<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="ThinkGeo.MapSuite.VehicleTracking.VehicleTracking" %>

<%@ Register Assembly="ThinkGeo.MapSuite.WebForms" Namespace="ThinkGeo.MapSuite.WebForms" TagPrefix="cc1" %>
<!DOCTYPE html>
<html xml:lang="en-us">
<head>
    <title>Vehicle Tracking</title>
    <meta charset="UTF-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <meta http-equiv="X-UA-Compatible" content="IE=Edge" />
    <link href="Images/MapSuite.ico" rel="shortcut icon" type="image/x-icon" />
    <link href="Styles/jquery-ui-1.10.4.custom.min.css" rel="stylesheet" type="text/css" />
    <link href="Styles/Site.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript" src="Scripts/modernizr-2.5.3.js"></script>
    <script src="Scripts/jquery-1.10.2.js" type="text/javascript"></script>
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server">
        </asp:ScriptManager>
        <div id="container">
            <div id="header">
                <div id="left-header">
                    <span id="header-mapsuite">Map Suite</span> <span id="header-title">Vehicle Tracking</span>
                </div>
            </div>
            <div id="content-container">
                <div id="leftContainer">
                    <div id="leftContent">
                        <h4>Click to refresh vehicles:</h4>
                        <div id="divRefresh" class="divBorder">
                            <div class="divSettingItem">
                                <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <asp:ImageButton ID="ibtnAutoRefreshButton" ToolTip="Auto Refresh" runat="server" ImageUrl="~/Images/autorefresh.png" OnClientClick="changeAutoRefreshButton(this);return true;"
                                            CssClass="imgButton" OnClick="ibtnAutoRefresh_Click" />
                                        <span class="title">Auto Refresh:</span>
                                        <asp:Label ID="lblStatus" runat="server" CssClass="title redTxt" Text="On"></asp:Label>
                                        <asp:Timer ID="tmAutoRefreshTimer" runat="server" Interval="5000" OnTick="tmAutoRefreshTimer_Tick"
                                            Enabled="true" />
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </div>
                            <div class="divSettingItem">
                                <asp:ImageButton ID="ibtnRefreshManually" ToolTip="Refresh" runat="server" OnClick="ibtnRefreshManually_Click"
                                    ImageUrl="~/Images/RefreshManually.png" CssClass="imgButton" />
                                <span class="title">Refresh Manually</span>
                            </div>
                        </div>
                        <h4>Interact with the map using these tools:
                        </h4>
                        <div id="divTrackMode" class="divBorder">
                            <asp:ImageButton ID="btnPan" ToolTip="Pan map" AlternateText="Pan" runat="server" class="active imgButton"
                                ImageUrl="~/Images/pan.png" />
                            <asp:ImageButton ID="btnDraw" ToolTip="Draw fences" AlternateText="Draw" runat="server" class="imgButton"
                                ImageUrl="~/Images/draw.png" />
                            <asp:ImageButton ID="btnMeasure" ToolTip="Measure distance" AlternateText="Measure" runat="server" class="imgButton"
                                ImageUrl="~/Images/measure.png" />
                        </div>
                        <div id="divTrackType" class="divSubBorder">
                            <asp:ImageButton ID="ibtnDrawPolygon" ToolTip="Track new fences" runat="server" class="imgButton" OnClick="ibtnDrawPolygon_Click"
                                ImageUrl="~/Images/draw_polygon.png" />
                            <asp:ImageButton ID="ibtnEditSpatialFences" ToolTip="Edit the selected fence" runat="server" class="imgButton" OnClick="ibtnEditSpatialFences_Click"
                                ImageUrl="~/Images/edit_polygon.png" />
                            <asp:ImageButton ID="ibtnDeleteSpatialFences" ToolTip="Delete" runat="server" class="imgButton" OnClick="ibtnDeleteSpatialFences_Click"
                                ImageUrl="~/Images/Remove_Polygon.png" />
                            <asp:ImageButton ID="ibtnSaveSpatialFences" ToolTip="Save" runat="server" class="imgButton" OnClick="ibtnSaveSpatialFences_Click"
                                ImageUrl="~/Images/save.png" />
                            <asp:ImageButton ID="ibtnCancelSpatialFences" ToolTip="Cancel" runat="server" class="imgButton" OnClick="ibtnCancelEditSpatialFences_Click"
                                ImageUrl="~/Images/clear.png" />
                            <!--Add a hide button to avoid the async Confirm dialog -->
                            <asp:Button ID="btnDeleteSpatialFence" runat="server" Text="Remove Fences" Style="display: none;" OnClick="ibtnDeleteSpatialFences_Click" />
                        </div>
                        <div id="divMeasure" class="divSubBorder">
                            <asp:ImageButton ID="ibtnMeasureLength" ToolTip="Measure line" runat="server" class="imgButton" ImageUrl="~/Images/line.png"
                                OnClick="ibtnMeasureLength_Click" />
                            <asp:ImageButton ID="ibtnMeasureArea" ToolTip="Measure polygon" runat="server" class="imgButton" ImageUrl="~/Images/Polygon.png"
                                OnClick="ibtnMeasureArea_Click" />
                            <asp:ImageButton ID="ibtnClearMeasure" ToolTip="Cancel" runat="server" class="imgButton" ImageUrl="~/Images/clear.png"
                                OnClick="ibtnClearMeasure_Click" />
                            <span class="title">Measure Unit:</span>
                            <asp:DropDownList ID="ddlMeasureUnit" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlMeasureUnit_SelectedIndexChanged">
                                <asp:ListItem Value="Metric">Metric</asp:ListItem>
                                <asp:ListItem Value="Imperial">Imperial</asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <div class="blueBanner">
                            Tracked Vehicles
                        </div>
                        <div class="divBorder" id="divVehicleList">
                            <asp:UpdatePanel ID="UpdatePanel3" runat="server" UpdateMode="Always">
                                <ContentTemplate>
                                    <asp:Repeater ID="rptVehicles" runat="server">
                                        <HeaderTemplate>
                                            <table id="tbVehiclelist">
                                        </HeaderTemplate>
                                        <ItemTemplate>
                                            <tr>
                                                <td rowspan="5" class="vehicleImg">
                                                    <img id="imgVehicle" alt="<%# Eval("Name")%>" onclick="zoomInToVehicle(<%# Eval("Longitude")%>,<%# Eval("Latitude")%>)"
                                                        src='<%# Eval("IconPath") %>' />
                                                </td>
                                                <td colspan="2" class="vehicleName">
                                                    <a href="#" id="btnZoomTo" onclick="zoomInToVehicle(<%# Eval("Longitude")%>,<%# Eval("Latitude")%>)">
                                                        <%# Eval("Name")%></a>
                                                </td>
                                            </tr>
                                            <tr class="vehicleTxt">
                                                <td colspan="2">
                                                    <img alt="ball" src='<%# Eval("MotionStateIconVirtualPath") %>' />
                                                    <span id="motionStatus" class="greenTxt">
                                                        <%# (Eval("MotionState").ToString() == "Motion") ? "In Motion" : "Idle"%>
                                                    </span>
                                                </td>
                                            </tr>
                                            <tr class="vehicleTxt">
                                                <td>Area:
                                                </td>
                                                <td id="isInFence">
                                                    <%# ((bool)Eval("IsInFence")) ? "<span class='redTxt'>In Restrict Area</span>" : "Out Of Restrict Area"%>
                                                </td>
                                            </tr>
                                            <tr class="vehicleTxt">
                                                <td>Speed:
                                                </td>
                                                <td>
                                                    <%# DataBinder.Eval(Container.DataItem, "Location.Speed") + " mph"%>
                                                </td>
                                            </tr>
                                            <tr class="vehicleTxt">
                                                <td>Duration:
                                                </td>
                                                <td>
                                                    <%# Eval("SpeedDuration") + " min"%>
                                                </td>
                                            </tr>
                                            <tr class="vehicleTxt">
                                                <td colspan="3">&nbsp;
                                                </td>
                                            </tr>
                                        </ItemTemplate>
                                        <FooterTemplate>
                                            </table>
                                        </FooterTemplate>
                                    </asp:Repeater>
                                </ContentTemplate>
                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="tmAutoRefreshTimer" EventName="Tick" />
                                </Triggers>
                            </asp:UpdatePanel>
                        </div>
                    </div>
                </div>
                <div id="toggle">
                    <img alt="collapse" src="Images/collapse.gif" />
                </div>
                <div id="map-content">
                    <asp:UpdatePanel ID="UpdatePanel2" style="width: 100%; height: 100%;" runat="server"
                        UpdateMode="Conditional">
                        <ContentTemplate>
                            <cc1:Map ID="Map1" runat="server" Height="100%" Width="100%" Visible="true">
                            </cc1:Map>
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="tmAutoRefreshTimer" EventName="Tick" />
                            <asp:AsyncPostBackTrigger ControlID="ibtnRefreshManually" />
                            <asp:AsyncPostBackTrigger ControlID="ibtnDrawPolygon" />
                            <asp:AsyncPostBackTrigger ControlID="ibtnSaveSpatialFences" />
                            <asp:AsyncPostBackTrigger ControlID="ibtnEditSpatialFences" />
                            <asp:AsyncPostBackTrigger ControlID="ibtnDeleteSpatialFences" />
                            <asp:AsyncPostBackTrigger ControlID="btnDeleteSpatialFence" />
                            <asp:AsyncPostBackTrigger ControlID="ibtnCancelSpatialFences" />
                            <asp:AsyncPostBackTrigger ControlID="ibtnMeasureLength" />
                            <asp:AsyncPostBackTrigger ControlID="ibtnMeasureArea" />
                            <asp:AsyncPostBackTrigger ControlID="ibtnClearMeasure" />
                            <asp:AsyncPostBackTrigger ControlID="ddlMeasureUnit" EventName="SelectedIndexChanged" />
                        </Triggers>
                    </asp:UpdatePanel>
                    <div id="divRemoveDialog">
                        Are you sure you want to delete the spatial fence you have selected?
                    </div>
                    <div id="divMessage">
                        Please choose a fence at first.
                    </div>
                </div>
            </div>
            <div id="footer">
                <span id="coordinate"><span id="spanMouseCoordinate"></span></span>
            </div>
        </div>
    </form>
    <script type="text/javascript" src="Scripts/jquery-ui-1.10.4.custom.min.js"></script>
    <script type="text/javascript" src="Scripts/ready-functions.js"></script>
</body>
</html>