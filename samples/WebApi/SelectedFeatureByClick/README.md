# Select Feature By Click Sample for WebApi

### Description
The purpose of this project is to show the technique for finding the feature the user clicked on. To give the user the expected behavior, a buffer in screen coordinates needs to be set so that the feature gets selected within a constant distance in screen coordinates to where the user clicked, regardless of the zoom level.

Please refer to [Wiki](http://wiki.thinkgeo.com/wiki/map_suite_web_for_webapi) for the details.

![Screenshot](https://github.com/ThinkGeo/SelectFeatureByClickSample-ForWebApi/blob/master/Screenshot.png)

### Requirements
This sample makes use of the following NuGet Packages

[MapSuite 10.0.0](https://www.nuget.org/packages?q=ThinkGeo)

### About the Code
```csharp
public HttpResponseMessage GetRoad([FromBody] JObject jObject)
{
    // get parameters
    double x = jObject.Property("x").Value.ToObject<double>();
    double y = jObject.Property("y").Value.ToObject<double>();
    double radius = jObject.Property("radius").Value.ToObject<double>();
    // get nearest feature
    string shapePath = HttpContext.Current.Server.MapPath("~/App_Data/Austinstreets.shp");
    var source = new ShapeFileFeatureSource(shapePath);
    source.Open();
    var features = source.GetFeaturesNearestTo(
        new PointShape(x, y),
        GeographyUnit.DecimalDegree,
        1,
        new string[1] { "NAME" },
        radius,
        DistanceUnit.Meter
        );

    // return response
    var result = new JObject();
    if (features.Count > 0)
    {
        string roadName = features[0].ColumnValues["NAME"];
        result.Add("name", JToken.FromObject(roadName));
        result.Add("success", JToken.FromObject(true));
    }
    else
    {
        result.Add("success", JToken.FromObject(false));
    }
            
    var response = new HttpResponseMessage(HttpStatusCode.OK);
    response.Content = new StringContent(result.ToString(), Encoding.UTF8, "application/json");

    return response;
}
```
### Getting Help

[Map Suite WebAPI Wiki Resources](http://wiki.thinkgeo.com/wiki/map_suite_web_for_webapi)

[Map Suite WebAPI Product Description](https://thinkgeo.com/ui-controls#web-platforms)

[ThinkGeo Community Site](http://community.thinkgeo.com/)

[ThinkGeo Web Site](http://www.thinkgeo.com)

### Key APIs
This example makes use of the following APIs:

- [ThinkGeo.MapSuite.Drawing.GeoCanvas](http://wiki.thinkgeo.com/wiki/api/thinkgeo.mapsuite.drawing.geocanvas)
- [ThinkGeo.MapSuite.Layers.ShapeFileFeatureLayer](http://wiki.thinkgeo.com/wiki/api/thinkgeo.mapsuite.layers.shapefilefeaturelayer)
- [ThinkGeo.MapSuite.WebApi.LayerOverlay](http://wiki.thinkgeo.com/wiki/api/thinkgeo.mapsuite.webapi.layeroverlay)

### About Map Suite
Map Suite is a set of powerful development components and services for the .Net Framework.

### About ThinkGeo
ThinkGeo is a GIS (Geographic Information Systems) company founded in 2004 and located in Frisco, TX. Our clients are in more than 40 industries including agriculture, energy, transportation, government, engineering, software development, and defense.
