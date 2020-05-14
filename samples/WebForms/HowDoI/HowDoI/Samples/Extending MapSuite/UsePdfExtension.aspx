<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UsePdfExtension.aspx.cs"
    Inherits="HowDoI.Samples.Extending_MapSuite.UsePdfExtension" %>

<%@ Register Assembly="ThinkGeo.MapSuite.WebForms" Namespace="ThinkGeo.MapSuite.WebForms" TagPrefix="cc1" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link href="~/theme/default/samplepic/style.css" rel="stylesheet" type="text/css" />
    <title>Print into PDF</title>

</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <cc1:Map ID="Map1" runat="server" Height="100%" Width="100%">
    </cc1:Map>
    <Description:DescriptionPanel ID="DescPanel" runat="server">
        Select option from the RadioButton list below, then click button to print current
        map into PDF document.
        <br />
        <br />
        <asp:RadioButtonList ID="rdlOptions" Font-Size="10" runat="server" RepeatDirection="Horizontal">
            <asp:ListItem Text="Landscape" Selected="True"></asp:ListItem>
            <asp:ListItem Text="Portrait"></asp:ListItem>
        </asp:RadioButtonList>
        <br />
        <asp:Button ID="btnToPdf" runat="server" Text="Print into PDF" OnClick="btnToPdf_Click" />
    </Description:DescriptionPanel>
    </form>
</body>
</html>
