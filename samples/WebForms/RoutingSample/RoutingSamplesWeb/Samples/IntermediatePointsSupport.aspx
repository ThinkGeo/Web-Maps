<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="IntermediatePointsSupport.aspx.cs"
    Inherits="ThinkGeo.MapSuite.RoutingSamples.IntermediatePointsSupport" %>

<%@ Register Assembly="ThinkGeo.MapSuite.WebForms" Namespace="ThinkGeo.MapSuite.WebForms" TagPrefix="cc1" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <link href="../theme/default/samplepic/style.css" rel="stylesheet" type="text/css" />
    <title>Intermediate Point Support</title>
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
                This sample demonstrates how to find the shortest path consisting of a start, end
                and intermediate points.
                <br />
                <br />
                <div style="width: 100%; text-align: right;">
                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Start Point:
                    <input type="text" id="txtStartPoint" readonly="readonly" runat="server" value="-97.728263,30.327438"
                        style="margin: 10px 5px 3px 5px; width: 140px;" />
                    <br />
                    Intermediate Point:
                    <input type="text" id="txtIntermediatePoint" readonly="readonly" runat="server" value="-97.750333,30.309056"
                        style="margin: 10px 5px 3px 5px; width: 140px;" />
                    <br />
                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;End Point:
                    <input type="text" id="txtEndPoint" readonly="readonly" runat="server" value="-97.726781,30.287880" style="margin: 3px 5px 6px 7px;
                        width: 140px;" />
                    <br />
                    <br />
                    <asp:CheckBox ID="chkAddIntermediate" AutoPostBack="true" Text="Route Via Intermediate Stop"
                        runat="server" OnCheckedChanged="chkAddIntermediate_CheckedChanged" />
                    &nbsp;&nbsp;&nbsp;
                </div>
            </Description:DescriptionPanel>
        </ContentTemplate>
    </asp:UpdatePanel>
    </form>
</body>
</html>
