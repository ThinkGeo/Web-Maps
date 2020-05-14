<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GetRoadInformationByRoadId.aspx.cs"
    Inherits="ThinkGeo.MapSuite.RoutingSamples.GetRoadInformationByRoadId" %>

<%@ Register Assembly="ThinkGeo.MapSuite.WebForms" Namespace="ThinkGeo.MapSuite.WebForms" TagPrefix="cc1" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <link href="../theme/default/samplepic/style.css" rel="stylesheet" type="text/css" />
    <title>Get Road Information By Road Id</title>
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
                Please click the Button below to show the road segment and its adjacent roads' information.
                <br />
                <br />
                Feature Id:
                <input type="text" id="txtId" value="8229" readonly="readonly" runat="server" style="margin: 8px; width: 180px;" />
                <br />
                <div style="text-align: right;">
                    <asp:Button ID="btnGetRouteInformation" runat="server" Text="Get Road Information"
                        Width="150px" OnClick="btnGetRouteInformation_Click" />&nbsp;&nbsp;
                    <br />
                    <br />
                    <fieldset>
                        <legend>Road Segment Information</legend>
                        <div style="text-align: left; padding: 5px;">
                            &nbsp;Start Point:
                            <input type="text" id="txtStartPoint" readonly="readonly" runat="server" style="margin: 5px 5px 3px 5px;" /><br />
                            &nbsp;&nbsp;End Point:
                            <input type="text" id="txtEndPoint" readonly="readonly" runat="server" style="margin: 5px 5px 3px 6px;" /><br />
                            Road Type:
                            <input type="text" id="txtRoadType" readonly="readonly" runat="server" style="margin: 5px;" /><br />
                            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Length:
                            <input type="text" id="txtLength" readonly="readonly" runat="server" style="margin: 5px 5px 3px 6px;
                                width: 120px;" />&nbsp;Meter<br />
                        </div>
                    </fieldset>
                </div>
            </Description:DescriptionPanel>
        </ContentTemplate>
    </asp:UpdatePanel>
    </form>
</body>
</html>
