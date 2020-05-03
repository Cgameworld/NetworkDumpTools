namespace RoadImporterXML
{
    //code from RoadImporter: https://github.com/citiesskylines-csur/RoadImporter
    public class TrainTrackAssetInfo : IAssetInfo
    {
        public class TrainTrackAIProperties
        {
            public int m_constructionCost = 1000;
            public int m_maintenanceCost = 2;
            public int m_noiseAccumulation = 10;
            public float m_noiseRadius = 40f;
            public string m_outsideConnection = null;
        }

        public class TrainBridgeAIProperties : TrainTrackAIProperties
        {
            public string m_bridgePillarInfo;
            public string m_middlePillarInfo;
            public int m_elevationCost = 2000;
            public float m_bridgePillarOffset;
            public float m_middlePillarOffset;
            public bool m_doubleLength;
            public bool m_canModify = true;
        }

        public class TrainTunnelAIProperties : TrainTrackAIProperties
        {
            public bool m_canModify = true;
        }


        public CSNetInfo basic = new CSNetInfo();
        public CSNetInfo elevated = new CSNetInfo();
        public CSNetInfo bridge = new CSNetInfo();
        public CSNetInfo slope = new CSNetInfo();
        public CSNetInfo tunnel = new CSNetInfo();

        public TrainTrackAIProperties basicAI = new TrainTrackAIProperties();
        public TrainBridgeAIProperties elevatedAI = new TrainBridgeAIProperties();
        public TrainBridgeAIProperties bridgeAI = new TrainBridgeAIProperties();
        public TrainTunnelAIProperties slopeAI = new TrainTunnelAIProperties();
        public TrainTunnelAIProperties tunnelAI = new TrainTunnelAIProperties();

        public NetModelInfo basicModel = new NetModelInfo();
        public NetModelInfo elevatedModel = new NetModelInfo();
        public NetModelInfo bridgeModel = new NetModelInfo();
        public NetModelInfo slopeModel = new NetModelInfo();
        public NetModelInfo tunnelModel = new NetModelInfo();

        public string name;



        public void ReadFromGame(NetInfo gameNetInfo)
        {
            this.name = gameNetInfo.name;
            RIUtils.CopyFromGame(gameNetInfo, this.basic);
            TrainTrackAI gameRoadAI = (TrainTrackAI)gameNetInfo.m_netAI;
            RIUtils.CopyFromGame(gameRoadAI.m_elevatedInfo, this.elevated);
            RIUtils.CopyFromGame(gameRoadAI.m_bridgeInfo, this.bridge);
            RIUtils.CopyFromGame(gameRoadAI.m_slopeInfo, this.slope);
            RIUtils.CopyFromGame(gameRoadAI.m_tunnelInfo, this.tunnel);

            RIUtils.CopyFromGame(gameRoadAI, this.basicAI);
            RIUtils.CopyFromGame(gameRoadAI.m_elevatedInfo?.GetAI(), this.elevatedAI);
            RIUtils.CopyFromGame(gameRoadAI.m_bridgeInfo?.GetAI(), this.bridgeAI);
            RIUtils.CopyFromGame(gameRoadAI.m_slopeInfo?.GetAI(), this.slopeAI);
            RIUtils.CopyFromGame(gameRoadAI.m_tunnelInfo?.GetAI(), this.tunnelAI);

            basicModel.Read(gameNetInfo, "Basic");
            elevatedModel.Read(gameRoadAI.m_elevatedInfo, "Elevated");
            bridgeModel.Read(gameRoadAI.m_bridgeInfo, "Bridge");
            slopeModel.Read(gameRoadAI.m_slopeInfo, "Slope");
            tunnelModel.Read(gameRoadAI.m_tunnelInfo, "Tunnel");
        }
 
    }
}
