<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="ThinkGeo.MapSuite.SiteSelection.Default" %>

<%@ Register Assembly="ThinkGeo.MapSuite.WebForms" Namespace="ThinkGeo.MapSuite.WebForms" TagPrefix="cc1" %>
<!DOCTYPE html>
<html xml:lang="en-us">
<head runat="server">
    <title>Site Selection</title>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <meta http-equiv="X-UA-Compatible" content="IE=Edge">
    <link href="Images/MapSuite.ico" rel="shortcut icon" type="Images/x-icon" />
    <link href="~/Styles/Site.css" rel="stylesheet" type="text/css" />
    <link href="Styles/jquery-ui-1.10.4.custom.min.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript" src="Scripts/modernizr-2.5.3.js"></script>
    <script src="Scripts/jquery-1.10.2.js" type="text/javascript"></script>
</head>
<body>
    <form id="Form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <div id="container">
        <div id="header">
            <div id="left-header">
                <span id="header-mapsuite">Map Suite</span> <span id="header-title">Site Selection</span>
            </div>
        </div>
        <div id="content-container">
            <div id="leftContainer">
                <div id="leftContent">
                    <h4>
                        Highlight points of this type:</h4>
                    <asp:UpdatePanel runat="server">
                        <ContentTemplate>
                            <div class="divBorder">
                                <asp:DropDownList ID="ddlCategory" AutoPostBack="True" runat="server" OnSelectedIndexChanged="ddlCategory_SelectedIndexChanged">
                                </asp:DropDownList>
                                <asp:DropDownList ID="ddlCategorySubtype" AutoPostBack="True" runat="server" OnSelectedIndexChanged="ddlCategorySubtype_SelectedIndexChanged">
                                </asp:DropDownList>
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                    <h4>
                        Place center pin on the map to highlight points of interest within a specified area
                        around where you clicked:
                    </h4>
                    <div id="divTrackMode" class="divBorder">
                        <asp:ImageButton ID="btnNormal" runat="server" ToolTip="Switch current map to pan mode."
                            ImageUrl="~/Images/pan.png" CommandArgument="PanMap" CssClass="active" OnCommand="TrackShapesCommand" />
                        <asp:ImageButton ID="btnDrawPoint" runat="server" ToolTip="Place the center pin on the map."
                            ImageUrl="~/Images/drawPoint.png" CommandArgument="DrawPoint" OnCommand="TrackShapesCommand" />
                        <asp:ImageButton ID="btnClearAll" runat="server" ToolTip="Reset your selection."
                            CommandArgument="ClearAll" OnCommand="TrackShapesCommand" ImageUrl="~/Images/clear.png" />
                    </div>
                    <h4>
                        Area Type:</h4>
                    <div class="divBorder">
                        <div>
                            <asp:RadioButton ID="rbtServiceArea" runat="server" Text=" Service Area" Checked="true"
                                GroupName="ServiceArea" />
                            <div id="divService">
                                Service Area in
                                <asp:TextBox runat="server" ID="tbxServiceArea" Width="40px">5</asp:TextBox>
                                minutes Driving
                            </div>
                        </div>
                        <div>
                            <asp:RadioButton ID="rbtBufferArea" runat="server" Text=" Buffered Area" GroupName="ServiceArea" />
                            <div id="divBuffer" hidden="hidden">
                                Buffered Distance:
                                <asp:TextBox runat="server" ID="tbxDistance" Width="40px">2</asp:TextBox>
                                <asp:DropDownList ID="ddlDistanceUnit" runat="server">
                                    <asp:ListItem Text="Mile"></asp:ListItem>
                                    <asp:ListItem Text="Kilometer"></asp:ListItem>
                                </asp:DropDownList>
                            </div>
                        </div>
                        <div>
                            <asp:Button ID="btnApply" CssClass="right-float" Width="76px" runat="server" Text="Apply"
                                OnClick="BtnApplyClick"></asp:Button>
                        </div>
                    </div>
                    <div class="blueBanner">
                        Potential Similar Sites
                    </div>
                    <asp:UpdatePanel ID="queryResultUpdatePanel" runat="server">
                        <ContentTemplate>
                            <asp:Repeater ID="repeaterQueryResult" runat="server" OnItemCommand="repeaterQueryResult_ItemCommand">
                                <HeaderTemplate>
                                    <table id="resultTable">
                                        <tr>
                                            <td class="header">
                                            </td>
                                            <td class="header">
                                                Name
                                            </td>
                                        </tr>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <tr>
                                        <td style="width: 30px">
                                            <asp:ImageButton ID="find" runat="server" ImageUrl="/Images/find.png" CommandArgument='<%# Eval("Wkt") %>' />
                                        </td>
                                        <td>
                                            <%# Eval("Name") %>
                                        </td>
                                    </tr>
                                </ItemTemplate>
                                <FooterTemplate>
                                    </table>
                                </FooterTemplate>
                            </asp:Repeater>
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="ddlCategory" />
                            <asp:AsyncPostBackTrigger ControlID="ddlCategorySubtype" />
                            <asp:AsyncPostBackTrigger ControlID="Map1" />
                            <asp:AsyncPostBackTrigger ControlID="btnApply" />
                        </Triggers>
                    </asp:UpdatePanel>
                </div>
            </div>
            <div id="toggle">
                <img alt="collapse" src="Images/collapse.gif" />
            </div>
            <asp:UpdatePanel ID="mapContentPanel" runat="server">
                <ContentTemplate>
                    <div id="map-content">
                        <cc1:Map ID="Map1" runat="server" Height="100%" Width="100%" OnTrackShapeFinished="Map1_TrackShapeFinished">
                        </cc1:Map>
                    </div>
                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="btnNormal" />
                    <asp:AsyncPostBackTrigger ControlID="btnDrawPoint" />
                    <asp:AsyncPostBackTrigger ControlID="btnClearAll" />
                    <asp:AsyncPostBackTrigger ControlID="btnApply" />
                </Triggers>
            </asp:UpdatePanel>
            <div id="dlgErrorMessage" title="Warning Message">
                <p>
                    Please note that this sample map is only able to analyze service areas within the
                    Frisco, TX city limits, which are indicated by a dashed red line. Click inside that
                    boundary for best results.</p>
            </div>
        </div>
        <div id="footer">
            <span id="spanMouseCoordinate"></span>
        </div>
        <script type="text/javascript" src="Scripts/ready-functions.js"></script>
        <script src="Scripts/jquery-ui-1.10.4.custom.min.js" type="text/javascript"></script>
    </div>
    </form>
</body>
</html>
