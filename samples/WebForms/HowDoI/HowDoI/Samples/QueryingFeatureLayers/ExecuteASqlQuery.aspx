<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ExecuteASqlQuery.aspx.cs"
    Inherits="HowDoI.Samples.Querying_Vector_Layers.ExecuteASqlQuery" %>

<%@ Register Assembly="ThinkGeo.MapSuite.WebForms" Namespace="ThinkGeo.MapSuite.WebForms" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link href="~/theme/default/samplepic/style.css" rel="stylesheet" type="text/css" />
    <title>Execute a SQL Query</title>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="Map1_Script" runat="server">
    </asp:ScriptManager>
    <cc1:Map ID="Map1" runat="server" Width="100%" Height="100%">
    </cc1:Map>
    <Description:DescriptionPanel ID="Description" runat="server">
        <br />
        <asp:TextBox ID="SQLTextBox" runat="server" TextMode="MultiLine" Rows="3" Width="250px"
            Font-Size="8" ReadOnly="true">Select CNTRY_NAME,POP_CNTRY from CNTRY02_3857 Where POP_CNTRY &gt; 100000000 Order by POP_CNTRY</asp:TextBox>
        <br />
        <asp:Button ID="btnExecute" runat="server" Text="Execute SQL" OnClick="btnExecute_Click" />
        <br />
        <br />
        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <asp:GridView ID="FeaturesGridView" runat="server" Width="85%" Height="120px" BackColor="White"
                    BorderColor="#CCCCCC" BorderStyle="None" BorderWidth="1px" CellPadding="3" AutoGenerateColumns="False"
                    Font-Size="10pt">
                    <FooterStyle BackColor="White" ForeColor="#000066" />
                    <RowStyle ForeColor="#000066" />
                    <Columns>
                        <asp:BoundField DataField="cntry_name" HeaderText="Country Name">
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:BoundField>
                        <asp:BoundField DataField="pop_cntry" HeaderText="Population" DataFormatString="{0:N0}">
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:BoundField>
                    </Columns>
                    <PagerStyle BackColor="White" ForeColor="#000066" HorizontalAlign="Left" />
                    <SelectedRowStyle BackColor="#669999" Font-Bold="True" ForeColor="White" />
                    <HeaderStyle BackColor="#006699" Font-Bold="True" ForeColor="White" />
                </asp:GridView>
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="btnExecute" />
            </Triggers>
        </asp:UpdatePanel>
    </Description:DescriptionPanel>
    </form>
</body>
</html>
