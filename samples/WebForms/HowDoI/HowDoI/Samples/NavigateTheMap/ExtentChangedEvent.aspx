<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ExtentChangedEvent.aspx.cs"
    Inherits="HowDoI.Samples.NavigateTheMap.ExtentChangedEvent" %>

<%@ Register Assembly="ThinkGeo.MapSuite.WebForms" Namespace="ThinkGeo.MapSuite.WebForms" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link href="~/theme/default/samplepic/style.css" rel="stylesheet" type="text/css" />
    <title>ExtentChangedEvent</title>
    <style type="text/css">
        .data
        {
            color: Red;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="Map1_Script" runat="server">
    </asp:ScriptManager>
    <cc1:Map ID="Map1" runat="server" Width="100%" Height="100%" OnExtentChanged="Map1_ExtentChanged">
    </cc1:Map>
    <Description:DescriptionPanel ID="DescPanel" runat="server">
        Pan or zoom the map to see the CurrentScale and CurrentExtent changed.
        <br />
        <br />
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                <div id="dataContainer" runat="server">
                    <b>CurrentScale:</b><br />
                    <asp:Label CssClass="data" ID="lbCurrentScale" runat="server" Text=""></asp:Label>
                    <br />
                    <br />
                    <b>CurrentExtent:</b>(left, top, right, bottom)<br />
                    <div id="divExtent" class="data" runat="server">
                    </div>
                </div>
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="Map1" />
            </Triggers>
        </asp:UpdatePanel>
    </Description:DescriptionPanel>
    </form>
</body>
</html>
