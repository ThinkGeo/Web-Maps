<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FindFeaturesWithinDistance.aspx.cs"
    Inherits="HowDoI.Samples.Querying_Vector_Layers.FindFeaturesWithinDistance" %>

<%@ Register Assembly="ThinkGeo.MapSuite.WebForms" Namespace="ThinkGeo.MapSuite.WebForms" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link href="~/theme/default/samplepic/style.css" rel="stylesheet" type="text/css" />
    <title>Find Features Within a Distance</title>
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
                Click on the map and we will find all the countries within the distance in the DropdownList.
                <br />
                <br />
                <asp:DropDownList ID="DistanceDropDownList" runat="server" Width="80" OnSelectedIndexChanged="DistanceDropDownList_SelectedIndexChanged"
                    AutoPostBack="True">
                    <asp:ListItem>500</asp:ListItem>
                    <asp:ListItem Selected="True">1,000</asp:ListItem>
                    <asp:ListItem>2,000</asp:ListItem>
                    <asp:ListItem>5,000</asp:ListItem>
                </asp:DropDownList>
                <asp:Label ID="Label2" runat="server" Text="KM"></asp:Label>
            </Description:DescriptionPanel>
        </ContentTemplate>
    </asp:UpdatePanel>
    </form>
</body>
</html>
