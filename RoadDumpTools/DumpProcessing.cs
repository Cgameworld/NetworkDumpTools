using System;
using System.IO;
using ColossalFramework.IO;
using ObjUnity3D;
using ColossalFramework;
using UnityEngine;
using ColossalFramework.UI;
using System.Linq;
using RoadDumpTools.Lib;
using System.Xml.Serialization;
using RoadImporterXML;

namespace RoadDumpTools
{
    public class DumpProcessing
    {
        ToolController sim = Singleton<ToolController>.instance;
        int filesExported;
        string exportedFilePaths;
        string importFolder = Path.Combine(DataLocation.addonsPath, "Import");
        string networkName;
        string filename;
        Material material;
        Material aprmaterial;
        Mesh roadMesh;
        Mesh roadMeshLod;
        int meshnum;
        NetInfo loadedPrefab;


        string diffuseTexturePath;
        string meshPath;
        string lodMeshPath;
        string aFilePath;
        string pFilePath;
        string rFilePath;

        Vector3[] newvertices;
        public string bulkErrorText;

        public string[] DumpNetworks(bool endPopup = true)
        {
            try
            {
                string networkName_init = sim.m_editPrefabInfo.name;
                FindMesh(networkName_init);
                var source = material.GetTexture("_MainTex") as Texture2D;
                var target = new Texture2D(source.width, source.height, TextureFormat.RGBAFloat, true);
                target.SetPixels(source.GetPixels());
                target.anisoLevel = source.anisoLevel; target.filterMode = source.filterMode;
                target.wrapMode = source.wrapMode; target.Apply();
                UnityEngine.Object.FindObjectOfType<NetProperties>().m_downwardDiffuse = target;

                bool flippingTextures = NetDumpPanel.instance.GetIfFlippedTextures;
                Texture2D aprsource = aprmaterial.GetTexture("_APRMap") as Texture2D;

                if (NetDumpPanel.instance.GetDumpMeshOnly && NetDumpPanel.instance.GetDumpDiffuseOnly)
                {
                    DumpTexture2D(FlipTexture(target, false, flippingTextures), diffuseTexturePath);
                    DumpMeshToOBJ(roadMesh, meshPath, loadedPrefab);
                    DumpMeshToOBJ(roadMeshLod, lodMeshPath, loadedPrefab);
                }
                else if (NetDumpPanel.instance.GetDumpDiffuseOnly)
                {
                    DumpTexture2D(FlipTexture(target, false, flippingTextures), diffuseTexturePath);
                }
                else if (NetDumpPanel.instance.GetDumpMeshOnly)
                {
                    DumpMeshToOBJ(roadMesh, meshPath, loadedPrefab);
                    DumpMeshToOBJ(roadMeshLod, lodMeshPath, loadedPrefab);
                }
                else
                {
                    DumpTexture2D(FlipTexture(target, false, flippingTextures), diffuseTexturePath);
                    DumpAPR(filename, FlipTexture(aprsource, false, flippingTextures), aFilePath, pFilePath, rFilePath, true);
                    //dump meshes
                    DumpMeshToOBJ(roadMesh, meshPath, loadedPrefab);
                    DumpMeshToOBJ(roadMeshLod, lodMeshPath, loadedPrefab);
                    DumpAPR(filename, FlipTexture(aprsource, false, flippingTextures), aFilePath, pFilePath, rFilePath, true);
                    //ExportNetInfoXML()


                    Debug.Log("roadimporter xml begin");

                    TextWriter writer = new StreamWriter(Path.Combine(Path.Combine(DataLocation.addonsPath, "Import"), $"{loadedPrefab.name}.xml"));

                    Debug.Log(loadedPrefab.GetType());

                    if (loadedPrefab.m_netAI.GetType() == typeof(RoadAI))
                    {
                        RoadAssetInfo roadAsset = new RoadAssetInfo();
                        roadAsset.ReadFromGame(loadedPrefab);

                        XmlSerializer ser = new XmlSerializer(typeof(RoadImporterXML.RoadAssetInfo));
                        ser.Serialize(writer, roadAsset);
                    }
                    else if (loadedPrefab.m_netAI.GetType() == typeof(TrainTrackAI))
                    {
                        TrainTrackAssetInfo trainAsset = new TrainTrackAssetInfo();
                        trainAsset.ReadFromGame(loadedPrefab);

                        XmlSerializer ser = new XmlSerializer(typeof(RoadImporterXML.TrainTrackAssetInfo));
                        ser.Serialize(writer, trainAsset);
                    }
                    else
                    {
                        throw new NotImplementedException("NetInfo XML Export Error!");
                    }
                    writer.Close();
                    Debug.Log("success!!!");
                }
                //for workshop roads display disclaimer!
                // display message
                //also add log for apr textures!

                ExceptionPanel panel = UIView.library.ShowModal<ExceptionPanel>("ExceptionPanel");

                string[] combinedPaths = { diffuseTexturePath, meshPath, lodMeshPath, aFilePath, pFilePath, rFilePath };

                exportedFilePaths = "";

                if (NetDumpPanel.instance.GetDumpMeshOnly)
                {
                    exportedFilePaths = "Mesh Dump Only\n" + meshPath + "\n" + lodMeshPath;
                    filesExported += 2;
                }
                else
                {
                    for (int i = 0; i < combinedPaths.Length; i++)
                    {
                        if (File.Exists(combinedPaths[i]))
                        {
                            exportedFilePaths += "\n" + combinedPaths[i];
                            filesExported += 1;
                        }
                    }
                }

                if (endPopup == true)
                {
                    panel.SetMessage("Network Dump Successful", "Network Name: " + networkName + "\n\nDumped Items:\n" + exportedFilePaths, false);
                }

            }
            catch (Exception e)
            {
                if (endPopup == true)
                {
                    ExceptionPanel panel = UIView.library.ShowModal<ExceptionPanel>("ExceptionPanel");
                    panel.SetMessage("Network Dump Failed", "" + e, false);
                    panel.GetComponentInChildren<UISprite>().spriteName = "IconError";
                    filesExported = 0;
                }
                else
                {
                    bulkErrorText = e.ToString();
                }
            }

            string[] returnArray = { filesExported.ToString(), exportedFilePaths, diffuseTexturePath.Substring(0, diffuseTexturePath.LastIndexOf("_"))};
            return returnArray;
        }

        public void FindMesh(string networkName_init)
        {
            if (networkName_init.Contains("_Data"))
            {
                networkName = networkName_init.Substring(0, networkName_init.Length - 1);
            }
            else { networkName = networkName_init; }

            if (NetDumpPanel.instance.GetCustomFilePrefix() == "")
            {
                Debug.Log("no custom prefix");
                if (networkName_init.Contains("_Data"))
                {
                    filename = networkName.Substring(0, networkName.Length - 5).Replace("/", string.Empty);
                }
                else { filename = networkName.Substring(0, networkName.Length - 1); }
            }
            else
            {
                Debug.Log("custom prefix");
                filename = NetDumpPanel.instance.GetCustomFilePrefix();
            }
            meshnum = 0;

            if (int.TryParse(NetDumpPanel.instance.MeshNumber, out meshnum))
            {
                if (meshnum > 1)
                {
                    filename = filename + "_mesh" + meshnum;
                }
                meshnum = meshnum - 1; //adjust for array
            }
            else
            {
                throw new System.ArgumentException("Mesh Number Not Found");
            }

            //check for elevations
            loadedPrefab = PrefabCollection<NetInfo>.FindLoaded(networkName);

            int netElevationIndex = NetDumpPanel.instance.GetNetEleIndex;
            switch (netElevationIndex) //if null throw error!
            {
                case 0:
                    Console.WriteLine("basic ground no change");
                    break;
                case 1:
                    Console.WriteLine("Elevated");
                    loadedPrefab = AssetEditorRoadUtils.TryGetElevated(loadedPrefab);
                    if (loadedPrefab == null)
                    {
                        throw new Exception("Elevated Elevation Does Not Exist");
                    }
                    if (!NetDumpPanel.instance.GetRemoveSuffix)
                    filename += " Elevated";
                    break;
                case 2:
                    Console.WriteLine("Bridge");
                    loadedPrefab = AssetEditorRoadUtils.TryGetBridge(loadedPrefab);
                    if (loadedPrefab == null)
                    {
                        throw new Exception("Bridge Elevation Does Not Exist");
                    }
                    if (!NetDumpPanel.instance.GetRemoveSuffix)
                    filename += " Bridge";
                    break;
                case 3:
                    loadedPrefab = AssetEditorRoadUtils.TryGetSlope(loadedPrefab);
                    if (loadedPrefab == null)
                    {
                        throw new Exception("Slope Elevation Does Not Exist");
                    }
                    if (!NetDumpPanel.instance.GetRemoveSuffix)
                    filename += " Slope";
                    break;
                case 4:
                    loadedPrefab = AssetEditorRoadUtils.TryGetTunnel(loadedPrefab);
                    if (loadedPrefab == null)
                    {
                        throw new Exception("Tunnel Elevation Does Not Exist");
                    }
                    if (!NetDumpPanel.instance.GetRemoveSuffix)
                    filename += " Tunnel";
                    break;
                default:
                    throw new System.ArgumentOutOfRangeException("No Elevations Found");

            }

            Debug.Log("Prefab: " + loadedPrefab);


            diffuseTexturePath = Path.Combine(importFolder, filename);
            meshPath = Path.Combine(importFolder, filename);
            lodMeshPath = Path.Combine(importFolder, filename);
            aFilePath = Path.Combine(importFolder, filename);
            pFilePath = Path.Combine(importFolder, filename);
            rFilePath = Path.Combine(importFolder, filename);
         
            if (NetDumpPanel.instance.NetworkType == "Segment")
            {
                material = loadedPrefab.m_segments[meshnum].m_segmentMaterial;
                diffuseTexturePath += "_d.png";
                meshPath += ".obj";
                lodMeshPath += "_lod.obj";
                aFilePath += "_a.png";
                pFilePath += "_p.png";
                rFilePath += "_r.png";
                roadMesh = loadedPrefab.m_segments[meshnum].m_mesh;
                roadMeshLod = loadedPrefab.m_segments[meshnum].m_lodMesh;
                aprmaterial = loadedPrefab.m_segments[meshnum].m_segmentMaterial;

            }
            else if (NetDumpPanel.instance.NetworkType == "Node")
            {
                material = loadedPrefab.m_nodes[meshnum].m_nodeMaterial;
                if (NetDumpPanel.instance.GetRemoveSuffix)
                {
                    diffuseTexturePath += "_d.png";
                    meshPath += ".obj";
                    lodMeshPath += "_lod.obj";
                    aFilePath += "_a.png";
                    pFilePath += "_p.png";
                    rFilePath += "_r.png";
                }
                else
                {
                    diffuseTexturePath += "_node_d.png";
                    meshPath += "_node.obj";
                    lodMeshPath += "_node_lod.obj";
                    aFilePath += "_node_a.png";
                    pFilePath += "_node_p.png";
                    rFilePath += "_node_r.png";
                }

                roadMesh = loadedPrefab.m_nodes[meshnum].m_mesh;
                roadMeshLod = loadedPrefab.m_nodes[meshnum].m_lodMesh;
                aprmaterial = loadedPrefab.m_nodes[meshnum].m_nodeMaterial;
            }
            else
            {
                throw new System.ArgumentException("Invalid network selection type");
            }


        }
        public Vector3[] VerticesFromMesh()
        {
            string networkName_init = sim.m_editPrefabInfo.name;
            FindMesh(networkName_init);
            DumpMeshToOBJ(roadMesh, "none", loadedPrefab, false);
            return newvertices;
        }

        //Texture flipping script from https://stackoverflow.com/questions/35950660/unity-180-rotation-for-a-texture2d-or-maybe-flip-both
        Texture2D FlipTexture(Texture2D original, bool upSideDown = true, bool isflip = true)
        {
            if (isflip == false)
            {
                Debug.Log("no flipping!!");
                return original;
            }
            Texture2D flipped = new Texture2D(original.width, original.height);

            int xN = original.width;
            int yN = original.height;


            for (int i = 0; i < xN; i++)
            {
                for (int j = 0; j < yN; j++)
                {
                    if (upSideDown)
                    {
                        flipped.SetPixel(j, xN - i - 1, original.GetPixel(j, i));
                    }
                    else
                    {
                        flipped.SetPixel(xN - i - 1, j, original.GetPixel(i, j));
                    }
                }
            }
            flipped.Apply();

            return flipped;
        }

        //Methods below taken from ModTools
        // https://github.com/bloodypenguin/Skylines-ModTools
        private static void DumpAPR(string assetName, Texture2D aprMap, string ap, string pp, string rp, bool extract)
        {
            if (aprMap == null)
            {
                return;
            }

            if (extract)
            {
                var length = aprMap.width * aprMap.height;
                var a1 = new Color32[length].Invert();
                var p1 = new Color32[length].Invert();
                var r1 = new Color32[length].Invert();
                aprMap.ExtractChannels(a1, p1, r1, null, true, true, true, true, true, false, false);

                //checks to see if the textures are the default color
                if (IsAlphaDefault(a1.ColorsToTexture(aprMap.width, aprMap.height)) == false)
                {
                    TextureUtil.DumpTextureToPNG(a1.ColorsToTexture(aprMap.width, aprMap.height), ap);
                }

                if (IsPavementOrRoadDefault(p1.ColorsToTexture(aprMap.width, aprMap.height)) == false)
                {
                    TextureUtil.DumpTextureToPNG(p1.ColorsToTexture(aprMap.width, aprMap.height), pp);
                }
                if (IsPavementOrRoadDefault(r1.ColorsToTexture(aprMap.width, aprMap.height)) == false)
                {
                    TextureUtil.DumpTextureToPNG(r1.ColorsToTexture(aprMap.width, aprMap.height), rp);
                }

            }
            else
            {
                TextureUtil.DumpTextureToPNG(aprMap, $"{assetName}_APR");
            }
        }


        public static bool IsAlphaDefault(Texture2D tex)
        {

            for (int x = 0; x < tex.width; x++)
            {
                for (int y = 0; y < tex.height; y++)
                {
                    if (tex.GetPixel(x, y).Equals(Color.white)) { }
                    else { return false; }
                }
            }
            return true;
        }

        public static bool IsPavementOrRoadDefault(Texture2D tex)
        {
            for (int x = 0; x < tex.width; x++)
            {
                for (int y = 0; y < tex.height; y++)
                {
                    if (tex.GetPixel(x, y).Equals(Color.black)) { }
                    else { return false; }
                }
            }
            return true;
        }


        public static void DumpTexture2D(Texture2D texture, string filename)
        {
            byte[] bytes;
            try
            {
                bytes = texture.EncodeToPNG();
            }
            catch
            {
                try
                {
                    bytes = texture.MakeReadable().EncodeToPNG();
                }
                catch (Exception ex)
                {
                    Debug.Log("There was an error while dumping the texture - " + ex.Message);
                    return;
                }
            }

            File.WriteAllBytes(filename, bytes);
            Debug.Log($"Texture dumped to \"{filename}\"");
        }


        public void DumpMeshToOBJ(Mesh mesh, string fileName, NetInfo loadedPb, bool isDumping = true)
        {
            fileName = Path.Combine(Path.Combine(DataLocation.addonsPath, "Import"), fileName);
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }

           // Debug.Log("dumpobj" + filename);
            try
            {
                // copy the relevant data to the temporary mesh
                // Debug.Log("mesh not readable");
                Mesh meshToDump = new Mesh
                {
                    vertices = mesh.vertices,
                    colors = mesh.colors,
                    triangles = mesh.triangles,
                    normals = mesh.normals,
                    tangents = mesh.tangents,
                    uv = mesh.uv,
                    uv2 = mesh.uv,
                    name = mesh.name //name = mesh.name.Split('_')[0] //removes extra _0 at end of displayed name
                };
                meshToDump.RecalculateBounds();
                
                if (NetDumpPanel.instance.enableMeshResize.isChecked)
                {
                    newvertices = meshToDump.vertices; //get vertices for other method
                    float[] enteredVals = NetDumpPanel.instance.enteredMeshPoints();
                    float[] newVertsX = new float[newvertices.Length];
                    float[] newVertsY = new float[newvertices.Length];
                    for (int a = 0; a<newvertices.Length; a++)
                    {
                        newVertsX[a] = newvertices[a].x;
                        for (int b = 0; b < enteredVals.Length; b = b + 2)
                        {
                            if (Mathf.Approximately(newVertsX[a], enteredVals[b]))
                            {
                                newvertices[a].x = enteredVals[b+1];
                            }
                        }
                    }


                    meshToDump.vertices = newvertices;

                    meshToDump.RecalculateBounds();
                }



                if (!isDumping)
                {
                    newvertices = meshToDump.vertices; //get vertices for other method
                    Debug.Log("copied new verts");
                }


                else
                {

                    using (var stream = new FileStream(fileName, FileMode.Create))
                    {
                        OBJLoader.ExportOBJ(meshToDump.EncodeOBJ(), stream);
                    }



                }
            }
            catch (Exception ex)
            {

                Debug.Log("Nope! Mesh dumping doesn't work");
                throw new System.ArgumentException("Mesh Dumping Error - Check Point Replacer Input\n");
            }

        }

    }
}
