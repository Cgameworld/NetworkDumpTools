using ColossalFramework.IO;
using ColossalFramework.Plugins;
using ColossalFramework.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace RoadDumpTools
{
    class LodImageGenerator
    {
        private string filePathsLog = "";

        public void GenerateLodImages(string filepath)
        {
            Texture2D texture1;
            string[] textureFileExtensions = { "_d.png", "_a.png", "_p.png", "_r.png"};

            try
            {
                DebugOutputPanel.AddMessage(PluginManager.MessageType.Message, "filename: " + filepath);
                if (filepath == "")
                {
                    if (NetDumpPanel.instance.GetCustomFilePrefix() != "")
                    {
                        string importFolder = Path.Combine(DataLocation.addonsPath, "Import");
                        filepath = Path.Combine(importFolder, NetDumpPanel.instance.GetCustomFilePrefix());
                    }
                    else
                    {
                        throw new System.Exception("No File Path Found! \nDump a network first or put a custom filename in the custom file prefix field\n");
                    }
                    
                }
                for (int i = 0; i < textureFileExtensions.Length; i++)
                {
                    if (File.Exists(filepath + textureFileExtensions[i]))
                    {
                        texture1 = new Texture2D(1, 1);
                        texture1.LoadImage(File.ReadAllBytes(filepath + textureFileExtensions[i]));
                        texture1.anisoLevel = 16;
                        texture1.MakeReadable();
                        TextureScaler.scale(texture1, 64, 64);
                        //turn off if elevated?
                        if (NetDumpPanel.instance.GetNetEleIndex == 0)
                        {
                            texture1 = FlipTexture(texture1, false);
                            DebugOutputPanel.AddMessage(PluginManager.MessageType.Message, "flipped texture1!");
                        }
                        string fileExt = "_lod" + textureFileExtensions[i];
                        string lodFilepath = filepath + fileExt;

                        DumpTexture2D(texture1, lodFilepath);
                        filePathsLog += lodFilepath + "\n";
                        NetDumpPanel.instance.dumpedFiles += lodFilepath + "\n";
                     }
                    else
                    { //make this try/catch?
                        Debug.Log(textureFileExtensions[i] + " failed");
                    }
                }

                if (NetDumpPanel.instance.GetNetEleIndex != 0)
                {
                    //bit of hack since for some reason uv maps are often completely different on lod meshes than main mesh on some vanilla elevations - ok since elevated sections are not used that much on a map anyway for custom roads. - do properly later maybe?
                    File.Copy(filepath + ".obj", filepath + "_lod.obj", true);
                }

                ExceptionPanel panel = UIView.library.ShowModal<ExceptionPanel>("ExceptionPanel");
                panel.SetMessage("LOD File Generation Successful", "Files Exported:\n" + filePathsLog, false);

            }
            catch (Exception e)
            {
                ExceptionPanel panel = UIView.library.ShowModal<ExceptionPanel>("ExceptionPanel");
                panel.SetMessage("LOD File Generation Failed", "" + e, false);
                panel.GetComponentInChildren<UISprite>().spriteName = "IconError";
            }
        }

        //from modtools
        public void DumpTexture2D(Texture2D texture, string filename)
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


    }
}
