<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UseRotationProjectionForAFeatureLayer.aspx.cs"
    Inherits="HowDoI.Samples.Projection.UseRotationProjectionForAFeatureLayer" %>

<%@ Register Assembly="ThinkGeo.MapSuite.WebForms" Namespace="ThinkGeo.MapSuite.WebForms" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link href="~/theme/default/samplepic/style.css" rel="stylesheet" type="text/css" />
    <title>Use RotationProjection for a feature layer</title>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="Map1_Script" runat="server">
    </asp:ScriptManager>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <cc1:Map ID="Map1" runat="server" Width="100%" Height="100%">
            </cc1:Map>
            <Description:DescriptionPanel ID="Description" runat="server">
                This sample applies rotation projection to a feature layer.
                <br />
                <table>
                    <tr>
                        <td>
                            <asp:Button ID="btnRotateCounterclockwise" runat="server" Text="Rotate Counterclockwise"
                                OnClick="btnRotateCounterclockwise_Click" Width="160px" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Button ID="btnRotateClockwise" runat="server" Text="Rotate Clockwise" OnClick="btnRotateClockwise_Click"
                                Width="160px" />
                        </td>
                    </tr>
                </table>
            </Description:DescriptionPanel>
        </ContentTemplate>
    </asp:UpdatePanel>
    </form>
</body>
</html>
