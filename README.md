WIP

![Main Panel](/Images/panel_image1.png "Panel")

This mod allows for the easy dumping of networks inside the Cities: Skylines Road Editor.

Files this tool dumps:

_d.png\
_a.png\
_p.png\
_r.png\
.obj\
_lod.obj

Use:
1) Open up the road editor and select a road to edit
2) Select the Elevation (Basic, Elevated, etc), Mesh Type (Segment, Node) and Mesh Number (The index number of the mesh)
3) Press the Dump Button
4) If successful, the road files are dumped in the Cities_Skylines\Addons\Import folder
5) Generate LOD images anytime afterwards by clicking "Make _lod.png Files" - this allows the lod images to be generated after editing the main textures

Options:
-Main Panel--
1) Net Elevation - Select the elevation to dump (Basic, Elevated, etc)
2) Mesh Type - Select the mesh type (Segment, Node)
3) Dump Mesh # - The index of the mesh in the Segment/Node area in the vanilla Road Properties panel (First mesh in the list is "1")
4) Dump Network - Dumps Network in \Import Folder - Only the relevant files are exported, If a network uses the default texture for a certain map type it is not exported
5) Make _lod.png Files - Generates LOD images of the related textures (_d,_p, etc) for last dumped mesh. If no mesh was dumped in the session, the files that match the custom file prefix (in export customization) will be used 

-Export Customization-
1) Only export the specific files you need - Filters are available to export the meshes only or the diffuse texture only
2) Flip Textures - Flip Textures horizontally after exporting
	 >The default setting is true for the "Basic" elevation but false for the other elvations. This option compensates for the dumped meshes being mirrored. When the texture isn't symmetrical, this feature needs to be disabled (i.e. Elevated Roads, Industries DLC roads, Certain custom roads)

3) Custom File Prefix - Dump the mesh with a custom filename
4) Remove Added Suffixes - Removes the added descriptors at the end of dumped file names such as ("Elevated" or "_mesh2 Tunnel _node")
	>Caution: Each new export regardless of elevation/mesh type will overwrite the previous file with the same name

-Mesh Resizing-
1) View Mesh Points - View all the points of the mesh at the cross section z=32
2) Point Replacer - enter the existing position (value that is in the Pos table in the Mesh Points Window) and the new position - All of the (x) points that match the existing postion in the main mesh and in the lod are replaced with the new value.
> (from left to right) Resets entered values | Changes entering mode | Add Row | Delete Row


FAQ:

Q: When reimporting a dumped road the texture is completely mismatched with the road
A: Try unchecking the Flip Textures Option under Export Customization
