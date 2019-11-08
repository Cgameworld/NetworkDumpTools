using System;
//from network skins
namespace RoadDumpTools.Lib
{
    public enum PillarType
    {
        BridgePillar,
        MiddlePillar,
        BridgePillar2,
        BridgePillar3
    }

    public static class PillarTypeExtensions
    {
        public static string GetDescription(this PillarType type)
        {
            switch (type)
            {
                case PillarType.BridgePillar: return "Bridge Pillar";
                case PillarType.MiddlePillar: return "Middle Pillar";
                case PillarType.BridgePillar2: return "Bridge Pillar 2";
                case PillarType.BridgePillar3: return "Bridge Pillar 3";
                default: throw new ArgumentOutOfRangeException(nameof(type));
            }
        }
    }
}