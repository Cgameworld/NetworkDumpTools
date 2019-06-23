using System;
using System.IO;
using System.Text.RegularExpressions;
using ColossalFramework.IO;
using UnityEngine;

namespace ModTools.Utils
{
    internal static class TextureUtil
    {
        //taken from directly from ModTools
       // https://github.com/bloodypenguin/Skylines-ModTools

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
                    //Logger.Error("There was an error while dumping the texture - " + ex.Message);
                    return;
                }
            }

            File.WriteAllBytes(filename, bytes);
            //Logger.Warning($"Texture dumped to \"{filename}\"");
        }

        public static Texture2D ToTexture2D(this Texture3D t3d)
        {
            var pixels = t3d.GetPixels();
            var width = t3d.width;
            var depth = t3d.depth;
            var height = t3d.height;
            var tex = new Texture2D(width * depth, height);
            for (var k = 0; k < depth; k++)
            {
                for (var i = 0; i < width; i++)
                {
                    for (var j = 0; j < height; j++)
                    {
                        tex.SetPixel(j * width + i, height - k - 1, pixels[width * depth * j + k * depth + i]);
                    }
                }
            }

            tex.Apply();
            return tex;
        }

        public static Texture2D ToTexture2D(this Cubemap cubemap)
        {
            var texture = new Texture2D(cubemap.width * 4, cubemap.height * 3);
            SetCubemapFace(texture, CubemapFace.PositiveX, cubemap, 1, 2);
            SetCubemapFace(texture, CubemapFace.PositiveY, cubemap, 0, 1);
            SetCubemapFace(texture, CubemapFace.PositiveZ, cubemap, 1, 1);
            SetCubemapFace(texture, CubemapFace.NegativeX, cubemap, 1, 0);
            SetCubemapFace(texture, CubemapFace.NegativeY, cubemap, 2, 1);
            SetCubemapFace(texture, CubemapFace.NegativeZ, cubemap, 1, 3);
            texture.Apply();
            return texture;
        }



        public static string LegalizeFileName(string illegal)
        {
            if (string.IsNullOrEmpty(illegal))
            {
                return DateTime.Now.ToString("yyyyMMddhhmmss");
            }

            var regexSearch = new string(Path.GetInvalidFileNameChars());
            var r = new Regex($"[{Regex.Escape(regexSearch)}]");
            return r.Replace(illegal, "_");
        }

        public static void DumpTextureToPNG(Texture previewTexture, string filename = null)
        {
            if (string.IsNullOrEmpty(filename))
            {
                var filenamePrefix = $"rt_dump_{LegalizeFileName(previewTexture.name)}";
                if (!File.Exists($"{filenamePrefix}.png"))
                {
                    filename = $"{filenamePrefix}.png";
                }
                else
                {
                    var i = 1;
                    while (File.Exists($"{filenamePrefix}_{i}.png"))
                    {
                        i++;
                    }

                    filename = $"{filenamePrefix}_{i}.png";
                }
            }
            else if (!filename.EndsWith(".png"))
            {
                filename = $"{filename}.png";
            }

            filename = Path.Combine(Path.Combine(DataLocation.addonsPath, "Import"), filename);

            if (File.Exists(filename))
            {
                File.Delete(filename);
            }

            if (previewTexture is Texture2D texture2D)
            {
                DumpTexture2D(texture2D, filename);
            }
            else if (previewTexture is RenderTexture renderTexture)
            {
                DumpTexture2D(renderTexture.ToTexture2D(), filename);
                //Logger.Warning($"Texture dumped to \"{filename}\"");
            }
            else if (previewTexture is Texture3D texture3D)
            {
                DumpTexture2D(texture3D.ToTexture2D(), filename);
            }
            else if (previewTexture is Cubemap cubemap)
            {
                DumpTexture2D(cubemap.ToTexture2D(), filename);
            }
            else
            {
                //Logger.Error($"Don't know how to dump type \"{previewTexture.GetType()}\"");
            }
        }

        public static Color32[] TextureToColors(this Texture2D texture2D)
        {
            Color32[] input;
            try
            {
                input = texture2D.GetPixels32();
            }
            catch
            {
                input = texture2D.MakeReadable().GetPixels32();
            }

            return input;
        }

        public static Texture2D ColorsToTexture(this Color32[] colors, int width, int height)
        {
            var texture2D = new Texture2D(width, height);
            texture2D.SetPixels32(colors);
            texture2D.Apply();
            return texture2D;
        }

        public static Color32[] Invert(this Color32[] colors)
        {
            var result = new Color32[colors.Length];
            for (var i = 0; i < colors.Length; i++)
            {
                result[i].r = (byte)(byte.MaxValue - colors[i].r);
                result[i].g = (byte)(byte.MaxValue - colors[i].g);
                result[i].b = (byte)(byte.MaxValue - colors[i].b);
                result[i].a = (byte)(byte.MaxValue - colors[i].a);
            }

            return result;
        }

        public static void ExtractChannels(this Texture2D texture, Color32[] r, Color32[] g, Color32[] b, Color32[] a, bool ralpha, bool galpha, bool balpha, bool rinvert, bool ginvert, bool binvert, bool ainvert)
        {
            var input = texture.TextureToColors();
            for (var index = 0; index < input.Length; ++index)
            {
                var rr = input[index].r;
                var gg = input[index].g;
                var bb = input[index].b;
                var aa = input[index].a;
                if (rinvert)
                {
                    rr = (byte)(byte.MaxValue - rr);
                }

                if (ginvert)
                {
                    gg = (byte)(byte.MaxValue - gg);
                }

                if (binvert)
                {
                    bb = (byte)(byte.MaxValue - bb);
                }

                if (ainvert)
                {
                    aa = (byte)(byte.MaxValue - aa);
                }

                if (r != null)
                {
                    if (ralpha)
                    {
                        r[index].r = rr;
                        r[index].g = rr;
                        r[index].b = rr;
                    }
                    else
                    {
                        r[index].r = rr;
                    }
                }

                if (g != null)
                {
                    if (galpha)
                    {
                        g[index].r = gg;
                        g[index].g = gg;
                        g[index].b = gg;
                    }
                    else
                    {
                        g[index].g = gg;
                    }
                }

                if (b != null)
                {
                    if (balpha)
                    {
                        b[index].r = bb;
                        b[index].g = bb;
                        b[index].b = bb;
                    }
                    else
                    {
                        b[index].b = bb;
                    }
                }

                if (a != null)
                {
                    a[index].r = aa;
                    a[index].g = aa;
                    a[index].b = aa;
                }
            }
        }

        private static Texture2D MakeReadable(this Texture texture)
        {
            //Logger.Warning($"Texture \"{texture.name}\" is marked as read-only, running workaround..");
            var rt = RenderTexture.GetTemporary(texture.width, texture.height, 0);
            Graphics.Blit(texture, rt);
            var tex = ToTexture2D(rt);
            RenderTexture.ReleaseTemporary(rt);
            return tex;
        }

        private static Texture2D ToTexture2D(this RenderTexture rt)
        {
            var oldRt = RenderTexture.active;
            RenderTexture.active = rt;
            var tex = new Texture2D(rt.width, rt.height);
            tex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
            tex.Apply();
            RenderTexture.active = oldRt;
            return tex;
        }

        private static void SetCubemapFace(Texture2D texture, CubemapFace face, Cubemap cubemap, int positionY, int positionX)
        {
            for (var x = 0; x < cubemap.height; x++)
            {
                for (var y = 0; y < cubemap.width; y++)
                {
                    var color = cubemap.GetPixel(face, x, y);
                    texture.SetPixel(positionX * cubemap.width + x, positionY * cubemap.height + y, color);
                }
            }
        }
    }
}