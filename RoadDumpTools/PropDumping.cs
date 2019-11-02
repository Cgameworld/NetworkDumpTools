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
        public int bulkDumpedSessionItems;
        int propsDumped = 0;

        public void Setup()
        {
            networkName_init = sim.m_editPrefabInfo.name;
            loadedPrefab = PrefabCollection<NetInfo>.FindLoaded(networkName_init);
        }
        public void DumpProps()
        {
            PropInfo a = loadedPrefab.m_lanes[0].m_laneProps.m_props[0].m_prop;
            DumpUtil.DumpMeshAndTextures(a.name, a.m_mesh, a.m_material);
        }

    }
}
