using ColossalFramework;
using ColossalFramework.IO;
using ColossalFramework.UI;
using System;
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
        string errorAddOn = "";

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
                NetDumpPanel.instance.netEle.selectedIndex = i;
                DumpAllWithinElevation(true);
            }
            SuccessModal(networkName_init);
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
                errorAddOn = dumpProcess.bulkErrorText + errorAddOn;
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
