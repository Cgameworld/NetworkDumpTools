using ColossalFramework.UI;
using UnityEngine;
using UIUtils = RoadDumpTools.UIUtils;
using MoreShortcuts.GUI;
using System.IO;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using Debug = UnityEngine.Debug;
using ColossalFramework;

namespace RoadDumpTools
{
    public class RoadExtrasAlert : UIPanel
    {

        public const int INITIAL_HEIGHT = 430;
        private UITextureAtlas m_atlas;
        private static RoadExtrasAlert _instance;
        private UITitleBar m_title;
        private UIButton okButton;
        public string extraType = "";
        public string assetEditorNew = "";
        private UILabel netEle_label;

        public static RoadExtrasAlert instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = UIView.GetAView().AddUIComponent(typeof(RoadExtrasAlert)) as RoadExtrasAlert;
                }
                return _instance;
            }
        }

        public void setExtraType(string input)
        {
            extraType = input;
            if (input == "Pillar")
            {
                assetEditorNew = "Building";
            }
            else
            {
                assetEditorNew = "Prop";
            }
            netEle_label.text = extraType + "s are not directly importable into the road editor\n\nTo reimport them, first:\n\n1) Click \"New Asset\" from the pause menu \n2) Make a New " + assetEditorNew + "\n3) Search for the" + " (assetname) " + extraType + " template \n (Workshop " + extraType + "s might be in a non-road building category) \n4) Restart the game from thepause menu or (--noWorkshop mode only: Click Reload Editor)\n5) Import new " + extraType + " in the vanilla road properties panel\n\nThis information is also in this mod's workshop description";
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
            width = 580;
            height = INITIAL_HEIGHT;
            absolutePosition = new Vector3(670, 300);

            //make setter!

            // Title Bar
            m_title = AddUIComponent<UITitleBar>();
            m_title.title = "Notice";
            m_title.GetComponentInChildren<UILabel>().textScale = 1.3f;

            UIPanel panel = AddUIComponent<UIPanel>();
            panel.atlas = UIUtils.GetAtlas("Ingame");
            panel.backgroundSprite = "GenericPanelDark";
            panel.relativePosition = new Vector2(20, 55);
            panel.size = new Vector2(width - 40, 300);

            netEle_label = panel.AddUIComponent<UILabel>();
            netEle_label.text = "Display Error";
            netEle_label.autoSize = false;
            netEle_label.wordWrap = true;
            netEle_label.width = 500f;
            netEle_label.height = 420f;
            netEle_label.relativePosition = new Vector2(20, 20);

            okButton = UIUtils.CreateButton(panel);
            okButton.normalBgSprite = "GenericPanel";
            okButton.text = "OK";
            okButton.textScale = 1f;
            okButton.relativePosition = new Vector2(400, 313);
            okButton.height = 47;
            okButton.width = 138;

            okButton.eventClick += (c, p) =>
            {
                if (isVisible)
                {
                    instance.Hide();
                }
            };

        }

        private void LoadResources()
        {

            string[] spriteNames = new string[]
            {
                "Folder",
                "Log",
                "OptionsCell",
                "OptionsCellDisabled"
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
