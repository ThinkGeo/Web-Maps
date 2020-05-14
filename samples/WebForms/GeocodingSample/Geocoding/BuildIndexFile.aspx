<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="BuildIndexFile.aspx.cs" Inherits="ThinkGeo.MapSuite.HowDoI.BuildIndexFile" %>




<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
 <link href="SampleFramework/theme/default/samplepic/style.css" rel="stylesheet" type="text/css" />
    <title></title>
   
</head>
<body>
    <form id="form1" runat="server">
     <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
   <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
      
          <cc1:Map ID="map1" runat="server" Height="100%" Width="100%">
            </cc1:Map>
  <Description:DescriptionPanel ID="descriptionPanel1" Height="600" runat="server">
 This sample demonstrates how to build index file based on a text file.
    <br />
             <br />
                            <asp:Button ID="btnBuild" runat="server" onclick="btnSearch_Click" 
                      Text="Build Index File" />
                           
                   
              <br />
             <br />
              <div style="height:500px; overflow:scroll;">
            <asp:GridView ID="dataGridViewDetail" Width="245px" runat="server">
            
            </asp:GridView>
            </div>
            </Description:DescriptionPanel>
        
          
     
  </ContentTemplate>
  </asp:UpdatePanel>
     
    </form>
</body>
</html>
