<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="HowDoI.Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>ThinkGeo Map Suite Web Samples</title>
</head>
<frameset rows="80,*" id="mainFrameSet" framespacing="0" frameborder="0">
        <frame  scrolling="no" id="tab" name="header" src="Header.htm">
        <frameset cols="250,5,*" id="contentFrameSet" frameSpacing="2" frameBorder="0">
            <frame name="navigation" scrolling="auto" src="Navigation.aspx" style="background-color:#e7eff7"></frame>
            <frame name="toggle" noresize="noresize" scrolling="no" src="Toggle.htm"></frame>
            <frameset rows="35,*" id="mainFrame" frameBorder="0" frameSpacing="0">
                <frame name="banner" noresize="noresize" src="" style="border-left:solid 1px #bdd1ec; padding:1px 5px 1px 3px;" scrolling="no" src="Banner.aspx"></frame>
                <frame name="content" src="" scrolling="no" style="border:solid 1px #bdd1ec; border-top-width:0px; padding:1px 3px 3px 1px;"></frame>
            </frameset>
        </frameset>
        <noframes>
            <body>
                <p>This page uses frames, but your browser does not support them.</p>
            </body>
        </noframes>
    </frameset>
</html>
