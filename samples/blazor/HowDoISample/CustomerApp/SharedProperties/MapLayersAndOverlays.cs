using ThinkGeo.Core;
using ThinkGeo.UI.Blazor;

namespace ThinkGeo.UI.Blazor.HowDoI.CustomerApp.SharedProperties
{
    /// <summary>
    /// Stores the runtime ThinkGeo overlays and in-memory feature layers used by the map.
    /// </summary>
    /// <remarks>
    /// This class separates rendering surfaces into overlays (map presentation containers) and
    /// feature layers (data containers), allowing styling and interaction logic to target each concern
    /// independently. All members are initialized by default to provide null-safe runtime access.
    /// Instances are intended to be long-lived and reused across map refresh cycles so selection state,
    /// style caches, and dynamic rendering behavior remain consistent.
    /// </remarks>
    public class MapLayersAndOverlays
    {
        #region Overlay Instances

        /// <summary>
        /// Base map raster tile overlay sourced from ThinkGeo Cloud.
        /// </summary>
        /// <remarks>
        /// Hosts the background map imagery and should be added beneath feature overlays.
        /// Theme and provider configuration are applied externally by map initialization logic.
        /// </remarks>
        public ThinkGeoCloudRasterMapsOverlay RasterOverlay = new();

        /// <summary>
        /// Interactive edit overlay used for drawing, editing, and map interaction workflows.
        /// </summary>
        /// <remarks>
        /// Used by the Blazor map component to manage user-driven geometry operations.
        /// Interaction mode changes (draw, pan, select) are coordinated through this overlay.
        /// </remarks>
        public EditOverlay MapEditOverlay = new();

        /// <summary>
        /// Overlay containing persisted feature layers such as drawn shapes, placed icons, and arrows.
        /// </summary>
        /// <remarks>
        /// Layer registration and style refresh operations typically target this overlay.
        /// Persisted data layers should remain stable in this container to avoid unnecessary re-creation.
        /// </remarks>
        public LayerOverlay FeaturesOverlay = new();

        /// <summary>
        /// Overlay containing transient or interaction-driven visual layers.
        /// </summary>
        /// <remarks>
        /// Commonly used for selection highlights and other dynamic map presentation elements.
        /// Keeping transient visuals separate reduces churn in persisted feature layers.
        /// </remarks>
        public LayerOverlay DynamicOverlay = new();

        #endregion

        #region In-Memory Feature Layers

        /// <summary>
        /// In-memory layer that stores user-drawn geometries (points, lines, polygons, circles).
        /// </summary>
        /// <remarks>
        /// Contains freeform user-authored geometry and is typically rendered in <see cref="FeaturesOverlay"/>.
        /// Feature schema generally includes metadata for naming, visibility, and styling decisions.
        /// </remarks>
        public InMemoryFeatureLayer DrawnFeaturesLayer = new InMemoryFeatureLayer();

        /// <summary>
        /// In-memory layer that stores placed icon/marker features.
        /// </summary>
        /// <remarks>
        /// Holds point features with icon metadata used by image point styles in feature rendering.
        /// Marker identity and display data are used by selection and persistence workflows.
        /// </remarks>
        public InMemoryFeatureLayer PlacedFeaturesLayer = new InMemoryFeatureLayer();

        /// <summary>
        /// In-memory layer used to render visual highlight geometry for selected features.
        /// </summary>
        /// <remarks>
        /// Selection visuals are usually rendered in <see cref="DynamicOverlay"/> so highlight state can refresh independently.
        /// This layer is regenerated as selection changes to keep highlight geometry in sync.
        /// </remarks>
        public InMemoryFeatureLayer SelectionHighlightLayer = new InMemoryFeatureLayer();

        /// <summary>
        /// In-memory layer used for generated directional arrow features associated with lines.
        /// </summary>
        /// <remarks>
        /// Stores derived arrow marker features created from line geometry to communicate direction of travel.
        /// Arrow features are typically recalculated when source line geometry or scale context changes.
        /// </remarks>
        public InMemoryFeatureLayer ArrowFeaturesLayer = new InMemoryFeatureLayer();

        #endregion
    }
}
