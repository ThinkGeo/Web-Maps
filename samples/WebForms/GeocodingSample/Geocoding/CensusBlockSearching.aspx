<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CensusBlockSearching.aspx.cs"
    Inherits="ThinkGeo.MapSuite.HowDoI.Censusblock" %>


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
                This sample demonstrates geocoding in census block of Texas.
                <br />
                <asp:DropDownList ID="cboSourceText" runat="server">
                    <asp:ListItem>CensusBlock:1000:010900</asp:ListItem>
                    <asp:ListItem>CensusBlock:1010:827805</asp:ListItem>
                    <asp:ListItem>CensusBlock:2110:824104</asp:ListItem>
                    <asp:ListItem>CensusBlock:3009:824503</asp:ListItem>
                    <asp:ListItem>CensusBlock:4015:830004</asp:ListItem>
                    <asp:ListItem>CensusBlock:5004:100700</asp:ListItem>
                    <asp:ListItem>CensusBlock:6013:804607</asp:ListItem>
                    <asp:ListItem>CensusBlock:7002:530200</asp:ListItem>
                    <asp:ListItem>CensusBlock:8005:650300</asp:ListItem>
                </asp:DropDownList>
                <asp:Button ID="btnSearch" Text="Search" runat="server" OnClick="btnSearch_Click" />
                <br />
                <br />
                <asp:ListBox AutoPostBack="true" ID="lstResult" OnSelectedIndexChanged="lstResult_SelectedIndexChanged"
                    Width="245px" Height="150" runat="server"></asp:ListBox>
                <br />
                <br />
                <div style="height: 350px; overflow: scroll;">
                    <asp:GridView ID="dataGridViewDetail" Width="245px" runat="server" AutoGenerateColumns="True">
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
