<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TrackShapeFinishedEvent.aspx.cs"
    Inherits="HowDoI.Samples.MapShapes.TrackShapeFinishedEvent" %>

<%@ Register Assembly="ThinkGeo.MapSuite.WebForms" Namespace="ThinkGeo.MapSuite.WebForms" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <link href="~/theme/default/samplepic/style.css" rel="stylesheet" type="text/css" />
    <title>Finished Track Shape Event</title>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <cc1:Map ID="Map1" runat="server" Width="100%" Height="100%" OnTrackShapeFinished="Map1_TrackShapeFinished">
            </cc1:Map>
            <Description:DescriptionPanel ID="DescPanel" runat="server">
                The sample demonstrates 'TrackShapeFinished' event of the map. If you attach handlers
                to this event, the map will postback each time when you finish drawing a shape.
                <hr />
                Click buttons below to draw shapes on the map.
                <br />
                <asp:ImageButton ID="buttonDrawPoint" runat="server" ToolTip="Draw Point" ImageUrl="~/theme/default/samplepic/point28.png"
                    OnClick="buttonDrawPoint_Click" />
                <asp:ImageButton ID="buttonDrawLine" runat="server" ToolTip="Draw Line" ImageUrl="~/theme/default/samplepic/line28.png"
                    OnClick="buttonDrawLine_Click" />
                <asp:ImageButton ID="buttonDrawRectangle" runat="server" ToolTip="Draw Rectangle"
                    ImageUrl="~/theme/default/samplepic/rectangle28.png" OnClick="buttonDrawRectangle_Click" />
                <asp:ImageButton ID="buttonDrawSquare" runat="server" ToolTip="Draw Square" ImageUrl="~/theme/default/samplepic/square28.png"
                    OnClick="buttonDrawSquare_Click" />
                <asp:ImageButton ID="buttonDrawPolygon" runat="server" ToolTip="Draw Polygon" ImageUrl="~/theme/default/samplepic/polygon28.png"
                    OnClick="buttonDrawPolygon_Click" />
                <asp:ImageButton ID="buttonDrawCircle" runat="server" ToolTip="Draw Circle" ImageUrl="~/theme/default/samplepic/circle28.png"
                    OnClick="buttonDrawCircle_Click" />
                <asp:ImageButton ID="buttonDrawEllipse" runat="server" ToolTip="Draw Ellipse" ImageUrl="~/theme/default/samplepic/ellipse28.png"
                    OnClick="buttonDrawEllipse_Click" />
                <asp:ImageButton ID="buttonEditShape" runat="server" ToolTip="Edit Shape" ImageUrl="~/theme/default/samplepic/btn_edit.gif"
                    OnClick="buttonEditShape_Click" />
            </Description:DescriptionPanel>
        </ContentTemplate>
    </asp:UpdatePanel>
    </form>
</body>
</html>
