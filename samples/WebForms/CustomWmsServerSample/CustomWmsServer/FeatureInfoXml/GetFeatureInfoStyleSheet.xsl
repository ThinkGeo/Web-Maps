<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl"
>
    <xsl:output method="html" indent="yes"/>
    <xsl:template match="/">
        <html>
            <head>
                <title>WmsServer GetFeatureInfo output</title>
            </head>
            <body>
                <table cellpadding="0" cellspacing="0" border="1" style="border-collapse:collapse;font-size:14px;">
                    <caption>FeatureInfo</caption>
                    <thead>
                        <tr style="background-color:#ccc">
                            <th>FeatureType</th>
                            <th>LayerName</th>
                        </tr>
                    </thead>
                    <tbody>
                        <xsl:for-each select="FeatureInfoResponses/FeatureInfo">
                            <tr>
                                <td>
                                    <xsl:value-of select="FeatureType"/>
                                </td>
                                <td>
                                    <xsl:value-of select="LayerName"/>
                                </td>
                            </tr>
                        </xsl:for-each>
                    </tbody>
                </table>
                <br/>
            </body>
        </html>
    </xsl:template>
</xsl:stylesheet>
