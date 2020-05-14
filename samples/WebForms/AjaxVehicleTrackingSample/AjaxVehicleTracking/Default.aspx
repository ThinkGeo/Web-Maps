<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="ThinkGeo.MapSuite.AjaxVehicleTracking.Default" %>

<%@ Register Assembly="ThinkGeo.MapSuite.WebForms" Namespace="ThinkGeo.MapSuite.WebForms" TagPrefix="cc1" %>
<!DOCTYPE html>
<html xml:lang="en-us">
<head id="Head1" runat="server">
    <title>Ajax Vehicle Tracking</title>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <meta http-equiv="X-UA-Compatible" content="IE=Edge">
    <link href="Images/MapSuite.ico" rel="shortcut icon" type="Images/x-icon" />
    <link href="Styles/jquery-ui-1.10.4.custom.min.css" rel="stylesheet" type="text/css" />
    <link href="Styles/Site.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript" src="Scripts/modernizr-2.5.3.js"></script>
</head>
<body>
    <form id="Form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <div id="container">
        <div id="header">
            <div id="left-header">
                <span id="header-mapsuite">Map Suite</span> <span id="header-title">Ajax Vehicle Tracking</span>
            </div>
        </div>
        <div id="content-container">
            <div id="leftContainer">
                <div id="leftContent">
                    <h4>
                        Click to refresh vehicles:</h4>
                    <div id="divRefresh" class="divBorder">
                        <div class="divSettingItem">
                            <img id="btnAutoRefresh" alt="Auto Refresh" title="Auto refresh" src="Images/AutoRefresh.png" class="imgButton" />
                            <span class="title">Auto Refresh:</span> <span id="autoRefreshStatus" class="title redTxt">
                                On</span>
                        </div>
                        <div class="divSettingItem">
                            <img id="btnRefresh" alt="Refresh Manually" title="Refresh" src="Images/RefreshManually.png" class="imgButton" />
                            <span class="title">Refresh Manually</span>
                        </div>
                    </div>
                    <h4>
                        Interact with the map using these tools:
                    </h4>
                    <div id="divTrackMode" class="divBorder">
                        <img id="btnPan" alt="Pan" title="Pan map" src="Images/pan.png" class="active imgButton" />
                        <img id="btnDraw" alt="Draw" title="Draw fences" src="Images/draw.png" class="imgButton" />
                        <img id="btnMeasure" alt="Measure" title="Measure distance" src="Images/measure.png" class="imgButton" />
                    </div>
                    <div id="divTrackType" class="divSubBorder">
                        <img id="btnDrawPolygon" alt="Draw" title="Track new fences" src="Images/draw_polygon.png" class="active imgButton" />
                        <img id="btnEditPolygon" alt="Edit" title="Edit the selected fence" src="Images/edit_polygon.png" class="imgButton" />
                        <img id="btnRemovePolygon" alt="Remove" title="Delete" src="Images/Remove_Polygon.png" class="imgButton" />
                        <img id="btnSave" alt="Save" title="Save" src="Images/save.png" class="imgButton" />
                        <img id="btnClearShapes" title="Cancel" alt="Cancel" src="Images/clear.png" class="imgButton" />
                    </div>
                    <div id="divMeasure" class="divSubBorder">
                        <img id="btnMeasureLength" alt="Distance" title="Measure line" src="Images/line.png" class="active imgButton" />
                        <img id="btnMeasureArea" alt="Area" title="Measure polygon" src="Images/Polygon.png" class="imgButton" />
                        <img id="btnClearMeasure" alt="Cancel" title="Cancel" src="Images/clear.png" class="imgButton" />
                        <span class="title">Measure Unit:</span>
                        <select>
                            <option value="Metric">Metric</option>
                            <option value="Imperial">Imperial</option>
                        </select>
                        <input id="mapMeasureMode" type="hidden" value="Area" />
                    </div>
                    <div class="blueBanner">
                        Tracked Vehicles
                    </div>
                    <div class="divBorder" id="divVehicleList">
                    </div>
                </div>
            </div>
            <div id="toggle">
                <img id="collapse" src="Images/collapse.gif" />
            </div>
            <div id="map-content">
                <cc1:Map ID="Map1" runat="server" Height="100%" Width="100%">
                </cc1:Map>
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
        <script src="Scripts/jquery-1.10.2.js" type="text/javascript"></script>
        <script src="Scripts/jquery-ui-1.10.4.custom.min.js" type="text/javascript"></script>
        <script type="text/javascript" src="Scripts/ready-functions.js"></script>
        <script type="text/javascript" src="Scripts/VehiclesPanelHelper.js"></script>
    </div>
    </form>
</body>
</html>
