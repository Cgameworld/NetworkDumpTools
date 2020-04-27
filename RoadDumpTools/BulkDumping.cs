using ColossalFramework;
using ColossalFramework.IO;
using ColossalFramework.UI;
using RoadImporterXML;
using System;
using System.IO;
using System.Xml.Serialization;
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
        string bulkDumpType = "";

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
            bulkDumpType = "Dumped All";

            if (NetDumpPanel.instance.exportRoadXML.isChecked)
            {
                Debug.Log("exportxml checked");
                ExportNetInfoXML();
                bulkDumpedSessionItems++;
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
            bulkDumpType = "Dumped All Within Elevation";
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
            bulkDumpType = "Dumped All in Mesh Type";
        }

        public int RoadsDumped => bulkDumpedSessionItems;

        public string LogMessage => "Bulk Road Dump - " + bulkDumpType + "\nNumber of Files Dumped: " + bulkDumpedSessionItems + "\n";

        private void SuccessModal(string networkName_init)
        {
            string importFolder = Path.Combine(DataLocation.addonsPath, "Import");
            ExceptionPanel panel = UIView.library.ShowModal<ExceptionPanel>("ExceptionPanel");
            panel.SetMessage("Bulk Network Dump Successful", "Network Name: " + networkName_init + "\nNumber of Files Dumped: " + bulkDumpedSessionItems + "\nExported To: " + importFolder +"\n", false);
        }

        private void ExportNetInfoXML()
        {
            Debug.Log("roadimporter xml begin");

            TextWriter writer = new StreamWriter(Path.Combine(Path.Combine(DataLocation.addonsPath, "Import"), $"{loadedPrefab.name}.xml"));

            Debug.Log(loadedPrefab.GetType());

            if (loadedPrefab.m_netAI.GetType() == typeof(RoadAI))
            {
                RoadAssetInfo roadAsset = new RoadAssetInfo();
                roadAsset.ReadFromGame(loadedPrefab);

                XmlSerializer ser = new XmlSerializer(typeof(RoadImporterXML.RoadAssetInfo));
                ser.Serialize(writer, roadAsset);
            }
            else if (loadedPrefab.m_netAI.GetType() == typeof(TrainTrackAI))
            {
                TrainTrackAssetInfo trainAsset = new TrainTrackAssetInfo();
                trainAsset.ReadFromGame(loadedPrefab);

                XmlSerializer ser = new XmlSerializer(typeof(RoadImporterXML.TrainTrackAssetInfo));
                ser.Serialize(writer, trainAsset);
            }
            else
            {
                throw new NotImplementedException("NetInfo XML Export Error!");
            }
            writer.Close();
            Debug.Log("success!!!");
        }

    }
}
