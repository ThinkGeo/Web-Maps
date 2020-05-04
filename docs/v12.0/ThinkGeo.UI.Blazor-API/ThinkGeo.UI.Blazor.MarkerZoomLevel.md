# MarkerZoomLevel


## Inheritance Hierarchy

+ `Object`
  + **`MarkerZoomLevel`**

## Members Summary

### Public Constructors Summary


|Name|
|---|
|[`MarkerZoomLevel()`](#markerzoomlevel)|

### Protected Constructors Summary


|Name|
|---|
|N/A|

### Public Properties Summary

|Name|Return Type|Description|
|---|---|---|
|[`ApplyUntilZoomLevel`](#applyuntilzoomlevel)|[`ApplyUntilZoomLevel`](../ThinkGeo.Core/ThinkGeo.Core.ApplyUntilZoomLevel.md)|Gets or sets the zoomlevel to which the styles will be applied.|
|[`CustomMarkerStyle`](#custommarkerstyle)|[`MarkerStyle`](ThinkGeo.UI.Blazor.MarkerStyle.md)|Gets or sets a style that can be any type of MarkerStyle.|
|[`DefaultMarkerStyle`](#defaultmarkerstyle)|[`PointMarkerStyle`](ThinkGeo.UI.Blazor.PointMarkerStyle.md)|Gets or sets the style that is applied to the markers if the CustomMarkerStyle is not defined.|

### Protected Properties Summary

|Name|Return Type|Description|
|---|---|---|
|N/A|N/A|N/A|

### Public Methods Summary


|Name|
|---|
|[`Equals(Object)`](#equalsobject)|
|[`GetHashCode()`](#gethashcode)|
|[`GetMarkers(IEnumerable<Feature>)`](#getmarkersienumerable<feature>)|
|[`GetType()`](#gettype)|
|[`ToString()`](#tostring)|

### Protected Methods Summary


|Name|
|---|
|[`Finalize()`](#finalize)|
|[`MemberwiseClone()`](#memberwiseclone)|

### Public Events Summary


|Name|Event Arguments|Description|
|---|---|---|
|N/A|N/A|N/A|

## Members Detail

### Public Constructors


|Name|
|---|
|[`MarkerZoomLevel()`](#markerzoomlevel)|

### Protected Constructors


### Public Properties

#### `ApplyUntilZoomLevel`

**Summary**

   *Gets or sets the zoomlevel to which the styles will be applied.*

**Remarks**

   *N/A*

**Return Value**

[`ApplyUntilZoomLevel`](../ThinkGeo.Core/ThinkGeo.Core.ApplyUntilZoomLevel.md)

---
#### `CustomMarkerStyle`

**Summary**

   *Gets or sets a style that can be any type of MarkerStyle.*

**Remarks**

   *The CustomMarkerStyle has a higher priority than the DefaultMarkerStyle. When you define both styles, the CustomMarkerStyle will be applied. The CustomMarkerStyle can be any kind of MarkerStyle while the DefaultMarkerStyle cannot.*

**Return Value**

[`MarkerStyle`](ThinkGeo.UI.Blazor.MarkerStyle.md)

---
#### `DefaultMarkerStyle`

**Summary**

   *Gets or sets the style that is applied to the markers if the CustomMarkerStyle is not defined.*

**Remarks**

   *N/A*

**Return Value**

[`PointMarkerStyle`](ThinkGeo.UI.Blazor.PointMarkerStyle.md)

---

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
#### `GetMarkers(IEnumerable<Feature>)`

**Summary**

   *Returns a collection of markers to which the styles will be applied when the current zoomlevel falls into the ranges defined.*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|Collection<[`Marker`](ThinkGeo.UI.Blazor.Marker.md)>|N/A|

**Parameters**

|Name|Type|Description|
|---|---|---|
|features|IEnumerable<[`Feature`](../ThinkGeo.Core/ThinkGeo.Core.Feature.md)>|A collection of markers to which the styles will be applied when the current zoomlevel falls into the ranges defined.|

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


