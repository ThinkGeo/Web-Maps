<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DrawEditShapes.aspx.cs"
    Inherits="HowDoI.DrawEditShapes" %>

<%@ Register Assembly="ThinkGeo.MapSuite.WebForms" Namespace="ThinkGeo.MapSuite.WebForms" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <link href="~/theme/default/samplepic/style.css" rel="stylesheet" type="text/css" />
    <title>Draw & Edit Shapes</title>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <cc1:Map ID="Map1" runat="server" Width="100%" Height="100%">
            </cc1:Map>
        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="buttonEditShape" />
            <asp:AsyncPostBackTrigger ControlID="buttonSubmit" />
        </Triggers>
    </asp:UpdatePanel>
    <Description:DescriptionPanel ID="DescPanel" runat="server">
        This sample demonstrates how to use EditOverlay to draw and edit shapes on the map.
        The EditOverlay is used to contain the features which you want to edit at the client
        side. All the features in this Overlay will be converted to client shapes. Of course,
        the new shapes you draw on the map will enter the EditOverlay after postback.
        <br />
        <br />
        Try to click buttons below to draw different shapes at client.
        <br />
        <asp:ImageButton ID="buttonNormal" runat="server" ToolTip="Normal Mode" ImageUrl="~/theme/default/samplepic/Cursor.png"
            OnClientClick="Map1.SetDrawMode('Normal');return false;" />
        <asp:ImageButton ID="buttonDrawPoint" runat="server" ToolTip="Draw Point" ImageUrl="~/theme/default/samplepic/point28.png"
            OnClientClick="Map1.SetDrawMode('Point');return false;" />
        <asp:ImageButton ID="buttonDrawLine" runat="server" ToolTip="Draw Line" ImageUrl="~/theme/default/samplepic/line28.png"
            OnClientClick="Map1.SetDrawMode('Line');return false;" />
        <asp:ImageButton ID="buttonDrawRectangle" runat="server" ToolTip="Draw Rectangle"
            ImageUrl="~/theme/default/samplepic/rectangle28.png" OnClientClick="Map1.SetDrawMode('Rectangle');return false;" />
        <asp:ImageButton ID="buttonDrawSquare" runat="server" OnClientClick="Map1.SetDrawMode('Square');return false;"
            ToolTip="Draw Square" ImageUrl="~/theme/default/samplepic/square28.png" />
        <asp:ImageButton ID="buttonDrawPolygon" runat="server" ToolTip="Draw Polygon" ImageUrl="~/theme/default/samplepic/polygon28.png"
            OnClientClick="Map1.SetDrawMode('Polygon');return false;" />
        <asp:ImageButton ID="buttonDrawCircle" runat="server" ToolTip="Draw Circle" ImageUrl="~/theme/default/samplepic/circle28.png"
            OnClientClick="Map1.SetDrawMode('Circle');return false;" />
        <asp:ImageButton ID="buttonDrawEllipse" runat="server" ToolTip="Draw Ellipse" ImageUrl="~/theme/default/samplepic/ellipse28.png"
            OnClientClick="Map1.SetDrawMode('Ellipse');return false;" />
        <asp:ImageButton ID="buttonFreeHand" runat="server" ToolTip="FreeHand" ImageUrl="~/theme/default/samplepic/freehand28.gif"
            OnClientClick="Map1.SetDrawMode('FreeHand');return false;" />
        <br />
        Try to click the button below to delete the previous shape you have drew.
        <br />
        <asp:ImageButton ID="buttonDeleteShape" runat="server" ToolTip="Delete Shape" ImageUrl="~/theme/default/img/Delete.gif"
            OnClientClick="Map1.CancelLatestFeature();return false;" />
        <br />
        Try to click the button below to save the new shapes to the server.
        <br />
        <asp:Button ID="buttonSubmit" runat="server" ToolTip="Normal Mode" Width="96px" OnClick="buttonSubmit_Click" Height="24px" Text="Save" />
        <br />
        Try to click the button below to convert the server shapes to the client shapes
        for editing.
        <br />
        <asp:Button ID ="buttonEditShape" runat="server" ToolTip="Normal Mode" width="96px"  OnClick="buttonEditShape_Click" Height="24px" Text="Edit" />
        </Description:DescriptionPanel>
    </form>
</body>
</html>
