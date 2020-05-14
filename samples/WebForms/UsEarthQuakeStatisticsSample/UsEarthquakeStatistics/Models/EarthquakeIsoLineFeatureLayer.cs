using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using ThinkGeo.MapSuite.Drawing;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Shapes;
using ThinkGeo.MapSuite.Styles;

namespace ThinkGeo.MapSuite.EarthquakeStatistics
{
    public class EarthquakeIsoLineFeatureLayer : FeatureLayer
    {
        private DynamicIsoLineLayer isoLineLayer;
        private Collection<GeoColor> levelAreaColors;
        private ClassBreakStyle levelClassBreakStyle;

        public EarthquakeIsoLineFeatureLayer()
            : this(null)
        { }

        public EarthquakeIsoLineFeatureLayer(ShapeFileFeatureSource featureSource)
        {
            FeatureSource = featureSource;
        }

        public new FeatureSource FeatureSource
        {
            get { return base.FeatureSource; }
            set
            {
                base.FeatureSource = value;
                Initialize();
            }
        }

        public Collection<double> IsoLineLevels
        {
            get
            {
                Collection<double> result = null;
                if (isoLineLayer != null)
                {
                    result = isoLineLayer.IsoLineLevels;
                }

                return result;
            }
        }

        public Collection<GeoColor> LevelAreaColors
        {
            get
            {
                if (levelAreaColors == null)
                {
                    levelAreaColors = new Collection<GeoColor>();
                    levelAreaColors.Add(GeoColor.FromHtml("#ffffbe"));
                    levelAreaColors.Add(GeoColor.FromHtml("#fdff9e"));
                    levelAreaColors.Add(GeoColor.FromHtml("#fdff37"));
                    levelAreaColors.Add(GeoColor.FromHtml("#fdda04"));
                    levelAreaColors.Add(GeoColor.FromHtml("#ffa701"));
                    levelAreaColors.Add(GeoColor.FromHtml("#ff6f02"));
                    levelAreaColors.Add(GeoColor.FromHtml("#ec0000"));
                    levelAreaColors.Add(GeoColor.FromHtml("#b90000"));
                    levelAreaColors.Add(GeoColor.FromHtml("#850100"));
                    levelAreaColors.Add(GeoColor.FromHtml("#620001"));
                    levelAreaColors.Add(GeoColor.FromHtml("#450005"));
                    levelAreaColors.Add(GeoColor.FromHtml("#2b0804"));
                }
                return levelAreaColors;
            }
        }

        public ClassBreakStyle LevelClassBreakStyle
        {
            get
            {
                if (levelClassBreakStyle == null)
                {
                    levelClassBreakStyle = new ClassBreakStyle(isoLineLayer.DataValueColumnName);
                    Collection<Style> firstStyles = new Collection<Style>();
                    firstStyles.Add(new AreaStyle(new GeoPen(GeoColor.FromHtml("#fe6b06"), 1), new GeoSolidBrush(new GeoColor(100, LevelAreaColors[0]))));
                    levelClassBreakStyle.ClassBreaks.Add(new ClassBreak(double.MinValue, firstStyles));
                    for (int i = 0; i < IsoLineLevels.Count - 1; i++)
                    {
                        Collection<Style> styles = new Collection<Style>();
                        styles.Add(new AreaStyle(new GeoPen(GeoColor.FromHtml("#fe6b06"), 1), new GeoSolidBrush(new GeoColor(100, LevelAreaColors[i + 1]))));
                        levelClassBreakStyle.ClassBreaks.Add(new ClassBreak(IsoLineLevels[i + 1], styles));
                    }
                }
                return levelClassBreakStyle;
            }
        }

        protected override void DrawCore(GeoCanvas canvas, Collection<SimpleCandidate> labelsInAllLayers)
        {
            isoLineLayer.Draw(canvas, labelsInAllLayers);
        }

        private void Initialize()
        {
            Dictionary<PointShape, double> dataPoints = new Dictionary<PointShape, double>();

            if (!FeatureSource.IsOpen)
            {
                FeatureSource.Open();
            }

            Collection<Feature> allFeatures = FeatureSource.GetAllFeatures(new Collection<string>() { Resources.LongitudeColumnName, Resources.LatitudeColumnName, Resources.MagnitudeColumnName });
            var featureColumnQuery = allFeatures.Where(n => double.Parse(n.ColumnValues[Resources.MagnitudeColumnName]) > 0).Select(n => new
            {
                Longitude = double.Parse(n.ColumnValues[Resources.LongitudeColumnName]),
                Latitude = double.Parse(n.ColumnValues[Resources.LatitudeColumnName]),
                Magnitude = double.Parse(n.ColumnValues[Resources.MagnitudeColumnName])
            });

            foreach (var column in featureColumnQuery)
            {
                dataPoints.Add(new PointShape(column.Longitude, column.Latitude), column.Magnitude);
            }

            double[] dataCollection = dataPoints.Select(n => n.Value).ToArray();
            Collection<double> isoLineLevels = new Collection<double>(GetClassBreakValues(dataCollection, 12).ToList());

            isoLineLayer = new DynamicIsoLineLayer(dataPoints, isoLineLevels, new InverseDistanceWeightedGridInterpolationModel(3, double.MaxValue), IsoLineType.ClosedLinesAsPolygons);

            isoLineLayer.CustomStyles.Add(LevelClassBreakStyle);

            //Create the text styles to label the lines
            TextStyle textStyle = TextStyles.CreateSimpleTextStyle(isoLineLayer.DataValueColumnName, "Arial", 8, DrawingFontStyles.Bold, GeoColor.StandardColors.Black, 0, 0);
            textStyle.HaloPen = new GeoPen(GeoColor.StandardColors.White, 2);
            textStyle.OverlappingRule = LabelOverlappingRule.NoOverlapping;
            textStyle.SplineType = SplineType.StandardSplining;
            textStyle.DuplicateRule = LabelDuplicateRule.UnlimitedDuplicateLabels;
            textStyle.TextLineSegmentRatio = 9999999;
            textStyle.FittingLineInScreen = true;
            textStyle.SuppressPartialLabels = true;
            textStyle.NumericFormat = "{0:0.00}";
            isoLineLayer.CustomStyles.Add(textStyle);
        }

        private Dictionary<PointShape, double> GetDataPoints()
        {
            return (from feature in FeatureSource.GetAllFeatures(GetReturningColumns())
                    where double.Parse(feature.ColumnValues[Resources.MagnitudeColumnName]) > 0
                    select new PointShape
                    {
                        X = double.Parse(feature.ColumnValues[Resources.LongitudeColumnName], CultureInfo.InvariantCulture),
                        Y = double.Parse(feature.ColumnValues[Resources.LatitudeColumnName], CultureInfo.InvariantCulture),
                        Z = double.Parse(feature.ColumnValues[Resources.MagnitudeColumnName], CultureInfo.InvariantCulture)
                    }).ToDictionary(point => point, point => point.Z);
        }

        private static Collection<double> GetClassBreakValues(IEnumerable<double> values, int count)
        {
            Collection<double> result = new Collection<double>();
            double[] sortedValues = values.OrderBy(v => v).ToArray();
            int classCount = sortedValues.Length / count;
            for (var i = 1; i < count; i++)
            {
                result.Add(sortedValues[i * classCount]);
            }

            return result;
        }

        private static IEnumerable<string> GetReturningColumns()
        {
            yield return Resources.LongitudeColumnName;
            yield return Resources.LatitudeColumnName;
            yield return Resources.MagnitudeColumnName;
        }
    }
}