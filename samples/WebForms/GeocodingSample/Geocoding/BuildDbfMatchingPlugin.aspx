<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="BuildDbfMatchingPlugin.aspx.cs" Inherits="HowDoISamples.BuildDbfMatchingPlugin" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc2" %>


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
Select or type in a country. We will match that county name in the created index file.
      <br />
      <br />
      <asp:Button ID="btnBuildIndex" runat="server" onclick="btnBuildIndex_Click" 
          Text="Build Index File" />
      <br />
<br />
  <cc2:ComboBox ID="cboSourceText"   runat="server" DropDownStyle="DropDown">
      <asp:ListItem >China</asp:ListItem>
<asp:ListItem>Australia</asp:ListItem>
<asp:ListItem>Mexico</asp:ListItem>
<asp:ListItem>France</asp:ListItem>
<asp:ListItem>Germany</asp:ListItem>
<asp:ListItem>Italy</asp:ListItem>
<asp:ListItem>Poland</asp:ListItem>
<asp:ListItem>Argentina</asp:ListItem>
      </cc2:ComboBox>

   <asp:Button  id="btnSearch" Text="Search" runat="server" 
                  onclick="btnSearch_Click" />
<br />
<br />

       <asp:ListBox AutoPostBack="true" ID="lstResult" OnSelectedIndexChanged="lstResult_SelectedIndexChanged" Width="245px" Height="150" runat="server"></asp:ListBox>

<br />
<br />

<div style="height:350px; overflow:scroll;">
  <asp:GridView ID="dataGridViewDetail" Width="245px"  runat="server"  AutoGenerateColumns="False">
            
                <Columns>
                    <asp:BoundField DataField="Key" HeaderText="Name" />
                    <asp:BoundField DataField="Value" HeaderText="Value" />
                </Columns>
                
            </asp:GridView>
            </div>

     
   
            
            
            </Description:DescriptionPanel>
        
  
     
  </ContentTemplate>
  </asp:UpdatePanel>
               <iframe id="ifraKeepSessionAlive" src="~/SampleFramework/KeepSessionAlive.aspx" visible="false" frameborder="0" width="0" height="0" runat="server"></iframe>

    </form>
</body>
</html>
