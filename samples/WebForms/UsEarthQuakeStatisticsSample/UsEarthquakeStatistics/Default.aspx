<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="ThinkGeo.MapSuite.EarthquakeStatistics._Default" %>

<%@ Register Assembly="ThinkGeo.MapSuite.WebForms" Namespace="ThinkGeo.MapSuite.WebForms" TagPrefix="cc1" %>
<!DOCTYPE html>
<html xml:lang="en-us">
<head>
    <title>US Earthquake Statistic</title>
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <meta http-equiv="X-UA-Compatible" content="IE=Edge" />
    <link href="Images/MapSuite.ico" rel="shortcut icon" type="Images/x-icon" />
    <link href="Styles/jquery-ui-1.10.4.custom.min.css" rel="stylesheet" type="text/css" />
    <link href="Styles/Site.css" rel="stylesheet" type="text/css" />
    <script src="Scripts/jquery-1.10.2.js" type="text/javascript"></script>
    <script src="Scripts/jquery-ui-1.10.4.custom.min.js" type="text/javascript"></script>
    <script src="Scripts/ready-functions.js" type="text/javascript"></script>
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server">
        </asp:ScriptManager>
        <div id="container">
            <div id="header">
                <div id="left-header">
                    <span id="header-mapsuite">Map Suite</span> <span id="header-title">US Earthquake Statistic</span>
                </div>
            </div>
            <div id="content-container">
                <div id="leftContainer">
                    <div id="leftContent">
                        <h4>Display Type:</h4>
                        <div id="divBasemaps">
                            <asp:ImageButton ID="btnHeatMap" runat="server" ToolTip="show the heat map" AlternateText="Heat Map"
                                ImageUrl="Images/heatMap.png" />
                            <asp:ImageButton ID="btnPointMap" runat="server" ToolTip="show the point map" AlternateText="Regular Point Map"
                                ImageUrl="Images/pointMap.png" />
                            <asp:ImageButton ID="btnIsolineMap" runat="server" ToolTip="show the isoline map"
                                AlternateText="IsoLines Map" ImageUrl="Images/IsoLine.png" />
                        </div>
                        <div id="divblueBanner">
                            Earthquake Information Explorer
                        </div>
                        <h4>Query Tool:</h4>
                        <div id="divTrackShapes" class="divBorder">
                            <asp:ImageButton ID="btnPanMap" runat="server" ToolTip="Pan the map" CommandArgument="Pan"
                                CssClass="active" OnCommand="ChangeTrackShapeMode_Command" ImageUrl="Images/Pan.png" />
                            <asp:ImageButton ID="btnDrawPolygon" runat="server" ToolTip="Draw a polygon" CommandArgument="DrawPolygon"
                                OnCommand="ChangeTrackShapeMode_Command" ImageUrl="Images/DrawPolygon.png" />
                            <asp:ImageButton ID="btnDrawRectangle" runat="server" ToolTip="Draw a rectangle"
                                CommandArgument="DrawRectangle" OnCommand="ChangeTrackShapeMode_Command" ImageUrl="Images/DrawRectangle.png" />
                            <asp:ImageButton ID="btnDrawCircle" runat="server" ToolTip="Draw a circle" CommandArgument="DrawCircle"
                                OnCommand="ChangeTrackShapeMode_Command" ImageUrl="Images/Drawcircle.png" />
                            <asp:ImageButton ID="btnClearAll" runat="server" ToolTip="Clear all" CommandArgument="ClearAll"
                                OnCommand="ChangeTrackShapeMode_Command" ImageUrl="Images/clear.png" />
                        </div>
                        <h4>Query Configuration:</h4>
                        <div id="divQueryPanel" class="divBorder">
                            <table>
                                <tr>
                                    <td colspan="3">
                                        <h5>Magnitude:
                                        </h5>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="cfgRangeTitle">
                                        <span>0</span>
                                    </td>
                                    <td class="slider">
                                        <div id="sliderFortxbMagnitude">
                                        </div>
                                    </td>
                                    <td class="cfgRangeTitle">
                                        <span>12</span>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="3">
                                        <h5>Depth of Hypocenter(Km):
                                        </h5>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="cfgRangeTitle">
                                        <span>0</span>
                                    </td>
                                    <td class="slider">
                                        <div id="sliderFortxbDepth">
                                        </div>
                                    </td>
                                    <td class="cfgRangeTitle">
                                        <span>300</span>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="3">
                                        <h5>Date(Year):
                                        </h5>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="cfgRangeTitle">
                                        <span>1568</span>
                                    </td>
                                    <td class="slider">
                                        <div id="sliderFortxbYear">
                                        </div>
                                    </td>
                                    <td class="cfgRangeTitle">
                                        <span>2010</span>
                                    </td>
                                </tr>
                            </table>
                        </div>
                    </div>
                </div>
                <div id="toggle">
                    <img id="collapse" src="Images/collapse.gif" />
                </div>
                <div id="map-content">
                    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                        <ContentTemplate>
                            <div id="mapContainer">
                                <cc1:Map ID="Map1" runat="server" Width="100%" Height="100%" OnTrackShapeFinished="Map1_TrackShapeFinished">
                                </cc1:Map>
                            </div>
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="btnHeatMap" />
                            <asp:AsyncPostBackTrigger ControlID="btnPointMap" />
                            <asp:AsyncPostBackTrigger ControlID="btnIsolineMap" />
                            <asp:AsyncPostBackTrigger ControlID="btnPanMap" />
                            <asp:AsyncPostBackTrigger ControlID="btnDrawCircle" />
                            <asp:AsyncPostBackTrigger ControlID="btnDrawRectangle" />
                            <asp:AsyncPostBackTrigger ControlID="btnDrawPolygon" />
                            <asp:AsyncPostBackTrigger ControlID="btnClearAll" />
                        </Triggers>
                    </asp:UpdatePanel>
                    <div id="resultContainer">
                        <asp:UpdatePanel ID="UpdatePanel2" runat="server" ChildrenAsTriggers="false" UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:Repeater ID="rptQueryResult" runat="server" OnItemDataBound="rptQueryResultItemDataBound"
                                    OnItemCommand="ibtnZoomToFeature">
                                    <HeaderTemplate>
                                        <table id="resultTable">
                                            <tr>
                                                <td class="result-header"></td>
                                                <td class="result-header">Year
                                                </td>
                                                <td class="result-header">Longitude
                                                </td>
                                                <td class="result-header">Latitude
                                                </td>
                                                <td class="result-header">Depth(KM)
                                                </td>
                                                <td class="result-header">Magnitude
                                                </td>
                                                <td class="result-header">Location
                                                </td>
                                            </tr>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <tr>
                                            <td>
                                                <asp:ImageButton ID="find" runat="server" ImageUrl="/Images/find.png" />
                                            </td>
                                            <td>
                                                <%# Eval("year")%>
                                            </td>
                                            <td>
                                                <%# Eval("longitude")%>
                                            </td>
                                            <td>
                                                <%# Eval("latitude")%>
                                            </td>
                                            <td>
                                                <%# Eval("depth_km")%>
                                            </td>
                                            <td>
                                                <%# Eval("magnitude")%>
                                            </td>
                                            <td>
                                                <%# Eval("location")%>
                                            </td>
                                        </tr>
                                    </ItemTemplate>
                                    <FooterTemplate>
                                        </table>
                                    </FooterTemplate>
                                </asp:Repeater>
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="Map1" />
                                <asp:AsyncPostBackTrigger ControlID="btnClearAll" />
                            </Triggers>
                        </asp:UpdatePanel>
                    </div>
                </div>
            </div>
            <div id="footer">
                <span id="spanMouseCoordinate"></span>
            </div>
            <div id="loading">
                <img id="loading-image" src="Images/loading.gif" alt="Loading..." />
            </div>
        </div>
    </form>
</body>
</html>