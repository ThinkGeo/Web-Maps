<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ConvertToFromWKB.aspx.cs"
    Inherits="HowDoI.Samples.Features.ConvertToFromWKB" %>

<%@ Register Assembly="ThinkGeo.MapSuite.WebForms" Namespace="ThinkGeo.MapSuite.WebForms" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link href="~/theme/default/samplepic/style.css" rel="stylesheet" type="text/css" />
    <title>Convert To/From Well Known Binary</title>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="Map1_Script" runat="server">
    </asp:ScriptManager>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <cc1:Map ID="Map1" runat="server" Width="100%" Height="100%">
            </cc1:Map>
            <Description:DescriptionPanel ID="DescPanel" runat="server">
                Click on the button to convert the shape to and from well known binary.
                <br />
                <br />
                <asp:TextBox ID="txtWKB" runat="server" TextMode="MultiLine" Rows="5" Width="96%"
                    ReadOnly="True" Text="AQMAAAABAAAABQAAAAAAAAAArFrAPQrXo3AqQkAAAAAAAKxawD0K16MwHEVAAAAAAMCxWMA9CtejMBxFQAAAAADAsVjAPQrXo3AqQkAAAAAAAKxawD0K16NwKkJA" />
                <br />
                <asp:Button ID="btnConvert" runat="server" Text="WKB  to  Feature" Width="96%" OnClick="btnConvert_Click" />
            </Description:DescriptionPanel>
        </ContentTemplate>
    </asp:UpdatePanel>
    </form>
</body>
</html>
