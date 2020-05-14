<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CensusBlockGroupSearching.aspx.cs"
    Inherits="ThinkGeo.MapSuite.HowDoI.CensusBlockGroup" %>


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
                This sample demonstrates geocoding in census block group of Texas.
                <br />
                <asp:DropDownList ID="cboSourceText" runat="server">
                    <asp:ListItem>CensusBlockGroup:1:000100</asp:ListItem>
                    <asp:ListItem>CensusBlockGroup:1:000300</asp:ListItem>
                    <asp:ListItem>CensusBlockGroup:2:243100</asp:ListItem>
                    <asp:ListItem>CensusBlockGroup:3:150800</asp:ListItem>
                    <asp:ListItem>CensusBlockGroup:4:170400</asp:ListItem>
                    <asp:ListItem>CensusBlockGroup:5:650400</asp:ListItem>
                    <asp:ListItem>CensusBlockGroup:6:150200</asp:ListItem>
                    <asp:ListItem>CensusBlockGroup:7:600300</asp:ListItem>
                    <asp:ListItem>CensusBlockGroup:8:501800</asp:ListItem>
                    <asp:ListItem>CensusBlockGroup:9:021100</asp:ListItem>
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
