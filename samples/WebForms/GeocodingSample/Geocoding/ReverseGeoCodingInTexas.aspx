<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ReverseGeoCodingInTexas.aspx.cs" Inherits="ThinkGeo.MapSuite.HowDoI.ReverseGeoCodingInTexas" %>


<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
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
This sample demonstrates reverse geocoding in Texas street addresses.
<br />
 
      <asp:UpdatePanel ID="UpdatePanel2" runat="server">
          <ContentTemplate>
              <asp:DropDownList ID="cboSourceText" runat="server" Height="16px">
                  <asp:ListItem Selected="True">42.020431 -87.666757</asp:ListItem>
                  <asp:ListItem>42.017069 -87.672102</asp:ListItem>
                  <asp:ListItem>42.016106 -87.668558</asp:ListItem>
                  <asp:ListItem>42.005451 -87.664937</asp:ListItem>
                  <asp:ListItem>42.011431 -87.678457</asp:ListItem>
                  <asp:ListItem>42.013912 -87.699847</asp:ListItem>
                  <asp:ListItem>42.010031 -87.686357</asp:ListItem>
                  <asp:ListItem>41.999531 -87.686557</asp:ListItem>
                  <asp:ListItem>41.986931 -87.690957</asp:ListItem>
                  <asp:ListItem>41.992631 -87.659456</asp:ListItem>
                  <asp:ListItem>41.995831 -87.672257</asp:ListItem>
                  <asp:ListItem>41.986831 -87.673356</asp:ListItem>
                  <asp:ListItem>41.987531 -87.658456</asp:ListItem>
                  <asp:ListItem>41.980931 -87.670056</asp:ListItem>
                  <asp:ListItem>41.971931 -87.671256</asp:ListItem>
                  <asp:ListItem>41.965644 -87.658961</asp:ListItem>
                  <asp:ListItem>41.958632 -87.653255</asp:ListItem>
                  <asp:ListItem>41.979431 -87.704057</asp:ListItem>
                  <asp:ListItem>41.962231 -87.693656</asp:ListItem>
              </asp:DropDownList>
              <asp:Button ID="btnSearch0" runat="server" onclick="btnSearch_Click" 
                  Text="Search" />
          </ContentTemplate>
      </asp:UpdatePanel>
                   
              <br />
              <br />
      <asp:ListBox AutoPostBack="true" ID="lstResult" OnSelectedIndexChanged="lstResult_SelectedIndexChanged" Width="245px" Height="150" runat="server"></asp:ListBox>
             <br />
             <br />
            <div style="height:350px; overflow:scroll;">
            <asp:GridView ID="dataGridViewDetail" Width="245px" runat="server" AutoGenerateColumns="False">
            
                <Columns>
                    <asp:BoundField DataField="Key" HeaderText="Name" />
                    <asp:BoundField DataField="Value" HeaderText="Value" />
                </Columns>
                
            </asp:GridView>
            </div>
            </Description:DescriptionPanel>
        
            <iframe id="ifraKeepSessionAlive" src="~/SampleFramework/KeepSessionAlive.aspx" visible="false" frameborder="0" width="0" height="0" runat="server"></iframe>
     
     
  </ContentTemplate>
  </asp:UpdatePanel>
     
    </form>
</body>
</html>
