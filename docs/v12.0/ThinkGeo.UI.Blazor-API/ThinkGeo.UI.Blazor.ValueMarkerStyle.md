# ValueMarkerStyle


## Inheritance Hierarchy

+ `Object`
  + [`MarkerStyle`](ThinkGeo.UI.Blazor.MarkerStyle.md)
    + **`ValueMarkerStyle`**

## Members Summary

### Public Constructors Summary


|Name|
|---|
|[`ValueMarkerStyle()`](#valuemarkerstyle)|
|[`ValueMarkerStyle(String)`](#valuemarkerstylestring)|
|[`ValueMarkerStyle(String,Collection<MarkerValueItem>)`](#valuemarkerstylestringcollection<markervalueitem>)|

### Protected Constructors Summary


|Name|
|---|
|N/A|

### Public Properties Summary

|Name|Return Type|Description|
|---|---|---|
|[`ColumnName`](#columnname)|`String`|Gets or sets the column name used to match with the value specified in the items.|
|[`IsActive`](#isactive)|`Boolean`|N/A|
|[`RequiredColumnNames`](#requiredcolumnnames)|Collection<`String`>|N/A|
|[`ValueItems`](#valueitems)|Collection<[`MarkerValueItem`](ThinkGeo.UI.Blazor.MarkerValueItem.md)>|Gets the collection of ValueItems. Each item can have a value to match.|

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
|[`GetRequiredColumnNames()`](#getrequiredcolumnnames)|
|[`GetType()`](#gettype)|
|[`ToString()`](#tostring)|

### Protected Methods Summary


|Name|
|---|
|[`Finalize()`](#finalize)|
|[`GetRequiredColumnNamesCore()`](#getrequiredcolumnnamescore)|
|[`MemberwiseClone()`](#memberwiseclone)|

### Public Events Summary


|Name|Event Arguments|Description|
|---|---|---|
|N/A|N/A|N/A|

## Members Detail

### Public Constructors


|Name|
|---|
|[`ValueMarkerStyle()`](#valuemarkerstyle)|
|[`ValueMarkerStyle(String)`](#valuemarkerstylestring)|
|[`ValueMarkerStyle(String,Collection<MarkerValueItem>)`](#valuemarkerstylestringcollection<markervalueitem>)|

### Protected Constructors


### Public Properties

#### `ColumnName`

**Summary**

   *Gets or sets the column name used to match with the value specified in the items.*

**Remarks**

   *This is the column whose values we will use for matching.*

**Return Value**

`String`

---
#### `IsActive`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`Boolean`

---
#### `RequiredColumnNames`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

Collection<`String`>

---
#### `ValueItems`

**Summary**

   *Gets the collection of ValueItems. Each item can have a value to match.*

**Remarks**

   *You will want to add MarkerValueItems to this collection. Each item can have its own style and matching string.*

**Return Value**

Collection<[`MarkerValueItem`](ThinkGeo.UI.Blazor.MarkerValueItem.md)>

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

   *N/A*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|Collection<[`Marker`](ThinkGeo.UI.Blazor.Marker.md)>|N/A|

**Parameters**

|Name|Type|Description|
|---|---|---|
|features|IEnumerable<[`Feature`](../ThinkGeo.Core/ThinkGeo.Core.Feature.md)>|N/A|

---
#### `GetRequiredColumnNames()`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|Collection<`String`>|N/A|

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
#### `GetRequiredColumnNamesCore()`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|Collection<`String`>|N/A|

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


