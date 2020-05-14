<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DistanceBetweenTwoPoints.aspx.cs"
    Inherits="HowDoI.Samples.Features.DistanceBetweenTwoPoints" %>

<%@ Register Assembly="ThinkGeo.MapSuite.WebForms" Namespace="ThinkGeo.MapSuite.WebForms" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link href="~/theme/default/samplepic/style.css" rel="stylesheet" type="text/css" />
    <title>Distance Between Two Points</title>
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
                Click the button to show the distance between the two points in the US and Britan.
                <br />
                <br />
                <asp:Button ID="btnGetDistance"  runat="server" Text="Get Distance"
                    OnClick="btnGetDistance_Click" />
                <br />
                <asp:Label ID="DistanceLabel" ForeColor="#0065ce" runat="server" Text=""></asp:Label>
            </Description:DescriptionPanel>
        </ContentTemplate>
    </asp:UpdatePanel>
    </form>
</body>
</html>
