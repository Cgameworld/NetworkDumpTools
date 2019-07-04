using ColossalFramework.UI;
using UnityEngine;
using UIUtils = SamsamTS.UIUtils;
using MoreShortcuts.GUI;
using System.IO;
using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace RoadDumpTools
{
    public class NetDumpPanel : UIPanel
    {
        public int dumpedSessionItems = 0;
        public string dumpedFiles = null;
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
        private UIButton openFileLog;
        private UIButton advancedOptionsButton;
        private UILabel advancedOptionsButtonToggle;

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
            height = 290;
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

            //make this segmented?
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

            advancedOptionsButton = UIUtils.CreateButtonSpriteImage(this, m_atlas);
            advancedOptionsButton.normalBgSprite = "SubBarButtonBase";
            //advancedOptionsButton.hoveredBgSprite = "ButtonMenuHovered"; fix this plz 
            //advancedOptionsButton.pressedBgSprite = "ButtonMenuPressed";
           // advancedOptionsButton.disabledBgSprite = "ButtonMenuDisabled";
            advancedOptionsButton.text = "Advanced Options";
            advancedOptionsButton.textScale = 0.8f;
            advancedOptionsButton.textPadding.top = 5;
            advancedOptionsButton.relativePosition = new Vector2(20, 173);
            advancedOptionsButton.height = 25;
            advancedOptionsButton.width = 240;
            advancedOptionsButton.tooltip = "Open more settings";

            advancedOptionsButtonToggle = UIUtils.CreateLabelSpriteImage(this, m_atlas);
            advancedOptionsButtonToggle.backgroundSprite = "PropertyGroupClosed";
            advancedOptionsButtonToggle.width = 18f;
            advancedOptionsButtonToggle.height = 18f;
            advancedOptionsButtonToggle.relativePosition = new Vector2(50, 177);


            //open seen when advanced settings are open
            //add custom file name box
            //add mesh mod config here
            //lod config?
            dumpMeshOnly = UIUtils.CreateCheckBox(this);
            dumpMeshOnly.text = "Dump Mesh Only";
            dumpMeshOnly.isChecked = false;
            dumpMeshOnly.relativePosition = new Vector2(50, 200);
            dumpMeshOnly.tooltip = "Only dump the meshes (files ending in .obj and _lod.obj)";
            dumpMeshOnly.isVisible = false;

            dumpDiffuseOnly = UIUtils.CreateCheckBox(this);
            dumpDiffuseOnly.text = "Dump Diffuse Only";
            dumpDiffuseOnly.isChecked = false;
            dumpDiffuseOnly.relativePosition = new Vector2(50, 230);
            dumpDiffuseOnly.tooltip = "Only dump the diffuse texture (file ending in _d.png)";
            dumpDiffuseOnly.isVisible = false;

            //and or label
            advancedOptionsButton.eventClick += (c, p) =>
            {
                if (isVisible)
                {
                    if (advancedOptionsButtonToggle.backgroundSprite == "PropertyGroupOpen")
                    {
                        advancedOptionsButtonToggle.backgroundSprite = "PropertyGroupClosed";
                        this.height = 290;
                        //maybe figure out refresh method!

                        openFileLog.relativePosition = new Vector2(248, height - dumpNet.height - 2);
                        dumpedFolderButton.relativePosition = new Vector2(210, height - dumpNet.height - 2);
                        dumpedTotal.relativePosition = new Vector2(10, height - dumpNet.height + 5);
                        dumpNet.relativePosition = new Vector2(70, height - dumpNet.height - 50);
                        dumpMeshOnly.isVisible = false;
                        dumpDiffuseOnly.isVisible = false;




                    }
                    else
                    {
                        advancedOptionsButtonToggle.backgroundSprite = "PropertyGroupOpen";
                        this.height = 360;


                        openFileLog.relativePosition = new Vector2(248, height - dumpNet.height - 2);
                        dumpedFolderButton.relativePosition = new Vector2(210, height - dumpNet.height - 2);
                        dumpedTotal.relativePosition = new Vector2(10, height - dumpNet.height + 5);
                        dumpNet.relativePosition = new Vector2(70, height - dumpNet.height - 50);
                        dumpMeshOnly.isVisible = true;
                        dumpDiffuseOnly.isVisible = true;
                    }

                }
            };

    

            dumpNet = UIUtils.CreateButton(this);
            dumpNet.text = "Dump Network";
            dumpNet.relativePosition = new Vector2(70, height - dumpNet.height - 50);
            dumpNet.width = 150;
            dumpNet.tooltip = "Dumps the network";

            dumpedTotal = AddUIComponent<UILabel>();
            dumpedTotal.text = "Total Dumped Items: (0)";
            dumpedTotal.textScale = 0.8f;
            dumpedTotal.textAlignment = UIHorizontalAlignment.Center;
            dumpedTotal.autoSize = false;
            dumpedTotal.width = 200f;
            dumpedTotal.height = 20f;
            dumpedTotal.relativePosition = new Vector2(10, height - dumpNet.height + 5);
            dumpedTotal.tooltip = "Total Items Dumped During Session (Duplicate Dumps Included)";

            dumpNet.eventClick += (c, p) =>
            {
                if (isVisible)
                {
                    DumpProcessing dumpProcess = new DumpProcessing();
                    dumpedSessionItems = Int32.Parse(dumpProcess.DumpNetworks()[0]) + dumpedSessionItems;
                    dumpedFiles += dumpProcess.DumpNetworks()[1];
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
                    string importPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) +  "\\Colossal Order" + "\\Cities_Skylines" + "\\Addons" + "\\Import";
                    Process.Start("explorer.exe", importPath);
                }
            };

            openFileLog = UIUtils.CreateButtonSpriteImage(this, m_atlas);
            openFileLog.normalBgSprite = "ButtonMenu";
            openFileLog.hoveredBgSprite = "ButtonMenuHovered";
            openFileLog.pressedBgSprite = "ButtonMenuPressed";
            openFileLog.disabledBgSprite = "ButtonMenuDisabled";
            openFileLog.normalFgSprite = "Log";
            openFileLog.relativePosition = new Vector2(248, height - dumpNet.height - 2);
            openFileLog.height = 25;
            openFileLog.width = 31;
            openFileLog.tooltip = "Open Export Log";

            openFileLog.eventClick += (c, p) =>
            {
                if (isVisible)
                {
                    ExceptionPanel logpanel = UIView.library.ShowModal<ExceptionPanel>("ExceptionPanel");
                    if (dumpedFiles == null)
                    {
                        logpanel.SetMessage("Dumped Files", "No Dumped Files", false);
                    }
                    else
                    {
                        logpanel.SetMessage("Dumped Files", dumpedSessionItems + " Files Dumped This Session\n" + dumpedFiles, false);
                    }

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
                "Log"
                
            };

            m_atlas = ResourceLoader.CreateTextureAtlas("RoadDumpTools", spriteNames, "RoadDumpTools.Icons.");

            UITextureAtlas defaultAtlas = ResourceLoader.GetAtlas("Ingame");
            Texture2D[] textures = new Texture2D[8];

            textures[0] = defaultAtlas["ButtonMenu"].texture;
            textures[1] = defaultAtlas["ButtonMenuFocused"].texture;
            textures[2] = defaultAtlas["ButtonMenuHovered"].texture;
            textures[3] = defaultAtlas["ButtonMenuPressed"].texture;
            textures[4] = defaultAtlas["ButtonMenuDisabled"].texture;

            UITextureAtlas mapAtlas = ResourceLoader.GetAtlas("InMapEditor");
            textures[5] = mapAtlas["SubBarButtonBase"].texture;
            textures[6] = mapAtlas["PropertyGroupClosed"].texture;
            textures[7] = mapAtlas["PropertyGroupOpen"].texture;
            

            ResourceLoader.AddTexturesInAtlas(m_atlas, textures);

        }


    }

}
