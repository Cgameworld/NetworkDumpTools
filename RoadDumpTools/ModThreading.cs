using ColossalFramework;
using ColossalFramework.IO;
using ColossalFramework.UI;
using ICities;
using ObjUnity3D;
using System;
using System.IO;
using UnityEngine;

namespace RoadDumpTools
{
    public class ModThreading : ThreadingExtensionBase
    {
        //private bool _processed = false;

        public override void OnUpdate(float realTimeDelta, float simulationTimeDelta)
        {

            if (ThreadingExt.inAssetEditor)
            {
                if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) && Input.GetKey(KeyCode.Comma))
                {
                    // if (_processed) return;
                    // _processed = true;
                    Debug.Log("keyboard shortcut ran!");
                    NetDumpPanel.instance.Show();
                    NetDumpPanel.instance.m_title.closeButton.isVisible = true;
                }

                else
                {
                    // not both keys pressed: Reset processed state
                    // _processed = false;
                }
            }

        }

    }
}