<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ScaleFeatureUpAndDown.aspx.cs"
    Inherits="HowDoI.Samples.Features.ScaleFeatureUpAndDown" %>

<%@ Register Assembly="ThinkGeo.MapSuite.WebForms" Namespace="ThinkGeo.MapSuite.WebForms" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link href="~/theme/default/samplepic/style.css" rel="stylesheet" type="text/css" />
    <title>Scale Feature Up & Down</title>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager runat="server">
    </asp:ScriptManager>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <cc1:Map ID="Map1" runat="server" Width="100%" Height="100%">
            </cc1:Map>
            <Description:DescriptionPanel ID="DescPanel" runat="server">
                Click one of the buttons to scale the feature up or down.
                <br />
                <br />
                <asp:Button ID="btnScaleUp" runat="server"  Text="Scale Up" Width="100"
                    OnClick="btnScaleUp_Click" />
                &nbsp;
                <asp:Button ID="btnScaleDown" runat="server"  Text="Scale Down" Width="100"
                    OnClick="btnScaleDown_Click" />
            </Description:DescriptionPanel>
        </ContentTemplate>
    </asp:UpdatePanel>
    </form>
</body>
</html>
