<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PanTheMap.aspx.cs" Inherits="HowDoI.Samples.PanTheMap" %>

<%@ Register Assembly="ThinkGeo.MapSuite.WebForms" Namespace="ThinkGeo.MapSuite.WebForms" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <link href="~/theme/default/samplepic/style.css" rel="stylesheet" type="text/css" />
    <title>Pan the Map</title>

    <script language="javascript" type="text/javascript">
        function panOffset(x, y) {
            var newX = Map1.GetCenter().lon + x;
            var newY = Map1.GetCenter().lat + y;
            Map1.PanToWorldCoordinate(newX, newY);
        }
    </script>

</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="Map1_Script" runat="server">
    </asp:ScriptManager>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <cc1:Map ID="Map1" runat="server" Width="100%" Height="100%">
            </cc1:Map>
        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="EnableMousePanCheckBox" />
            <asp:AsyncPostBackTrigger ControlID="EnableKeyboardPanCheckBox" />
            <asp:AsyncPostBackTrigger ControlID="DisableMouseZoomCheckBox" />
            <asp:AsyncPostBackTrigger ControlID="btnPanLeft" />
            <asp:AsyncPostBackTrigger ControlID="btnPanRight" />
            <asp:AsyncPostBackTrigger ControlID="btnPanUp" />
            <asp:AsyncPostBackTrigger ControlID="btnPanDown" />
        </Triggers>
    </asp:UpdatePanel>
    <Description:DescriptionPanel ID="DescPanel" runat="server">
        Click one of the directional buttons to pan the map using either client or server
        side code.
        <br />
        <br />
        <asp:Label ID="Label1" runat="server" ForeColor="#0065ce" Font-Bold="True" Text="Pan by your mouse:"
            Font-Size="10"></asp:Label>
        <br />
        <asp:CheckBox ID="EnableMousePanCheckBox" runat="server" Text="Enable panning by mouse"
            Font-Size="10" AutoPostBack="True" OnCheckedChanged="EnableMousePanCheckBox_CheckedChanged" />
        <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <asp:CheckBox ID="DisableMouseZoomCheckBox" runat="server" Text="Disable zoom by mouse"
                    Font-Size="10" AutoPostBack="True" OnCheckedChanged="DisableMouseZoomCheckBox_CheckedChanged"
                    Enabled="false" />
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="EnableMousePanCheckBox" />
            </Triggers>
        </asp:UpdatePanel>
        <br />
        <asp:Label ID="Label4" runat="server" ForeColor="#0065ce" Font-Bold="True" Text="Pan by your keyboard:"
            Font-Size="10"></asp:Label>
        <br />
        <asp:CheckBox ID="EnableKeyboardPanCheckBox" runat="server" Text="Enable panning by keyboard"
            Font-Size="10" AutoPostBack="True" OnCheckedChanged="EnableKeyboardPanCheckBox_CheckedChanged" />
        <br />
        <br />
        <asp:Label ID="Label2" runat="server" ForeColor="#0065ce" Font-Bold="True" Text="Pan using the client API:"
            Font-Size="10"></asp:Label>
        <br />
        <asp:Button ID="Button1" runat="server" Width="100" Text="Pan Left" OnClientClick="panOffset(-100000,0);return false;" />
        <asp:Button ID="Button2" runat="server" Width="100" Text="Pan Right" OnClientClick="panOffset(100000,0);return false;" />
        <asp:Button ID="Button3" runat="server" Width="100" Text="Pan Up" OnClientClick="panOffset(0,100000);return false;" />
        <asp:Button ID="Button4" runat="server" Width="100" Text="Pan Down" OnClientClick="panOffset(0,-100000);return false;" />
        <br />
        <br />
        <asp:Label ID="Label3" runat="server" ForeColor="#0065ce" Font-Bold="True" Text="Pan using the server API:"
            Font-Size="10"></asp:Label>
        <br />
        <asp:Button ID="btnPanLeft" runat="server" Width="100" Text="Pan Left" OnClick="btnPanLeft_Click" />
        <asp:Button ID="btnPanRight" runat="server" Width="100" Text="Pan Right" OnClick="btnPanRight_Click" />
        <asp:Button ID="btnPanUp" runat="server" Width="100" Text="Pan Up" OnClick="btnPanUp_Click" />
        <asp:Button ID="btnPanDown" runat="server" Width="100" Text="Pan Down" OnClick="btnPanDown_Click" />
    </Description:DescriptionPanel>
    </form>
</body>
</html>
