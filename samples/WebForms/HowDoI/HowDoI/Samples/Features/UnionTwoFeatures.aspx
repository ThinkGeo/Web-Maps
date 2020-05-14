<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UnionTwoFeatures.aspx.cs"
    Inherits="HowDoI.Samples.Features.UnionTwoFeatures" %>

<%@ Register Assembly="ThinkGeo.MapSuite.WebForms" Namespace="ThinkGeo.MapSuite.WebForms" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link href="~/theme/default/samplepic/style.css" rel="stylesheet" type="text/css" />
    <title>Union Two Features</title>
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
            <asp:AsyncPostBackTrigger ControlID="btnUnion" EventName="Click" />
        </Triggers>
    </asp:UpdatePanel>
    <Description:DescriptionPanel ID="DescPanel" runat="server">
        Click the button to union the two features.
        <br />
        <br />
        <asp:Button ID="btnUnion"  runat="server" Text="Union" Width="120"
            OnClick="btnUnion_Click" />
    </Description:DescriptionPanel>
    </form>
</body>
</html>
