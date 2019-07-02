using ColossalFramework.UI;
using UnityEngine;
using UIUtils = SamsamTS.UIUtils;
using MoreShortcuts.GUI;
using System.IO;
using System;
using System.Diagnostics;

namespace RoadDumpTools
{
    public class NetDumpPanel : UIPanel
    {
        public int dumpedSessionItems = 0;
        private UITextureAtlas m_atlas;

        private UITitleBar m_title;
        private UITextField seginput;
        private UIDropDown net_type;
        private UIDropDown netEle;
        private UIButton dumpNet;

        private UILabel dumpedTotal;
        private UICheckBox dumpMeshOnly;
        private UICheckBox dumpDiffuseOnly;

        private UIButton dumpedFolderButton;

        //public UICheckBox dumpedFolderOpen;

        private static NetDumpPanel _instance;

        private static int downoffset = 20;

        public static NetDumpPanel instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = UIView.GetAView().AddUIComponent(typeof(NetDumpPanel)) as NetDumpPanel;
                }
                return _instance;
            }
        }

        public override void Start()
        {
            LoadResources();

            atlas = UIUtils.GetAtlas("Ingame");
            backgroundSprite = "MenuPanel";
            color = new Color32(255, 255, 255, 255);
            isVisible = false;
            canFocus = true;
            isInteractive = true;
            clipChildren = true;
            width = 285;
            height = 350;
            relativePosition = new Vector3(0, 55);

            // Title Bar
            m_title = AddUIComponent<UITitleBar>();
            m_title.title = "Network Dump Tools";

            UILabel net_type_label = AddUIComponent<UILabel>();
            net_type_label.text = "Mesh Type:";
            net_type_label.autoSize = false;
            net_type_label.width = 120f;
            net_type_label.height = 20f;
            net_type_label.relativePosition = new Vector2(40, 60);

            net_type = UIUtils.CreateDropDown(this);
            net_type.width = 105;
            net_type.AddItem("Segment");
            net_type.AddItem("Node");
            net_type.selectedIndex = 0;
            net_type.relativePosition = new Vector3(140, 55);

            UILabel netEle_label = AddUIComponent<UILabel>();
            netEle_label.text = "Net Elevation:";
            netEle_label.autoSize = false;
            netEle_label.width = 124f;
            netEle_label.height = 20f;
            netEle_label.relativePosition = new Vector2(25, 100);

            netEle = UIUtils.CreateDropDown(this);
            netEle.width = 110;
            netEle.AddItem("Basic");
            netEle.AddItem("Elevated");
            netEle.AddItem("Bridge");
            netEle.AddItem("Slope");
            netEle.AddItem("Tunnel");
            netEle.selectedIndex = 0;
            netEle.relativePosition = new Vector3(149, 95);

            UILabel seglabel = AddUIComponent<UILabel>();
            seglabel.text = "Dump Mesh #:";
            seglabel.autoSize = false;
            seglabel.width = 125f;
            seglabel.height = 20f;
            seglabel.relativePosition = new Vector2(50, 140);

            seginput = UIUtils.CreateTextField(this);
            seginput.text = "1";
            seginput.width = 40f;
            seginput.height = 25f;
            seginput.padding = new RectOffset(6, 6, 6, 6);
            seginput.relativePosition = new Vector3(175, 135);
            seginput.tooltip = "Enter the mesh you want to extract here\nExample: To dump the second mesh for a segment enter '2'";

            //write code to check one at a time;
            dumpMeshOnly = UIUtils.CreateCheckBox(this);
            dumpMeshOnly.text = "Dump Mesh Only";
            dumpMeshOnly.isChecked = false;
            dumpMeshOnly.relativePosition = new Vector2(50, 175);
            dumpMeshOnly.tooltip = "Only dump the meshes (files ending in .obj and _lod.obj)";

            dumpDiffuseOnly = UIUtils.CreateCheckBox(this);
            dumpDiffuseOnly.text = "Dump Diffuse Only";
            dumpDiffuseOnly.isChecked = false;
            dumpDiffuseOnly.relativePosition = new Vector2(50, 205);
            dumpDiffuseOnly.tooltip = "Only dump the diffuse texture (file ending in _d.png)";

            dumpNet = UIUtils.CreateButton(this);
            dumpNet.text = "Dump Network";
            dumpNet.relativePosition = new Vector2(70, height - dumpNet.height - 40);
            dumpNet.width = 150;

            dumpedTotal = AddUIComponent<UILabel>();
            dumpedTotal.text = "Total Dumped Items: (0)";
            dumpedTotal.textScale = 0.8f;
            dumpedTotal.autoSize = false;
            dumpedTotal.width = 170f;
            dumpedTotal.height = 20f;
            dumpedTotal.relativePosition = new Vector2(20, height - dumpNet.height + 5);
            dumpedTotal.tooltip = "Total Items Dumped During Session - Includes Duplicate Dumps?";

            dumpNet.eventClick += (c, p) =>
            {
                if (isVisible)
                {
                    DumpProcessing dumpProcess = new DumpProcessing();
                    dumpedSessionItems = dumpProcess.DumpNetworks() + dumpedSessionItems;
                    dumpedTotal.text = "Total Dumped Items: (" + dumpedSessionItems.ToString() + ")";
                }
            };

           

            //list files dumped and open import folder button!

            
            dumpedFolderButton = UIUtils.CreateButtonSpriteImage(this, m_atlas);
            dumpedFolderButton.normalBgSprite = "ButtonMenu";
            dumpedFolderButton.hoveredBgSprite = "ButtonMenuHovered";
            dumpedFolderButton.pressedBgSprite = "ButtonMenuPressed";
            dumpedFolderButton.disabledBgSprite = "ButtonMenuDisabled";
            dumpedFolderButton.normalFgSprite = "Folder";
            dumpedFolderButton.relativePosition = new Vector2(210, height - dumpNet.height-2);
            dumpedFolderButton.height = 25;
            dumpedFolderButton.width = 31;
            dumpedFolderButton.tooltip = "Open Import Folder (file dump location)";

            dumpedFolderButton.eventClick += (c, p) =>
            {
                if (isVisible)
                {
                    //windows only for now
                    //not working lookat other code?
                    string importPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) +  "\\Colossal Order" + "\\Cities_Skylines" + "\\Addons" + "\\Import";
                    Process.Start("explorer.exe", importPath);
                }
            };

        }

        public string getNetType()
        {
            return net_type.selectedValue;
        }

            private void LoadResources()
        {
            string[] spriteNames = new string[]
            {
                "Folder",
                "FolderDisabled",
                "FolderFocused",
                "FolderHovered",
                "FolderPressed"
            };

            m_atlas = ResourceLoader.CreateTextureAtlas("RoadDumpTools", spriteNames, "RoadDumpTools.Icons.");

            UITextureAtlas defaultAtlas = ResourceLoader.GetAtlas("Ingame");
            Texture2D[] textures = new Texture2D[]
            {
                defaultAtlas["ButtonMenu"].texture,
                defaultAtlas["ButtonMenuFocused"].texture,
                defaultAtlas["ButtonMenuHovered"].texture,
                defaultAtlas["ButtonMenuPressed"].texture,
                defaultAtlas["ButtonMenuDisabled"].texture
            };

            ResourceLoader.AddTexturesInAtlas(m_atlas, textures);
        }


    }

}
