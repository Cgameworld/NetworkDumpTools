WIP

![](Images/panel-image1.png)

This mod allows for the easy dumping of roads inside the Cities: Skylines Road Editor.


Files this tool dumps:

_d.png\
_a.png\
_p.png\
_r.png\
.obj\
_lod.obj

Use:
1) Open up the road editor and select a road to edit
2) Press the control and comma keys (ctrl+,) together to open the tool inside the asset editor
3) Press the Dump Button
4) If successful, the road files are dumped in the Cities_Skylines\Addons\Import folder
5) Generate LOD images afterwards by clicking "Make _lod.png Files"

Features:
1) Dump any part of a network, just select the Elevation (Basic, Elevated, etc), Mesh Type (Segment, Node) and Mesh Number (The place the mesh appears in the list)
2) Only the relevant files are exported, If a network uses the default texture for a certain map type it is not exported
3) Generating LOD pngs is separate from dumping so (if you want) you can edit the main texture and generate a lod file of the edited file after editing

-Export Customization-
1) Only export the specific files you need - Filters are avalible to export the meshes only or the diffuse texture only
2) Dump the mesh with a custom filename

-Mesh Reszing-
1) Find and replace points 
2) View all the points of the mesh at the cross section z=32
3) Number of fields is customizable
4) (add more explaination)

FAQ:

Q: When reimporting a dumped road the texture is completely mismatched with the road
A: Try unchecking the Flip Textures Option under Export Customization
