<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ChangePointLabelPlacement.aspx.cs"
    Inherits="HowDoI.Samples.Labeling.ChangePointLabelPlacement" %>

<%@ Register Assembly="ThinkGeo.MapSuite.WebForms" Namespace="ThinkGeo.MapSuite.WebForms" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link href="~/theme/default/samplepic/style.css" rel="stylesheet" type="text/css" />
    <title>Change Point Label Placement</title>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="Map1_Script" runat="server">
    </asp:ScriptManager>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <cc1:Map ID="Map1" runat="server" Width="100%" Height="100%">
            </cc1:Map>
            <Description:DescriptionPanel ID="DescPanel" runat="server">
                Switch item in the combo and see the position of labels.
                <br />
                <br />
                <asp:DropDownList ID="PointPlacementDropDownList" runat="server" OnSelectedIndexChanged="PointPlacementDropDownList_SelectedIndexChanged"
                    AutoPostBack="True">
                    <asp:ListItem>UpperLeft</asp:ListItem>
                    <asp:ListItem>UpperCenter</asp:ListItem>
                    <asp:ListItem>UpperRight</asp:ListItem>
                    <asp:ListItem Selected="True">CenterRight</asp:ListItem>
                    <asp:ListItem>Center</asp:ListItem>
                    <asp:ListItem>CenterLeft</asp:ListItem>
                    <asp:ListItem>LowerLeft</asp:ListItem>
                    <asp:ListItem>LowerCenter</asp:ListItem>
                    <asp:ListItem>LowerRight</asp:ListItem>
                </asp:DropDownList>
            </Description:DescriptionPanel>
        </ContentTemplate>
    </asp:UpdatePanel>
    </form>
</body>
</html>
