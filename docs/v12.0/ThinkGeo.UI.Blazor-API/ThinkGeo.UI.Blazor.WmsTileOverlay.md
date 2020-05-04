# WmsTileOverlay


## Inheritance Hierarchy

+ `Object`
  + `ComponentBase`
    + [`Overlay`](ThinkGeo.UI.Blazor.Overlay.md)
      + [`TileOverlay`](ThinkGeo.UI.Blazor.TileOverlay.md)
        + **`WmsTileOverlay`**

## Members Summary

### Public Constructors Summary


|Name|
|---|
|[`WmsTileOverlay()`](#wmstileoverlay)|

### Protected Constructors Summary


|Name|
|---|
|N/A|

### Public Properties Summary

|Name|Return Type|Description|
|---|---|---|
|[`Id`](#id)|`String`|N/A|
|[`IsVisible`](#isvisible)|`Boolean`|N/A|
|[`MaxExtent`](#maxextent)|[`RectangleShape`](../ThinkGeo.Core/ThinkGeo.Core.RectangleShape.md)|N/A|
|[`Parameters`](#parameters)|Dictionary<`String`,`String`>|Gets or sets the parameters of WMS Server.|
|[`ServerUris`](#serveruris)|Collection<`Uri`>|Gets or sets the hosts of WMS.|
|[`TileHeight`](#tileheight)|`Int32`|N/A|
|[`TileWidth`](#tilewidth)|`Int32`|N/A|
|[`WrappingMode`](#wrappingmode)|[`WrappingMode`](../ThinkGeo.Core/ThinkGeo.Core.WrappingMode.md)|N/A|

### Protected Properties Summary

|Name|Return Type|Description|
|---|---|---|
|[`JSRuntime`](#jsruntime)|`IJSRuntime`|N/A|
|[`MapView`](#mapview)|[`MapView`](ThinkGeo.UI.Blazor.MapView.md)|N/A|

### Public Methods Summary


|Name|
|---|
|[`Equals(Object)`](#equalsobject)|
|[`GetHashCode()`](#gethashcode)|
|[`GetType()`](#gettype)|
|[`RedrawAsync()`](#redrawasync)|
|[`SetParametersAsync(ParameterView)`](#setparametersasyncparameterview)|
|[`ToString()`](#tostring)|

### Protected Methods Summary


|Name|
|---|
|[`BuildRenderTree(RenderTreeBuilder)`](#buildrendertreerendertreebuilder)|
|[`Finalize()`](#finalize)|
|[`InitAsync()`](#initasync)|
|[`InvokeAsync(Action)`](#invokeasyncaction)|
|[`InvokeAsync(Func<Task>)`](#invokeasyncfunc<task>)|
|[`MemberwiseClone()`](#memberwiseclone)|
|[`OnAfterRender(Boolean)`](#onafterrenderboolean)|
|[`OnAfterRenderAsync(Boolean)`](#onafterrenderasyncboolean)|
|[`OnInitialized()`](#oninitialized)|
|[`OnInitializedAsync()`](#oninitializedasync)|
|[`OnParametersSet()`](#onparametersset)|
|[`OnParametersSetAsync()`](#onparameterssetasync)|
|[`ReleaseAsync()`](#releaseasync)|
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
|[`WmsTileOverlay()`](#wmstileoverlay)|

### Protected Constructors


### Public Properties

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
#### `MaxExtent`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

[`RectangleShape`](../ThinkGeo.Core/ThinkGeo.Core.RectangleShape.md)

---
#### `Parameters`

**Summary**

   *Gets or sets the parameters of WMS Server.*

**Remarks**

   *N/A*

**Return Value**

Dictionary<`String`,`String`>

---
#### `ServerUris`

**Summary**

   *Gets or sets the hosts of WMS.*

**Remarks**

   *N/A*

**Return Value**

Collection<`Uri`>

---
#### `TileHeight`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`Int32`

---
#### `TileWidth`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`Int32`

---
#### `WrappingMode`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

[`WrappingMode`](../ThinkGeo.Core/ThinkGeo.Core.WrappingMode.md)

---

### Protected Properties

#### `JSRuntime`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`IJSRuntime`

---
#### `MapView`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

[`MapView`](ThinkGeo.UI.Blazor.MapView.md)

---

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
#### `RedrawAsync()`

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
|__builder|`RenderTreeBuilder`|N/A|

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
#### `InitAsync()`

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
#### `ReleaseAsync()`

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


