# MarkerValueItem


## Inheritance Hierarchy

+ `Object`
  + **`MarkerValueItem`**

## Members Summary

### Public Constructors Summary


|Name|
|---|
|[`MarkerValueItem()`](#markervalueitem)|
|[`MarkerValueItem(String)`](#markervalueitemstring)|
|[`MarkerValueItem(String,MarkerStyle)`](#markervalueitemstringmarkerstyle)|

### Protected Constructors Summary


|Name|
|---|
|N/A|

### Public Properties Summary

|Name|Return Type|Description|
|---|---|---|
|[`CustomMarkerStyle`](#custommarkerstyle)|[`MarkerStyle`](ThinkGeo.UI.Blazor.MarkerStyle.md)|Gets or sets the style that could be any type of MarkerStyle.|
|[`DefaultMarkerStyle`](#defaultmarkerstyle)|[`PointMarkerStyle`](ThinkGeo.UI.Blazor.PointMarkerStyle.md)|Gets or sets the marker style that will be applied to the markers if the CustomMarkerStyle is not defined.|
|[`Value`](#value)|`String`|Gets or sets the value that we will use to match with the feature data. If the value matches, we will use the style for this item.|

### Protected Properties Summary

|Name|Return Type|Description|
|---|---|---|
|N/A|N/A|N/A|

### Public Methods Summary


|Name|
|---|
|[`Equals(Object)`](#equalsobject)|
|[`GetHashCode()`](#gethashcode)|
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
|[`MarkerValueItem()`](#markervalueitem)|
|[`MarkerValueItem(String)`](#markervalueitemstring)|
|[`MarkerValueItem(String,MarkerStyle)`](#markervalueitemstringmarkerstyle)|

### Protected Constructors


### Public Properties

#### `CustomMarkerStyle`

**Summary**

   *Gets or sets the style that could be any type of MarkerStyle.*

**Remarks**

   *The CustomMarkerStyle could be any type of MarkerStyle. This style has a higher priority than the DefaultMarkerStyle. This means that if you defined both DefaultMarkerStyle and CustomMarkerStyle, the CustomMarkerStyle will be used.*

**Return Value**

[`MarkerStyle`](ThinkGeo.UI.Blazor.MarkerStyle.md)

---
#### `DefaultMarkerStyle`

**Summary**

   *Gets or sets the marker style that will be applied to the markers if the CustomMarkerStyle is not defined.*

**Remarks**

   *N/A*

**Return Value**

[`PointMarkerStyle`](ThinkGeo.UI.Blazor.PointMarkerStyle.md)

---
#### `Value`

**Summary**

   *Gets or sets the value that we will use to match with the feature data. If the value matches, we will use the style for this item.*

**Remarks**

   *N/A*

**Return Value**

`String`

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


