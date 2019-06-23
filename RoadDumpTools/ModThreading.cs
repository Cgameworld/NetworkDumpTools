using ColossalFramework;
using ColossalFramework.IO;
using ColossalFramework.UI;
using ICities;
using ModTools.Utils;
using ObjUnity3D;
using System;
using System.IO;
using UnityEngine;

namespace RoadDumpTools
{
    public class ModThreading : ThreadingExtensionBase
    {
        private bool _processed = false;
        ToolController sim = Singleton<ToolController>.instance;

        public override void OnUpdate(float realTimeDelta, float simulationTimeDelta)
        {
            try
            {
                if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) && Input.GetKey(KeyCode.Comma))
                {
                    // cancel if they key input was already processed in a previous frame
                    if (_processed) return;
                    _processed = true;


                    string importFolder = Path.Combine(DataLocation.addonsPath, "Import");
                    string networkName_init = sim.m_editPrefabInfo.name;
                    string networkName;
                    string filename;
                    if (networkName_init.Contains("_Data"))
                    {
                        networkName = networkName_init.Substring(0, networkName_init.Length - 1);
                    }
                    else { networkName = networkName_init; }

                    Debug.Log(networkName);

                    var material = PrefabCollection<NetInfo>.FindLoaded(networkName).m_segments[0].m_segmentMaterial;
                    var source = material.GetTexture("_MainTex") as Texture2D;
                    var target = new Texture2D(source.width, source.height, TextureFormat.RGBAFloat, true);
                    target.SetPixels(source.GetPixels());
                    target.anisoLevel = source.anisoLevel; target.filterMode = source.filterMode;
                    target.wrapMode = source.wrapMode; target.Apply();
                    UnityEngine.Object.FindObjectOfType<NetProperties>().m_downwardDiffuse = target;

                    if (networkName_init.Contains("_Data"))
                    {
                        filename = networkName.Substring(0, networkName.Length - 5).Replace("/", string.Empty);
                    }
                    else { filename = networkName.Substring(0, networkName.Length - 1); }

                    string diffuseTexturePath = Path.Combine(importFolder, filename + "_d.png");
                    string meshPath = Path.Combine(importFolder, filename + ".obj");
                    string lodMeshPath = Path.Combine(importFolder, filename + "_lod.obj");

                    //DumpTexture2D(target, diffuseTexturePath);
                    DumpTexture2D(FlipTexture(target, false), diffuseTexturePath);

                    //dump meshes
                    Mesh roadMesh = PrefabCollection<NetInfo>.FindLoaded(networkName).m_segments[0].m_mesh;
                    DumpMeshToOBJ(roadMesh, meshPath);

                    Mesh roadMeshLod = PrefabCollection<NetInfo>.FindLoaded(networkName).m_segments[0].m_lodMesh;
                    DumpMeshToOBJ(roadMeshLod, lodMeshPath);

                    var aprmaterial = PrefabCollection<NetInfo>.FindLoaded(networkName).m_segments[0].m_segmentMaterial;
                    var aprsource = aprmaterial.GetTexture("_APRMap") as Texture2D;

                    DumpAPR(filename, FlipTexture(aprsource,false));

                    //for workshop roads display disclaimer!
                    // display message
                    ExceptionPanel panel = UIView.library.ShowModal<ExceptionPanel>("ExceptionPanel");
                    panel.SetMessage("Road Dump", "Network Name: " + networkName + "\n\nDumped Items:\n" + diffuseTexturePath + "\n" + meshPath + "\n" 
                        + lodMeshPath, false);

                }

                else
                {
                    // not both keys pressed: Reset processed state
                    _processed = false;
                }


            }
            catch (Exception e)
            {

                ExceptionPanel panel = UIView.library.ShowModal<ExceptionPanel>("ExceptionPanel");
                panel.SetMessage("Road Dump (Failed)", "" + e, false);
            }
        }

        //Texture flipping script from https://stackoverflow.com/questions/35950660/unity-180-rotation-for-a-texture2d-or-maybe-flip-both
        Texture2D FlipTexture(Texture2D original, bool upSideDown = true)
        {

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
        private static void DumpAPR(string assetName, Texture2D aprMap, bool extract = true)
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
                TextureUtil.DumpTextureToPNG(a1.ColorsToTexture(aprMap.width, aprMap.height), $"{assetName}_a");
                TextureUtil.DumpTextureToPNG(p1.ColorsToTexture(aprMap.width, aprMap.height), $"{assetName}_p");
                TextureUtil.DumpTextureToPNG(r1.ColorsToTexture(aprMap.width, aprMap.height), $"{assetName}_r");
            }
            else
            {
                TextureUtil.DumpTextureToPNG(aprMap, $"{assetName}_APR");
            }
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

            var meshToDump = mesh;

            if (!mesh.isReadable)
            {
                try
                {
                    // copy the relevant data to the temporary mesh
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
                {;
                    return;
                }
            }

            try
            {
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