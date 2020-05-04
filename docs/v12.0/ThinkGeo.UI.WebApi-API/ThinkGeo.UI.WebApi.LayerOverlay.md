# LayerOverlay


## Inheritance Hierarchy

+ `Object`
  + [`Overlay`](ThinkGeo.UI.WebApi.Overlay.md)
    + **`LayerOverlay`**

## Members Summary

### Public Constructors Summary


|Name|
|---|
|[`LayerOverlay()`](#layeroverlay)|
|[`LayerOverlay(String)`](#layeroverlaystring)|
|[`LayerOverlay(IEnumerable<Layer>)`](#layeroverlayienumerable<layer>)|
|[`LayerOverlay(String,IEnumerable<Layer>)`](#layeroverlaystringienumerable<layer>)|

### Protected Constructors Summary


|Name|
|---|
|N/A|

### Public Properties Summary

|Name|Return Type|Description|
|---|---|---|
|[`DrawingExceptionMode`](#drawingexceptionmode)|[`DrawingExceptionMode`](../ThinkGeo.Core/ThinkGeo.Core.DrawingExceptionMode.md)|N/A|
|[`Id`](#id)|`String`|N/A|
|[`IsVisible`](#isvisible)|`Boolean`|N/A|
|[`Layers`](#layers)|GeoCollection<[`Layer`](../ThinkGeo.Core/ThinkGeo.Core.Layer.md)>|Gets the layers.|
|[`TileCache`](#tilecache)|[`RasterTileCache`](../ThinkGeo.Core/ThinkGeo.Core.RasterTileCache.md)|Gets or sets the tile cache.|
|[`TileHeight`](#tileheight)|`Int32`|Gets or sets the height of the tile.|
|[`TileWidth`](#tilewidth)|`Int32`|Gets or sets the width of the tile.|

### Protected Properties Summary

|Name|Return Type|Description|
|---|---|---|
|N/A|N/A|N/A|

### Public Methods Summary


|Name|
|---|
|[`Draw(GeoCanvas)`](#drawgeocanvas)|
|[`Equals(Object)`](#equalsobject)|
|[`GetHashCode()`](#gethashcode)|
|[`GetType()`](#gettype)|
|[`ToString()`](#tostring)|

### Protected Methods Summary


|Name|
|---|
|[`DrawCore(GeoCanvas)`](#drawcoregeocanvas)|
|[`DrawException(GeoCanvas,Exception)`](#drawexceptiongeocanvasexception)|
|[`DrawExceptionCore(GeoCanvas,Exception)`](#drawexceptioncoregeocanvasexception)|
|[`Finalize()`](#finalize)|
|[`MemberwiseClone()`](#memberwiseclone)|
|[`OnDrawing(DrawingOverlayEventArgs)`](#ondrawingdrawingoverlayeventargs)|
|[`OnDrawingLayer(DrawingLayerOverlayEventArgs)`](#ondrawinglayerdrawinglayeroverlayeventargs)|
|[`OnDrawn(DrawnOverlayEventArgs)`](#ondrawndrawnoverlayeventargs)|
|[`OnDrawnLayer(DrawnLayerOverlayEventArgs)`](#ondrawnlayerdrawnlayeroverlayeventargs)|

### Public Events Summary


|Name|Event Arguments|Description|
|---|---|---|
|[`DrawingLayer`](#drawinglayer)|[`DrawingLayerOverlayEventArgs`](ThinkGeo.UI.WebApi.DrawingLayerOverlayEventArgs.md)|N/A|
|[`DrawnLayer`](#drawnlayer)|[`DrawnLayerOverlayEventArgs`](ThinkGeo.UI.WebApi.DrawnLayerOverlayEventArgs.md)|N/A|
|[`Drawing`](#drawing)|[`DrawingOverlayEventArgs`](ThinkGeo.UI.WebApi.DrawingOverlayEventArgs.md)|N/A|
|[`Drawn`](#drawn)|[`DrawnOverlayEventArgs`](ThinkGeo.UI.WebApi.DrawnOverlayEventArgs.md)|N/A|

## Members Detail

### Public Constructors


|Name|
|---|
|[`LayerOverlay()`](#layeroverlay)|
|[`LayerOverlay(String)`](#layeroverlaystring)|
|[`LayerOverlay(IEnumerable<Layer>)`](#layeroverlayienumerable<layer>)|
|[`LayerOverlay(String,IEnumerable<Layer>)`](#layeroverlaystringienumerable<layer>)|

### Protected Constructors


### Public Properties

#### `DrawingExceptionMode`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

[`DrawingExceptionMode`](../ThinkGeo.Core/ThinkGeo.Core.DrawingExceptionMode.md)

---
#### `Id`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`String`

---
#### `IsVisible`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`Boolean`

---
#### `Layers`

**Summary**

   *Gets the layers.*

**Remarks**

   *N/A*

**Return Value**

GeoCollection<[`Layer`](../ThinkGeo.Core/ThinkGeo.Core.Layer.md)>

---
#### `TileCache`

**Summary**

   *Gets or sets the tile cache.*

**Remarks**

   *N/A*

**Return Value**

[`RasterTileCache`](../ThinkGeo.Core/ThinkGeo.Core.RasterTileCache.md)

---
#### `TileHeight`

**Summary**

   *Gets or sets the height of the tile.*

**Remarks**

   *N/A*

**Return Value**

`Int32`

---
#### `TileWidth`

**Summary**

   *Gets or sets the width of the tile.*

**Remarks**

   *N/A*

**Return Value**

`Int32`

---

### Protected Properties


### Public Methods

#### `Draw(GeoCanvas)`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|`Void`|N/A|

**Parameters**

|Name|Type|Description|
|---|---|---|
|geoCanvas|[`GeoCanvas`](../ThinkGeo.Core/ThinkGeo.Core.GeoCanvas.md)|N/A|

---
#### `Equals(Object)`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|`Boolean`|N/A|

**Parameters**

|Name|Type|Description|
|---|---|---|
|obj|`Object`|N/A|

---
#### `GetHashCode()`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|`Int32`|N/A|

**Parameters**

|Name|Type|Description|
|---|---|---|
|N/A|N/A|N/A|

---
#### `GetType()`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|`Type`|N/A|

**Parameters**

|Name|Type|Description|
|---|---|---|
|N/A|N/A|N/A|

---
#### `ToString()`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|`String`|N/A|

**Parameters**

|Name|Type|Description|
|---|---|---|
|N/A|N/A|N/A|

---

### Protected Methods

#### `DrawCore(GeoCanvas)`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|`Void`|N/A|

**Parameters**

|Name|Type|Description|
|---|---|---|
|geoCanvas|[`GeoCanvas`](../ThinkGeo.Core/ThinkGeo.Core.GeoCanvas.md)|N/A|

---
#### `DrawException(GeoCanvas,Exception)`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|`Void`|N/A|

**Parameters**

|Name|Type|Description|
|---|---|---|
|geoCanvas|[`GeoCanvas`](../ThinkGeo.Core/ThinkGeo.Core.GeoCanvas.md)|N/A|
|e|`Exception`|N/A|

---
#### `DrawExceptionCore(GeoCanvas,Exception)`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|`Void`|N/A|

**Parameters**

|Name|Type|Description|
|---|---|---|
|geoCanvas|[`GeoCanvas`](../ThinkGeo.Core/ThinkGeo.Core.GeoCanvas.md)|N/A|
|e|`Exception`|N/A|

---
#### `Finalize()`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|`Void`|N/A|

**Parameters**

|Name|Type|Description|
|---|---|---|
|N/A|N/A|N/A|

---
#### `MemberwiseClone()`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|`Object`|N/A|

**Parameters**

|Name|Type|Description|
|---|---|---|
|N/A|N/A|N/A|

---
#### `OnDrawing(DrawingOverlayEventArgs)`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|`Void`|N/A|

**Parameters**

|Name|Type|Description|
|---|---|---|
|e|[`DrawingOverlayEventArgs`](ThinkGeo.UI.WebApi.DrawingOverlayEventArgs.md)|N/A|

---
#### `OnDrawingLayer(DrawingLayerOverlayEventArgs)`

**Summary**

   *Called when the  event occurs.*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|`Void`|N/A|

**Parameters**

|Name|Type|Description|
|---|---|---|
|e|[`DrawingLayerOverlayEventArgs`](ThinkGeo.UI.WebApi.DrawingLayerOverlayEventArgs.md)|The  instance containing the event data.|

---
#### `OnDrawn(DrawnOverlayEventArgs)`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|`Void`|N/A|

**Parameters**

|Name|Type|Description|
|---|---|---|
|e|[`DrawnOverlayEventArgs`](ThinkGeo.UI.WebApi.DrawnOverlayEventArgs.md)|N/A|

---
#### `OnDrawnLayer(DrawnLayerOverlayEventArgs)`

**Summary**

   *Called when the  event occurs.*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|`Void`|N/A|

**Parameters**

|Name|Type|Description|
|---|---|---|
|e|[`DrawnLayerOverlayEventArgs`](ThinkGeo.UI.WebApi.DrawnLayerOverlayEventArgs.md)|The  instance containing the event data.|

---

### Public Events

#### DrawingLayer

*N/A*

**Remarks**

*N/A*

**Event Arguments**

[`DrawingLayerOverlayEventArgs`](ThinkGeo.UI.WebApi.DrawingLayerOverlayEventArgs.md)

#### DrawnLayer

*N/A*

**Remarks**

*N/A*

**Event Arguments**

[`DrawnLayerOverlayEventArgs`](ThinkGeo.UI.WebApi.DrawnLayerOverlayEventArgs.md)

#### Drawing

*N/A*

**Remarks**

*N/A*

**Event Arguments**

[`DrawingOverlayEventArgs`](ThinkGeo.UI.WebApi.DrawingOverlayEventArgs.md)

#### Drawn

*N/A*

**Remarks**

*N/A*

**Event Arguments**

[`DrawnOverlayEventArgs`](ThinkGeo.UI.WebApi.DrawnOverlayEventArgs.md)


