<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SetBackgroundColor.aspx.cs"
    Inherits="HowDoI.Samples.SetBackgroundColor" %>

<%@ Register Assembly="ThinkGeo.MapSuite.WebForms" Namespace="ThinkGeo.MapSuite.WebForms" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <link href="~/theme/default/samplepic/style.css" rel="stylesheet" type="text/css" />
    <title>Set the Background Color</title>
    <style type="text/css">
        .btcolor
        {
            width: 18px;
            height: 18px;
            border: #333333 1px solid;
            cursor: hand;
        }
    </style>
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
            <asp:AsyncPostBackTrigger ControlID="Button1" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="Button2" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="Button3" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="Button4" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="Button5" EventName="Click" />
        </Triggers>
    </asp:UpdatePanel>
    <Description:DescriptionPanel ID="DescPanel" runat="server">
        Click on one of the squares below to change the background color.
        <br />
        <br />
        <div>
            <asp:Button runat="server" ID="Button1" CssClass="btcolor" BackColor="#336600" OnClick="ChangeBgColor" />
            <asp:Button runat="server" ID="Button2" CssClass="btcolor" BackColor="#99cc66" OnClick="ChangeBgColor" />
            <asp:Button runat="server" ID="Button3" CssClass="btcolor" BackColor="#3399ff" OnClick="ChangeBgColor" />
            <asp:Button runat="server" ID="Button4" CssClass="btcolor" BackColor="#ffcc99" OnClick="ChangeBgColor" />
            <asp:Button runat="server" ID="Button5" CssClass="btcolor" BackColor="#ffcc00" OnClick="ChangeBgColor" />
        </div>
    </Description:DescriptionPanel>
    </form>
</body>
</html>
