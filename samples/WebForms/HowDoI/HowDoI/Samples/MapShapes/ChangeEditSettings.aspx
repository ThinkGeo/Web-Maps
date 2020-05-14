<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ChangeEditSettings.aspx.cs"
    Inherits="HowDoI.Samples.Features.ChangeEditSettings" %>

<%@ Register Assembly="ThinkGeo.MapSuite.WebForms" Namespace="ThinkGeo.MapSuite.WebForms" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link href="~/theme/default/samplepic/style.css" rel="stylesheet" type="text/css" />
    <title>Change EditSettings</title>

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
                Check the check boxes below and click the shape to see how they affect the shape editing.
                <br />
                <br />
                Edit Settings:<br />
                <table border="0">
                    <tr>
                        <td style="width: 100px;">
                            <asp:CheckBox ID="CheckBoxRotate" AutoPostBack="true" runat="server" OnCheckedChanged="CheckBoxChanged"
                                Text="Rotatable" />
                        </td>
                        <td>
                            <asp:CheckBox ID="CheckBoxResize" AutoPostBack="true" runat="server" OnCheckedChanged="CheckBoxChanged"
                                Text="Resizable" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:CheckBox ID="CheckBoxReshape" AutoPostBack="true" runat="server" OnCheckedChanged="CheckBoxChanged"
                                Text="Reshapable" />
                        </td>
                        <td>
                            <asp:CheckBox ID="CheckBoxDrag" AutoPostBack="true" runat="server" OnCheckedChanged="CheckBoxChanged"
                                Text="Draggable" />
                        </td>
                    </tr>
                </table>
            </Description:DescriptionPanel>
        </ContentTemplate>
    </asp:UpdatePanel>
    </form>
</body>
</html>
