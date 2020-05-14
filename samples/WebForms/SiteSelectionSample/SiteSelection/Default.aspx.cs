/*===========================================
    Backgrounds for this sample are powered by ThinkGeo Cloud Maps and require
    An API Key. These were sent to you via email when you signed up
    with ThinkGeo, or you can register now at https://cloud.thinkgeo.com.
===========================================*/

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Globalization;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using ThinkGeo.MapSuite.Drawing;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Shapes;
using ThinkGeo.MapSuite.Styles;
using ThinkGeo.MapSuite.WebForms;

namespace ThinkGeo.MapSuite.SiteSelection
{
    public partial class Default : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Initialize map
                Map1.MapUnit = GeographyUnit.Meter;
                Map1.ZoomLevelSet = new ThinkGeoCloudMapsZoomLevelSet();
                Map1.MapTools.OverlaySwitcher.Enabled = true;
                Map1.MapTools.OverlaySwitcher.BaseOverlayTitle = "ThinkGeo Cloud Maps: ";
                AddOverlays();
                Map1.CurrentExtent = new RectangleShape(-10788072.2328016, 3923530.35787306, -10769555.4090135, 3906993.24816589);

                // Initialize dropdownlist
                InitializePoiCategories();

                // Do a default search based on the initial candidate site
                BtnApplyClick(null, null);
            }
        }

        protected void BtnApplyClick(object sender, EventArgs e)
        {
            DisplayServiceAreas();
            DisplayQueriedPois();
            RefreshScaleBarUnitSystem();
        }

        protected void ddlCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            ddlCategorySubtype.DataSource = GetCategorySubtypes(ddlCategory.SelectedValue);
            ddlCategorySubtype.DataBind();
            ddlCategorySubtype.SelectedIndex = 0;

            DisplayQueriedPois();
            Map1.DynamicOverlay.Redraw();
        }

        protected void ddlCategorySubtype_SelectedIndexChanged(object sender, EventArgs e)
        {
            DisplayQueriedPois();
        }

        protected void Map1_TrackShapeFinished(object sender, EventArgs e)
        {
            if (Map1.EditOverlay.Features.Count > 0)
            {
                PointShape drawnPoint = Map1.EditOverlay.Features[0].GetShape() as PointShape;

                ShapeFileFeatureLayer restrictedLayer = Map1.DynamicOverlay.Layers["restricted"] as ShapeFileFeatureLayer;
                restrictedLayer.Open();
                Collection<Feature> features = restrictedLayer.QueryTools.GetFeaturesContaining(drawnPoint, ReturningColumnsType.NoColumns);
                restrictedLayer.Close();

                if (features.Count <= 0) // Drawn feature is out of the supported area (out of Frisco TX.)
                {
                    // Show warning information when the drawn point locate out of restricted layer.
                    ScriptManager.RegisterStartupScript(mapContentPanel, mapContentPanel.GetType(), "Warning", @"$('#dlgErrorMessage').dialog('open');$('.ui-dialog-titlebar').hide();", true);
                }
                else
                {
                    // Draw tracked Point
                    DisplayCandidateSite(drawnPoint);

                    // Draw the result
                    DisplayServiceAreas();
                    DisplayQueriedPois();
                    Map1.DynamicOverlay.Redraw();
                }
                Map1.EditOverlay.Features.Clear();
            }
        }

        protected void repeaterQueryResult_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            ImageButton zoomToImageButton = e.CommandSource as ImageButton;
            if (zoomToImageButton != null)
            {
                string wkt = zoomToImageButton.CommandArgument;
                BaseShape shape = BaseShape.CreateShapeFromWellKnownData(wkt);
                Map1.ZoomTo(shape.GetCenterPoint(), new ZoomLevelSet().ZoomLevel20.Scale);
            }
        }

        protected void TrackShapesCommand(object sender, CommandEventArgs e)
        {
            switch (e.CommandArgument.ToString())
            {
                case "DrawPoint":
                    Map1.EditOverlay.TrackMode = TrackMode.Point;
                    break;

                case "ClearAll":
                    // clear service area and markers
                    InMemoryFeatureLayer serviceAreaLayer = (InMemoryFeatureLayer)Map1.DynamicOverlay.Layers["serviceArea"];
                    serviceAreaLayer.InternalFeatures.Clear();
                    Map1.MarkerOverlay.FeatureSource.InternalFeatures.Clear();

                    // Clear result table
                    repeaterQueryResult.Visible = false;
                    repeaterQueryResult.DataSource = InternalHelper.GetQueryResultDefination();
                    repeaterQueryResult.DataBind();

                    // Clear POI markers on map.
                    Map1.EditOverlay.FeatureSource.InternalFeatures.Clear();
                    InMemoryMarkerOverlay markerOverlay = Map1.CustomOverlays["DrawnPointOverlay"] as InMemoryMarkerOverlay;
                    markerOverlay.FeatureSource.InternalFeatures.Clear();

                    Map1.DynamicOverlay.Redraw();
                    break;

                default:
                    Map1.EditOverlay.TrackMode = TrackMode.None;
                    break;
            }
        }

        private void InitializePoiCategories()
        {
            Collection<string> categories = new Collection<string>();
            foreach (Layer layer in Map1.DynamicOverlay.Layers)
            {
                if (!string.IsNullOrEmpty(layer.Name) && !layer.Name.Equals("Public Facilites") && !layer.Name.Equals("restricted"))
                {
                    categories.Add(layer.Name);
                }
            }
            ddlCategory.DataSource = categories;
            ddlCategory.DataBind();
            ddlCategory.SelectedIndex = 2;

            ddlCategorySubtype.DataSource = GetCategorySubtypes(ddlCategory.Items[2].Value);
            ddlCategorySubtype.DataBind();
            ddlCategorySubtype.SelectedIndex = 0;
        }

        private void RefreshScaleBarUnitSystem()
        {
            ScaleBarAdornmentLayer adornmentLayer = Map1.AdornmentOverlay.Layers["ScaleBar"] as ScaleBarAdornmentLayer;

            DistanceUnit unit = (DistanceUnit)Enum.Parse(typeof(DistanceUnit), ddlDistanceUnit.SelectedValue);
            switch (unit)
            {
                case DistanceUnit.Kilometer:
                    adornmentLayer.UnitFamily = UnitSystem.Metric;
                    break;

                case DistanceUnit.Mile:
                    adornmentLayer.UnitFamily = UnitSystem.Imperial;
                    break;
            }
        }

        private Collection<string> GetCategorySubtypes(string categoryName)
        {
            Collection<string> subtypes = new Collection<string> { "All" };
            if (categoryName.Equals("Hotels", StringComparison.OrdinalIgnoreCase))
            {
                subtypes.Add("1 ~ 100");
                subtypes.Add("100 ~ 150");
                subtypes.Add("150 ~ 200");
                subtypes.Add("200 ~ 300");
                subtypes.Add("300 ~ 400");
                subtypes.Add("400 ~ 500");
                subtypes.Add(">= 500");
            }
            else
            {
                string searchedColumnName = InternalHelper.GetDbfColumnByPoiType(categoryName);

                ShapeFileFeatureLayer queryLayer = Map1.DynamicOverlay.Layers[ddlCategory.SelectedValue] as ShapeFileFeatureLayer;
                queryLayer.Open();
                Collection<DistinctColumnValue> columnValues = queryLayer.FeatureSource.GetDistinctColumnValues(searchedColumnName);
                foreach (DistinctColumnValue value in columnValues)
                {
                    if (!subtypes.Contains(value.ColumnValue) && !string.IsNullOrEmpty(value.ColumnValue))
                    {
                        subtypes.Add(value.ColumnValue);
                    }
                }
            }

            return subtypes;
        }

        private void DisplayCandidateSite(PointShape drawPoint)
        {
            InMemoryMarkerOverlay markerOverlay = Map1.CustomOverlays["DrawnPointOverlay"] as InMemoryMarkerOverlay;
            if (markerOverlay != null)
            {
                markerOverlay.FeatureSource.InternalFeatures.Clear();
                markerOverlay.FeatureSource.InternalFeatures.Add(new Feature(drawPoint));
            }
        }

        private void DisplayServiceAreas()
        {
            InMemoryMarkerOverlay markerOverlay = Map1.CustomOverlays["DrawnPointOverlay"] as InMemoryMarkerOverlay;
            PointShape startLocation = markerOverlay.FeatureSource.InternalFeatures[0].GetShape() as PointShape;

            AccessibleAreaAnalyst analysis;
            if (rbtServiceArea.Checked)
            {
                analysis = new RouteAccessibleAreaAnalyst(startLocation, Map1.MapUnit)
                {
                    StreetShapeFilePathName = MapPath("~/App_Data/Street.shp"),
                    DrivingTimeInMinutes = int.Parse(tbxServiceArea.Text, CultureInfo.InvariantCulture)
                };
            }
            else
            {
                analysis = new BufferAccessibleAreaAnalyst(startLocation, Map1.MapUnit)
                {
                    Distance = double.Parse(tbxDistance.Text),
                    DistanceUnit = (DistanceUnit)Enum.Parse(typeof(DistanceUnit), ddlDistanceUnit.SelectedValue)
                };
            }

            BaseShape calculatedServiceArea = analysis.CreateAccessibleArea();
            InMemoryFeatureLayer serviceAreaLayer = (InMemoryFeatureLayer)Map1.DynamicOverlay.Layers["serviceArea"];
            serviceAreaLayer.InternalFeatures.Clear();
            serviceAreaLayer.Open();
            serviceAreaLayer.InternalFeatures.Add(new Feature(calculatedServiceArea));
            Map1.CurrentExtent = serviceAreaLayer.GetBoundingBox();
        }

        private void DisplayQueriedPois()
        {
            // Get querying Poi layer
            ShapeFileFeatureLayer queriedLayer = Map1.DynamicOverlay.Layers[ddlCategory.SelectedValue] as ShapeFileFeatureLayer;
            queriedLayer.Open();
            // Get all POIs in Service Area
            InMemoryFeatureLayer serviceAreaLayer = (InMemoryFeatureLayer)Map1.DynamicOverlay.Layers["serviceArea"];
            BaseShape calculatedServiceArea = serviceAreaLayer.InternalFeatures[0].GetShape();
            Collection<Feature> featuresInServiceArea = queriedLayer.QueryTools.GetFeaturesWithin(calculatedServiceArea, ReturningColumnsType.AllColumns);
            InMemoryFeatureLayer highlightFeatureLayer = (InMemoryFeatureLayer)Map1.DynamicOverlay.Layers["highlightFeatureLayer"];
            highlightFeatureLayer.InternalFeatures.Clear();
            highlightFeatureLayer.ZoomLevelSet.ZoomLevel01.DefaultPointStyle = queriedLayer.ZoomLevelSet.ZoomLevel10.DefaultPointStyle;

            // Filter the features by specific query.
            Collection<Feature> filteredQueryFeatures = FilterFeaturesByQueryConfiguration(featuresInServiceArea);

            // Prepare the queried result table
            DataTable queriedResult = InternalHelper.GetQueryResultDefination();
            if (filteredQueryFeatures.Count > 0)
            {
                foreach (Feature feature in filteredQueryFeatures)
                {
                    DataRow row = queriedResult.NewRow();
                    row["WKT"] = feature.GetWellKnownText();
                    row["Name"] = feature.ColumnValues["Name"];
                    queriedResult.Rows.Add(row);
                    highlightFeatureLayer.InternalFeatures.Add(feature);
                }
                DisplyQueriedPoiOnMap(filteredQueryFeatures);
            }
            else
            {
                DataRow row = queriedResult.NewRow();
                row["Name"] = "No results found.";
                queriedResult.Rows.Add(row);
            }

            repeaterQueryResult.Visible = true;
            repeaterQueryResult.DataSource = queriedResult;
            repeaterQueryResult.DataBind();
        }

        private void DisplyQueriedPoiOnMap(Collection<Feature> features)
        {
            StringBuilder popupHtml = new StringBuilder("<table>");
            popupHtml.Append("<tr><td colspan='2' class='popupTitle'>[#NAME#]</td></tr>");
            popupHtml.Append("<tr class='popupText'><td>ADDR:</td><td>[#ADDRESS#]</td></tr>");
            popupHtml.Append("<tr><td colspan='2'><div class='hrLine'></div></td></tr>");

            Map1.MarkerOverlay.FeatureSource.Open();
            foreach (var columnValue in features[0].ColumnValues)
            {
                if (!columnValue.Key.Equals("NAME", StringComparison.OrdinalIgnoreCase) && !columnValue.Key.Equals("ADDRESS", StringComparison.OrdinalIgnoreCase))
                {
                    popupHtml.Append(string.Format("<tr class='vehicleTxt'><td>{0} : </td><td>[#{0}#]</td></tr>", columnValue.Key));
                }
            }
            popupHtml.Append("</table>");
            Map1.MarkerOverlay.ZoomLevelSet.ZoomLevel01.DefaultMarkerStyle.Popup.ContentHtml = popupHtml.ToString();
            Map1.MarkerOverlay.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            Map1.MarkerOverlay.FeatureSource.Clear();
            foreach (Feature feature in features)
            {
                Map1.MarkerOverlay.FeatureSource.InternalFeatures.Add(feature);
            }
        }

        private Collection<Feature> FilterFeaturesByQueryConfiguration(IEnumerable<Feature> allFeatures)
        {
            string poiCatagroy = ddlCategory.SelectedValue.Trim();
            string buildType = ddlCategorySubtype.SelectedValue.Trim();
            string queriedColumn = InternalHelper.GetDbfColumnByPoiType(poiCatagroy);

            Collection<Feature> queriedPois = new Collection<Feature>();
            foreach (Feature feature in allFeatures)
            {
                if (buildType.Equals("All", StringComparison.OrdinalIgnoreCase))
                {
                    queriedPois.Add(feature);
                }
                else
                {
                    // Deal with "Hotels" specifically
                    if (poiCatagroy.Equals("Hotels", StringComparison.OrdinalIgnoreCase))
                    {
                        int rooms = int.Parse(feature.ColumnValues[queriedColumn], CultureInfo.InvariantCulture);

                        string[] values = buildType.Split('~');
                        if (values.Length >= 2)
                        {
                            if (int.Parse(values[0], CultureInfo.InvariantCulture) <= rooms &&
                                int.Parse(values[1], CultureInfo.InvariantCulture) >= rooms)
                            {
                                queriedPois.Add(feature);
                            }
                        }
                        else if (values.Length == 1)
                        {
                            int maxValue = int.Parse(values[0].TrimStart(new[] { '>', '=', ' ' }),
                                CultureInfo.InvariantCulture);
                            if (rooms > maxValue)
                            {
                                queriedPois.Add(feature);
                            }
                        }
                    }
                    else if (feature.ColumnValues[queriedColumn].Equals(buildType, StringComparison.OrdinalIgnoreCase))
                    {
                        queriedPois.Add(feature);
                    }
                }
            }

            return queriedPois;
        }

        private void AddOverlays()
        {
            // Please input your ThinkGeo Cloud API Key to enable the background map.
            ThinkGeoCloudRasterMapsOverlay cloudLightMapOverlay = new ThinkGeoCloudRasterMapsOverlay("ThinkGeo Cloud API Key");
            cloudLightMapOverlay.Name = "Light";
            cloudLightMapOverlay.MapType = ThinkGeoCloudRasterMapsMapType.Light;
            Map1.CustomOverlays.Add(cloudLightMapOverlay);

            ThinkGeoCloudRasterMapsOverlay cloudDarkMapOverlay = new ThinkGeoCloudRasterMapsOverlay("ThinkGeo Cloud API Key");
            cloudDarkMapOverlay.Name = "Dark";
            cloudDarkMapOverlay.MapType = ThinkGeoCloudRasterMapsMapType.Dark;
            Map1.CustomOverlays.Add(cloudDarkMapOverlay);

            ThinkGeoCloudRasterMapsOverlay cloudAerialMapOverlay = new ThinkGeoCloudRasterMapsOverlay("ThinkGeo Cloud API Key");
            cloudAerialMapOverlay.Name = "Aerial";
            cloudAerialMapOverlay.MapType = ThinkGeoCloudRasterMapsMapType.Aerial;
            Map1.CustomOverlays.Add(cloudAerialMapOverlay);

            ThinkGeoCloudRasterMapsOverlay cloudHybridMapOverlay = new ThinkGeoCloudRasterMapsOverlay("ThinkGeo Cloud API Key");
            cloudHybridMapOverlay.Name = "Hybrid";
            cloudHybridMapOverlay.MapType = ThinkGeoCloudRasterMapsMapType.Hybrid;
            Map1.CustomOverlays.Add(cloudHybridMapOverlay);

            // Queried Service Overlay
            InMemoryFeatureLayer serviceAreaLayer = new InMemoryFeatureLayer();
            GeoColor serviceAreaGeoColor = new GeoColor(120, GeoColor.FromHtml("#1749c9"));
            serviceAreaLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyles.CreateSimpleAreaStyle(serviceAreaGeoColor, GeoColor.FromHtml("#fefec1"), 2, LineDashStyle.Solid);
            serviceAreaLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            Map1.DynamicOverlay.Layers.Add("serviceArea", serviceAreaLayer);

            // POI Overlay
            Proj4Projection proj4 = new Proj4Projection();
            proj4.InternalProjectionParametersString = Proj4Projection.GetDecimalDegreesParametersString();
            proj4.ExternalProjectionParametersString = Proj4Projection.GetSphericalMercatorParametersString();
            proj4.Open();

            ShapeFileFeatureLayer hotelsLayer = new ShapeFileFeatureLayer(MapPath("~/App_Data/POIs/Hotels.shp"), GeoFileReadWriteMode.Read);
            hotelsLayer.Name = Resource.Hotels;
            hotelsLayer.Transparency = 120f;
            hotelsLayer.ZoomLevelSet.ZoomLevel10.DefaultPointStyle = new PointStyle(new GeoImage(MapPath("~/Images/Hotel.png")));
            hotelsLayer.ZoomLevelSet.ZoomLevel10.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            hotelsLayer.FeatureSource.Projection = proj4;
            Map1.DynamicOverlay.Layers.Add(hotelsLayer.Name, hotelsLayer);

            ShapeFileFeatureLayer medicalFacilitesLayer = new ShapeFileFeatureLayer(MapPath("~/App_Data/POIs/Medical_Facilities.shp"), GeoFileReadWriteMode.Read);
            medicalFacilitesLayer.Name = Resource.MedicalFacilites;
            medicalFacilitesLayer.Transparency = 120f;
            medicalFacilitesLayer.ZoomLevelSet.ZoomLevel10.DefaultPointStyle = new PointStyle(new GeoImage(MapPath("~/Images/DrugStore.png")));
            medicalFacilitesLayer.ZoomLevelSet.ZoomLevel10.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            medicalFacilitesLayer.FeatureSource.Projection = proj4;
            Map1.DynamicOverlay.Layers.Add(medicalFacilitesLayer.Name, medicalFacilitesLayer);

            ShapeFileFeatureLayer publicFacilitesLayer = new ShapeFileFeatureLayer(MapPath("~/App_Data/POIs/Public_Facilities.shp"), GeoFileReadWriteMode.Read);
            publicFacilitesLayer.Name = Resource.PublicFacilites;
            publicFacilitesLayer.Transparency = 120f;
            publicFacilitesLayer.ZoomLevelSet.ZoomLevel10.DefaultPointStyle = new PointStyle(new GeoImage(MapPath("~/Images/public_facility.png")));
            publicFacilitesLayer.ZoomLevelSet.ZoomLevel10.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            publicFacilitesLayer.FeatureSource.Projection = proj4;
            Map1.DynamicOverlay.Layers.Add(publicFacilitesLayer.Name, publicFacilitesLayer);

            ShapeFileFeatureLayer restaurantsLayer = new ShapeFileFeatureLayer(MapPath("~/App_Data/POIs/Restaurants.shp"), GeoFileReadWriteMode.Read);
            restaurantsLayer.Name = Resource.Restaurants;
            restaurantsLayer.Transparency = 120f;
            restaurantsLayer.ZoomLevelSet.ZoomLevel10.DefaultPointStyle = new PointStyle(new GeoImage(MapPath("~/Images/restaurant.png")));
            restaurantsLayer.ZoomLevelSet.ZoomLevel10.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            restaurantsLayer.FeatureSource.Projection = proj4;
            Map1.DynamicOverlay.Layers.Add(restaurantsLayer.Name, restaurantsLayer);

            ShapeFileFeatureLayer schoolsLayer = new ShapeFileFeatureLayer(MapPath("~/App_Data/POIs/Schools.shp"), GeoFileReadWriteMode.Read);
            schoolsLayer.Name = Resource.Schools;
            schoolsLayer.Transparency = 120f;
            schoolsLayer.ZoomLevelSet.ZoomLevel10.DefaultPointStyle = new PointStyle(new GeoImage(MapPath("~/Images/school.png")));
            schoolsLayer.ZoomLevelSet.ZoomLevel10.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            schoolsLayer.FeatureSource.Projection = proj4;
            Map1.DynamicOverlay.Layers.Add(schoolsLayer.Name, schoolsLayer);
            Map1.DynamicOverlay.IsVisibleInOverlaySwitcher = false;

            // Restrict area Overlay
            ShapeFileFeatureLayer restrictedLayer = new ShapeFileFeatureLayer(MapPath("~/App_Data/CityLimitPolygon.shp"), GeoFileReadWriteMode.Read);
            AreaStyle extentStyle = new AreaStyle();
            extentStyle.CustomAreaStyles.Add(new AreaStyle(new GeoSolidBrush(GeoColor.SimpleColors.Transparent)) { OutlinePen = new GeoPen(GeoColor.SimpleColors.White, 5.5f) });
            extentStyle.CustomAreaStyles.Add(new AreaStyle(new GeoSolidBrush(GeoColor.SimpleColors.Transparent)) { OutlinePen = new GeoPen(GeoColor.SimpleColors.Red, 1.5f) { DashStyle = LineDashStyle.Dash } });
            restrictedLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = extentStyle;
            restrictedLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            restrictedLayer.FeatureSource.Projection = proj4;
            Map1.DynamicOverlay.Layers.Add("restricted", restrictedLayer);

            InMemoryFeatureLayer highlightFeatureLayer = new InMemoryFeatureLayer();
            highlightFeatureLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            Map1.DynamicOverlay.Layers.Add("highlightFeatureLayer", highlightFeatureLayer);

            // Marker Overlay
            Map1.MarkerOverlay.ZoomLevelSet.ZoomLevel01.DefaultMarkerStyle.WebImage = new WebImage("/Images/selectedHalo.png") { ImageOffsetX = -16, ImageOffsetY = -16 };
            Map1.MarkerOverlay.ZoomLevelSet.ZoomLevel01.DefaultMarkerStyle.Popup.BorderWidth = 1;
            Map1.MarkerOverlay.ZoomLevelSet.ZoomLevel01.DefaultMarkerStyle.Popup.BorderColor = GeoColor.StandardColors.Gray;
            Map1.MarkerOverlay.ZoomLevelSet.ZoomLevel01.DefaultMarkerStyle.Popup.AutoSize = true;
            Map1.MarkerOverlay.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            Map1.MarkerOverlay.IsVisibleInOverlaySwitcher = false;

            // Drawn Point
            InMemoryMarkerOverlay markerOverlay = new InMemoryMarkerOverlay("DrawnPointOverlay");
            markerOverlay.FeatureSource.InternalFeatures.Add(new Feature(new PointShape(-10776838.0796536, 3912346.50475619)));     // Add a initial point for query
            markerOverlay.IsVisibleInOverlaySwitcher = false;
            markerOverlay.ZoomLevelSet.ZoomLevel01.DefaultMarkerStyle.WebImage = new WebImage("/Images/drawPoint.png") { ImageOffsetX = -16, ImageOffsetY = -32 };
            markerOverlay.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            Map1.CustomOverlays.Add(markerOverlay);

            // Legend Layer
            LegendAdornmentLayer legendlayer = new LegendAdornmentLayer { Height = 135, Location = AdornmentLocation.LowerRight };
            Map1.AdornmentOverlay.Layers.Add(legendlayer);

            LegendItem hotelsLayeritem = new LegendItem();
            hotelsLayeritem.ImageStyle = hotelsLayer.ZoomLevelSet.ZoomLevel10.DefaultPointStyle;
            hotelsLayeritem.TextStyle = WorldStreetsTextStyles.GeneralPurpose(Resource.Hotels, 10);
            legendlayer.LegendItems.Add("hotels", hotelsLayeritem);

            LegendItem medicalFacilitesLayeritem = new LegendItem();
            medicalFacilitesLayeritem.ImageStyle = medicalFacilitesLayer.ZoomLevelSet.ZoomLevel10.DefaultPointStyle;
            medicalFacilitesLayeritem.TextStyle = WorldStreetsTextStyles.GeneralPurpose(Resource.MedicalFacilites, 10);
            legendlayer.LegendItems.Add("medicalFacilites", medicalFacilitesLayeritem);

            LegendItem publicFacilitesLayeritem = new LegendItem();
            publicFacilitesLayeritem.ImageStyle = publicFacilitesLayer.ZoomLevelSet.ZoomLevel10.DefaultPointStyle;
            publicFacilitesLayeritem.TextStyle = WorldStreetsTextStyles.GeneralPurpose(Resource.PublicFacilites, 10);
            legendlayer.LegendItems.Add("publicFacilites", publicFacilitesLayeritem);

            LegendItem restaurantsLayeritem = new LegendItem();
            restaurantsLayeritem.ImageStyle = restaurantsLayer.ZoomLevelSet.ZoomLevel10.DefaultPointStyle;
            restaurantsLayeritem.TextStyle = WorldStreetsTextStyles.GeneralPurpose(Resource.Restaurants, 10);
            legendlayer.LegendItems.Add("restaurants", restaurantsLayeritem);

            LegendItem schoolsLayeritem = new LegendItem();
            schoolsLayeritem.ImageStyle = schoolsLayer.ZoomLevelSet.ZoomLevel10.DefaultPointStyle;
            schoolsLayeritem.TextStyle = WorldStreetsTextStyles.GeneralPurpose(Resource.Schools, 10);
            legendlayer.LegendItems.Add("schools", schoolsLayeritem);

            // Scale bar layer
            ScaleBarAdornmentLayer scaleBarAdormentLayer = new ScaleBarAdornmentLayer();
            scaleBarAdormentLayer.Location = AdornmentLocation.LowerLeft;
            scaleBarAdormentLayer.XOffsetInPixel = 10;
            Map1.AdornmentOverlay.Layers.Add("ScaleBar", scaleBarAdormentLayer);
            Map1.AdornmentOverlay.IsVisibleInOverlaySwitcher = false;
        }
    }
}