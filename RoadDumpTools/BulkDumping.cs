using ColossalFramework;
using ColossalFramework.IO;
using ColossalFramework.UI;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace RoadDumpTools
{
    public class BulkDumping
    {
        ToolController sim = Singleton<ToolController>.instance;
        public string networkName_init;
        NetInfo loadedPrefab;
        public int bulkDumpedSessionItems;
        private int netEleItems;
        List<int> dumpableElevationIndexes = new List<int>();

        public void Setup()
        {
            networkName_init = sim.m_editPrefabInfo.name;
            loadedPrefab = PrefabCollection<NetInfo>.FindLoaded(networkName_init);
        }

        public void DumpAllMeshes()
        {
            netEleItems = NetDumpPanel.instance.netEle.items.Length;
            for (int i = 0; i < netEleItems; i++)
            {
                Debug.Log("i " + i);
                CheckElevation(i);
            }

            foreach (var item in dumpableElevationIndexes)
            {
                Debug.Log("netele_index" + item);
                NetDumpPanel.instance.netEle.selectedIndex = item;
                //DumpAllWithinElevation(true);
            }
                
/*
            for (int i = 0; i < netEleItems; i++)
            {
                NetDumpPanel.instance.netEle.selectedIndex = i;
                DumpAllWithinElevation(true);
            }
            */
        }
        public void CheckElevation(int elevationIndex)
        {
            switch (elevationIndex)
            {
                case 0:
                    dumpableElevationIndexes.Add(elevationIndex);
                    break;
                case 1:
                    loadedPrefab = AssetEditorRoadUtils.TryGetElevated(loadedPrefab);
                    if (loadedPrefab != null)
                    {
                        dumpableElevationIndexes.Add(elevationIndex);
                    }
                    break;
                case 2:
                    Console.WriteLine("Bridge");
                    loadedPrefab = AssetEditorRoadUtils.TryGetBridge(loadedPrefab);
                    if (loadedPrefab != null)
                    {
                        dumpableElevationIndexes.Add(elevationIndex);
                    }
                    break;
                case 3:
                    loadedPrefab = AssetEditorRoadUtils.TryGetSlope(loadedPrefab);
                    if (loadedPrefab != null)
                    {
                        dumpableElevationIndexes.Add(elevationIndex);
                    }
                    break;
                case 4:
                    loadedPrefab = AssetEditorRoadUtils.TryGetTunnel(loadedPrefab);
                    if (loadedPrefab != null)
                    {
                        dumpableElevationIndexes.Add(elevationIndex);
                    }
                    break;
            }          
        }
        public void DumpAllWithinElevation(bool isNested)
        {
            NetDumpPanel.instance.net_type.selectedIndex = 0;
            DumpAllWithinType(true);
            NetDumpPanel.instance.net_type.selectedIndex = 1;
            DumpAllWithinType(true);
            if (isNested == false)
            {
                SuccessModal(networkName_init);
            }
        }
        public void DumpAllWithinType(bool isNested)
        {
            int segmentAmount = loadedPrefab.m_segments.Length;
            for (int i = 0; i < loadedPrefab.m_segments.Length; i++)
            {
                NetDumpPanel.instance.seginput.text = (i + 1).ToString();
                DumpProcessing dumpProcess = new DumpProcessing();
                bool endPopup = false;
                bulkDumpedSessionItems = Int32.Parse(dumpProcess.DumpNetworks(endPopup)[0]) + bulkDumpedSessionItems;
            }

            if (isNested == false)
            {
                SuccessModal(networkName_init);
            }
        }

        private void SuccessModal(string networkName_init)
        {
            string importFolder = Path.Combine(DataLocation.addonsPath, "Import");
            ExceptionPanel panel = UIView.library.ShowModal<ExceptionPanel>("ExceptionPanel");
            panel.SetMessage("Bulk Network Dump Successful", "Network Name: " + networkName_init + "\nExported To: " + importFolder, false);
        }
    }
}
