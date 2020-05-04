# WebApiExtentHelper


## Inheritance Hierarchy

+ `Object`
  + **`WebApiExtentHelper`**

## Members Summary

### Public Constructors Summary


|Name|
|---|
|N/A|

### Protected Constructors Summary


|Name|
|---|
|N/A|

### Public Properties Summary

|Name|Return Type|Description|
|---|---|---|
|N/A|N/A|N/A|

### Protected Properties Summary

|Name|Return Type|Description|
|---|---|---|
|N/A|N/A|N/A|

### Public Methods Summary


|Name|
|---|
|[`Equals(Object)`](#equalsobject)|
|[`GetBoundingBoxForXyz(Int64,Int64,Int32,GeographyUnit)`](#getboundingboxforxyzint64int64int32geographyunit)|
|[`GetBoundingBoxForXyz(Int64,Int64,Int32,GeographyUnit,TileMatrix)`](#getboundingboxforxyzint64int64int32geographyunittilematrix)|
|[`GetBoundingBoxForXyz(Int64,Int64,Int32,GeographyUnit,ZoomLevelSet)`](#getboundingboxforxyzint64int64int32geographyunitzoomlevelset)|
|[`GetBoundingBoxForXyz(Int64,Int64,Int32,GeographyUnit,ZoomLevelSet,TileMatrix)`](#getboundingboxforxyzint64int64int32geographyunitzoomlevelsettilematrix)|
|[`GetBoundingBoxForXyz(Int64,Int64,Int32,Int32,Int32,GeographyUnit,ZoomLevelSet,TileMatrix)`](#getboundingboxforxyzint64int64int32int32int32geographyunitzoomlevelsettilematrix)|
|[`GetHashCode()`](#gethashcode)|
|[`GetType()`](#gettype)|
|[`ToString()`](#tostring)|

### Protected Methods Summary


|Name|
|---|
|[`Finalize()`](#finalize)|
|[`GetSnappedScale(RectangleShape,Double,GeographyUnit)`](#getsnappedscalerectangleshapedoublegeographyunit)|
|[`GetSnappedZoomLevelIndex(Double,GeographyUnit)`](#getsnappedzoomlevelindexdoublegeographyunit)|
|[`MemberwiseClone()`](#memberwiseclone)|

### Public Events Summary


|Name|Event Arguments|Description|
|---|---|---|
|N/A|N/A|N/A|

## Members Detail

### Public Constructors


|Name|
|---|
|N/A|

### Protected Constructors


### Public Properties


### Protected Properties


### Public Methods

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
#### `GetBoundingBoxForXyz(Int64,Int64,Int32,GeographyUnit)`

**Summary**

   *Gets the BBox for xyz tile.*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|[`RectangleShape`](../ThinkGeo.Core/ThinkGeo.Core.RectangleShape.md)|The BBox of the tile, which is an instance of|

**Parameters**

|Name|Type|Description|
|---|---|---|
|x|`Int64`|The value of x.|
|y|`Int64`|The value of y.|
|z|`Int32`|The value of z.|
|mapUnit|[`GeographyUnit`](../ThinkGeo.Core/ThinkGeo.Core.GeographyUnit.md)|The unit of geography.|

---
#### `GetBoundingBoxForXyz(Int64,Int64,Int32,GeographyUnit,TileMatrix)`

**Summary**

   *Gets the BBox for xyz tile.*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|[`RectangleShape`](../ThinkGeo.Core/ThinkGeo.Core.RectangleShape.md)|The BBox of the tile, which is an instance of|

**Parameters**

|Name|Type|Description|
|---|---|---|
|x|`Int64`|The value of x.|
|y|`Int64`|The value of y.|
|z|`Int32`|The value of z.|
|mapUnit|[`GeographyUnit`](../ThinkGeo.Core/ThinkGeo.Core.GeographyUnit.md)|The unit of geography.|
|tileMatrix|[`TileMatrix`](../ThinkGeo.Core/ThinkGeo.Core.TileMatrix.md)|The tile matrix for calculating the tile system.|

---
#### `GetBoundingBoxForXyz(Int64,Int64,Int32,GeographyUnit,ZoomLevelSet)`

**Summary**

   *Gets the BBox for xyz tile.*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|[`RectangleShape`](../ThinkGeo.Core/ThinkGeo.Core.RectangleShape.md)|The BBox of the tile, which is an instance of|

**Parameters**

|Name|Type|Description|
|---|---|---|
|x|`Int64`|The value of x.|
|y|`Int64`|The value of y.|
|z|`Int32`|The value of z.|
|mapUnit|[`GeographyUnit`](../ThinkGeo.Core/ThinkGeo.Core.GeographyUnit.md)|The unit of geography.|
|zoomLevelSet|[`ZoomLevelSet`](../ThinkGeo.Core/ThinkGeo.Core.ZoomLevelSet.md)|The ZoomLevel collection.|

---
#### `GetBoundingBoxForXyz(Int64,Int64,Int32,GeographyUnit,ZoomLevelSet,TileMatrix)`

**Summary**

   *Gets the BBox for xyz tile.*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|[`RectangleShape`](../ThinkGeo.Core/ThinkGeo.Core.RectangleShape.md)|The BBox of the tile, which is an instance of|

**Parameters**

|Name|Type|Description|
|---|---|---|
|x|`Int64`|The value of x.|
|y|`Int64`|The value of y.|
|z|`Int32`|The value of z.|
|mapUnit|[`GeographyUnit`](../ThinkGeo.Core/ThinkGeo.Core.GeographyUnit.md)|The unit of geography.|
|zoomLevelSet|[`ZoomLevelSet`](../ThinkGeo.Core/ThinkGeo.Core.ZoomLevelSet.md)|The ZoomLevel collection.|
|tileMatrix|[`TileMatrix`](../ThinkGeo.Core/ThinkGeo.Core.TileMatrix.md)|The tile matrix for calculating the tile system.|

---
#### `GetBoundingBoxForXyz(Int64,Int64,Int32,Int32,Int32,GeographyUnit,ZoomLevelSet,TileMatrix)`

**Summary**

   *Gets the BBox for xyz tile.*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|[`RectangleShape`](../ThinkGeo.Core/ThinkGeo.Core.RectangleShape.md)|The BBox of the tile, which is an instance of|

**Parameters**

|Name|Type|Description|
|---|---|---|
|x|`Int64`|The value of x.|
|y|`Int64`|The value of y.|
|z|`Int32`|The value of z.|
|tileWidth|`Int32`|The width of tile.|
|tileHeight|`Int32`|The height of tile.|
|mapUnit|[`GeographyUnit`](../ThinkGeo.Core/ThinkGeo.Core.GeographyUnit.md)|The unit of geography.|
|zoomLevelSet|[`ZoomLevelSet`](../ThinkGeo.Core/ThinkGeo.Core.ZoomLevelSet.md)|The ZoomLevel collection.|
|tileMatrix|[`TileMatrix`](../ThinkGeo.Core/ThinkGeo.Core.TileMatrix.md)|The tile matrix for calculating the tile system.|

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
#### `GetSnappedScale(RectangleShape,Double,GeographyUnit)`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|`Double`|N/A|

**Parameters**

|Name|Type|Description|
|---|---|---|
|worldExtent|[`RectangleShape`](../ThinkGeo.Core/ThinkGeo.Core.RectangleShape.md)|N/A|
|screenWidth|`Double`|N/A|
|worldExtentUnit|[`GeographyUnit`](../ThinkGeo.Core/ThinkGeo.Core.GeographyUnit.md)|N/A|

---
#### `GetSnappedZoomLevelIndex(Double,GeographyUnit)`

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
|scale|`Double`|N/A|
|geographyUnit|[`GeographyUnit`](../ThinkGeo.Core/ThinkGeo.Core.GeographyUnit.md)|N/A|

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

### Public Events


