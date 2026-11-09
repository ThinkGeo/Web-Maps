# OpenDRIVE lane map for the Lane-Level Navigation sample

A lane-level HD map evaluated from ASAM OpenDRIVE into GeoJSON: every lane as a
polygon, every road mark as a line with its paint type, curbs, ground symbols, and
the vertical level of each road section where one road passes over another.
`XodrWolfsburgRoute.json` is a drive through the map: lane centerline points tagged
with the lane they lie in.

| files | source | license |
|---|---|---|
| `XodrWolfsburg*` | DLR "5G Living Lab" HD map of Wolfsburg, surveyed by atlatec - the drive the sample follows, 3.7 km of multi-lane arterials. https://doi.org/10.5281/zenodo.7072631 | CC BY 4.0 (`LICENSE-Wolfsburg-CC-BY-4.0.txt`, `CITATION-Wolfsburg.cff`) |

The DLR map is prototypic and published for research and development; its authors
guarantee neither completeness nor correctness. It is sample data here, not
navigation data.

The conversion from `.xodr` was done with the `tools/xodr-to-lanes.py` script that
ships with the desktop HowDoI samples (thinkgeo-desktop-maps, `samples/wpf/HowDoISample.v15`).
