<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="ThinkGeo.MapSuite.USDemographicMap._Default" %>

<%@ Register Assembly="ThinkGeo.MapSuite.WebForms" Namespace="ThinkGeo.MapSuite.WebForms" TagPrefix="cc1" %>
<!DOCTYPE html>
<html xml:lang="en-us">
<head id="Head1" runat="server">
    <title>US Demographic Map</title>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <meta http-equiv="X-UA-Compatible" content="IE=Edge">
    <link href="Images/MapSuite.ico" rel="shortcut icon" type="Images/x-icon" />
    <link href="Styles/jquery-ui-1.10.4.custom.css" rel="stylesheet" type="text/css" />
    <link href="Styles/bootstrap.min.css" rel="stylesheet" type="text/css" />
    <link href="Styles/bootstrap-colorselector.css" rel="stylesheet" type="text/css" />
    <link href="Styles/Site.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript" src="Scripts/modernizr-2.5.3.js"></script>
    <script src="Scripts/jquery-1.10.2.js" type="text/javascript"></script>
    <script src="Scripts/jquery-ui-1.10.4.custom.js" type="text/javascript"></script>
</head>
<body>
    <form id="Form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server">
        </asp:ScriptManager>
        <div id="container">
            <div id="header">
                <div id="left-header">
                    <span id="header-mapsuite">Map Suite</span> <span id="header-title">US Demographic Map</span>
                </div>
            </div>
            <div id="content-container">
                <div id="leftContainer">
                    <div id="leftContent">
                        <h4>Select data for display:</h4>
                        <div id="accordion">
                            <asp:Repeater ID="rptCategoryList" runat="server" OnItemDataBound="rptCategoryList_ItemDataBound">
                                <ItemTemplate>
                                    <h3>
                                        <img alt="Population" src='<%#Eval("CategoryImage") %>' />
                                        <asp:Label ID="lbTitle" runat="server" CssClass="accordion-header" Text='<%#Eval("Title")%>'></asp:Label>
                                        <input type="image" class="accordion-header-right" alt="Pie" title="Chart Map" src="Images/pie.png"
                                            style="visibility: hidden;" />
                                    </h3>
                                    <div>
                                        <ul>
                                            <asp:Repeater ID="rptSubCategoryList" runat="server">
                                                <ItemTemplate>
                                                    <li><span class="accordion-content">
                                                        <%#Eval("Alias") %></span>
                                                        <input id="ckbSelected" class="accordion-content-right" type="checkbox" checked="checked"
                                                            style="visibility: hidden; margin: 8px 10px 0 0;" />
                                                        <img class="accordion-content-right" alt="DotDensity" title="Present the data with Dot Density." src="Images/DotDensity.png" />
                                                        <img class="accordion-content-right" alt="Thematic" title="Present the data in Thematic Colors." src="Images/Thematic.png" />
                                                        <img class="accordion-content-right" alt="ValueCircle" title="Present the data in Value Circles." src="Images/ValueCircle.png" />
                                                </ItemTemplate>
                                            </asp:Repeater>
                                        </ul>
                                    </div>
                                </ItemTemplate>
                            </asp:Repeater>
                        </div>
                        <div class="settings">
                            <div class="settings">
                                <span id="spanBaseColor" class="settingTitle">Display Start Color:</span>
                                <select id="colorStartselector">
                                </select>
                            </div>
                            <div id="divEndColor" class="settings">
                                <span class="settingTitle">Display End Color:</span>
                                <select id="colorEndselector">
                                </select>
                            </div>
                            <div id="divColorWheel" class="settings">
                                <span class="settingTitle">ColorWheelDirection:</span>
                                <select id="slColorWheelDirection" class="settingItem">
                                    <option value="Clockwise">Clockwise</option>
                                    <option value="CounterClockwise" selected="selected">CounterClockwise</option>
                                </select>
                            </div>
                        </div>
                        <div id="divSlider" class="settings">
                            <span id="spanSliderTitle" class="settingTitle"></span>
                            <div id="slider" class="settingItem">
                            </div>
                        </div>
                        <input type="hidden" id="clientStatusKeeper" value="" />
                    </div>
                </div>
                <div id="toggle">
                    <img alt="collapse" src="Images/collapse.gif" />
                </div>
                <div id="map-content">
                    <cc1:Map ID="Map1" runat="server" Height="100%" Width="100%">
                    </cc1:Map>
                </div>
            </div>
            <div id="footer">
                <span id="spanMouseCoordinate"></span>
            </div>
        </div>
        <script type="text/javascript" src="Scripts/bootstrap.min.js"></script>
        <script src="Scripts/bootstrap-colorselector.js" type="text/javascript"></script>
        <script type="text/javascript" src="Scripts/ready-functions.js"></script>
    </form>
</body>
</html>