<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GetTheFeaturesCount.aspx.cs"
    Inherits="HowDoI.Samples.Querying_Vector_Layers.GetTheFeaturesCount" %>

<%@ Register Assembly="ThinkGeo.MapSuite.WebForms" Namespace="ThinkGeo.MapSuite.WebForms" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link href="~/theme/default/samplepic/style.css" rel="stylesheet" type="text/css" />
    <title>Get the Features Count</title>
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
                Click the button to find out how many features are in the feature layer.
                <br />
                <br />
                <asp:Button ID="btnGetCount" runat="server" Text="Get the features count" OnClick="btnGetCount_Click" />
                <br />
                <asp:Label ID="FeatureCountLabel" ForeColor="#0065ce" runat="server" Text=""></asp:Label>
            </Description:DescriptionPanel>
        </ContentTemplate>
    </asp:UpdatePanel>
    </form>
</body>
</html>
