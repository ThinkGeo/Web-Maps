<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CreateASpatialIndexForAShapeFileFeatureLayer.aspx.cs"
    Inherits="HowDoI.Samples.DataProviders.CreateASpatialIndexForAShapeFileFeatureLayer" %>

<%@ Register Assembly="ThinkGeo.MapSuite.WebForms" Namespace="ThinkGeo.MapSuite.WebForms" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <link href="~/theme/default/samplepic/style.css" rel="stylesheet" type="text/css" />
    <title>Create a spatial index for a ShapeFileFeatureLayer</title>
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
                Click the button below to generate spatial index file.
                <br />
                <br />
                <asp:Button ID="btnSpatial" runat="server" Text="Build Spatial Index" OnClick="btnSpatial_Click" />
                <br />
                <br />
                <asp:Label ID="Label1" ForeColor="#0065CE" runat="server"></asp:Label>
                <br />
            </Description:DescriptionPanel>
        </ContentTemplate>
    </asp:UpdatePanel>
    </form>
</body>
</html>
