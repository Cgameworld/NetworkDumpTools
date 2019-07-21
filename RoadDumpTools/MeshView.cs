using ColossalFramework.UI;
using UnityEngine;
using UIUtils = SamsamTS.UIUtils;
using MoreShortcuts.GUI;
using System.IO;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using Debug = UnityEngine.Debug;

namespace RoadDumpTools
{
    public class MeshView : UIPanel
    {

        public const int INITIAL_HEIGHT = 320;

        private UITextureAtlas m_atlas;

        private static MeshView _instance;

        private UITitleBar m_title;
        private UILabel TestLine;

        public static MeshView instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = UIView.GetAView().AddUIComponent(typeof(MeshView)) as MeshView;
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
            width = 900;
            height = INITIAL_HEIGHT;
            relativePosition = new Vector3(510, 105);

            // Title Bar
            m_title = AddUIComponent<UITitleBar>();
            m_title.title = "Mesh Cross Section";

            SetLines();

            //Line1.transform.Rotate(0f, 0f, 40f);        


        }

        public void SetLines()
        {
            int[] xPoints = { 20, 30, 70, 40 };
            UnityEngine.Debug.Log("xpoints" + xPoints);
            UIPanel[] Lines = new UIPanel[xPoints.Length];
            for (int i = 0; i < xPoints.Length; i++)
            {
                Lines[i] = new UIPanel();
                Lines[i] = UIUtils.CreatePanelSpriteImage(this, m_atlas);
                Lines[i].backgroundSprite = "Pixel";
                Lines[i].width = xPoints[i];
                Lines[i].height = 4f;  //line 2pixels tall
                Lines[i].relativePosition = new Vector2(300, 200+xPoints[i]);
                UnityEngine.Debug.Log("Line " + i + " postion" + Lines[i].relativePosition);
            }
        }
        private void LoadResources()
            {
                string[] spriteNames = new string[]
                {
                "Folder",
                "Log",
                "Pixel"
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
