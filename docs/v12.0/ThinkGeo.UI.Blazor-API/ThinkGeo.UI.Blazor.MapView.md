# MapView


## Inheritance Hierarchy

+ `Object`
  + `ComponentBase`
    + **`MapView`**

## Members Summary

### Public Constructors Summary


|Name|
|---|
|[`MapView()`](#mapview)|

### Protected Constructors Summary


|Name|
|---|
|N/A|

### Public Properties Summary

|Name|Return Type|Description|
|---|---|---|
|[`AdornmentOverlay`](#adornmentoverlay)|[`AdornmentOverlay`](ThinkGeo.UI.Blazor.AdornmentOverlay.md)|N/A|
|[`AdornmentOverlaySetting`](#adornmentoverlaysetting)|`RenderFragment`|N/A|
|[`BackgroundColor`](#backgroundcolor)|[`GeoColor`](../ThinkGeo.Core/ThinkGeo.Core.GeoColor.md)|N/A|
|[`Center`](#center)|[`PointShape`](../ThinkGeo.Core/ThinkGeo.Core.PointShape.md)|N/A|
|[`EditOverlay`](#editoverlay)|[`EditOverlay`](ThinkGeo.UI.Blazor.EditOverlay.md)|N/A|
|[`EditOverlaySetting`](#editoverlaysetting)|`RenderFragment`|N/A|
|[`Height`](#height)|`Int32`|N/A|
|[`Id`](#id)|`String`|N/A|
|[`IsLoaded`](#isloaded)|`Boolean`|N/A|
|[`MapTools`](#maptools)|[`MapTools`](ThinkGeo.UI.Blazor.MapTools.md)|N/A|
|[`MapToolsSetting`](#maptoolssetting)|`RenderFragment`|N/A|
|[`MapUnit`](#mapunit)|[`GeographyUnit`](../ThinkGeo.Core/ThinkGeo.Core.GeographyUnit.md)|N/A|
|[`MapViewSizeUnitType`](#mapviewsizeunittype)|[`MapViewSizeUnitType`](ThinkGeo.UI.Blazor.MapViewSizeUnitType.md)|N/A|
|[`MarkerOverlay`](#markeroverlay)|[`MarkerOverlay`](ThinkGeo.UI.Blazor.MarkerOverlay.md)|N/A|
|[`MarkerOverlaySetting`](#markeroverlaysetting)|`RenderFragment`|N/A|
|[`OnClick`](#onclick)|EventCallback<[`ClickedMapViewEventArgs`](ThinkGeo.UI.Blazor.ClickedMapViewEventArgs.md)>|N/A|
|[`OnCurrentExtentChanged`](#oncurrentextentchanged)|EventCallback<[`CurrentExtentChangedMapViewEventArgs`](ThinkGeo.UI.Blazor.CurrentExtentChangedMapViewEventArgs.md)>|N/A|
|[`OnDoubleClick`](#ondoubleclick)|EventCallback<[`DoubleClickedMapViewEventArgs`](ThinkGeo.UI.Blazor.DoubleClickedMapViewEventArgs.md)>|N/A|
|[`OnMouseMove`](#onmousemove)|EventCallback<[`MouseMovingMapViewEventArgs`](ThinkGeo.UI.Blazor.MouseMovingMapViewEventArgs.md)>|N/A|
|[`OnSizeChanged`](#onsizechanged)|EventCallback<[`SizeChangedMapViewEventArgs`](ThinkGeo.UI.Blazor.SizeChangedMapViewEventArgs.md)>|N/A|
|[`Overlays`](#overlays)|GeoCollection<[`Overlay`](ThinkGeo.UI.Blazor.Overlay.md)>|N/A|
|[`OverlaysSetting`](#overlayssetting)|`RenderFragment`|N/A|
|[`PopupOverlay`](#popupoverlay)|[`PopupOverlay`](ThinkGeo.UI.Blazor.PopupOverlay.md)|N/A|
|[`PopupOverlaySetting`](#popupoverlaysetting)|`RenderFragment`|N/A|
|[`ProgressiveZooming`](#progressivezooming)|`Boolean`|N/A|
|[`RestrictExtent`](#restrictextent)|[`RectangleShape`](../ThinkGeo.Core/ThinkGeo.Core.RectangleShape.md)|N/A|
|[`Width`](#width)|`Int32`|N/A|
|[`Zoom`](#zoom)|`Int32`|N/A|
|[`ZoomLevelSet`](#zoomlevelset)|[`ZoomLevelSet`](../ThinkGeo.Core/ThinkGeo.Core.ZoomLevelSet.md)|N/A|

### Protected Properties Summary

|Name|Return Type|Description|
|---|---|---|
|[`JSRuntime`](#jsruntime)|`IJSRuntime`|N/A|

### Public Methods Summary


|Name|
|---|
|[`Equals(Object)`](#equalsobject)|
|[`GetCenterAsync()`](#getcenterasync)|
|[`GetCurrentExtentAsync()`](#getcurrentextentasync)|
|[`GetHashCode()`](#gethashcode)|
|[`GetType()`](#gettype)|
|[`PanToAsync(PointShape)`](#pantoasyncpointshape)|
|[`SetCenterAsync(PointShape)`](#setcenterasyncpointshape)|
|[`SetParametersAsync(ParameterView)`](#setparametersasyncparameterview)|
|[`ToString()`](#tostring)|
|[`ZoomInAsync()`](#zoominasync)|
|[`ZoomOutAsync()`](#zoomoutasync)|
|[`ZoomToCenterAsync(Int32,PointShape)`](#zoomtocenterasyncint32pointshape)|

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
|[`MapView()`](#mapview)|

### Protected Constructors


### Public Properties

#### `AdornmentOverlay`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

[`AdornmentOverlay`](ThinkGeo.UI.Blazor.AdornmentOverlay.md)

---
#### `AdornmentOverlaySetting`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`RenderFragment`

---
#### `BackgroundColor`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

[`GeoColor`](../ThinkGeo.Core/ThinkGeo.Core.GeoColor.md)

---
#### `Center`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

[`PointShape`](../ThinkGeo.Core/ThinkGeo.Core.PointShape.md)

---
#### `EditOverlay`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

[`EditOverlay`](ThinkGeo.UI.Blazor.EditOverlay.md)

---
#### `EditOverlaySetting`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`RenderFragment`

---
#### `Height`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`Int32`

---
#### `Id`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`String`

---
#### `IsLoaded`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`Boolean`

---
#### `MapTools`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

[`MapTools`](ThinkGeo.UI.Blazor.MapTools.md)

---
#### `MapToolsSetting`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`RenderFragment`

---
#### `MapUnit`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

[`GeographyUnit`](../ThinkGeo.Core/ThinkGeo.Core.GeographyUnit.md)

---
#### `MapViewSizeUnitType`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

[`MapViewSizeUnitType`](ThinkGeo.UI.Blazor.MapViewSizeUnitType.md)

---
#### `MarkerOverlay`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

[`MarkerOverlay`](ThinkGeo.UI.Blazor.MarkerOverlay.md)

---
#### `MarkerOverlaySetting`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`RenderFragment`

---
#### `OnClick`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

EventCallback<[`ClickedMapViewEventArgs`](ThinkGeo.UI.Blazor.ClickedMapViewEventArgs.md)>

---
#### `OnCurrentExtentChanged`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

EventCallback<[`CurrentExtentChangedMapViewEventArgs`](ThinkGeo.UI.Blazor.CurrentExtentChangedMapViewEventArgs.md)>

---
#### `OnDoubleClick`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

EventCallback<[`DoubleClickedMapViewEventArgs`](ThinkGeo.UI.Blazor.DoubleClickedMapViewEventArgs.md)>

---
#### `OnMouseMove`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

EventCallback<[`MouseMovingMapViewEventArgs`](ThinkGeo.UI.Blazor.MouseMovingMapViewEventArgs.md)>

---
#### `OnSizeChanged`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

EventCallback<[`SizeChangedMapViewEventArgs`](ThinkGeo.UI.Blazor.SizeChangedMapViewEventArgs.md)>

---
#### `Overlays`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

GeoCollection<[`Overlay`](ThinkGeo.UI.Blazor.Overlay.md)>

---
#### `OverlaysSetting`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`RenderFragment`

---
#### `PopupOverlay`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

[`PopupOverlay`](ThinkGeo.UI.Blazor.PopupOverlay.md)

---
#### `PopupOverlaySetting`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`RenderFragment`

---
#### `ProgressiveZooming`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`Boolean`

---
#### `RestrictExtent`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

[`RectangleShape`](../ThinkGeo.Core/ThinkGeo.Core.RectangleShape.md)

---
#### `Width`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`Int32`

---
#### `Zoom`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`Int32`

---
#### `ZoomLevelSet`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

[`ZoomLevelSet`](../ThinkGeo.Core/ThinkGeo.Core.ZoomLevelSet.md)

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
#### `GetCenterAsync()`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|Task<[`PointShape`](../ThinkGeo.Core/ThinkGeo.Core.PointShape.md)>|N/A|

**Parameters**

|Name|Type|Description|
|---|---|---|
|N/A|N/A|N/A|

---
#### `GetCurrentExtentAsync()`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|Task<[`RectangleShape`](../ThinkGeo.Core/ThinkGeo.Core.RectangleShape.md)>|N/A|

**Parameters**

|Name|Type|Description|
|---|---|---|
|N/A|N/A|N/A|

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
#### `PanToAsync(PointShape)`

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
|center|[`PointShape`](../ThinkGeo.Core/ThinkGeo.Core.PointShape.md)|N/A|

---
#### `SetCenterAsync(PointShape)`

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
|center|[`PointShape`](../ThinkGeo.Core/ThinkGeo.Core.PointShape.md)|N/A|

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
#### `ZoomInAsync()`

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
#### `ZoomOutAsync()`

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
#### `ZoomToCenterAsync(Int32,PointShape)`

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
|zoom|`Int32`|N/A|
|center|[`PointShape`](../ThinkGeo.Core/ThinkGeo.Core.PointShape.md)|N/A|

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


