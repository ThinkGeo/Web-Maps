<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GetColumnData.aspx.cs"
    Inherits="HowDoI.Samples.Querying_Vector_Layers.GetColumnData" %>

<%@ Register Assembly="ThinkGeo.MapSuite.WebForms" Namespace="ThinkGeo.MapSuite.WebForms" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link href="~/theme/default/samplepic/style.css" rel="stylesheet" type="text/css" />
    <title>Get Column Data</title>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="Map1_Script" runat="server">
    </asp:ScriptManager>
    <cc1:Map ID="Map1" runat="server" Width="100%" Height="100%">
    </cc1:Map>
    <Description:DescriptionPanel ID="Description" runat="server">
        This sample gets all the column data of a feature layer and displayed
        them.
        <br />
        <br />
        <asp:GridView ID="FeaturesGridView" runat="server" Width="96%" Font-Size="10px"
            Font-Names="verdana" CellPadding="3" AutoGenerateColumns="False" BackColor="White"
            BorderColor="#CCCCCC" BorderStyle="None" BorderWidth="1px">
            <FooterStyle BackColor="White" ForeColor="#000066" />
            <RowStyle ForeColor="#000066" />
            <Columns>
                <asp:BoundField DataField="ColumnName" HeaderText="Column Name">
                    <ItemStyle HorizontalAlign="Center" />
                </asp:BoundField>
                <asp:BoundField DataField="MaxLength" HeaderText="Max Length">
                    <ItemStyle HorizontalAlign="Center" />
                </asp:BoundField>
                <asp:BoundField DataField="TypeName" HeaderText="Type Name">
                    <ItemStyle HorizontalAlign="Center" />
                </asp:BoundField>
            </Columns>
            <PagerStyle BackColor="White" ForeColor="#000066" HorizontalAlign="Left" Font-Size="10pt" />
            <SelectedRowStyle BackColor="#669999" Font-Bold="True" ForeColor="White" />
            <HeaderStyle BackColor="#006699" Font-Bold="True" ForeColor="White" />
        </asp:GridView>
    </Description:DescriptionPanel>
    </form>
</body>
</html>
