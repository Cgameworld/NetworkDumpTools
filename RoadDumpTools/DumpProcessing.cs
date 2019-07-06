using System;
using System.IO;
using ColossalFramework.IO;
using ModTools.Utils;
using ObjUnity3D;
using ColossalFramework;
using UnityEngine;
using ColossalFramework.UI;

namespace RoadDumpTools
{
    public class DumpProcessing
    {
        ToolController sim = Singleton<ToolController>.instance;
        string networkName_init;
        int filesExported;
        string exportedFilePaths;

        public string [] DumpNetworks()
        {
            try
            {
                // cancel if they key input was already processed in a previous frame
                networkName_init = sim.m_editPrefabInfo.name;
                string importFolder = Path.Combine(DataLocation.addonsPath, "Import");
                string networkName;
                string filename;
                Material material;
                Material aprmaterial;
                Mesh roadMesh;
                Mesh roadMeshLod;
                
                if (networkName_init.Contains("_Data"))
                {
                    networkName = networkName_init.Substring(0, networkName_init.Length - 1);
                }
                else { networkName = networkName_init; }

                //Debug.Log(networkName);

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
                Debug.Log("filename: aa : " + filename);
                int meshnum = 0;

                if (int.TryParse(NetDumpPanel.instance.MeshNumber, out meshnum))
                {
                    if (meshnum > 1)
                    {
                        Debug.Log("mnbefore" + meshnum);
                        filename = filename + "_mesh" + meshnum;
                    }
                    meshnum = meshnum - 1; //adjust for array
                    Debug.Log("mnafter" + meshnum);
                }
                else
                {
                    throw new System.ArgumentException("Mesh Number Not Found");
                }


                string diffuseTexturePath = Path.Combine(importFolder, filename);
                string meshPath = Path.Combine(importFolder, filename);
                string lodMeshPath = Path.Combine(importFolder, filename);
                string aFilePath = Path.Combine(importFolder, filename);
                string pFilePath = Path.Combine(importFolder, filename);
                string rFilePath = Path.Combine(importFolder, filename);

                Debug.Log("meshnum" + meshnum);

                

                if (NetDumpPanel.instance.NetworkType== "Segment")
                {
                    material = PrefabCollection<NetInfo>.FindLoaded(networkName).m_segments[meshnum].m_segmentMaterial;
                    diffuseTexturePath += "_d.png";
                    meshPath += ".obj";
                    lodMeshPath += "_lod.obj";
                    aFilePath += "_a.png";
                    pFilePath += "_p.png";
                    rFilePath += "_r.png";
                    roadMesh = PrefabCollection<NetInfo>.FindLoaded(networkName).m_segments[meshnum].m_mesh;
                    roadMeshLod = PrefabCollection<NetInfo>.FindLoaded(networkName).m_segments[meshnum].m_lodMesh;
                    aprmaterial = PrefabCollection<NetInfo>.FindLoaded(networkName).m_segments[meshnum].m_segmentMaterial;

                }
                else if (NetDumpPanel.instance.NetworkType== "Node")
                {
                    material = PrefabCollection<NetInfo>.FindLoaded(networkName).m_nodes[meshnum].m_nodeMaterial;
                    diffuseTexturePath += "_node_d.png";
                    meshPath += "_node.obj";
                    lodMeshPath += "_node_lod.obj";
                    aFilePath += "_node_a.png";
                    pFilePath += "_node_p.png";
                    rFilePath += "_node_r.png";
                    roadMesh = PrefabCollection<NetInfo>.FindLoaded(networkName).m_nodes[meshnum].m_mesh;
                    roadMeshLod = PrefabCollection<NetInfo>.FindLoaded(networkName).m_nodes[meshnum].m_lodMesh;
                    aprmaterial = PrefabCollection<NetInfo>.FindLoaded(networkName).m_nodes[meshnum].m_nodeMaterial;
                }
                else
                {
                    throw new System.ArgumentException("Invalid network selection type");
                }



                var source = material.GetTexture("_MainTex") as Texture2D;
                var target = new Texture2D(source.width, source.height, TextureFormat.RGBAFloat, true);
                target.SetPixels(source.GetPixels());
                target.anisoLevel = source.anisoLevel; target.filterMode = source.filterMode;
                target.wrapMode = source.wrapMode; target.Apply();
                UnityEngine.Object.FindObjectOfType<NetProperties>().m_downwardDiffuse = target;



                /**features to add
                 * 
                 * 
                 * 
                 * add reset settings button (nah)
                 * when texture is default color don't export! (also add to log)
                 * 
                 * add lod generator for exisiting files
                 * doesn't support specular no nets have it?
                 * add feature export all maps anyway - etc?
                 * collapsable menu to hide advanced features?
                 * add log button shows log of all exported roads? - do it by adding file name to string as it
                 * keyboard shortcut reimports road exported (find method in ILSPY?)!
                 * if mesh problems import in blender - check UV - reexport as fbx!
                 */

                bool flippingTextures = NetDumpPanel.instance.GetIfFlippedTextures;
                Texture2D aprsource = aprmaterial.GetTexture("_APRMap") as Texture2D;

                if (NetDumpPanel.instance.GetDumpMeshOnly && NetDumpPanel.instance.GetDumpDiffuseOnly)
                {
                    DumpTexture2D(FlipTexture(target, false, flippingTextures), diffuseTexturePath);
                    DumpMeshToOBJ(roadMesh, meshPath);
                    DumpMeshToOBJ(roadMeshLod, lodMeshPath);
                }
                else if (NetDumpPanel.instance.GetDumpDiffuseOnly)
                {
                    DumpTexture2D(FlipTexture(target, false, flippingTextures), diffuseTexturePath);
                }
                else if (NetDumpPanel.instance.GetDumpMeshOnly)
                {
                    DumpMeshToOBJ(roadMesh, meshPath);
                    DumpMeshToOBJ(roadMeshLod, lodMeshPath);
                }
                else
                {
                    DumpTexture2D(FlipTexture(target, false, flippingTextures), diffuseTexturePath);
                    DumpAPR(filename, FlipTexture(aprsource, false, flippingTextures), aFilePath, pFilePath, rFilePath, true);
                    Debug.Log("default dump setting!");

                    //dump meshes
                    DumpMeshToOBJ(roadMesh, meshPath);
                    DumpMeshToOBJ(roadMeshLod, lodMeshPath);
                    DumpAPR(filename, FlipTexture(aprsource, false, flippingTextures), aFilePath, pFilePath, rFilePath, true);
                }
                //for workshop roads display disclaimer!
                // display message
                //also add log for apr textures!

                ExceptionPanel panel = UIView.library.ShowModal<ExceptionPanel>("ExceptionPanel");

                string [] combinedPaths =   {diffuseTexturePath, meshPath, lodMeshPath, aFilePath, pFilePath, rFilePath };

                exportedFilePaths = "";

                for (int i=0; i<combinedPaths.Length; i++)
                {
                    if (File.Exists(combinedPaths[i]))
                    {
                        exportedFilePaths += "\n" + combinedPaths[i];
                        filesExported += 1;
                    }
                }

                panel.SetMessage("Network Dump Successful", "Network Name: " + networkName + "\n\nDumped Items:\n" + exportedFilePaths, false);

            }
            catch (Exception e)
            {

                ExceptionPanel panel = UIView.library.ShowModal<ExceptionPanel>("ExceptionPanel");
                panel.SetMessage("Network Dump Failed", "" + e, false);

                filesExported = 0;
            }

            string[] returnArray = { filesExported.ToString(), exportedFilePaths };
            return returnArray;
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
                    if (tex.GetPixel(x, y).Equals(Color.white)){}
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
                    if (tex.GetPixel(x, y).Equals(Color.black)){}
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


        public static void DumpMeshToOBJ(Mesh mesh, string fileName)
        {
            fileName = Path.Combine(Path.Combine(DataLocation.addonsPath, "Import"), fileName);
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }

            
            /*
            Debug.Log("mesh vertices for Mesh: " + fileName);
            string outputVerts = "";

            Vector3[] vertices;
            vertices = mesh.vertices;

            for (int i = 0; i < vertices.Length; i++)
            {
                if (vertices[i].x == 5.0f)
                {
                    Debug.Log("checked x worked!");
                    outputVerts += vertices[i] + "\n";
                    vertices[i].x = 6.0f;
                }
                if (vertices[i].x == -5.0f)
                {
                    Debug.Log("checked x worked!");
                    outputVerts += vertices[i] + "\n";
                    vertices[i].x = -6.0f;
                }

            }
            Debug.Log(outputVerts);

            mesh.vertices = vertices;
            mesh.RecalculateBounds();
            //recalc here less overhead  

            string outputVerts2 = "";
            for (int i = 0; i < mesh.vertices.Length; i++)
            {
                    outputVerts2 += mesh.vertices[i] + "afterappl\n";

            }
            Debug.Log(outputVerts2);
            */

            var meshToDump = mesh;

            if (!mesh.isReadable)
            {
                try
                {
                    // copy the relevant data to the temporary mesh
                    Debug.Log("mesh not readable");
                    meshToDump = new Mesh
                    {
                        vertices = mesh.vertices,
                        colors = mesh.colors,
                        triangles = mesh.triangles,
                        normals = mesh.normals,
                        tangents = mesh.tangents,
                    };
                    meshToDump.RecalculateBounds();
                }
                catch (Exception ex)
                {
                    ;
                    return;
                }
            }

            try
            {
                Debug.Log("mesh readable");
                using (var stream = new FileStream(fileName, FileMode.Create))
                {
                    OBJLoader.ExportOBJ(meshToDump.EncodeOBJ(), stream);
                }
            }
            catch (Exception ex)
            {
            }
        }



    }
}
