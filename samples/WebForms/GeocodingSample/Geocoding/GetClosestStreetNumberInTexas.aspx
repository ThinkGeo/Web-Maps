<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GetClosestStreetNumberInTexas.aspx.cs"
    Inherits="ThinkGeo.MapSuite.HowDoI.GetClosestStreetNumberInTexas" %>


<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <link href="SampleFramework/theme/default/samplepic/style.css" rel="stylesheet" type="text/css" />
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <cc1:Map ID="map1" runat="server" Height="100%" Width="100%">
            </cc1:Map>
            <Description:DescriptionPanel ID="descriptionPanel1" Height="600" runat="server">
                This sample demonstrates geocoding in Texas street addresses.
                <br />
                <asp:DropDownList ID="cboSourceText" runat="server">
                    <asp:ListItem>5300 N Winthrop Ave</asp:ListItem>
                    <asp:ListItem>1401 W Ainslie St</asp:ListItem>
                    <asp:ListItem>7463 N Marshfield Ave</asp:ListItem>
                    <asp:ListItem>7430 N Seeley Ave</asp:ListItem>
                    <asp:ListItem>7400 N Greenview Ave</asp:ListItem>
                    <asp:ListItem>1101 W Farwell Ave</asp:ListItem>
                    <asp:ListItem>1401 W Estes Ave</asp:ListItem>
                    <asp:ListItem>6900 N Glenwood Ave</asp:ListItem>
                    <asp:ListItem>7098 N Wolcott Ave</asp:ListItem>
                    <asp:ListItem>7300 N Claremont Ave</asp:ListItem>
                    <asp:ListItem>2631 W Fargo Ave</asp:ListItem>
                    <asp:ListItem>2201 W Greenleaf Ave</asp:ListItem>
                    <asp:ListItem>6500 N Bell Ave</asp:ListItem>
                    <asp:ListItem>2833 W North Shore Ave</asp:ListItem>
                    <asp:ListItem>3051 W Hood Ave</asp:ListItem>
                    <asp:ListItem>2633 W Rosemont Ave</asp:ListItem>
                    <asp:ListItem>6200 N Claremont Ave</asp:ListItem>
                    <asp:ListItem>1101 W Glenlake Ave</asp:ListItem>
                    <asp:ListItem>1701 W Thome Ave</asp:ListItem>
                    <asp:ListItem>5746 N Wayne Ave</asp:ListItem>
                </asp:DropDownList>
                <asp:Button ID="btnSearch" Text="Search" runat="server" OnClick="btnSearch_Click" />
                <br />
                <br />
                <asp:CheckBox ID="chkGetClosestStreetNumber" runat="server" Text="Get Closest Street Number" />
                <br />
                <br />
                <asp:ListBox AutoPostBack="true" ID="lstResult" OnSelectedIndexChanged="lstResult_SelectedIndexChanged"
                    Width="245px" Height="150" runat="server"></asp:ListBox>
                <br />
                <br />
                <div style="height: 350px; overflow: scroll;">
                    <asp:GridView ID="dataGridViewDetail" Width="245px" runat="server" AutoGenerateColumns="False">
                        <Columns>
                            <asp:BoundField DataField="Key" HeaderText="Name" />
                            <asp:BoundField DataField="Value" HeaderText="Value" />
                        </Columns>
                    </asp:GridView>
                </div>
            </Description:DescriptionPanel>
        </ContentTemplate>
    </asp:UpdatePanel>
        <iframe id="ifraKeepSessionAlive" src="~/SampleFramework/KeepSessionAlive.aspx" visible="false"
            frameborder="0" width="0" height="0" runat="server"></iframe>
    </form>
</body>
</html>
