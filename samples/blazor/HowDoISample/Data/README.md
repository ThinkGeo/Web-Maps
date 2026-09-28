# Sample data

Where the data under this folder comes from.

| data | used by | source |
|---|---|---|
| `countries/`, `place/`, `rivers/` | Load Feature Source | Natural Earth 4.1.0 (public domain) |
| `Shapefile/Countries02` | Render Based on Rules | World countries with ISO codes and currency, ThinkGeo sample data |
| `USStates`, `usStatesCensus2010` | Spatial Query, US Demographic Map | US states with US Census attributes, ThinkGeo sample data |
| `Shapefile/Parks`, `Streets`, `Hotels` | Render Points, Lines and Polygons | City of Frisco, Texas open data (EPSG:2276) |
| `Shapefile/Frisco_Coyote_Sightings` | Heatmap | City of Frisco coyote-sighting reports (EPSG:2276); reporter names and contacts removed, only the sighting attributes remain |
| `Csv/Frisco_Mosquitos.csv` | Isolines | Mosquito trap counts around Frisco, Web Mercator |
| `Csv/vehicle-route.csv` | Vehicle Navigation | A GPS trace through Lower Manhattan |
| `Ndfd/ds.wspd.tif`, `ds.wdir.tif` | Wind as a Vector Field | NOAA National Digital Forecast Database wind speed and direction, converted from GRIB (public domain) |
| `OpenDrive/` | Lane-Level Navigation | DLR HD map of Wolfsburg, CC BY 4.0 - see `OpenDrive/README.md` |
| `GeoTiff/World.tif`, `World.ecw`, `World2.jpg` | Project a Raster, Load Raster Source | ThinkGeo sample world imagery |
| `Data.xml` | Draw and Modify Geometries | Hand-drawn sample shapes |
