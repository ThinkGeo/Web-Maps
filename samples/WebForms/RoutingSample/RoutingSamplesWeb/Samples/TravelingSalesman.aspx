<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TravelingSalesman.aspx.cs"
    Inherits="ThinkGeo.MapSuite.RoutingSamples.TravelingSalesman" %>

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
                This sample demonstrates how to generate the optimal sequence of visiting locations,
                this is the traveling salesman problem, or TSP.The iterations is a number that you
                can set to improve the accuracy of RoutingResult, the bigger, the more accurate,
                but consume more time.
                <br />
                <br />
                Start Coordinates:
                <input type="text" id="txtStartId" readonly="readonly" runat="server" value="-97.7110,30.2701"
                    style="margin: 10px 5px 3px 5px;" />
                <br />
                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Iterations:
                <input type="text" id="txtIterations" runat="server" value="100" style="margin: 3px 5px 12px 7px;" />
                <br />
                <fieldset>
                    <legend>Visit Locations List</legend>
                    <div>
                        <asp:ListBox ID="lsbLocations" runat="server" Width="240px">
                            <asp:ListItem>-97.7186,30.2899</asp:ListItem>
                            <asp:ListItem>-97.7256,30.2885</asp:ListItem>
                            <asp:ListItem>-97.7295,30.2836</asp:ListItem>
                            <asp:ListItem>-97.7259,30.2750</asp:ListItem>
                            <asp:ListItem>-97.7197,30.2718</asp:ListItem>
                            <asp:ListItem>-97.7117,30.2780</asp:ListItem>
                            <asp:ListItem>-97.7161,30.2849</asp:ListItem>
                        </asp:ListBox>
                    </div>
                </fieldset>
                <br />
                <div style="text-align: left; margin: 5px;">
                    Total Distance:
                    <input type="text" id="txtDistance" readonly="readonly" runat="server" value="" style="margin: 10px 5px 8px 4px;" /><br />
                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Time Used:
                    <asp:TextBox ID="txtTime" ReadOnly="true" runat="server"></asp:TextBox>
                </div>
                <div style="width: 100%; text-align: right;">
                    <asp:Button ID="btnRoute" runat="server" Text="Find The Best Route" OnClick="btnRoute_Click" />&nbsp;&nbsp;&nbsp;
                </div>
            </Description:DescriptionPanel>
        </ContentTemplate>
    </asp:UpdatePanel>
    </form>
</body>
</html>
