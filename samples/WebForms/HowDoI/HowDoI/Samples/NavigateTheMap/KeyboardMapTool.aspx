<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="KeyboardMapTool.aspx.cs"
    Inherits="HowDoI.Samples.KeyboardMapTool" %>

<%@ Register Assembly="ThinkGeo.MapSuite.WebForms" Namespace="ThinkGeo.MapSuite.WebForms" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <link href="~/theme/default/samplepic/style.css" rel="stylesheet" type="text/css" />
    <title>Keyboard Map Tool</title>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="Map1_Script" runat="server">
    </asp:ScriptManager>
    <cc1:Map ID="Map1" runat="server" Width="100%" Height="100%">
    </cc1:Map>
    <Description:DescriptionPanel ID="DescPanel" runat="server">
        Try the keys in the table below to operate the map.
        <br />
        <br />
        <table cellpadding="2" cellspacing="2" border="1" width="250">

            <script type="text/javascript">
                var script = '';
                var content = [];
                content.push('Left Arrow:Pan left with the offset specified by PanPixels');
                content.push('Right Arrow:Pan right with the offset specified by PanPixels');
                content.push('Up Arrow:Pan up with the offset specified by PanPixels');
                content.push('Down Arrow:Pan down with the offset specified by PanPixels');
                content.push('Page Up:Pan up with 0.75*Map.Height offset');
                content.push('Page Down:Pan down with 0.75*Map.Height offset');
                content.push('Home:Pan left with 0.75*Map.Width offset');
                content.push('End:Pan right with 0.75*Map.Width offset');
                content.push('+:Zoom In');
                content.push('-:zoom out');
                
                for(var i=0; i<content.length; i++){
                    script += '<tr>';
                    script += '<td>' + content[i].split(':')[0] + '</td>'
                    script += '<td>' + content[i].split(':')[1] + '</td>'
                    script += '</tr>';
                }
                
                document.write(script);
            </script>
        </table>
    </Description:DescriptionPanel>
    </form>
</body>
</html>
