﻿using ColossalFramework.UI;
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

        public const int INITIAL_HEIGHT = 325;
       
        private UITextureAtlas m_atlas;

        private UITitleBar m_title;
        private UITextField seginput;
        private UIDropDown net_type;
        private UIDropDown netEle;
        private UIButton dumpNet;

        private UILabel dumpedTotal;
        private UICheckBox dumpMeshOnly;
        private UICheckBox dumpDiffuseOnly;
        private UICheckBox flippedTextures;
        private UITextField customFilePrefix;

        private UIButton dumpedFolderButton;
        private UIButton openFileLog;
        private UIButton advancedOptionsButton;
        private UILabel advancedOptionsButtonToggle;
        private UIButton meshResizeButton;
        private UILabel meshResizeButtonToggle;

        //public UICheckBox dumpedFolderOpen;


        private static NetDumpPanel _instance;

        public int exportCustOffset = 0;
        public int exportMeshOffset = 0;
        private Vector3 meshResizeButtonIntial;
        private Vector3 meshResizeButtonToggleIntial;
        private UICheckBox enableMeshResize;
        private Vector3 halfWidthLabelIntial;
        private UITextField halfWidth;
        private Vector3 enableMeshResizeIntial;
        private UILabel halfWidthLabel;
        private Vector3 halfWidthIntial;
        private UILabel pavementEdgeLabel;
        private Vector3 pavementEdgeLabelIntial;
        private UITextField pavementEdge;
        private Vector3 pavementEdgeIntial;

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
            height = INITIAL_HEIGHT;
            relativePosition = new Vector3(0, 55);

            // Title Bar
            m_title = AddUIComponent<UITitleBar>();
            m_title.title = "Network Dump Tools";


            UILabel netEle_label = AddUIComponent<UILabel>();
            netEle_label.text = "Net Elevation:";
            netEle_label.autoSize = false;
            netEle_label.width = 124f;
            netEle_label.height = 20f;
            netEle_label.relativePosition = new Vector2(25, 60);

            netEle = UIUtils.CreateDropDown(this);
            netEle.width = 110;
            netEle.AddItem("Basic");
            netEle.AddItem("Elevated");
            netEle.AddItem("Bridge");
            netEle.AddItem("Slope");
            netEle.AddItem("Tunnel");
            netEle.selectedIndex = 0;
            netEle.relativePosition = new Vector3(149, 55);

            UILabel net_type_label = AddUIComponent<UILabel>();
            net_type_label.text = "Mesh Type:";
            net_type_label.autoSize = false;
            net_type_label.width = 120f;
            net_type_label.height = 20f;
            net_type_label.relativePosition = new Vector2(40, 100);

            //make this segmented?
            net_type = UIUtils.CreateDropDown(this);
            net_type.width = 105;
            net_type.AddItem("Segment");
            net_type.AddItem("Node");
            net_type.selectedIndex = 0;
            net_type.relativePosition = new Vector3(140, 95);

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
            advancedOptionsButton.hoveredBgSprite = "SubBarButtonBaseHovered";
            advancedOptionsButton.text = "Export Customization";
            advancedOptionsButton.textScale = 0.8f;
            advancedOptionsButton.textPadding.top = 5;
            advancedOptionsButton.relativePosition = new Vector2(20, 173);
            advancedOptionsButton.height = 25;
            advancedOptionsButton.width = 240;
            advancedOptionsButton.tooltip = "Filter items to export and set custom file names";

            advancedOptionsButtonToggle = UIUtils.CreateLabelSpriteImage(this, m_atlas);
            advancedOptionsButtonToggle.backgroundSprite = "PropertyGroupClosed";
            advancedOptionsButtonToggle.width = 18f;
            advancedOptionsButtonToggle.height = 18f;
            advancedOptionsButtonToggle.relativePosition = new Vector2(45, 177);

            //open seen when advanced settings are open
            //add custom file name box
            //add mesh mod config here
            //lod config?
            dumpMeshOnly = UIUtils.CreateCheckBox(this);
            dumpMeshOnly.text = "Dump Mesh Only";
            dumpMeshOnly.isChecked = false;
            dumpMeshOnly.relativePosition = new Vector2(50, 210);
            dumpMeshOnly.tooltip = "Only dump the meshes (files ending in .obj and _lod.obj)";
            dumpMeshOnly.isVisible = false;

            dumpDiffuseOnly = UIUtils.CreateCheckBox(this);
            dumpDiffuseOnly.text = "Dump Diffuse Only";
            dumpDiffuseOnly.isChecked = false;
            dumpDiffuseOnly.relativePosition = new Vector2(50, 240);
            dumpDiffuseOnly.tooltip = "Only dump the diffuse texture (file ending in _d.png)";
            dumpDiffuseOnly.isVisible = false;

            flippedTextures = UIUtils.CreateCheckBox(this);
            flippedTextures.text = "Flip Dumped Textures";
            flippedTextures.isChecked = true;
            flippedTextures.relativePosition = new Vector2(50, 270);
            flippedTextures.tooltip = "Flip textures horizontally after exporting\n(default setting is true)\nThe game stores a horizontally flipped version of\n the original texture, this corrects it by flipping it back \n Important only for asymmetrical roads for elevated???";
            flippedTextures.isVisible = false;
            ///when dropdown selected to others uncheck this one?

            UILabel customFilePrefixLabel = AddUIComponent<UILabel>();
            customFilePrefixLabel.text = "Custom File Prefix:";
            customFilePrefixLabel.textAlignment = UIHorizontalAlignment.Center;
            customFilePrefixLabel.autoSize = false;
            customFilePrefixLabel.width = 225f;
            customFilePrefixLabel.height = 20f;
            customFilePrefixLabel.relativePosition = new Vector2(30, 300);
            customFilePrefixLabel.isVisible = false;

            customFilePrefix = UIUtils.CreateTextField(this);
            customFilePrefix.width = 225f;
            customFilePrefix.height = 28f;
            customFilePrefix.padding = new RectOffset(6, 6, 6, 6);
            customFilePrefix.relativePosition = new Vector3(30, 320);
            customFilePrefix.tooltip = "Enter a custom file prefix here for the dumped files\nAsset name is the file name if left blank";
            customFilePrefix.isVisible = false;

            //and or label
            advancedOptionsButton.eventClick += (c, p) =>
            {
                if (isVisible)
                {
                    if (advancedOptionsButtonToggle.backgroundSprite == "PropertyGroupOpen")
                    {
                        exportCustOffset = 0;
                        advancedOptionsButtonToggle.backgroundSprite = "PropertyGroupClosed";
                        dumpMeshOnly.isVisible = false;
                        dumpDiffuseOnly.isVisible = false;
                        flippedTextures.isVisible = false;
                        customFilePrefixLabel.isVisible = false;
                        customFilePrefix.isVisible = false;
                    }
                    else
                    {
                        exportCustOffset = 155;
                        advancedOptionsButtonToggle.backgroundSprite = "PropertyGroupOpen";
                        dumpMeshOnly.isVisible = true;
                        dumpDiffuseOnly.isVisible = true;
                        flippedTextures.isVisible = true;
                        customFilePrefixLabel.isVisible = true;
                        customFilePrefix.isVisible = true;
                    }
                    this.height = INITIAL_HEIGHT + exportCustOffset + exportMeshOffset;
                    RefreshFooterItems();

                }
            };




            meshResizeButton = UIUtils.CreateButtonSpriteImage(this, m_atlas);
            meshResizeButton.normalBgSprite = "SubBarButtonBase";
            meshResizeButton.hoveredBgSprite = "SubBarButtonBaseHovered";
            meshResizeButton.text = "Mesh Resizing";
            meshResizeButton.textScale = 0.8f;
            meshResizeButton.textPadding.top = 5;
            meshResizeButton.relativePosition = new Vector2(20, 208);
            meshResizeButtonIntial = meshResizeButton.relativePosition;
            meshResizeButton.height = 25;
            meshResizeButton.width = 240;
            meshResizeButton.tooltip = "Customize the size of the exported mesh";


            meshResizeButtonToggle = UIUtils.CreateLabelSpriteImage(this, m_atlas);
            meshResizeButtonToggle.backgroundSprite = "PropertyGroupClosed";
            meshResizeButtonToggle.width = 18f;
            meshResizeButtonToggle.height = 18f;
            meshResizeButtonToggle.relativePosition = new Vector2(70, 212);
            meshResizeButtonToggleIntial = meshResizeButtonToggle.relativePosition;

            enableMeshResize = UIUtils.CreateCheckBox(this);
            enableMeshResize.width = 150f;
            enableMeshResize.text = "Enabled";
            enableMeshResize.isChecked = false;
            enableMeshResize.relativePosition = new Vector2(90, 242);
            enableMeshResize.tooltip = "Enable Mesh Resizing (Experimental)\nOnly works for some road networks";
            enableMeshResize.isVisible = false;
            enableMeshResizeIntial = enableMeshResize.relativePosition;

            halfWidthLabel = AddUIComponent<UILabel>();
            halfWidthLabel.text = "Half Width:";
            halfWidthLabel.autoSize = false;
            halfWidthLabel.width = 105f;
            halfWidthLabel.height = 20f;
            halfWidthLabel.relativePosition = new Vector2(70, 272);
            halfWidthLabel.isVisible = false;
            halfWidthLabelIntial = halfWidthLabel.relativePosition;

            halfWidth = UIUtils.CreateTextField(this);
            halfWidth.text = "";
            halfWidth.width = 40f;
            halfWidth.height = 25f;
            halfWidth.padding = new RectOffset(6, 6, 6, 6);
            halfWidth.relativePosition = new Vector3(175, 267);
            halfWidth.tooltip = "Enter the half width you want the mesh to be\n\nCheck the properties panel in road properties to see current\nhalf width and be sure to change after reimporting."; //change move to checkbox
            halfWidth.isVisible = false;
            halfWidthIntial = halfWidth.relativePosition;

            pavementEdgeLabel = AddUIComponent<UILabel>();
            pavementEdgeLabel.text = "Pavement Edge:";
            pavementEdgeLabel.autoSize = false;
            pavementEdgeLabel.width = 140f;
            pavementEdgeLabel.height = 20f;
            pavementEdgeLabel.relativePosition = new Vector2(52, 307);
            pavementEdgeLabel.isVisible = false;
            pavementEdgeLabelIntial = pavementEdgeLabel.relativePosition;

            pavementEdge = UIUtils.CreateTextField(this);
            pavementEdge.text = "";
            pavementEdge.width = 40f;
            pavementEdge.height = 25f;
            pavementEdge.padding = new RectOffset(6, 6, 6, 6);
            pavementEdge.relativePosition = new Vector3(192, 302);
            pavementEdge.tooltip = "Enter the pavement width you for the mesh\n\nCheck the properties panel in road properties to see current\npavement width and be sure to change after reimporting.";
            pavementEdge.isVisible = false;
            pavementEdgeIntial = pavementEdge.relativePosition;



            meshResizeButton.eventClick += (c, p) =>
            {
                if (isVisible)
                {

                    if (meshResizeButtonToggle.backgroundSprite == "PropertyGroupOpen")
                    {
                        exportMeshOffset = 0;
                        meshResizeButtonToggle.backgroundSprite = "PropertyGroupClosed";
                        enableMeshResize.isVisible = false;
                        halfWidthLabel.isVisible = false;
                        halfWidth.isVisible = false;
                        pavementEdgeLabel.isVisible = false;
                        pavementEdge.isVisible = false;
                    }
                    else
                    {
                        exportMeshOffset = 100;
                        meshResizeButtonToggle.backgroundSprite = "PropertyGroupOpen";
                        enableMeshResize.isVisible = true;
                        halfWidthLabel.isVisible = true;
                        halfWidth.isVisible = true;
                        pavementEdgeLabel.isVisible = true;
                        pavementEdge.isVisible = true;


                    }
                    this.height = INITIAL_HEIGHT + exportCustOffset + exportMeshOffset;
                    RefreshFooterItems();
                }
            };

            


            dumpNet = UIUtils.CreateButton(this);
            dumpNet.text = "Dump Network";
            dumpNet.textScale = 1f;
            dumpNet.relativePosition = new Vector2(40, height - dumpNet.height - 50);
            dumpNet.width = 200;
            dumpNet.tooltip = "Dumps the network";

            dumpedTotal = AddUIComponent<UILabel>();
            dumpedTotal.text = "Total Dumped Items: (0)";
            dumpedTotal.textScale = 0.8f;
            dumpedTotal.textAlignment = UIHorizontalAlignment.Center;
            dumpedTotal.autoSize = false;
            dumpedTotal.width = 200f;
            dumpedTotal.height = 20f;
            dumpedTotal.relativePosition = new Vector2(10, height - dumpNet.height);
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

        public void RefreshFooterItems()
        {
            openFileLog.relativePosition = new Vector2(248, height - dumpNet.height - 2);
            dumpedFolderButton.relativePosition = new Vector2(210, height - dumpNet.height - 2);
            dumpedTotal.relativePosition = new Vector2(10, height - dumpNet.height + 5);
            dumpNet.relativePosition = new Vector2(40, height - dumpNet.height - 50);

            UnityEngine.Debug.Log("relativepos y" + meshResizeButton.relativePosition.y);

            meshResizeButton.relativePosition = meshResizeButtonIntial + new Vector3(0, exportCustOffset);
            meshResizeButtonToggle.relativePosition = meshResizeButtonToggleIntial + new Vector3(0, exportCustOffset);

            enableMeshResize.relativePosition = enableMeshResizeIntial + new Vector3(0, exportCustOffset);
            halfWidthLabel.relativePosition = halfWidthLabelIntial + new Vector3(0, exportCustOffset);
            halfWidth.relativePosition = halfWidthIntial + new Vector3(0, exportCustOffset);
            pavementEdgeLabel.relativePosition = pavementEdgeLabelIntial + new Vector3(0, exportCustOffset);
            pavementEdge.relativePosition = pavementEdgeIntial +new Vector3(0, exportCustOffset);
        }

        public string MeshNumber => seginput.text;
        public int GetNetEleIndex => netEle.selectedIndex;
        public string NetworkType => net_type.selectedValue;
        public string GetCustomFilePrefix() => customFilePrefix.text;
        public bool GetDumpMeshOnly => dumpMeshOnly.isChecked;
        public bool GetDumpDiffuseOnly => dumpDiffuseOnly.isChecked;
        public bool GetIfFlippedTextures => flippedTextures.isChecked;
        public bool GetIfMeshResize => enableMeshResize.isChecked;
        public string GetCustomHalfWidth => halfWidth.text;
        public string GetCustomPavementEdge => pavementEdge.text;
        private void LoadResources()
        {
            string[] spriteNames = new string[]
            {
                "Folder",
                "Log"
                
            };

            m_atlas = ResourceLoader.CreateTextureAtlas("RoadDumpTools", spriteNames, "RoadDumpTools.Icons.");

            UITextureAtlas defaultAtlas = ResourceLoader.GetAtlas("Ingame");
            Texture2D[] textures = new Texture2D[10];

            textures[0] = defaultAtlas["ButtonMenu"].texture;
            textures[1] = defaultAtlas["ButtonMenuFocused"].texture;
            textures[2] = defaultAtlas["ButtonMenuHovered"].texture;
            textures[3] = defaultAtlas["ButtonMenuPressed"].texture;
            textures[4] = defaultAtlas["ButtonMenuDisabled"].texture;

            UITextureAtlas mapAtlas = ResourceLoader.GetAtlas("InMapEditor");
            textures[5] = mapAtlas["SubBarButtonBase"].texture;
            textures[6] = mapAtlas["SubBarButtonBaseHovered"].texture;
            textures[7] = mapAtlas["SubBarButtonBaseDisabled"].texture;
            textures[8] = mapAtlas["PropertyGroupClosed"].texture;
            textures[9] = mapAtlas["PropertyGroupOpen"].texture;
            

            ResourceLoader.AddTexturesInAtlas(m_atlas, textures);

        }


    }

}
