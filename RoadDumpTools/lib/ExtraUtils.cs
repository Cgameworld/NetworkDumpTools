using ColossalFramework;
using ColossalFramework.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace RoadDumpTools.Lib
{
    public static class ExtraUtils
    {
        //future refactor here with other methods?
        public static void ShowErrorWindow(string header, string message)
        {
            ExceptionPanel panel = UIView.library.ShowModal<ExceptionPanel>("ExceptionPanel");
            panel.SetMessage(header, message, false);
            panel.GetComponentInChildren<UISprite>().spriteName = "IconError";
        }

        public static string FormatNetworkName()
        {
            string networkName;

            if (NetDumpPanel.instance.GetCustomFilePrefix() == "")
            {

                var networkName_init = Singleton<ToolController>.instance.m_editPrefabInfo.name;

                
                if (networkName_init.Contains("_Data"))
                {
                    networkName = networkName_init.Substring(0, networkName_init.Length - 6).Replace("/", string.Empty);
                }
                else
                {
                    networkName = networkName_init.Substring(0, networkName_init.Length - 1);
                }
            }
            else
            {
               networkName = NetDumpPanel.instance.GetCustomFilePrefix();
            }

            return networkName;
        }

        public static int DumpPropsofString(NetInfo loadedPrefab, string findString)
        {
            Debug.Log("Standalone Method Fired");
            int num = 0;
            for (int i = 0; i < loadedPrefab.m_lanes.Length; i++)
            {
                NetLaneProps LaneJProps = loadedPrefab.m_lanes[i].m_laneProps;
                for (int j = 0; j < LaneJProps.m_props.Length; j++)
                {
                    PropInfo a = loadedPrefab.m_lanes[i].m_laneProps.m_props[j].m_prop;
                    if (a.name.Contains(findString))
                    {
                        DumpUtil.DumpMeshAndTextures(a.name, a.m_mesh, a.m_material);
                        num++;
                    }
                }
            }
            return num;
        }
    }
}
