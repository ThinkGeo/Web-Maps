<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Banner.aspx.cs" Inherits="HowDoI.Banner" %>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>banner</title>
    <style type="text/css">
        BODY
        {
            margin: 0;
        }
        A:link, A:visited
        {
            color: #FFFFFF;
            text-decoration: none;
        }
        A:hover
        {
            text-decoration: underline;
        }
        .tab:link, .tab:visited
        {
            display: block;
            float: right;
            width: 80px;
            padding: 1px 5px;
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
        .tab:hover, .tab:active
        {
            text-decoration: none;
            color: #fff;
        }
        .contentTitle
        {
            margin: 1px 0px 1px 0px;
            padding: 3px;
            border: 1px solid #D9E6F2;
            background-color:#e7eff7;
            vertical-align: middle;
            line-height: normal;
        }
        #pageTitle
        {
            color: #0066a7;
            font-family: verdana;
            font-weight: bold;
            font-size: 13px;
            text-align: left;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div class="contentTitle">
        <table width="95%" border="0" cellspacing="0">
            <tr>
                <td id="pageTitle" runat="server">
                </td>
                <td align="right" valign="bottom">
                    <a id="ViewOnGitHub" runat="server" class="tab" style="width: 100px" href="#" target="_blank">View In GitHub</a>
                    <a id="CSharp" runat="server" class="tab" href="#" target="content">C# Source</a>
                    <a id="Sample" runat="server" class="tab" href="#" target="content">Sample</a>
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
