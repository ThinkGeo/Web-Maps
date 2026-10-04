# Blazor Map Navigation Sample Changes

This document summarizes the changes made to the Blazor **Map Navigation** sample to align its functionality and presentation with the WPF v15 Map Navigation sample.

## Initial Map View

- Centered the map on the Empire State Building at latitude `40.74843661` and longitude `-73.9856654`.
- Set the initial Blazor zoom level to `12`.
- Set the initial bearing to `30` degrees, which corresponds to the WPF rotation value of `-30` degrees.
- Added an Empire State Building marker using the image copied from the WPF v15 sample.

## Live Map Information

Added a readout at the bottom center of the map that updates as the view changes and displays:

- Center latitude
- Center longitude
- Rotation
- Zoom level
- Scale

The layout follows the WPF v15 sample, with the center coordinates on the first line and the rotation, zoom, and scale values on the second line.

## Camera Panel

Added a camera panel on the right side of the map with the following controls:

- **3D Buildings** checkbox for showing or hiding building extrusions.
- **Pitch** slider with a range from 0 to 80 degrees.
- **Dark Theme / Light Theme** button for switching the vector map style.

The 3D building styles are prepared in advance so that switching the checkbox does not require rebuilding the style during every interaction.

## Compass

- Copied the original north-arrow image from the WPF v15 sample.
- Positioned the compass in the upper-right corner of the map using the same 40-by-40-pixel size and spacing as the WPF sample.
- Connected the compass directly to the MapLibre `rotate` event so that it rotates continuously while the user rotates the map.
- Clicking the compass resets the map to north-up without changing its center or scale.

## Default Extent Button

- Copied the original globe image from the WPF v15 sample.
- Positioned it directly below the compass.
- Clicking the globe returns the map to the Empire State Building at scale `100,000` and bearing `30` degrees, matching the WPF v15 default view.

## Navigation Bar

Added a WPF-style navigation bar to the upper-left corner of the map:

- Four directional buttons pan the map up, down, left, or right.
- The plus button zooms in.
- The minus button zooms out.
- The vertical slider changes the zoom continuously and follows zoom changes made through other map interactions.
- The center blue dot returns the map to the Empire State Building at displayed zoom level `19` and rotation `-30`.

The navigation controls run directly in the browser to provide immediate feedback without waiting for a Blazor Server round trip.

## Files Changed

- `Pages/Map_Navigation.razor`
  - Map configuration, marker, readout, camera controls, compass, default extent button, and navigation bar.
- `Pages/_Host.cshtml`
  - Browser-side event handling for continuous compass rotation and navigation bar interaction.
- `wwwroot/images/empire_state_building.png`
  - Empire State Building marker copied from the WPF v15 sample.
- `wwwroot/images/icon_north_arrow.png`
  - Compass image copied from the WPF v15 sample.
- `wwwroot/images/icon_globe_black.png`
  - Default extent image copied from the WPF v15 sample.

## Validation

The Blazor HowDoI solution was built in Release configuration after the changes:

```text
Build succeeded.
0 errors.
```

The remaining build warnings were pre-existing warnings in unrelated samples.
