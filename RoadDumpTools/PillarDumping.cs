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
        private string propDumpedMesssage;

        public void Setup()
        {
            networkName_init = sim.m_editPrefabInfo.name;
            loadedPrefab = PrefabCollection<NetInfo>.FindLoaded(networkName_init);
        }

        public void DumpPillar()
        {
            try
            {
                string extraType = "Pillar";

                NetInfo elevatedNet = AssetEditorRoadUtils.TryGetElevated(loadedPrefab);
                DumpBuildingInfo(GetActivePillar(elevatedNet, PillarType.BridgePillar));
                DumpBuildingInfo(GetActivePillar(elevatedNet, PillarType.MiddlePillar));

                NetInfo bridgeNet = AssetEditorRoadUtils.TryGetBridge(loadedPrefab);
                DumpBuildingInfo(GetActivePillar(bridgeNet, PillarType.BridgePillar));
                DumpBuildingInfo(GetActivePillar(bridgeNet, PillarType.MiddlePillar));

                //check ground elevation for things such as monorail net pillars etc
                DumpBuildingInfo(GetActivePillar(loadedPrefab, PillarType.BridgePillar));
                DumpBuildingInfo(GetActivePillar(loadedPrefab, PillarType.BridgePillar2));
                DumpBuildingInfo(GetActivePillar(loadedPrefab, PillarType.BridgePillar3));
                DumpBuildingInfo(GetActivePillar(loadedPrefab, PillarType.MiddlePillar));

                //edge case of roads with monorail pillars in the middle
                int propdumpnum = ExtraUtils.DumpPropsofString(loadedPrefab, "Monorail Pylon"); 
                Debug.Log("propdumpnum:" + propdumpnum);
                if (propdumpnum != 0)
                {
                    propDumpedMesssage = "\n" + "Pillar Props Dumped: " + propdumpnum;
                    extraType = "Pillar/Prop";
                }
                
                if (pillarsDumped != 0)
                {
                    string importFolder = Path.Combine(DataLocation.addonsPath, "Import");
                    ExceptionPanel panel = UIView.library.ShowModal<ExceptionPanel>("ExceptionPanel");
                    panel.SetMessage("Pillar Dumping Successful", "Network Name: " + networkName_init + "\nExported To: " + importFolder + propDumpedMesssage + "\nPillars Dumped: " + pillarsDumped, false);
                    //explain how to replace after that not that straightfoward (restart the game or reload the asset editor with no workshop on!)

                    RoadExtrasAlert.instance.setExtraType(extraType);
                    RoadExtrasAlert.instance.Show();
                }
                else
                {
                    Lib.ExtraUtils.ShowErrorWindow("Pillar Dumping Failed", "No Pillars Found");
                }
            }
            catch (Exception e)
            {
                Lib.ExtraUtils.ShowErrorWindow("Pillar Dumping Failed", e.ToString());
            }
        }

        public int PillarsDumped => pillarsDumped;

        public string LogMessage => "Pillars Dumped: " + pillarsDumped;

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
            var ma = prefab.m_netAI as MonorailTrackAI;

            if (ta != null)
            {
                return (type == PillarType.BridgePillar) ? ta.m_bridgePillarInfo : ta.m_middlePillarInfo;
            }
            else if (ra != null)
            {
                return (type == PillarType.BridgePillar) ? ra.m_bridgePillarInfo : ra.m_middlePillarInfo;
            }
            else if (pa != null)
            {
                return (type == PillarType.BridgePillar) ? pa.m_bridgePillarInfo : null;
            }
            else if (ma != null)
            {
                if (type == PillarType.BridgePillar)
                {
                    return ma.m_bridgePillarInfo;
                }
                else if (type == PillarType.BridgePillar2)
                {
                    return ma.m_bridgePillarInfo2;
                }
                else if (type == PillarType.BridgePillar3)
                {
                    return ma.m_bridgePillarInfo3;
                }
                else
                {
                    return ma.m_middlePillarInfo;
                }
            }
            else
            {
                return null;
            }
        }



    }
}
