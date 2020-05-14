<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AddAdditionalCustomPropertiesAndMethods.aspx.cs"
    Inherits="HowDoI.Samples.Extending_MapSuite.AddAdditionalCustomPropertiesAndMethods" %>

<%@ Register Assembly="ThinkGeo.MapSuite.WebForms" Namespace="ThinkGeo.MapSuite.WebForms" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link href="~/theme/default/samplepic/style.css" rel="stylesheet" type="text/css" />
    <title>Add additional custom properties and methods</title>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <cc1:Map ID="Map1" runat="server" Width="100%" Height="100%">
            </cc1:Map>
        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="ddlSecurityLevel" />
        </Triggers>
    </asp:UpdatePanel>
    <Description:DescriptionPanel ID="DescPanel" runat="server">
    This sample simply adds additional customer properties and methods to your objects.
        <br />
        <br />
        SecurityLevel&nbsp;&nbsp;<asp:DropDownList ID="ddlSecurityLevel" runat="server" AutoPostBack="True"
            OnSelectedIndexChanged="ddlSecurityLevel_SelectedIndexChanged">
            <asp:ListItem Text="AdministrationLevel"></asp:ListItem>
            <asp:ListItem Text="">AverageUsageLevel1</asp:ListItem>
            <asp:ListItem Text="">AverageUsageLevel2</asp:ListItem>
        </asp:DropDownList>
    </Description:DescriptionPanel>
    </form>
</body>
</html>
