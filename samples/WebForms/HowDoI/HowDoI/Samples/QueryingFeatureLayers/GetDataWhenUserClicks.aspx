<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GetDataWhenUserClicks.aspx.cs"
    Inherits="HowDoI.Samples.Querying_Vector_Layers.GetDataWhenUserClicks" %>

<%@ Register Assembly="ThinkGeo.MapSuite.WebForms" Namespace="ThinkGeo.MapSuite.WebForms" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">

    <script type="text/javascript" src="../Script/JScript.js"></script>

    <link href="~/theme/default/samplepic/style.css" rel="stylesheet" type="text/css" />
    <title>Get Data When User Clicks</title>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="Map1_Script" runat="server">
    </asp:ScriptManager>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <cc1:Map ID="Map1" runat="server" Width="100%" Height="100%" OnClick="Map1_Click">
            </cc1:Map>
            <Description:DescriptionPanel ID="Description" runat="server">
                Click on a country to display data about it.
                <br />
                <br />
                <asp:GridView ID="FeaturesGridView" runat="server" Width="80%" Font-Size="10px" Font-Names="Verdana"
                    HorizontalAlign="Center" AutoGenerateColumns="False" BackColor="White" BorderColor="#CCCCCC"
                    BorderStyle="None" BorderWidth="1px" CellPadding="3">
                    <FooterStyle BackColor="White" ForeColor="#000066" />
                    <RowStyle ForeColor="#000066" />
                    <Columns>
                        <asp:BoundField DataField="CountryName" HeaderText="Country Name">
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:BoundField>
                        <asp:BoundField DataField="Population" HeaderText="Population">
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:BoundField>
                    </Columns>
                    <PagerStyle BackColor="White" ForeColor="#000066" HorizontalAlign="Left" />
                    <SelectedRowStyle BackColor="#669999" Font-Bold="True" ForeColor="White" />
                    <HeaderStyle BackColor="#006699" Font-Bold="True" ForeColor="White" />
                </asp:GridView>
            </Description:DescriptionPanel>
        </ContentTemplate>
    </asp:UpdatePanel>
    </form>
</body>
</html>
