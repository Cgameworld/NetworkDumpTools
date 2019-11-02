using ColossalFramework;
using ColossalFramework.IO;
using RoadDumpTools.Lib;
using System;
using System.IO;
using UnityEngine;

namespace RoadDumpTools
{
    public class PillarDumping
    {
        ToolController sim = Singleton<ToolController>.instance;
        public string networkName_init;
        NetInfo loadedPrefab;
        public int bulkDumpedSessionItems;
        private int netEleItems;
        string errorAddOn = "";

        public void Setup()
        {
            networkName_init = sim.m_editPrefabInfo.name;
            loadedPrefab = PrefabCollection<NetInfo>.FindLoaded(networkName_init);
        }

        public void DumpPillar()
        {
            NetInfo bridgeNet = AssetEditorRoadUtils.TryGetBridge(loadedPrefab);
            Debug.Log("num of bridge segments" + bridgeNet.m_segments.Length);
            BuildingInfo bridgePillar = GetActivePillar(bridgeNet, PillarType.BridgePillar);
            Debug.Log("got pillar: " + bridgePillar.name);
            //Debug.Log(bridgePillar.ToString());

            //Lib.DumpUtil.DumpMeshToOBJ(bridgePillar.m_mesh, bridgePillar.m_mesh.name);
            Debug.Log(bridgePillar.m_mesh.bounds);
            Lib.DumpUtil.DumpMeshAndTextures(bridgePillar.name, bridgePillar.m_mesh, bridgePillar.m_material);
            Debug.Log("BridgePillar DUMPED");

            //loadedPrefab = AssetEditorRoadUtils.TryGetElevated(loadedPrefab);
            //var a = AssetEditorRoadUtils.TryGetElevated(loadedPrefab);

            //RoadBridgeAI roadBridge = loadedPrefab.m_netAI as RoadBridgeAI;
            //DumpMeshToOBJ(roadBridge.m_bridgePillarInfo.m_mesh, "pillarmesh");
            //loadedPrefab = AssetEditorRoadUtils.TryGetBridge(loadedPrefab);
        }

        //from network skins
        public BuildingInfo GetActivePillar(NetInfo prefab, PillarType type)
        {
            if (prefab == null) return null;

            var ta = prefab.m_netAI as TrainTrackBridgeAI;
            var ra = prefab.m_netAI as RoadBridgeAI;
            var pa = prefab.m_netAI as PedestrianBridgeAI;

            if (ta != null)
            {
                Debug.Log("ta_notnull");
                return (type == PillarType.BridgePillar) ? ta.m_bridgePillarInfo : ta.m_middlePillarInfo;
            }
            else if (ra != null)
            {
                Debug.Log("ra_notnull");
                return (type == PillarType.BridgePillar) ? ra.m_bridgePillarInfo : ra.m_middlePillarInfo;
            }
            else if (pa != null)
            {
                Debug.Log("pa_notnull");
                return (type == PillarType.BridgePillar) ? pa.m_bridgePillarInfo : null;
            }
            else
            {
                Debug.Log("empty!-n");
                return null;
            }
        }



    }
}
