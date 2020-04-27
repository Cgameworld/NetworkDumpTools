using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RoadImporterXML
{
    //code from RoadImporter: https://github.com/citiesskylines-csur/RoadImporter
    public class NetModelInfo
    {
        public class CSMesh
        {
            public int index = -1;
            public float[] color = { 1, 1, 1 };
            public string shader = "";
            public string texture = "";
            public string name = "";
        }

        public string mode = "Basic";
        public CSMesh[] segmentMeshes;
        public CSMesh[] nodeMeshes;

        public void Read(NetInfo gameNet, string _mode)
        {
            this.mode = _mode;
            if (gameNet == null) return;
            nodeMeshes = new CSMesh[gameNet.m_nodes.Length];
            segmentMeshes = new CSMesh[gameNet.m_segments.Length];
            for (int i = 0; i < nodeMeshes.Length; i++)
            {
                nodeMeshes[i] = new CSMesh
                {
                    index = i,
                    name = gameNet.m_nodes[i].m_mesh.name,
                    shader = gameNet.m_nodes[i].m_material.shader.name,
                };
                for (int d = 0; d < 3; d++)
                {
                    nodeMeshes[i].color[d] = gameNet.m_nodes[i].m_material.color[d];
                }
            }
            for (int i = 0; i < segmentMeshes.Length; i++)
            {
                segmentMeshes[i] = new CSMesh
                {
                    index = i,
                    name = gameNet.m_segments[i].m_mesh.name,
                    shader = gameNet.m_segments[i].m_material.shader.name,
                };
                for (int d = 0; d < 3; d++)
                {
                    segmentMeshes[i].color[d] = gameNet.m_segments[i].m_material.color[d];
                }
            }
        }
    }
}
