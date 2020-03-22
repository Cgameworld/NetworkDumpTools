using System;
using System.IO;
using ColossalFramework.IO;
using ObjUnity3D;
using UnityEngine;

//from ModTools
namespace RoadDumpTools.Lib
{
    internal static class DumpUtil
    {
        public static void DumpMeshAndTextures(string assetName, Mesh mesh, Material material = null)
        {
            assetName = FileUtil.LegalizeFileName(assetName.Replace("_Data", string.Empty));

            if (mesh?.isReadable == true)
            {
                DumpMeshToOBJ(mesh, $"{assetName}.obj");
            }

            if (material != null)
            {
                DumpTextures(assetName, material);
            }
        }

        public static void DumpTextures(string assetName, Material material)
        {
            assetName = FileUtil.LegalizeFileName(assetName.Replace("_Data", string.Empty));
            DumpMainTex(assetName, (Texture2D)material.GetTexture("_MainTex"));
            DumpACI(assetName, (Texture2D)material.GetTexture("_ACIMap"));
            DumpXYS(assetName, (Texture2D)material.GetTexture("_XYSMap"));
            DumpXYCA(assetName, (Texture2D)material.GetTexture("_XYCAMap"));
            DumpAPR(assetName, (Texture2D)material.GetTexture("_APRMap"));
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
                {
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

        public static void DumpMainTex(string assetName, Texture2D mainTex, bool extract = true)
        {
            if (mainTex == null)
            {
                return;
            }

            if (extract)
            {
                var length = mainTex.width * mainTex.height;
                var r = TextureUtil.BuildBlankTextureColors(length);
                mainTex.ExtractChannels(r, r, r, null, false, false, false, false, false, false, false);
                TextureUtil.DumpTextureToPNG(r.ColorsToTexture(mainTex.width, mainTex.height), $"{assetName}_d");
            }
            else
            {
                TextureUtil.DumpTextureToPNG(mainTex, $"{assetName}_MainTex");
            }
        }

        public static void DumpACI(string assetName, Texture2D aciMap, bool extract = true)
        {
            if (aciMap == null)
            {
                return;
            }

            if (extract)
            {
                var length = aciMap.width * aciMap.height;
                var r = TextureUtil.BuildBlankTextureColors(length);
                var g = TextureUtil.BuildBlankTextureColors(length);
                var b = TextureUtil.BuildBlankTextureColors(length);
                aciMap.ExtractChannels(r, g, b, null, true, true, true, true, true, false, false);
                TextureUtil.DumpTextureToPNG(r.ColorsToTexture(aciMap.width, aciMap.height), $"{assetName}_a");
                TextureUtil.DumpTextureToPNG(g.ColorsToTexture(aciMap.width, aciMap.height), $"{assetName}_c");
                TextureUtil.DumpTextureToPNG(b.ColorsToTexture(aciMap.width, aciMap.height), $"{assetName}_i");
            }
            else
            {
                TextureUtil.DumpTextureToPNG(aciMap, $"{assetName}_ACI");
            }
        }

        private static void DumpXYS(string assetName, Texture2D xysMap, bool extract = true)
        {
            if (xysMap == null)
            {
                return;
            }

            if (extract)
            {
                var length = xysMap.width * xysMap.height;
                var r1 = TextureUtil.BuildBlankTextureColors(length);
                var b1 = TextureUtil.BuildBlankTextureColors(length);
                xysMap.ExtractChannels(r1, r1, b1, null, false, false, true, false, false, true, false);
                TextureUtil.DumpTextureToPNG(r1.ColorsToTexture(xysMap.width, xysMap.height), $"{assetName}_n");
                TextureUtil.DumpTextureToPNG(b1.ColorsToTexture(xysMap.width, xysMap.height), $"{assetName}_s");
            }
            else
            {
                TextureUtil.DumpTextureToPNG(xysMap, $"{assetName}_XYS");
            }
        }

        private static void DumpXYCA(string assetName, Texture2D xycaMap, bool extract = true)
        {
            if (xycaMap == null)
            {
                return;
            }

            if (extract)
            {
                var length = xycaMap.width * xycaMap.height;
                var r1 = TextureUtil.BuildBlankTextureColors(length);
                var b1 = TextureUtil.BuildBlankTextureColors(length);
                var a1 = TextureUtil.BuildBlankTextureColors(length);
                xycaMap.ExtractChannels(r1, r1, b1, a1, false, false, true, false, true, true, true);
                TextureUtil.DumpTextureToPNG(r1.ColorsToTexture(xycaMap.width, xycaMap.height), $"{assetName}_n");
                TextureUtil.DumpTextureToPNG(b1.ColorsToTexture(xycaMap.width, xycaMap.height), $"{assetName}_c");
                TextureUtil.DumpTextureToPNG(a1.ColorsToTexture(xycaMap.width, xycaMap.height), $"{assetName}_a");
            }
            else
            {
                TextureUtil.DumpTextureToPNG(xycaMap, $"{assetName}_XYCA");
            }
        }

        public static void DumpAPR(string assetName, Texture2D aprMap, bool extract = true)
        {
            if (aprMap == null)
            {
                return;
            }

            if (extract)
            {
                var length = aprMap.width * aprMap.height;
                var a1 = TextureUtil.BuildBlankTextureColors(length);
                var p1 = TextureUtil.BuildBlankTextureColors(length);
                var r1 = TextureUtil.BuildBlankTextureColors(length);
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
    }
}