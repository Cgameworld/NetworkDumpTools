using ColossalFramework.UI;
using UnityEngine;
using UIUtils = RoadDumpTools.UIUtils;
using System.IO;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using Debug = UnityEngine.Debug;
using System.Linq;
using RoadDumpTools.Lib;
using ColossalFramework;

namespace RoadDumpTools
{
    public class PointListView : UIPanel
    {

        public const int INITIAL_HEIGHT = 590;

        private UITextureAtlas m_atlas;

        private static PointListView _instance;

        private UITitleBar m_title;
        private List<UITextField> coordBox;
        private UIScrollablePanel gridscroll;

        public static PointListView instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = UIView.GetAView().AddUIComponent(typeof(PointListView)) as PointListView;
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
            width = 180;
            height = INITIAL_HEIGHT;
            relativePosition = new Vector3(310, 55);

            // Title Bar
            m_title = AddUIComponent<UITitleBar>();
            m_title.title = "Mesh Points";

            UIPanel panel = AddUIComponent<UIPanel>();
            panel.relativePosition = new Vector2(20, 55);
            panel.size = new Vector2(145, 530);
            gridscroll = UIUtils.CreateScrollBox(panel, m_atlas);
            gridscroll.size = new Vector2(130, 510);

            UILabel titleLabel = gridscroll.AddUIComponent<UILabel>();
            titleLabel.text = "   Pos    Height";
            titleLabel.tooltip = "Position from center of mesh | Height from ground level\n(default height for road surface is -0.3m)";
            titleLabel.textAlignment = UIHorizontalAlignment.Center;
            titleLabel.autoSize = false;
            titleLabel.width = 120f;
            titleLabel.height = 30f;
            titleLabel.relativePosition = new Vector2(0, 0);
            titleLabel.isVisible = true; //[textboxNum]

            coordBox = new List<UITextField>();
            //GenerateGrid();
            GetMeshPoints();


            //GetMeshPoints();
        }

        public void GenerateGrid(int textboxNum)
        {
            int row = 0;
            for (int i = 0; i < textboxNum; i++)
            {
                coordBox.Add(new UITextField());
                coordBox[i] = UIUtils.CreateTextFieldCell(gridscroll, m_atlas);
                coordBox[i].isInteractive = false;
                coordBox[i].normalBgSprite = "OptionsCellDisabled";
                coordBox[i].textColor = new Color32(255, 255, 255, 255);
                coordBox[i].name = "Point Text Box " + i;
                coordBox[i].width = 60f;
                coordBox[i].height = 25f;  //line 2pixels tall
                if (i % 2 == 0)
                {
                    coordBox[i].relativePosition = new Vector2(0, 25 + ((coordBox[i].height - 2) * row));
                }
                else
                {
                    coordBox[i].relativePosition = new Vector2(58, 25 + ((coordBox[i].height - 2) * row));
                    row++;
                }

            }
        }
        public void GetMeshPoints()
        {
            var prefab = PrefabCollection<NetInfo>.FindLoaded(Singleton<ToolController>.instance.m_editPrefabInfo.name);
            var meshnum = int.Parse(NetDumpPanel.instance.MeshNumber)-1;
            Vector3[] meshVertices = prefab.m_segments[meshnum].m_mesh.vertices;

            Debug.Log("coordBoxCount: " + coordBox.Count);


            for (int i = 0; i < coordBox.Count; i++)
            {
                    DestroyImmediate(coordBox[i].gameObject);
            }
           coordBox.Clear();


            Debug.Log("grab new points?");

            //List<List<float>> xyvertices = new List<List<float>>();
            List<KeyValuePair<float,float>> xylist = new List<KeyValuePair<float, float>>();

            for (int a = 0; a < meshVertices.Length; a++)
            {
                Console.WriteLine("loop" + a);
                if (Mathf.Approximately(meshVertices[a].z, 32f))
                {
                    var element = new KeyValuePair<float, float>(meshVertices[a].x, meshVertices[a].y);
                    xylist.Add(element);
                }
            }
            Debug.Log("works?");

            //xyvertices.Sort((sa1, sa2) => sa1[0].CompareTo(sa2[0]));  //sort by x
            //xyvertices.Sort((sa1, sa2) => sa1[1].CompareTo(sa2[1]));  //sort by y
            //xyvertices.Sort((sa1, sa2) => sa1[0].CompareTo(sa2[0]));  //sort by x
            //Debug.Log("1works?");
            //ebug.Log(xyvertices.Count + "xyvertices.Count");
            //
            foreach (var item in xylist)
            {
                Debug.Log("x: " + item.Key + "|  y: " + item.Value);
            }

            //generate grid needed?
            Debug.Log("xylistcount: " + xylist.Count);
            GenerateGrid(xylist.Count*2);

            int cell = 0;
            foreach (var item in xylist)
            {
                coordBox[cell].text = Math.Round(item.Key,1).ToString();
                coordBox[cell+1].text = Math.Round(item.Value, 1).ToString();
                Debug.Log("aworks? cell" + cell);
                cell = cell + 2;
            }

            /*
            int cell = 0;
            for (int i = 0; i < xyvertices.Count; i++)
            {
                Debug.Log("bworks? cell" + cell);
                if (i >= xyvertices.Count - 1 || !xyvertices[i][0].Equals(xyvertices[i + 1][0]) || !xyvertices[i][1].Equals(xyvertices[i + 1][1]))
                {
                    coordBox[cell].text = Math.Round(xyvertices[i][0], 1).ToString();
                    coordBox[cell + 1].text = Math.Round(xyvertices[i][1], 1).ToString();
                }
                Debug.Log("aworks? cell" + cell);
                cell = cell + 2;
            }
            Debug.Log("5works?");

            */
            //textboxNum = cell;
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
