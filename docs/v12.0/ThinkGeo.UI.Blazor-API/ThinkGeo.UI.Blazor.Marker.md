# Marker


## Inheritance Hierarchy

+ `Object`
  + `ComponentBase`
    + **`Marker`**

## Members Summary

### Public Constructors Summary


|Name|
|---|
|[`Marker()`](#marker)|
|[`Marker(PointShape,MarkerStyle)`](#markerpointshapemarkerstyle)|

### Protected Constructors Summary


|Name|
|---|
|N/A|

### Public Properties Summary

|Name|Return Type|Description|
|---|---|---|
|[`Id`](#id)|`String`|Gets or sets ID of the marker.|
|[`OnClick`](#onclick)|EventCallback<[`ClickedMarkerEventArgs`](ThinkGeo.UI.Blazor.ClickedMarkerEventArgs.md)>|Occurs when the user clicks on the marker.|
|[`Position`](#position)|[`PointShape`](../ThinkGeo.Core/ThinkGeo.Core.PointShape.md)|Gets or sets the location of the marker.|
|[`Style`](#style)|[`MarkerStyle`](ThinkGeo.UI.Blazor.MarkerStyle.md)|Gets or sets the style of the marker.|

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
|[`SetParametersAsync(ParameterView)`](#setparametersasyncparameterview)|
|[`ToString()`](#tostring)|

### Protected Methods Summary


|Name|
|---|
|[`BuildRenderTree(RenderTreeBuilder)`](#buildrendertreerendertreebuilder)|
|[`Finalize()`](#finalize)|
|[`InvokeAsync(Action)`](#invokeasyncaction)|
|[`InvokeAsync(Func<Task>)`](#invokeasyncfunc<task>)|
|[`MemberwiseClone()`](#memberwiseclone)|
|[`OnAfterRender(Boolean)`](#onafterrenderboolean)|
|[`OnAfterRenderAsync(Boolean)`](#onafterrenderasyncboolean)|
|[`OnInitialized()`](#oninitialized)|
|[`OnInitializedAsync()`](#oninitializedasync)|
|[`OnParametersSet()`](#onparametersset)|
|[`OnParametersSetAsync()`](#onparameterssetasync)|
|[`ShouldRender()`](#shouldrender)|
|[`StateHasChanged()`](#statehaschanged)|

### Public Events Summary


|Name|Event Arguments|Description|
|---|---|---|
|N/A|N/A|N/A|

## Members Detail

### Public Constructors


|Name|
|---|
|[`Marker()`](#marker)|
|[`Marker(PointShape,MarkerStyle)`](#markerpointshapemarkerstyle)|

### Protected Constructors


### Public Properties

#### `Id`

**Summary**

   *Gets or sets ID of the marker.*

**Remarks**

   *N/A*

**Return Value**

`String`

---
#### `OnClick`

**Summary**

   *Occurs when the user clicks on the marker.*

**Remarks**

   *N/A*

**Return Value**

EventCallback<[`ClickedMarkerEventArgs`](ThinkGeo.UI.Blazor.ClickedMarkerEventArgs.md)>

---
#### `Position`

**Summary**

   *Gets or sets the location of the marker.*

**Remarks**

   *N/A*

**Return Value**

[`PointShape`](../ThinkGeo.Core/ThinkGeo.Core.PointShape.md)

---
#### `Style`

**Summary**

   *Gets or sets the style of the marker.*

**Remarks**

   *N/A*

**Return Value**

[`MarkerStyle`](ThinkGeo.UI.Blazor.MarkerStyle.md)

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
#### `SetParametersAsync(ParameterView)`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|`Task`|N/A|

**Parameters**

|Name|Type|Description|
|---|---|---|
|parameters|`ParameterView`|N/A|

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

#### `BuildRenderTree(RenderTreeBuilder)`

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
|builder|`RenderTreeBuilder`|N/A|

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
#### `InvokeAsync(Action)`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|`Task`|N/A|

**Parameters**

|Name|Type|Description|
|---|---|---|
|workItem|`Action`|N/A|

---
#### `InvokeAsync(Func<Task>)`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|`Task`|N/A|

**Parameters**

|Name|Type|Description|
|---|---|---|
|workItem|Func<`Task`>|N/A|

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
#### `OnAfterRender(Boolean)`

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
|firstRender|`Boolean`|N/A|

---
#### `OnAfterRenderAsync(Boolean)`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|`Task`|N/A|

**Parameters**

|Name|Type|Description|
|---|---|---|
|firstRender|`Boolean`|N/A|

---
#### `OnInitialized()`

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
#### `OnInitializedAsync()`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|`Task`|N/A|

**Parameters**

|Name|Type|Description|
|---|---|---|
|N/A|N/A|N/A|

---
#### `OnParametersSet()`

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
#### `OnParametersSetAsync()`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|`Task`|N/A|

**Parameters**

|Name|Type|Description|
|---|---|---|
|N/A|N/A|N/A|

---
#### `ShouldRender()`

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
|N/A|N/A|N/A|

---
#### `StateHasChanged()`

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

### Public Events


