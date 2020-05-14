<%@ Page Language="C#" MasterPageFile="Site1.Master" AutoEventWireup="true" CodeBehind="UsingMasterPage.aspx.cs" Inherits="HowDoI.Samples.UsingMasterPage" Title="Using Master Page" %>

<%@ Register Assembly="ThinkGeo.MapSuite.WebForms" Namespace="ThinkGeo.MapSuite.WebForms" TagPrefix="cc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <cc1:Map ID="Map1" runat="server" Height="100%" Width="100%">
    </cc1:Map>
</asp:Content>
