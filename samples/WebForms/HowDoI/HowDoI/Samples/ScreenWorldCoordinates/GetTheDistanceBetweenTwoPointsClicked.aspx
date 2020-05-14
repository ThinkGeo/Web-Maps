<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GetTheDistanceBetweenTwoPointsClicked.aspx.cs"
    Inherits="HowDoI.GetTheDistanceBetweenTwoPointsClicked" %>

<%@ Register Assembly="ThinkGeo.MapSuite.WebForms" Namespace="ThinkGeo.MapSuite.WebForms" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <link href="~/theme/default/samplepic/style.css" rel="stylesheet" type="text/css" />
    <title>Get Distance Between Two Points</title>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="Map1_Script" runat="server">
    </asp:ScriptManager>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <cc1:Map ID="Map1" runat="server" Width="100%" Height="100%" OnClick="Map1_Click">
            </cc1:Map>
        </ContentTemplate>
    </asp:UpdatePanel>
    <Description:DescriptionPanel ID="Description" runat="server">
        Click two different places on the map to see the distance between them.
    </Description:DescriptionPanel>
    </form>
</body>
</html>
