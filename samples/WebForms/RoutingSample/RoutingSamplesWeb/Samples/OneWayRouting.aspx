<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="OneWayRouting.aspx.cs"
    Inherits="ThinkGeo.MapSuite.RoutingSamples.OneWayRouting" %>

<%@ Register Assembly="ThinkGeo.MapSuite.WebForms" Namespace="ThinkGeo.MapSuite.WebForms" TagPrefix="cc1" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <link href="../theme/default/samplepic/style.css" rel="stylesheet" type="text/css" />
    <title></title>
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
                Click the Button below to switch the start and end road Feature Ids, and then show
                the shortest path.
                <br />
                <br />
                Start FeatureId:
                <input type="text" id="txtStartId" runat="server" readonly="readonly" value="13700" style="margin: 10px 5px 3px 5px;" />
                <br />
                &nbsp;End FeatureId:
                <input type="text" id="txtEndId" runat="server" readonly="readonly" value="13698" style="margin: 3px 5px 12px 7px;" />
                <br />
                <div style="width: 100%; text-align: right;">
                    <asp:Button ID="btnRoute" runat="server" Text="Switch Start And End Roads" OnClick="btnRoute_Click" />&nbsp;&nbsp;&nbsp;
                </div>
            </Description:DescriptionPanel>
        </ContentTemplate>
    </asp:UpdatePanel>
    </form>
</body>
</html>
