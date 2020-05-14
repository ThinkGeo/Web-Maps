<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ServiceAreaDefinition.aspx.cs"
    Inherits="ThinkGeo.MapSuite.RoutingSamples.ServiceAreaDefinition" %>

<%@ Register Assembly="ThinkGeo.MapSuite.WebForms" Namespace="ThinkGeo.MapSuite.WebForms" TagPrefix="cc1" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <link href="../theme/default/samplepic/style.css" rel="stylesheet" type="text/css" />
    <title>Service Area Defination</title>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <cc1:Map ID="Map1" runat="server" Height="100%" Width="100%">
            </cc1:Map>
            <Description:DescriptionPanel ID="DescPanel" runat="server">
                Click the button below to generate the service area around the source road. For
                instance, the 2 minute service area for a point includes all the roads that can
                be reached within2 minutes at a specified speed.
                <br />
                <br />
                Speed(Per Hour):
                <input type="text" id="txtSpeed" runat="server" readonly="readonly" value="80" style="margin: 10px 5px 3px 5px;
                    width: 40px;" />
                <asp:DropDownList ID="ddlSpeedUnit" runat="server">
                    <asp:ListItem Text="Kilometer" Value="Kilometer" Selected="True"></asp:ListItem>
                    <asp:ListItem Text="Mile" Value="Mile"></asp:ListItem>
                </asp:DropDownList>
                <br />
                &nbsp;&nbsp;Driving Minutes:
                <input type="text" id="txtDrivingTime" readonly="readonly" runat="server" value="2"
                    style="margin: 10px 5px 3px 5px; width: 100px;" />&nbsp;Minutes
                <br />
                Source FeatureId:
                <input type="text" id="txtSourceFeatureId" readonly="readonly" runat="server" value="7700"
                    style="margin: 3px 5px 6px 4px; width: 140px;" />
                <br />
                <br />
                <div style="width: 100%; text-align: right;">
                    <asp:Button ID="btnRoute" runat="server" Text="Generate Service Area" OnClick="btnRoute_Click" />
                    &nbsp;&nbsp;&nbsp;
                </div>
            </Description:DescriptionPanel>
        </ContentTemplate>
    </asp:UpdatePanel>
    </form>
</body>
</html>
