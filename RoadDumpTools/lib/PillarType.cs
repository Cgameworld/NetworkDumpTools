using System;
//from network skins
namespace RoadDumpTools.Lib
{
    public enum PillarType
    {
        BridgePillar,
        MiddlePillar
    }

    public static class PillarTypeExtensions
    {
        public static string GetDescription(this PillarType type)
        {
            switch (type)
            {
                case PillarType.BridgePillar: return "Bridge Pillar";
                case PillarType.MiddlePillar: return "Middle Pillar";
                default: throw new ArgumentOutOfRangeException(nameof(type));
            }
        }
    }
}