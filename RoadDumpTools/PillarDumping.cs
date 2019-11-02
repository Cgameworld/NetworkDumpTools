using ColossalFramework;
using ColossalFramework.IO;
using ColossalFramework.UI;
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
        int pillarsDumped = 0;

        public void Setup()
        {
            networkName_init = sim.m_editPrefabInfo.name;
            loadedPrefab = PrefabCollection<NetInfo>.FindLoaded(networkName_init);
        }

        public void DumpPillar()
        {
            NetInfo elevatedNet = AssetEditorRoadUtils.TryGetElevated(loadedPrefab);
            DumpBuildingInfo(GetActivePillar(elevatedNet, PillarType.BridgePillar));
            DumpBuildingInfo(GetActivePillar(elevatedNet, PillarType.MiddlePillar));

            NetInfo bridgeNet = AssetEditorRoadUtils.TryGetBridge(loadedPrefab);
            DumpBuildingInfo(GetActivePillar(bridgeNet, PillarType.BridgePillar));
            DumpBuildingInfo(GetActivePillar(bridgeNet, PillarType.MiddlePillar));

            string importFolder = Path.Combine(DataLocation.addonsPath, "Import");
            ExceptionPanel panel = UIView.library.ShowModal<ExceptionPanel>("ExceptionPanel");
            panel.SetMessage("Pillar Dumping Successful", "Network Name: " + networkName_init + "\nExported To: " + importFolder + "\nPillars Dumped: " + pillarsDumped, false);
            //explain how to replace after that not that straightfoward (restart the game or reload the asset editor with no workshop on!)

        }

        private void DumpBuildingInfo(BuildingInfo pillar)
        {
            if (pillar != null)
            {
                DumpUtil.DumpMeshAndTextures(pillar.name, pillar.m_mesh, pillar.m_material);
                pillarsDumped += 1;
            }
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
