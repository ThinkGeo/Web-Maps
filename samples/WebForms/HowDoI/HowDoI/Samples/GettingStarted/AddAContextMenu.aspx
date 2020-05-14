<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AddAContextMenu.aspx.cs"
    Inherits="HowDoI.Samples.AddAContextMenu" %>

<%@ Register Assembly="ThinkGeo.MapSuite.WebForms" Namespace="ThinkGeo.MapSuite.WebForms" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <link href="~/theme/default/samplepic/style.css" rel="stylesheet" type="text/css" />
    <title>Add a Context Menu</title>

    <script type="text/javascript">
        function popupInfomation(){
            alert("Infomation popup by ContextMenu");
        }
    </script>

</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="Map1_Script" runat="server">
    </asp:ScriptManager>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <cc1:Map ID="Map1" runat="server" Width="100%" Height="100%">
            </cc1:Map>
        </ContentTemplate>
    </asp:UpdatePanel>
    <Description:DescriptionPanel ID="DescPanel" runat="server">
        Right click on the map to show the context menu and select an item.
        <br />
        <br />
        <asp:UpdatePanel ID="UpdatePanel2" runat="server">
            <ContentTemplate>
                World-X :
                <input id="Longitude" style="width: 120px;" runat="server" type="text" value="" disabled="disabled"
                    class="txt_normal" /><br />
                World-Y :
                <input id="Latitude" style="width: 120px;" runat="server" type="text" value="" disabled="disabled"
                    class="txt_normal" /></ContentTemplate>
        </asp:UpdatePanel>
    </Description:DescriptionPanel>
    </form>
</body>
</html>

