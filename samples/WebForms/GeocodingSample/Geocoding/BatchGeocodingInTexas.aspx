<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="BatchGeocodingInTexas.aspx.cs" Inherits="ThinkGeo.MapSuite.HowDoI.BatchGeocodingInTexas" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title></title>
         <link href="SampleFramework/theme/default/samplepic/style.css" rel="stylesheet" type="text/css" />

</head>
<body>
   <form id="form1" runat="server">
     <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
   <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
      
          <cc1:Map ID="map1" runat="server" Height="100%" Width="100%">
            </cc1:Map>
             </ContentTemplate>
  </asp:UpdatePanel>
  <Description:DescriptionPanel ID="descriptionPanel1" Height="600" runat="server">
 <p>This sample demonstrates batch geocoding in Texas street addresses. It would load houndreds of street addresses
 from TXT file and geocode them to get information.</p>
 <table>
 <tr>
 <td>Total Record Count</td><td><asp:TextBox  ID="txtTotalRecordCount" ReadOnly="true" runat="server"></asp:TextBox></td>
 </tr>
 <tr>
 <td>Success Rate</td><td><asp:TextBox ReadOnly="true" runat="server" ID="txtSuccessRate"> </asp:TextBox></td>
 </tr>
 <tr>
 <td>Total Time</td><td><asp:TextBox ReadOnly="true" ID="txtTotalTime" runat="server"></asp:TextBox></td>
 </tr>
 <tr>
 <td>Time Per Rec</td><td><asp:TextBox ID="txtTimePerRec" ReadOnly="true" runat="server"></asp:TextBox></td>
 </tr>
  <tr>
 <td>Records Per Second</td><td><asp:TextBox ID="txtRecPerSecond" ReadOnly="true" runat="server"></asp:TextBox></td>
 </tr>
 <tr>
 <td colspan="2"> 
     <asp:Button id="btnBatchGeocode" Text="Batch Geocode" runat="server" 
                  onclick="btnBatchGeoCode_Click" /></td>
 </tr>
 </table>
     
            <div style="height:350px; overflow:scroll;">
            <asp:GridView  ID="dataGridViewDetail" runat="server" Width="245px"  AutoGenerateColumns="False" 
                     BorderStyle="Solid" BorderWidth="1"                      
                     onrowdatabound="dataGridViewDetail_RowDataBound" 
                     onselectedindexchanged="dataGridViewDetail_SelectedIndexChanged">
             
                <Columns>
                    <asp:BoundField DataField="Key" HeaderText="Address" />
                    <asp:BoundField DataField="Value" HeaderText="Location" />
                </Columns>
                <SelectedRowStyle BackColor="Blue" />
            </asp:GridView>
            </div>
            </Description:DescriptionPanel>
        
          
       <iframe id="ifraKeepSessionAlive" src="~/SampleFramework/KeepSessionAlive.aspx" visible="false" frameborder="0" width="0" height="0" runat="server"></iframe>

  
     
    </form>
</body>
</html>
