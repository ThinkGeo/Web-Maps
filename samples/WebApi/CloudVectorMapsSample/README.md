# ThinkGeo Cloud Vector Maps Sample for WebApi

### Description

This sample demonstrates how you can draw the map with Vector Tiles requested from ThinkGeo Cloud Services in your Map Suite GIS applications, with any style you want from [StyleJSON (Mapping Definition Grammar)](https://wiki.thinkgeo.com/wiki/thinkgeo_stylejson). It will show you how to use the XyzFileBitmapTileCache and XyzFileVectorTileCache to improve the performance of map rendering. It supports 3 built-in default map styles and more awasome styles from StyleJSON file you passed in, by 'Custom': 
- Light
- Dark
- TransparentBackground
- Custom

ThinkGeo Cloud Vector Maps support would work in all of the Map Suite controls such as Wpf, Web, MVC, WebApi, Android and iOS.

Please refer to [Wiki](https://wiki.thinkgeo.com/wiki/map_suite_web_for_webapi) for the details.

![Screenshot](https://github.com/ThinkGeo/ThinkGeoCloudVectorMapsSample-ForWebApi/blob/master/Screenshot.gif)

### Requirements
This sample makes use of the following NuGet Packages

[MapSuite 10.5.0](https://www.nuget.org/packages?q=ThinkGeo)

### About the Code
```csharp
 [Route("tile/{mapType}/{z}/{x}/{y}")]
 [HttpGet]
 public HttpResponseMessage GetTile(string mapType, int z, int x, int y)
 {

     LayerOverlay layerOverlay = new LayerOverlay();
     ThinkGeoCloudVectorMapsFeatureLayer thinkGeoCloudVectorMapsFeatureLayer = null;

     if (mapType == "light")
     {
         thinkGeoCloudVectorMapsFeatureLayer = new ThinkGeoCloudVectorMapsFeatureLayer(cloudServiceClientId, cloudServiceClientSecret);
     }
     else if (mapType == "dark")
     {
         thinkGeoCloudVectorMapsFeatureLayer = new ThinkGeoCloudVectorMapsFeatureLayer(cloudServiceClientId, cloudServiceClientSecret, ThinkGeoCloudVectorMapsMapType.Dark);
     }
     else if (mapType == "transparentBackground")
     {
         thinkGeoCloudVectorMapsFeatureLayer = new ThinkGeoCloudVectorMapsFeatureLayer(cloudServiceClientId, cloudServiceClientSecret, ThinkGeoCloudVectorMapsMapType.TransparentBackground);
     }
     else if (mapType == "custom")
     {
         string styleJsonFilePath = Path.Combine(baseDirectory, "AppData", "mutedblue.json");
         thinkGeoCloudVectorMapsFeatureLayer = new ThinkGeoCloudVectorMapsFeatureLayer(cloudServiceClientId, cloudServiceClientSecret, new Uri(styleJsonFilePath, UriKind.Relative));
     }

     var bitmapTileCache = new XyzFileBitmapTileCache(Path.Combine(baseDirectory, "Cache", "ImageTileCache"), $"{thinkGeoCloudVectorMapsFeatureLayer.MapType}_{3857}", ThinkGeoCloudVectorMapsFeatureLayer.GetZoomLevelSet(), new MapSuiteTileMatrix(295829515.16123772, 512, 512, GeographyUnit.Meter, thinkGeoCloudVectorMapsFeatureLayer.GetTileMatrixBoundingBox()));
     thinkGeoCloudVectorMapsFeatureLayer.BitmapTileCache = bitmapTileCache;
     var vectorTileCache = new FileVectorTileCache(Path.Combine(baseDirectory, "Cache", "VectorTileCache"), "3857");
     thinkGeoCloudVectorMapsFeatureLayer.VectorTileCache = vectorTileCache;

     layerOverlay.SetTileMatrix(thinkGeoCloudVectorMapsFeatureLayer.GetTileMatrixBoundingBox(), GeographyUnit.Meter);
     layerOverlay.Layers.Add(thinkGeoCloudVectorMapsFeatureLayer);

     return DrawTileImage(layerOverlay, x, y, z);
 }

 private static HttpResponseMessage DrawTileImage(LayerOverlay layerOverlay, int x, int y, int z)
 {
     using (Bitmap bitmap = new Bitmap(512, 512))
     {
         PlatformGeoCanvas geoCanvas = new PlatformGeoCanvas();
         //RectangleShape boundingBox = WebApiExtentHelper.GetBoundingBoxForXyz(x, y, z, layerOverlay.GetTileMatrix().TileWidth, layerOverlay.GetTileMatrix().TileHeight, GeographyUnit.Meter, ThinkGeoCloudVectorMapsFeatureLayer.GetZoomLevelSet(), layerOverlay.GetTileMatrix());
         RectangleShape boundingBox = WebApiExtentHelper.GetBoundingBoxForXyz(x, y, z, GeographyUnit.Meter);
         geoCanvas.BeginDrawing(bitmap, boundingBox, GeographyUnit.Meter);

         if (layerOverlay != null)
         {
             layerOverlay.Draw(geoCanvas);
         }
         geoCanvas.EndDrawing();

         MemoryStream memoryStream = new MemoryStream();
         bitmap.Save(memoryStream, ImageFormat.Png);

         HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.OK)
         {
             Content = new ByteArrayContent(memoryStream.ToArray())
         };
         message.Content.Headers.ContentType = new MediaTypeHeaderValue("image/png");

         return message;
     }
 }
```
### Getting Help

[Map Suite Desktop for WebAPI Wiki Resources](https://wiki.thinkgeo.com/wiki/map_suite_web_for_webapi)

[Map Suite Desktop for WebAPI Product Description](https://thinkgeo.com/gis-ui-web#features)

[ThinkGeo Community Site](http://community.thinkgeo.com/)

[ThinkGeo Web Site](http://www.thinkgeo.com)

### Key APIs
This example makes use of the following APIs:

Working...


### About Map Suite
Map Suite is a set of powerful development components and services for the .Net Framework.

### About ThinkGeo
ThinkGeo is a GIS (Geographic Information Systems) company founded in 2004 and located in Frisco, TX. Our clients are in more than 40 industries including agriculture, energy, transportation, government, engineering, software development, and defense.
