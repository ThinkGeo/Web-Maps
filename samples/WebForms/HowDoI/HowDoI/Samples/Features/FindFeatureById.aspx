<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FindFeatureById.aspx.cs"
    Inherits="HowDoI.Samples.Features.FindFeatureById" %>

<%@ Register Assembly="ThinkGeo.MapSuite.WebForms" Namespace="ThinkGeo.MapSuite.WebForms" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link href="~/theme/default/samplepic/style.css" rel="stylesheet" type="text/css" />
    <title>Find By Id</title>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="Map1_Script" runat="server">
    </asp:ScriptManager>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <cc1:Map ID="Map1" runat="server" Width="100%" Height="100%">
            </cc1:Map>
        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="btnFind" EventName="Click" />
        </Triggers>
    </asp:UpdatePanel>
    <Description:DescriptionPanel ID="DescPanel" runat="server">
        Click the button to find the feature based on its Id in the textbox.
        <br />
        <br />
        <asp:Label ID="LabelFeatureId" runat="server" Text="FeatureId(1-251):" Font-Size="10"></asp:Label>
        <asp:TextBox ID="txtFeatureId" runat="server" Text="103" Width="40" MaxLength="3"
            ReadOnly="true"></asp:TextBox>
        <br />
        <br />
        <asp:Button ID="btnFind" runat="server" Text="Get the feature" OnClick="btnFind_Click" />
        <br />
    </Description:DescriptionPanel>
    </form>
</body>
</html>
