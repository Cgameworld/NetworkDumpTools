using ColossalFramework.UI;
using UnityEngine;
using UIUtils = RoadDumpTools.UIUtils;
using System.IO;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using Debug = UnityEngine.Debug;
using ColossalFramework;
using RoadDumpTools.Lib;

namespace RoadDumpTools
{
    public class CrossSectionViewWindow : UIPanel
    {

        public const int INITIAL_HEIGHT = 430;
        private UITextureAtlas m_atlas;
        private static CrossSectionViewWindow _instance;
        private UITitleBar m_title;

        public static CrossSectionViewWindow instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = UIView.GetAView().AddUIComponent(typeof(CrossSectionViewWindow)) as CrossSectionViewWindow;
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
