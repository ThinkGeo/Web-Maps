<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FuzzyLogicSearching.aspx.cs"
    Inherits="ThinkGeo.MapSuite.HowDoI.FuzzyLogicSearching" %>


<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="SampleFramework/theme/default/samplepic/style.css" rel="stylesheet" type="text/css" />
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
                This sample demonstrates how to use SoundExDbfMatchingPlugIn to implement fuzzy
                searching on USA cities.
                <br />
                <asp:DropDownList ID="cboSourceText" runat="server">
                    <asp:ListItem>New Yorke</asp:ListItem>
                    <asp:ListItem>Philadelphi</asp:ListItem>
                    <asp:ListItem>Chiccago</asp:ListItem>
                    <asp:ListItem>Hoston</asp:ListItem>
                    <asp:ListItem>San Digo</asp:ListItem>
                    <asp:ListItem>Dalas</asp:ListItem>
                    <asp:ListItem>Miamii</asp:ListItem>
                    <asp:ListItem>Detrot</asp:ListItem>
                    <asp:ListItem>Bufalo</asp:ListItem>
                    <asp:ListItem>Clevland</asp:ListItem>
                </asp:DropDownList>
                <asp:Button ID="btnSearch" Text="Search" runat="server" OnClick="btnSearch_Click" />
                <br />
                <br />
                <asp:ListBox AutoPostBack="true" ID="lstResult" OnSelectedIndexChanged="lstResult_SelectedIndexChanged"
                    Width="245px" Height="150" runat="server"></asp:ListBox>
                <br />
                <br />
                <div style="height: 350px; overflow: scroll;">
                    <asp:GridView ID="dataGridViewDetail" runat="server" Width="245px" AutoGenerateColumns="False">
                        <Columns>
                            <asp:BoundField DataField="Key" HeaderText="Name" />
                            <asp:BoundField DataField="Value" HeaderText="Value" />
                        </Columns>
                    </asp:GridView>
                </div>
            </Description:DescriptionPanel>
            <iframe id="ifraKeepSessionAlive" src="~/SampleFramework/KeepSessionAlive.aspx" visible="false"
                frameborder="0" width="0" height="0" runat="server"></iframe>
        </ContentTemplate>
    </asp:UpdatePanel>
    </form>
</body>
</html>
