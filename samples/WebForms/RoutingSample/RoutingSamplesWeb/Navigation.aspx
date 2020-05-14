<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Navigation.aspx.cs" Inherits="ThinkGeo.MapSuite.RoutingSamples.Navigation" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Navigation Page</title>
    <style type="text/css">
        .tab
        {
            text-align: center;
            margin-left: 8px;
            background: #b1e7ff;
            color: #003c5f;
            border: 1px solid #003c5f;
            border-left: 1px solid #fff;
            border-top: 1px solid #fff;
            font-family: Arial, Helvetica, sans-serif;
            font-size: 12px;
            font-weight: bold;
        }
        .txt
        {
            border: 1px solid #003c5f;
            margin-left: 5px;
        }
    </style>

    <script type="text/javascript">
        var oldLink = null;

        function chgColor(e) {
            var clickObj;
            if (window.event && window.event.srcElement.tagName.toUpperCase() == 'A') {
                clickObj = window.event.srcElement;
            } else if (e && e.target.tagName.toUpperCase() == 'A') {
                clickObj = e.target;
            }

            if (clickObj) {
                if (clickObj == oldLink) return;
                clickObj.style.color = "red";
                if (oldLink) oldLink.style.color = "black";
                oldLink = clickObj;
            }
        }

        function hidenTree() {
            document.getElementById("TreeViewContainer").style.display = "none";
        }

        function keydownFilter() {
            if (event.keyCode == 13) {
                document.getElementById('<%=Button1.ClientID%>').click();
                return false;
            }
            return true;
        }

        document.onclick = chgColor;
    </script>

</head>
<body style="background-color: #e7eff7; margin: 0px; padding: 4px;" onclick="chgColor()"
    onkeydown="return keydownFilter();">
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <div>
        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
            <ContentTemplate>
                <asp:TextBox ID="filter" Width="160px" runat="server" CssClass="txt"></asp:TextBox>
                <asp:Button ID="Button1" runat="server" Text="Filter" Width="60px" CssClass="tab"
                    OnClick="Button1_Click" OnClientClick="hidenTree();" />
                <hr />
                <div id="TreeViewContainer" runat="server">
                    <asp:TreeView ID="SampleListTreeView" ExpandDepth="1" runat="server" Font-Size="8"
                        ForeColor="black" NodeIndent="10" Font-Names="verdana">
                        <ParentNodeStyle Font-Bold="True" />
                        <RootNodeStyle Font-Bold="True" />
                    </asp:TreeView>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
        <asp:UpdateProgress AssociatedUpdatePanelID="UpdatePanel1" ID="UpdateProgress1" runat="server">
            <ProgressTemplate>
                <div style="font-size: 12px; margin-left: 20px; font-weight: bold; position: absolute;
                    color: Red; top: 40px; z-index: 999;">
                    Loading...
                </div>
            </ProgressTemplate>
        </asp:UpdateProgress>
    </div>
    </form>
</body>
</html>
