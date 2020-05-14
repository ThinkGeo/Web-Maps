<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UseDifferentAlgorithms.aspx.cs"
    Inherits="ThinkGeo.MapSuite.RoutingSamples.UseDifferentAlgorithms" %>

<%@ Register Assembly="ThinkGeo.MapSuite.WebForms" Namespace="ThinkGeo.MapSuite.WebForms" TagPrefix="cc1" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <link href="../theme/default/samplepic/style.css" rel="stylesheet" type="text/css" />
    <title>Use Different Algorithm</title>
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
                Please select the different algorithm to find the path.
                <br />
                <br />
                Start FeatureId:
                <input type="text" id="txtStartId" runat="server" readonly="readonly" value="4226" style="margin: 10px 5px 3px 5px;" />
                <br />
                &nbsp;End FeatureId:
                <input type="text" id="txtEndId" runat="server" readonly="readonly" value="8094" style="margin: 3px 5px 6px 7px;" />
                <br />
                &nbsp;&nbsp;&nbsp;Algorithm:
                <asp:DropDownList ID="ddlAlgorithm" AutoPostBack="true" runat="server" OnSelectedIndexChanged="ddlAlgorithm_SelectedIndexChanged">
                    <asp:ListItem Value="AStar">A*</asp:ListItem>
                    <asp:ListItem Selected="True">Dijkstra</asp:ListItem>
                    <asp:ListItem>Bidirectional</asp:ListItem>
                </asp:DropDownList>
            </Description:DescriptionPanel>
        </ContentTemplate>
    </asp:UpdatePanel>
    </form>
</body>
</html>
