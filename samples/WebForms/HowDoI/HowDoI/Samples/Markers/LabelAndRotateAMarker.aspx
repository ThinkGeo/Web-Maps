<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LabelAndRotateAMarker.aspx.cs"
    Inherits="HowDoI.LabelAndRotateAMarker" %>

<%@ Register Assembly="ThinkGeo.MapSuite.WebForms" Namespace="ThinkGeo.MapSuite.WebForms" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link href="~/theme/default/samplepic/style.css" rel="stylesheet" type="text/css" />
    <title>Label and Rotate a Marker</title>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <cc1:Map ID="Map1" runat="server" Width="100%" Height="100%">
            </cc1:Map>
            <Description:DescriptionPanel ID="DescPanel" runat="server">
                Click the checkbox to show a custom drawn arrow on the marker then click the button
                to rotate it.
                <br />
                <br />
                <asp:CheckBox ID="ShowTextCheckBox" runat="server" AutoPostBack="True" OnCheckedChanged="ShowTextCheckBox_CheckedChanged"
                    Text="Show text on the marker" Font-Size="10" />
                <br />
                <br />
                <asp:Button ID="RotateButton" runat="server" Text="Rotate the Marker" OnClick="RotateButton_Click" />
            </Description:DescriptionPanel>
        </ContentTemplate>
    </asp:UpdatePanel>
    </form>
</body>
</html>
