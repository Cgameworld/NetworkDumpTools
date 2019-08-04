using ColossalFramework.UI;
using ICities;
using UnityEngine;

namespace RoadDumpTools
{
    public class ThreadingExt : LoadingExtensionBase
    {

        public override void OnLevelLoaded(LoadMode mode)
        {
            UIView v = UIView.GetAView();
            UIComponent uic = v.AddUIComponent(typeof(NetDumpPanel));

            NetDumpPanel.instance.Show(); //extra needed to intialize

            GameObject.FindObjectOfType<ToolController>().eventEditPrefabChanged += (info) =>
            {
                NetDumpPanel.instance.Show();
            };



        }

    }
}
