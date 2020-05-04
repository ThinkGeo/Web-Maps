# MarkerStyle


## Inheritance Hierarchy

+ `Object`
  + **`MarkerStyle`**
    + [`PointMarkerStyle`](ThinkGeo.UI.Blazor.PointMarkerStyle.md)
    + [`ValueMarkerStyle`](ThinkGeo.UI.Blazor.ValueMarkerStyle.md)

## Members Summary

### Public Constructors Summary


|Name|
|---|
|N/A|

### Protected Constructors Summary


|Name|
|---|
|[`MarkerStyle()`](#markerstyle)|

### Public Properties Summary

|Name|Return Type|Description|
|---|---|---|
|[`IsActive`](#isactive)|`Boolean`|This property gets and sets the active status of the style.|
|[`RequiredColumnNames`](#requiredcolumnnames)|Collection<`String`>|This property gets the collection of fields that are required for the style.|

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
|N/A|

### Protected Constructors

#### `MarkerStyle()`

**Summary**

   *Initialize an instance of the MarkerStyle class.*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|N/A||

**Parameters**

|Name|Type|Description|
|---|---|---|
|N/A|N/A|N/A|

---

### Public Properties

#### `IsActive`

**Summary**

   *This property gets and sets the active status of the style.*

**Remarks**

   *If the style is not active then it will not draw.*

**Return Value**

`Boolean`

---
#### `RequiredColumnNames`

**Summary**

   *This property gets the collection of fields that are required for the style.*

**Remarks**

   *This property gets the collection of fields that are required for the style. These are in addition to any other columns you specify in styles that inherit from this one. For example, if you have use a ValueStyle and it requires a column name for the value comparison, then that column does not need to be in this collection. You only use the RequiredColumnNames for columns you need beyond those required by specific inherited styles.*

**Return Value**

Collection<`String`>

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

   *The abstract method returns a collection of markers from the features specified.*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|Collection<[`Marker`](ThinkGeo.UI.Blazor.Marker.md)>|N/A|

**Parameters**

|Name|Type|Description|
|---|---|---|
|features|IEnumerable<[`Feature`](../ThinkGeo.Core/ThinkGeo.Core.Feature.md)>|A collection of features that the markers will be created from.|

---
#### `GetRequiredColumnNames()`

**Summary**

   *This method returns the column data for each feature that is required for the style to properly draw.*

**Remarks**

   *This method is the concrete wrapper for the abstract method GetRequiredColumnNamesCore. In this method, we return the column names that are required for the style to draw the feature properly. For example, if you have a style that colors areas blue when a certain column value is over 100, then you need to be sure you include that column name. This will ensure that the column data is returned to you in the feature when it is ready to draw. In many of the styles, we add properties to allow the user to specify which field they need; then, in the GetRequiredColumnNamesCore we read that property and add it to the collection. As this is a concrete public method that wraps a Core method, we reserve the right to add events and other logic to pre- or post-process data returned by the Core version of the method. In this way, we leave our framework open on our end, but also allow you the developer to extend our logic to suit your needs. If you have questions about this, please contact our support team as we would be happy to work with you on extending our framework.*

**Return Value**

|Type|Description|
|---|---|
|Collection<`String`>|This method returns a collection of column names that the style needs.|

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

   *This method returns the column data for each feature that is required for the style to properly draw.*

**Remarks**

   *This abstract method is called from the concrete public method GetRequiredFieldNames. In this method, we return the column names that are required for the style to draw the feature properly. For example, if you have a style that colors areas blue when a certain column value is over 100, then you need to be sure you include that column name. This will ensure that the column data is returned to you in the feature when it is ready to draw. In many of the styles, we add properties to allow the user to specify which field they need; then, in the GetRequiredColumnNamesCore we read that property and add it to the collection.*

**Return Value**

|Type|Description|
|---|---|
|Collection<`String`>|This method returns a collection of column names that the style needs.|

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


