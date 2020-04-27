using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RoadImporterXML
{
    //code from RoadImporter: https://github.com/citiesskylines-csur/RoadImporter
        public class RoadAssetInfo : IAssetInfo
        {
            public class RoadAIProperties
            {
                public bool m_trafficLights;
                public bool m_highwayRules;
                public bool m_accumulateSnow = true;
                public int m_noiseAccumulation = 10;
                public float m_noiseRadius = 40f;
                public float m_centerAreaWidth;
                public int m_constructionCost = 1000;
                public int m_maintenanceCost = 2;
                public string m_outsideConnection = null;
            }

            public class BridgeAIProperties : RoadAIProperties
            {
                public string m_bridgePillarInfo;
                public string m_middlePillarInfo;
                public int m_elevationCost = 2000;
                public float m_bridgePillarOffset;
                public float m_middlePillarOffset;
                public bool m_doubleLength;
                public bool m_canModify = true;
            }

            public class TunnelAIProperties : RoadAIProperties
            {
                public bool m_canModify = true;
            }


            public CSNetInfo basic;
            public CSNetInfo elevated;
            public CSNetInfo bridge;
            public CSNetInfo slope;
            public CSNetInfo tunnel;

            public RoadAIProperties basicAI;
            public BridgeAIProperties elevatedAI;
            public BridgeAIProperties bridgeAI;
            public TunnelAIProperties slopeAI;
            public TunnelAIProperties tunnelAI;

            public NetModelInfo basicModel = new NetModelInfo();
            public NetModelInfo elevatedModel = new NetModelInfo();
            public NetModelInfo bridgeModel = new NetModelInfo();
            public NetModelInfo slopeModel = new NetModelInfo();
            public NetModelInfo tunnelModel = new NetModelInfo();

            public string name;

            public void ReadFromGame(NetInfo gameNetInfo)
            {
                this.name = gameNetInfo.name;
                basic = new CSNetInfo();
                elevated = new CSNetInfo();
                bridge = new CSNetInfo();
                slope = new CSNetInfo();
                tunnel = new CSNetInfo();

                basicAI = new RoadAIProperties();
                elevatedAI = new BridgeAIProperties();
                bridgeAI = new BridgeAIProperties();
                slopeAI = new TunnelAIProperties();
                tunnelAI = new TunnelAIProperties();


                RIUtils.CopyFromGame(gameNetInfo, this.basic);
                RoadAI gameRoadAI = (RoadAI)gameNetInfo.m_netAI;

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
