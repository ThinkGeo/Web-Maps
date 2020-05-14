<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CreateAScaleLineAdornmentLayer.aspx.cs"
    Inherits="HowDoI.Samples.NavigateTheMap.CreateAScaleLineAdornmentLayer" %>

<%@ Register Assembly="ThinkGeo.MapSuite.WebForms" Namespace="ThinkGeo.MapSuite.WebForms" TagPrefix="cc1" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <link href="~/theme/default/samplepic/style.css" rel="stylesheet" type="text/css" />
    <title>Create a ScaleLineAdornmentLayer</title>
    <style type="text/css">
        html
        {
            height: 100%;
        }
        .descContent
        {
            font-family: :Verdana;
            font-size: 11px;
        }
        input
        {
            margin: 3px;
        }
    </style>
</head>
<body style="height: 100%;">
    <form id="form1" runat="server" style="height: 100%;">
    <asp:ScriptManager ID="Map1_Script" runat="server">
    </asp:ScriptManager>
    <asp:UpdatePanel ID="UpdatePanel" runat="server">
        <ContentTemplate>
            <cc1:Map ID="Map1" runat="server" Width="100%" Height="100%">
            </cc1:Map>
        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="ddlScaleLineLocation" />
        </Triggers>
    </asp:UpdatePanel>
    <Description:DescriptionPanel ID="DescPanel" runat="server">
        <table border="0" cellpadding="0" cellspacing="0">
            <tr>
                <td>
                    <span class="descContent">Select location for ScaleLine</span>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:DropDownList ID="ddlScaleLineLocation" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlScaleLineLocation_SelectedIndexChanged">
                        <asp:ListItem Text="UpperLeft" Value="UpperLeft"></asp:ListItem>
                        <asp:ListItem Text="UpperCenter" Value="UpperCenter"></asp:ListItem>
                        <asp:ListItem Text="UpperRight" Value="UpperRight"></asp:ListItem>
                        <asp:ListItem Text="CenterLeft" Value="CenterLeft"></asp:ListItem>
                        <asp:ListItem Text="Center" Value="Center"></asp:ListItem>
                        <asp:ListItem Text="CenterRight" Value="CenterRight"></asp:ListItem>
                        <asp:ListItem Text="LowerLeft" Value="LowerLeft"></asp:ListItem>
                        <asp:ListItem Text="LowerCenter" Value="LowerCenter"></asp:ListItem>
                        <asp:ListItem Text="LowerRight" Value="LowerRight"></asp:ListItem>
                    </asp:DropDownList>
                </td>
            </tr>
        </table>
    </Description:DescriptionPanel>
    </form>
</body>
</html>
