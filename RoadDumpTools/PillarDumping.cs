using ColossalFramework;

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
            loadedPrefab.
        }


    }
}
