using ColossalFramework.UI;
using MoreShortcuts.GUI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;
using UIUtils = RoadDumpTools.UIUtils;

namespace RoadDumpTools
{
    public class NetDumpPanel : UIPanel
    {
        public int dumpedSessionItems = 0;
        public string dumpedFiles = null;

        public const int INITIAL_HEIGHT = 360;

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
        public UICheckBox enableMeshResize;
        private Vector3 enableMeshResizeIntial;
        private UIButton openMeshPoints;
        private UIButton lodGen;
        private UICheckBox removeSuffix;
        private UIScrollablePanel gridscroll;
        private List<UITextField> coordBox;
        private Vector3 openMeshPointsIntial;
        private bool done;
        private UIButton addCoordRow;
        private UIButton removeCoordRow;
        private UIButton refreshCoordRows;
        private float celloffset;
        private UIButton gridModeToggle;
        private bool advancedToggled;

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

            removeSuffix = UIUtils.CreateCheckBox(this);
            removeSuffix.text = "Remove Added Suffixes";
            removeSuffix.isChecked = false;
            removeSuffix.relativePosition = new Vector2(30, 360);
            removeSuffix.tooltip = "NOFUNCTIONALRemoves the added descriptors at the end of dumped file names\n\n" +
                "Example: An Elevated Node network without this option enabled would\nhave Elevated_node added to the end of each file name";
            removeSuffix.isVisible = false;

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
                        removeSuffix.isVisible = false;
                    }
                    else
                    {
                        exportCustOffset = 185;
                        advancedOptionsButtonToggle.backgroundSprite = "PropertyGroupOpen";
                        dumpMeshOnly.isVisible = true;
                        dumpDiffuseOnly.isVisible = true;
                        flippedTextures.isVisible = true;
                        customFilePrefixLabel.isVisible = true;
                        customFilePrefix.isVisible = true;
                        removeSuffix.isVisible = true;
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

            openMeshPoints = UIUtils.CreateButton(this);
            openMeshPoints.text = "View Mesh Points";
            openMeshPoints.textScale = 0.95f;
            openMeshPoints.relativePosition = new Vector3(40, 272);
            openMeshPoints.width = 200;
            openMeshPoints.tooltip = "View the (x,y) points for the mesh at the  z=32 cross section";
            openMeshPoints.isVisible = false;
            openMeshPointsIntial = openMeshPoints.relativePosition;

            openMeshPoints.eventClick += (c, p) =>
            {
                PointListView.instance.Show();
                PointListView.instance.GetMeshPoints();
                openMeshPoints.text = "Refresh Mesh Points";
            };



            UIPanel panel = AddUIComponent<UIPanel>();
            panel.relativePosition = new Vector2(17, 315);
            panel.size = new Vector2(255, 135);
            panel.isVisible = false;
            gridscroll = UIUtils.CreateScrollBox(panel, m_atlas);
            gridscroll.size = new Vector2(230, 138);

            UILabel titleLabel = gridscroll.AddUIComponent<UILabel>();
            titleLabel.text = "            Existing         New";
            titleLabel.tooltip = "Enter values to change";
            //titleLabel.textAlignment = UIHorizontalAlignment.Center;
            titleLabel.autoSize = false;
            titleLabel.width = 230f;
            titleLabel.height = 30f;
            titleLabel.relativePosition = new Vector2(-5, 0);
            titleLabel.isVisible = true;

            UILabel boxInfoLabel = gridscroll.AddUIComponent<UILabel>();
            boxInfoLabel.text = "          Position     Position";
            boxInfoLabel.tooltip = "Position from center of mesh | Height from ground level\n(default height for road surface is -0.3m)";
            boxInfoLabel.autoSize = false;
            //boxInfoLabel.textScale = 0.85f;
            boxInfoLabel.width = 230f;
            boxInfoLabel.height = 25f;
            boxInfoLabel.relativePosition = new Vector2(0, 20);
            boxInfoLabel.isVisible = true;

            //coordBox = new UITextField[30];
            coordBox = new List<UITextField>();
            celloffset = 0;
            AddCellFieldsTwo(8);
            for (int a = 0; a<8; a++) //fixes intial 5 px shift to the right for text boxes - find better solution??????
            {
                coordBox[a].relativePosition = coordBox[a].relativePosition + new Vector3(-5f, 0f);
            }

            UIPanel gridButtons = AddUIComponent<UIPanel>();
            gridButtons.relativePosition = new Vector2(0, 458);
            gridButtons.size = new Vector2(285, 30);
            gridButtons.isVisible = false;
            gridButtons.name = "Grid Buttons";

            refreshCoordRows = UIUtils.CreateButtonSpriteImage(gridButtons, m_atlas);
            refreshCoordRows.normalBgSprite = "ButtonMenu";
            refreshCoordRows.hoveredBgSprite = "ButtonMenuHovered";
            refreshCoordRows.pressedBgSprite = "ButtonMenuPressed";
            refreshCoordRows.disabledBgSprite = "ButtonMenuDisabled";
            refreshCoordRows.normalFgSprite = "Refresh";
            refreshCoordRows.relativePosition = new Vector2(25, 0);
            refreshCoordRows.height = 25;
            refreshCoordRows.width = 31;
            refreshCoordRows.tooltip = "Reset";

            refreshCoordRows.eventClick += (c, p) =>
            {
                if (isVisible)
                {
                    Debug.Log("Clicked Refresh! \n coordboxcount: " + coordBox.Count);


                    //ConfirmPanel confirmingpanel = UIView.library.ShowModal<ConfirmPanel>("ConfirmPanel");


                    ConfirmPanel.ShowModal("Reset Confimation", "Are you sure you want to clear all entered points and reset the number of rows?", delegate (UIComponent comp, int ret)
                    {

                       /* for (int i = 0; i < coordBox.Count; i++)
                        {
                            Debug.Log(coordBox[i].text + "  ");
                        }
                        */

                        if (ret != 1)
                        {
                            Debug.Log("pressed no!");
                            return;
                        }

                        int coordboxSize = coordBox.Count;
                        for (int k = 0; k < coordboxSize; k++)
                        {
                            Debug.Log("coordBox.Count: " + coordBox.Count);
                            coordBox[coordBox.Count - 1].isVisible = false;
                            coordBox.RemoveAt(coordBox.Count - 1);
                        }
                        celloffset = 0;
                        AddCellFieldsFour(16);


                    });

                   

                }
            };

            gridModeToggle = UIUtils.CreateButtonSpriteImage(gridButtons, m_atlas);
            //gridModeToggle.normalBgSprite = "ButtonMenu";
            gridModeToggle.normalBgSprite = "ButtonMenuDisabled";
            //gridModeToggle.hoveredBgSprite = "ButtonMenuHovered";
            //gridModeToggle.pressedBgSprite = "ButtonMenuPressed";
            //gridModeToggle.disabledBgSprite = "ButtonMenuDisabled";
            //gridModeToggle.relativePosition = new Vector2(170, 445);
            gridModeToggle.height = 25;
            gridModeToggle.relativePosition = new Vector2(75, 0);
            gridModeToggle.width = 85;
            gridModeToggle.text = "Basic";
            gridModeToggle.tooltip = "[FUTURE] Point Entering Mode (Switches Between Basic/Advanced";
            advancedToggled = false;

            gridModeToggle.eventClick += (c, p) =>
            {
                if (isVisible)
                {
                    if (!advancedToggled)
                    {
                        gridModeToggle.relativePosition = new Vector2(65, 0);
                        gridModeToggle.width = 105;
                        gridModeToggle.text = "Advanced";
                        boxInfoLabel.text = "    Pos    Height       Pos    Height ";
                        boxInfoLabel.textScale = 0.85f;
                        titleLabel.text = "  Existing Pts          New Pts";
                        advancedToggled = true;
                    }
                    else
                    {
                        gridModeToggle.relativePosition = new Vector2(75, 0);
                        gridModeToggle.width = 85;
                        gridModeToggle.text = "Basic";
                        titleLabel.text = "            Existing         New";
                        boxInfoLabel.text = "          Position     Position";
                        boxInfoLabel.textScale = 1f;
                        advancedToggled = false;
                    }

                }
            };


            addCoordRow = UIUtils.CreateButtonSpriteImage(gridButtons, m_atlas);
            addCoordRow.normalBgSprite = "ButtonMenu";
            addCoordRow.hoveredBgSprite = "ButtonMenuHovered";
            addCoordRow.pressedBgSprite = "ButtonMenuPressed";
            addCoordRow.disabledBgSprite = "ButtonMenuDisabled";
            addCoordRow.normalFgSprite = "Add";
            //addCoordRow.relativePosition = new Vector2(170, 445);
            addCoordRow.relativePosition = new Vector2(179, 0);
            addCoordRow.height = 25;
            addCoordRow.width = 31;
            addCoordRow.tooltip = "Add Row";

            addCoordRow.eventClick += (c, p) =>
            {
                if (isVisible)
                {
                    Debug.Log("Clicked Add!");
                    gridscroll.scrollPosition = new Vector2(0f, 0f);
                    AddCellFieldsTwo(2);
                }
            };

            removeCoordRow = UIUtils.CreateButtonSpriteImage(gridButtons, m_atlas);
            removeCoordRow.normalBgSprite = "ButtonMenu";
            removeCoordRow.hoveredBgSprite = "ButtonMenuHovered";
            removeCoordRow.pressedBgSprite = "ButtonMenuPressed";
            removeCoordRow.disabledBgSprite = "ButtonMenuDisabled";
            removeCoordRow.normalFgSprite = "Remove";
            //removeCoordRow.relativePosition = new Vector2(208, 445);
            removeCoordRow.relativePosition = new Vector2(217, 0);
            removeCoordRow.height = 25;
            removeCoordRow.width = 31;
            removeCoordRow.tooltip = "Remove Row";

            removeCoordRow.eventClick += (c, p) =>
            {
                if (isVisible)
                {
                    Debug.Log("Clicked Remove!");
                    for (int j = 1; j < 3; j++)
                    {
                        //DestroyImmediate(coordBox[coordBox.Count - j]);
                        //coordBox[coordBox.Count - 1].isEnabled = false;
                        //coordBox[coordBox.Count - 1].text = "Removed";
                        coordBox[coordBox.Count - 1].isVisible = false;
                        coordBox.RemoveAt(coordBox.Count - 1);
                        //coordBox.Count = coordBox.Count - 1;
                        Debug.Log("coordBox.Count: " + coordBox.Count + " | j: " + j);
                    }
                    celloffset -= 23; // pushes offset up by 23 pixel (make dynamic?)

                }
            };





            //UITextField seginput;

            meshResizeButton.eventClick += (c, p) =>
            {
                if (isVisible)
                {

                    if (meshResizeButtonToggle.backgroundSprite == "PropertyGroupOpen")
                    {
                        exportMeshOffset = 0;
                        meshResizeButtonToggle.backgroundSprite = "PropertyGroupClosed";
                        enableMeshResize.isVisible = false;
                        openMeshPoints.isVisible = false;
                        panel.isVisible = false;
                        gridButtons.isVisible = false;

                    }
                    else
                    {
                        exportMeshOffset = 250;
                        meshResizeButtonToggle.backgroundSprite = "PropertyGroupOpen";
                        enableMeshResize.isVisible = true;
                        openMeshPoints.isVisible = true;
                        panel.isVisible = true;
                        gridButtons.isVisible = true;


                    }
                    this.height = INITIAL_HEIGHT + exportCustOffset + exportMeshOffset;
                    RefreshFooterItems();
                }
            };




            dumpNet = UIUtils.CreateButton(this);
            dumpNet.text = "Dump Network";
            dumpNet.textScale = 1f;
            dumpNet.relativePosition = new Vector2(40, height - dumpNet.height - 85);
            dumpNet.width = 200;
            dumpNet.tooltip = "Dumps the network";

            lodGen = UIUtils.CreateButton(this);
            lodGen.text = "Make _lod.png Files";
            lodGen.textScale = 1f;
            lodGen.relativePosition = new Vector2(40, height - lodGen.height - 45);
            lodGen.width = 200;
            lodGen.tooltip = "Generates lod .png files\nMust dump network first or have existing matching files in the import folder";
            lodGen.isEnabled = false;

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
            dumpedFolderButton.relativePosition = new Vector2(210, height - dumpNet.height - 2);
            dumpedFolderButton.height = 25;
            dumpedFolderButton.width = 31;
            dumpedFolderButton.tooltip = "Open Import Folder (file dump location)";

            dumpedFolderButton.eventClick += (c, p) =>
            {
                if (isVisible)
                {
                    //windows only for now
                    string importPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Colossal Order" + "\\Cities_Skylines" + "\\Addons" + "\\Import";
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

        public override void Update()
        {
            //allows tabbing between fields for mesh point replacement
            if (Input.GetKey(KeyCode.Tab))
            {
                if (!done)
                {
                    for (int i = 0; i < coordBox.Count; i++)
                    {
                        if (coordBox[i].hasFocus)
                        {
                            coordBox[i + 1].Focus();
                            break;
                        }
                    }
                    done = true;
                }
            }
            else
            {
                done = false;
            }
        }

        public void AddCellFieldsFour(int cellnum)
        {
            int row = 0;
            int cell = 0;
            Debug.Log("load1 coordboxcount: " + coordBox.Count);
            int coordBoxOffset = coordBox.Count;

            for (int i = 0; i < cellnum; i++)
            {
                cell = i + coordBoxOffset;
                coordBox.Add(new UITextField());
                Debug.Log("editing cell:  " + cell);
                coordBox[cell] = UIUtils.CreateTextFieldCell(gridscroll, m_atlas);
                coordBox[cell].name = "Text Box " + cell;
                coordBox[cell].width = 55f;
                coordBox[cell].height = 25f;  //line 2pixels tall
                coordBox[cell].text = cell.ToString();
                if (i % 4 == 0)
                {
                    coordBox[cell].relativePosition = new Vector2(0, celloffset + 40f + ((coordBox[i].height - 2) * row));
                }
                else if (i % 4 == 1)
                {
                    coordBox[cell].relativePosition = new Vector2(53, celloffset + 40f + ((coordBox[i].height - 2) * row));
                }
                else if (i % 4 == 2)
                {
                    coordBox[cell].relativePosition = new Vector2(118, celloffset + 40f + ((coordBox[i].height - 2) * row));
                }
                else
                {
                    coordBox[cell].relativePosition = new Vector2(173, celloffset + 40f + ((coordBox[i].height - 2) * row));
                    row++;
                }

                coordBox[i].eventTextChanged += (c, p) =>
                {
                    Debug.Log("Box " + i + " text changed!!");
                };
            }

            Debug.Log("cellnum  " + cellnum);
            celloffset += (coordBox[cellnum - 1].height - 2) * row; //add final cell height for next time
        }


        public void AddCellFieldsTwo(int cellnum)
        {
            int row = 0;
            int cell = 0;
            int coordBoxOffset = coordBox.Count;

            for (int i = 0; i < cellnum; i++)
            {
                cell = i + coordBoxOffset;
                coordBox.Add(new UITextField());
                Debug.Log("editing cell:  " + cell);
                coordBox[cell] = UIUtils.CreateTextFieldCell(gridscroll, m_atlas);
                coordBox[cell].name = "Text Box " + cell;
                coordBox[cell].width = 80f;
                coordBox[cell].height = 25f;  //line 2pixels tall
                coordBox[cell].text = cell.ToString();
                if (i % 2 == 0)
                {
                    coordBox[cell].relativePosition = new Vector2(37, celloffset + 40f + ((coordBox[i].height - 2) * row));
                }
                else
                {
                    coordBox[cell].relativePosition = new Vector2(127, celloffset + 40f + ((coordBox[i].height - 2) * row));
                    row++;
                }

                coordBox[i].eventTextChanged += (c, p) =>
                {
                    Debug.Log("Box " + i + " text changed!!");
                };
            }

            Debug.Log("cellnum  " + cellnum);
            celloffset += (coordBox[cellnum - 1].height - 2) * row; //add final cell height for next time
        }

        public void RefreshFooterItems()
        {
            openFileLog.relativePosition = new Vector2(248, height - dumpNet.height - 2);
            dumpedFolderButton.relativePosition = new Vector2(210, height - dumpNet.height - 2);
            dumpedTotal.relativePosition = new Vector2(10, height - dumpNet.height + 5);
            dumpNet.relativePosition = new Vector2(40, height - dumpNet.height - 85);
            lodGen.relativePosition = new Vector2(40, height - lodGen.height - 45);

            UnityEngine.Debug.Log("relativepos y" + meshResizeButton.relativePosition.y);

            meshResizeButton.relativePosition = meshResizeButtonIntial + new Vector3(0, exportCustOffset);
            meshResizeButtonToggle.relativePosition = meshResizeButtonToggleIntial + new Vector3(0, exportCustOffset);

            enableMeshResize.relativePosition = enableMeshResizeIntial + new Vector3(0, exportCustOffset);
            openMeshPoints.relativePosition = openMeshPointsIntial + new Vector3(0, exportCustOffset);
        }

        public string MeshNumber => seginput.text;
        public int GetNetEleIndex => netEle.selectedIndex;
        public string NetworkType => net_type.selectedValue;
        public string GetCustomFilePrefix() => customFilePrefix.text;
        public bool GetDumpMeshOnly => dumpMeshOnly.isChecked;
        public bool GetDumpDiffuseOnly => dumpDiffuseOnly.isChecked;
        public bool GetIfFlippedTextures => flippedTextures.isChecked;
        public bool GetIfMeshResize => enableMeshResize.isChecked;

        public float [] enteredMeshPoints()
        {
            float[] points = new float[coordBox.Count];

            for (int i = 0; i< coordBox.Count; i++)
            {
               // Debug.Log("init _ p");
                points[i] = float.Parse(coordBox[i].text);
               // Debug.Log("added point " + points[i]);
            }
            return points;
        }

        private void LoadResources()
        {
            string[] spriteNames = new string[]
            {
                "Folder",
                "Log",
                "OptionsCell",
                "OptionsCellDisabled",
                "Add",
                "Remove",
                "Refresh"
            };

            m_atlas = ResourceLoader.CreateTextureAtlas("RoadDumpTools", spriteNames, "RoadDumpTools.Icons.");

            UITextureAtlas defaultAtlas = ResourceLoader.GetAtlas("Ingame");
            Texture2D[] textures = new Texture2D[13];

            textures[0] = defaultAtlas["ButtonMenu"].texture;
            textures[1] = defaultAtlas["ButtonMenuFocused"].texture;
            textures[2] = defaultAtlas["ButtonMenuHovered"].texture;
            textures[3] = defaultAtlas["ButtonMenuPressed"].texture;
            textures[4] = defaultAtlas["ButtonMenuDisabled"].texture;
            textures[5] = defaultAtlas["EmptySprite"].texture;
            textures[6] = defaultAtlas["ScrollbarTrack"].texture;
            textures[7] = defaultAtlas["ScrollbarThumb"].texture;

            UITextureAtlas mapAtlas = ResourceLoader.GetAtlas("InMapEditor");
            textures[8] = mapAtlas["SubBarButtonBase"].texture;
            textures[9] = mapAtlas["SubBarButtonBaseHovered"].texture;
            textures[10] = mapAtlas["SubBarButtonBaseDisabled"].texture;
            textures[11] = mapAtlas["PropertyGroupClosed"].texture;
            textures[12] = mapAtlas["PropertyGroupOpen"].texture;


            ResourceLoader.AddTexturesInAtlas(m_atlas, textures);

        }


    }

}
