using RoadDumpTools.Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace RoadImporterXML
{
    //modfied version of code from RoadImporter: https://github.com/citiesskylines-csur/RoadImporter
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

            Debug.Log("gamenetName" + gameNet.name);

            var netname = ExtraUtils.FormatNetworkName();
            //get elevation?
            Debug.Log("mode " + mode);

            if (_mode == "Basic")
            {
                _mode = "";
            }
            else
            {
                _mode = " " + _mode;
            }

            for (int i = 0; i < nodeMeshes.Length; i++)
            {

                nodeMeshes[i] = new CSMesh();
                nodeMeshes[i].index = i;
                if (i != 0)
                {
                    nodeMeshes[i].name = netname + "_mesh" + (i + 1) + _mode;
                    nodeMeshes[i].texture = netname + "_mesh" + (i + 1) + _mode;
                }
                else
                {
                    nodeMeshes[i].name = netname + _mode;
                    nodeMeshes[i].texture = netname  + _mode;
                }
                Debug.Log("nodemesh1");
                nodeMeshes[i].name = nodeMeshes[i].name + "_node";

                nodeMeshes[i].shader = gameNet.m_nodes[i].m_material.shader.name;

                for (int d = 0; d < 3; d++)
                {
                    nodeMeshes[i].color[d] = gameNet.m_nodes[i].m_material.color[d];
                }
                Debug.Log("nodemesh2");
            }
            Debug.Log("segmesh1");
            for (int i = 0; i < segmentMeshes.Length; i++)
            {
                segmentMeshes[i] = new CSMesh();
                Debug.Log("segmesh2");
                segmentMeshes[i].index = i;
                if (i != 0)
                {
                    segmentMeshes[i].name = netname + "_mesh" + (i + 1) + _mode;
                    segmentMeshes[i].texture = netname + "_mesh" + (i + 1) + _mode;
                    Debug.Log("segmesh3");
                }
                else
                {
                    segmentMeshes[i].name = netname + _mode;
                    segmentMeshes[i].texture = netname + _mode;
                }

                //Debug.Log("segmesh4");
                segmentMeshes[i].shader = gameNet.m_segments[i].m_material.shader.name;


                for (int d = 0; d < 3; d++)
                {
                    segmentMeshes[i].color[d] = gameNet.m_segments[i].m_material.color[d];
                }
            }


            //hack for dumped slope and tunnel meshes - removes first entry in list
            Debug.Log("_mode: " + _mode);
            if (_mode == " Slope" || _mode == " Tunnel")
            {
                CSMesh[] tempnodeMeshes = nodeMeshes;
                CSMesh[] tempsegmentMeshes = segmentMeshes;

                nodeMeshes = new CSMesh[gameNet.m_nodes.Length - 1];
                for(int i = 0; i< nodeMeshes.Length; i++)
                {
                    nodeMeshes[i] = tempnodeMeshes[i + 1];
                }
                segmentMeshes = new CSMesh[gameNet.m_segments.Length - 1];
                for (int i = 0; i < segmentMeshes.Length; i++)
                {
                    segmentMeshes[i] = tempsegmentMeshes[i + 1];
                }
            }
        }
    }
}
