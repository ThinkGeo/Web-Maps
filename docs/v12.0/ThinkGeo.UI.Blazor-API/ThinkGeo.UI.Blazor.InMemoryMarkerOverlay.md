# InMemoryMarkerOverlay


## Inheritance Hierarchy

+ `Object`
  + `ComponentBase`
    + [`Overlay`](ThinkGeo.UI.Blazor.Overlay.md)
      + [`MarkerOverlay`](ThinkGeo.UI.Blazor.MarkerOverlay.md)
        + **`InMemoryMarkerOverlay`**

## Members Summary

### Public Constructors Summary


|Name|
|---|
|[`InMemoryMarkerOverlay()`](#inmemorymarkeroverlay)|
|[`InMemoryMarkerOverlay(String)`](#inmemorymarkeroverlaystring)|

### Protected Constructors Summary


|Name|
|---|
|N/A|

### Public Properties Summary

|Name|Return Type|Description|
|---|---|---|
|[`FeatureSource`](#featuresource)|[`InMemoryFeatureSource`](../ThinkGeo.Core/ThinkGeo.Core.InMemoryFeatureSource.md)|Gets a FeatureSource object by which you can perform queries on the features in the overlay.|
|[`Id`](#id)|`String`|N/A|
|[`IsVisible`](#isvisible)|`Boolean`|N/A|
|[`OnClick`](#onclick)|EventCallback<[`ClickedMarkerOverlayEventArgs`](ThinkGeo.UI.Blazor.ClickedMarkerOverlayEventArgs.md)>|N/A|
|[`OnMouseOut`](#onmouseout)|EventCallback<[`MouseOutMarkerOverlayEventArgs`](ThinkGeo.UI.Blazor.MouseOutMarkerOverlayEventArgs.md)>|N/A|
|[`OnMouseOver`](#onmouseover)|EventCallback<[`MouseOverMarkerOverlayEventArgs`](ThinkGeo.UI.Blazor.MouseOverMarkerOverlayEventArgs.md)>|N/A|
|[`ZoomLevelSet`](#zoomlevelset)|[`MarkerZoomLevelSet`](ThinkGeo.UI.Blazor.MarkerZoomLevelSet.md)|Gets the ZoomLevelSet object that controls the markers' generation.|

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
|[`GetMarkers(RectangleShape,Int32)`](#getmarkersrectangleshapeint32)|
|[`GetType()`](#gettype)|
|[`RedrawAsync()`](#redrawasync)|
|[`SetParametersAsync(ParameterView)`](#setparametersasyncparameterview)|
|[`ToString()`](#tostring)|

### Protected Methods Summary


|Name|
|---|
|[`AddMarker(Marker)`](#addmarkermarker)|
|[`BuildRenderTree(RenderTreeBuilder)`](#buildrendertreerendertreebuilder)|
|[`Finalize()`](#finalize)|
|[`GetMarkersCore(RectangleShape,Int32)`](#getmarkerscorerectangleshapeint32)|
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
|[`InMemoryMarkerOverlay()`](#inmemorymarkeroverlay)|
|[`InMemoryMarkerOverlay(String)`](#inmemorymarkeroverlaystring)|

### Protected Constructors


### Public Properties

#### `FeatureSource`

**Summary**

   *Gets a FeatureSource object by which you can perform queries on the features in the overlay.*

**Remarks**

   *N/A*

**Return Value**

[`InMemoryFeatureSource`](../ThinkGeo.Core/ThinkGeo.Core.InMemoryFeatureSource.md)

---
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
#### `OnClick`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

EventCallback<[`ClickedMarkerOverlayEventArgs`](ThinkGeo.UI.Blazor.ClickedMarkerOverlayEventArgs.md)>

---
#### `OnMouseOut`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

EventCallback<[`MouseOutMarkerOverlayEventArgs`](ThinkGeo.UI.Blazor.MouseOutMarkerOverlayEventArgs.md)>

---
#### `OnMouseOver`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

EventCallback<[`MouseOverMarkerOverlayEventArgs`](ThinkGeo.UI.Blazor.MouseOverMarkerOverlayEventArgs.md)>

---
#### `ZoomLevelSet`

**Summary**

   *Gets the ZoomLevelSet object that controls the markers' generation.*

**Remarks**

   *N/A*

**Return Value**

[`MarkerZoomLevelSet`](ThinkGeo.UI.Blazor.MarkerZoomLevelSet.md)

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
#### `GetMarkers(RectangleShape,Int32)`

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
|worldExtent|[`RectangleShape`](../ThinkGeo.Core/ThinkGeo.Core.RectangleShape.md)|N/A|
|currentZoomLevelId|`Int32`|N/A|

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

#### `AddMarker(Marker)`

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
|marker|[`Marker`](ThinkGeo.UI.Blazor.Marker.md)|N/A|

---
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
#### `GetMarkersCore(RectangleShape,Int32)`

**Summary**

   *Returns a collection of markers based on the extent and zoomlevel that you passed in.*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|Collection<[`Marker`](ThinkGeo.UI.Blazor.Marker.md)>|A collection of marker objects that will be serialized to the client.|

**Parameters**

|Name|Type|Description|
|---|---|---|
|worldExtent|[`RectangleShape`](../ThinkGeo.Core/ThinkGeo.Core.RectangleShape.md)|The extent that includes all of the markers that you want to serialize.|
|currentZoomLevelId|`Int32`|An int value that indicates which zoomlevel the map is currently at.|

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


