<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CreateTextFileMatchingPlugIn.aspx.cs"
    Inherits="ThinkGeo.MapSuite.HowDoI.CreateTextFileMatchingPlugIn" %>


<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
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
                This sample demonstrates how to create a TextFileMatchingPlugIn for USA counties.
                <asp:DropDownList ID="cboSourceText" runat="server">
                    <asp:ListItem>Union County</asp:ListItem>
                    <asp:ListItem>Van Buren County</asp:ListItem>
                    <asp:ListItem>Kiowa County</asp:ListItem>
                    <asp:ListItem>Logan County</asp:ListItem>
                    <asp:ListItem>Anderson County</asp:ListItem>
                    <asp:ListItem>Pawnee County</asp:ListItem>
                    <asp:ListItem>Wallace County</asp:ListItem>
                    <asp:ListItem>Saline County</asp:ListItem>
                    <asp:ListItem>Ballard County</asp:ListItem>
                    <asp:ListItem>Breckinridge County</asp:ListItem>
                    <asp:ListItem>Fulton County</asp:ListItem>
                    <asp:ListItem>Linn County</asp:ListItem>
                    <asp:ListItem>Daviess County</asp:ListItem>
                    <asp:ListItem>Grant County</asp:ListItem>
                    <asp:ListItem>Green County</asp:ListItem>
                    <asp:ListItem>Hancock County</asp:ListItem>
                    <asp:ListItem>Graves County</asp:ListItem>
                    <asp:ListItem>Hickman County</asp:ListItem>
                    <asp:ListItem>Harrison County</asp:ListItem>
                    <asp:ListItem>Jackson County</asp:ListItem>
                </asp:DropDownList>
                <asp:Button ID="btnSearch" Text="Search" runat="server" OnClick="btnSearch_Click" />
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
            <iframe id="ifraKeepSessionAlive" src="~/SampleFramework/KeepSessionAlive.aspx" visible="false" frameborder="0" width="0" height="0" runat="server"></iframe>
        </ContentTemplate>
    </asp:UpdatePanel>
    </form>
</body>
</html>
