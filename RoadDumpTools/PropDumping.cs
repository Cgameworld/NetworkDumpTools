using ColossalFramework;
using ColossalFramework.IO;
using ColossalFramework.UI;
using RoadDumpTools.Lib;
using System;
using System.IO;
using UnityEngine;

namespace RoadDumpTools
{
    public class PropDumping
    {
        ToolController sim = Singleton<ToolController>.instance;
        public string networkName_init;
        NetInfo loadedPrefab;
        int propsDumped = 0;
        string propType = "";

        public void Setup()
        {
            networkName_init = sim.m_editPrefabInfo.name;
            loadedPrefab = PrefabCollection<NetInfo>.FindLoaded(networkName_init);
        }
        public void DumpProps()
        {
            //so far dumps all props in road - problem is that dumping the mesh for most props doesn't work since most are "unreadable" in ModTools
            //tried investigating getting mesh from the sharedassets11.assets file in /Cities_Data but can't figure out how to load the file directly into the mod

            //Open to any pointers in how to solve this, If you are reading this and know a way feel free to make a new issue in GitHub
            Debug.Log(loadedPrefab.m_lanes.Length + " Lanes Exist");
            for (int i = 0; i < loadedPrefab.m_lanes.Length; i++)
            {
                NetLaneProps LaneJProps = loadedPrefab.m_lanes[i].m_laneProps;
                Debug.Log("lane" + i + "props len: " + LaneJProps.m_props.Length);

                for (int j = 0; j < LaneJProps.m_props.Length; j++)
                {
                    PropInfo a = loadedPrefab.m_lanes[i].m_laneProps.m_props[j].m_prop;
                        Debug.Log(a.name);
                        DumpUtil.DumpMeshAndTextures(a.name, a.m_mesh, a.m_material);
                }
            }
        }

        public void DumpArrows()
        {
            try
            {
                //already made the UI so instead of dumping everything, just dumping objects that fully work (lane arrows)
                DumpPropsofString("Arrow");

                if (propsDumped != 0)
                {
                    propType = "Lane Arrow";
                    SuccessModal();

                    RoadExtrasAlert.instance.setExtraType(propType);
                    RoadExtrasAlert.instance.Show();
                }
                else
                {
                    Lib.ExtraUtils.ShowErrorWindow("Lane Arrow Prop Dumping Failed", "No Lane Arrows Found");
                }
            }
            catch (Exception e)
            {
                Lib.ExtraUtils.ShowErrorWindow("Lane Arrow Prop Dumping Failed", e.ToString());
            }
        }

        private void DumpPropsofString(string findString)
        {
            for (int i = 0; i < loadedPrefab.m_lanes.Length; i++)
            {
                NetLaneProps LaneJProps = loadedPrefab.m_lanes[i].m_laneProps;
                for (int j = 0; j < LaneJProps.m_props.Length; j++)
                {
                    PropInfo a = loadedPrefab.m_lanes[i].m_laneProps.m_props[j].m_prop;
                    if (a.name.Contains(findString))
                    {
                        DumpUtil.DumpMeshAndTextures(a.name, a.m_mesh, a.m_material);
                        propsDumped += 1;
                    }
                }
            }
        }

        private void SuccessModal()
        {
            string importFolder = Path.Combine(DataLocation.addonsPath, "Import");
            ExceptionPanel panel = UIView.library.ShowModal<ExceptionPanel>("ExceptionPanel");
            panel.SetMessage(propType + " Prop Dump Successful", "Number of Files Dumped: " + propsDumped + "\nExported To: " + importFolder + "\n", false);
        }

        public int PropsDumped => propsDumped;

        public string LogMessage =>  propType + " Props Dumped: " + propsDumped;

    }
}
