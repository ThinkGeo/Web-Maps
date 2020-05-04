# Overlay


## Inheritance Hierarchy

+ `Object`
  + `ComponentBase`
    + **`Overlay`**
      + [`MarkerOverlay`](ThinkGeo.UI.Blazor.MarkerOverlay.md)
      + [`TileOverlay`](ThinkGeo.UI.Blazor.TileOverlay.md)
      + [`AdornmentOverlay`](ThinkGeo.UI.Blazor.AdornmentOverlay.md)
      + [`EditOverlay`](ThinkGeo.UI.Blazor.EditOverlay.md)
      + [`PopupOverlay`](ThinkGeo.UI.Blazor.PopupOverlay.md)
      + [`ThinkGeoCloudRasterMapsOverlay`](ThinkGeo.UI.Blazor.ThinkGeoCloudRasterMapsOverlay.md)
      + [`ThinkGeoCloudVectorMapsOverlay`](ThinkGeo.UI.Blazor.ThinkGeoCloudVectorMapsOverlay.md)

## Members Summary

### Public Constructors Summary


|Name|
|---|
|N/A|

### Protected Constructors Summary


|Name|
|---|
|[`Overlay()`](#overlay)|

### Public Properties Summary

|Name|Return Type|Description|
|---|---|---|
|[`Id`](#id)|`String`|Gets or sets Id.|
|[`IsVisible`](#isvisible)|`Boolean`|Indicating whether the overlay is visible or not. Defaults to true.|

### Protected Properties Summary

|Name|Return Type|Description|
|---|---|---|
|[`JSRuntime`](#jsruntime)|`IJSRuntime`|Gets JSRuntime.|
|[`MapView`](#mapview)|[`MapView`](ThinkGeo.UI.Blazor.MapView.md)|Gets or sets the MapView which this overlay is included in.|

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
|N/A|

### Protected Constructors

#### `Overlay()`

**Summary**

   *N/A*

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

#### `Id`

**Summary**

   *Gets or sets Id.*

**Remarks**

   *N/A*

**Return Value**

`String`

---
#### `IsVisible`

**Summary**

   *Indicating whether the overlay is visible or not. Defaults to true.*

**Remarks**

   *N/A*

**Return Value**

`Boolean`

---

### Protected Properties

#### `JSRuntime`

**Summary**

   *Gets JSRuntime.*

**Remarks**

   *N/A*

**Return Value**

`IJSRuntime`

---
#### `MapView`

**Summary**

   *Gets or sets the MapView which this overlay is included in.*

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

   *Redraw overlay.*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|`Task`||

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
#### `InitAsync()`

**Summary**

   *Initializes overlay, it is called automatically while map is initializing.*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|`Task`||

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

   *Release the resource.*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|`Task`||

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


