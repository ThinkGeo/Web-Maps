<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="ThinkGeo.MapSuite.HowDoI.Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
    <head runat="server">
        <title>ThinkGeo MapSuite Geocoder Web Samples</title>

        <!-- Hotjar Tracking Code for https://thinkgeo.com/ -->
        <script>
            (function(h,o,t,j,a,r){
                h.hj=h.hj||function(){(h.hj.q=h.hj.q||[]).push(arguments)};
                h._hjSettings={hjid:839459,hjsv:6};
                a=o.getElementsByTagName('head')[0];
                r=o.createElement('script');r.async=1;
                r.src=t+h._hjSettings.hjid+j+h._hjSettings.hjsv;
                a.appendChild(r);
            })(window,document,'https://static.hotjar.com/c/hotjar-','.js?sv=');
        </script>

        <!-- Global site tag (gtag.js) - Google Analytics -->
        <script async src="https://www.googletagmanager.com/gtag/js?id=UA-300816-11"></script>
        <script>
          window.dataLayer = window.dataLayer || [];
          function gtag(){dataLayer.push(arguments);}
          gtag('js', new Date());

          gtag('config', 'UA-300816-11');
        </script>
    </head>
   <frameset rows="80,*" id="mainFrameSet" framespacing="0" frameborder="0">
        <frame  scrolling="no" id="tab" name="header" src="SampleFramework/Header.htm">
        <frameset cols="250,5,*" id="contentFrameSet" frameSpacing="2" frameBorder="0">
            <frame name="navigation" scrolling="auto" src="SampleFramework/Navigation.aspx" style="background-color:#e7eff7"></frame>
            <frame name="toggle" noresize="noresize" scrolling="no" src="SampleFramework/Toggle.htm"></frame>
            <frameset rows="35,*" id="mainFrame" frameBorder="0" frameSpacing="0">
                <frame name="banner" noresize="noresize" src="" style="border-left:solid 1px #bdd1ec; padding:1px 5px 1px 3px;" scrolling="no" src="SampleFramework/Banner.aspx"></frame>
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
