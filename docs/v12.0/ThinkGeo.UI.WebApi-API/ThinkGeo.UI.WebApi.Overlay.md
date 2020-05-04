# Overlay


## Inheritance Hierarchy

+ `Object`
  + **`Overlay`**
    + [`LayerOverlay`](ThinkGeo.UI.WebApi.LayerOverlay.md)

## Members Summary

### Public Constructors Summary


|Name|
|---|
|N/A|

### Protected Constructors Summary


|Name|
|---|
|[`Overlay()`](#overlay)|
|[`Overlay(String)`](#overlaystring)|

### Public Properties Summary

|Name|Return Type|Description|
|---|---|---|
|[`DrawingExceptionMode`](#drawingexceptionmode)|[`DrawingExceptionMode`](../ThinkGeo.Core/ThinkGeo.Core.DrawingExceptionMode.md)|Gets or sets the drawing exception mode.|
|[`Id`](#id)|`String`|Gets the ID.|
|[`IsVisible`](#isvisible)|`Boolean`|Gets or sets a value indicating whether this overlay is visible.|

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
|[`OnDrawn(DrawnOverlayEventArgs)`](#ondrawndrawnoverlayeventargs)|

### Public Events Summary


|Name|Event Arguments|Description|
|---|---|---|
|[`Drawing`](#drawing)|[`DrawingOverlayEventArgs`](ThinkGeo.UI.WebApi.DrawingOverlayEventArgs.md)|N/A|
|[`Drawn`](#drawn)|[`DrawnOverlayEventArgs`](ThinkGeo.UI.WebApi.DrawnOverlayEventArgs.md)|N/A|

## Members Detail

### Public Constructors


|Name|
|---|
|N/A|

### Protected Constructors

#### `Overlay()`

**Summary**

   *Initializes a new instance of the  class.*

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
#### `Overlay(String)`

**Summary**

   *Initializes a new instance of the  class.*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|N/A||

**Parameters**

|Name|Type|Description|
|---|---|---|
|id|`String`|The ID.|

---

### Public Properties

#### `DrawingExceptionMode`

**Summary**

   *Gets or sets the drawing exception mode.*

**Remarks**

   *N/A*

**Return Value**

[`DrawingExceptionMode`](../ThinkGeo.Core/ThinkGeo.Core.DrawingExceptionMode.md)

---
#### `Id`

**Summary**

   *Gets the ID.*

**Remarks**

   *N/A*

**Return Value**

`String`

---
#### `IsVisible`

**Summary**

   *Gets or sets a value indicating whether this overlay is visible.*

**Remarks**

   *N/A*

**Return Value**

`Boolean`

---

### Protected Properties


### Public Methods

#### `Draw(GeoCanvas)`

**Summary**

   *Draws the tile on GeoCanvas.*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|`Void`|N/A|

**Parameters**

|Name|Type|Description|
|---|---|---|
|geoCanvas|[`GeoCanvas`](../ThinkGeo.Core/ThinkGeo.Core.GeoCanvas.md)|The GeoCanvas.|

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

   *The core process of Draw.*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|`Void`|N/A|

**Parameters**

|Name|Type|Description|
|---|---|---|
|geoCanvas|[`GeoCanvas`](../ThinkGeo.Core/ThinkGeo.Core.GeoCanvas.md)|The geo canvas.|

---
#### `DrawException(GeoCanvas,Exception)`

**Summary**

   *This method will draw on the canvas when an exception occurs during the drawing process.*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|`Void`|N/A|

**Parameters**

|Name|Type|Description|
|---|---|---|
|geoCanvas|[`GeoCanvas`](../ThinkGeo.Core/ThinkGeo.Core.GeoCanvas.md)|This parameter is the canvas object to draw on.|
|e|`Exception`|This parameter is the exception that is occurring.|

---
#### `DrawExceptionCore(GeoCanvas,Exception)`

**Summary**

   *This method is the Core method of DrawException, which can be overridden if you want to change its logic. This method will draw on the canvas when an exception occurs during drawing process.*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|`Void`|N/A|

**Parameters**

|Name|Type|Description|
|---|---|---|
|geoCanvas|[`GeoCanvas`](../ThinkGeo.Core/ThinkGeo.Core.GeoCanvas.md)|This parameter is the canvas object to draw on.|
|e|`Exception`|This parameter is the exception that is occurring.|

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

   *This event will be fired before Overlay is drawn.*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|`Void`|N/A|

**Parameters**

|Name|Type|Description|
|---|---|---|
|e|[`DrawingOverlayEventArgs`](ThinkGeo.UI.WebApi.DrawingOverlayEventArgs.md)|The OverlayDrawingEventArgs passed for the event raised.|

---
#### `OnDrawn(DrawnOverlayEventArgs)`

**Summary**

   *This event will be fired after Overlay is drawn.*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|`Void`|N/A|

**Parameters**

|Name|Type|Description|
|---|---|---|
|e|[`DrawnOverlayEventArgs`](ThinkGeo.UI.WebApi.DrawnOverlayEventArgs.md)|The OverlayDrawnEventArgs passed for the event raised.|

---

### Public Events

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


