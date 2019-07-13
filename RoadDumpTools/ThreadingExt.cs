using ColossalFramework.UI;
using ICities;
using RoadDumpTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UIUtils = SamsamTS.UIUtils;

namespace RoadDumpTools
{
    public class ThreadingExt : LoadingExtensionBase
    {
        public override void OnLevelLoaded(LoadMode mode)
        {
           if (mode == LoadMode.LoadAsset || mode == LoadMode.NewAsset)
            {
               UIView v = UIView.GetAView();
               UIComponent uic = v.AddUIComponent(typeof(NetDumpPanel));   
            }

        }

    }
}
